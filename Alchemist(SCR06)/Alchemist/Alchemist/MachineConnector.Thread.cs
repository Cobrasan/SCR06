using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;

namespace Alchemist
{
    public partial class MachineConnector
    {
		// キュー
		protected Queue sendQueue = Queue.Synchronized(new Queue());              /* リクエストキュー   */

        protected enum SendStatus
        {
            Ready,				// 準備中
            Sending,			// 送信中
            AckWaiting,			// Ack待ち
			CommandProcessing	// コマンド処理中
        };
		
        protected enum RecvStatus
        {
            Ready,          // 準備中
			HeaderReading,	// ヘッダ読み込み中
            CommandProcessing      // Ack待ち
        };
        
        /* 送信関連 */
		protected SendStatus sendStatus = SendStatus.Ready;                       /// 送信状態
		protected Stopwatch sendTimer = new Stopwatch();                          /// 送信タイマー
		protected int sendRetryCount = 0;                                         /// リトライ回数
		protected CommandStruct sendCommand;                                        /// 送信コマンド
		protected CommandStruct sendAckCommand;                                    /// Ackコマンド
                                                                                 
        /* 受信関連 */
		protected RecvStatus recvStatus = RecvStatus.Ready;                       /// 受信状態
		protected CommandStruct recvCommand;                                       /// 受信コマンド
		protected byte[] buffer = new byte[7 + 512];                              /// 受信バッファ
		
		/* ポーリング関連 */
		protected Stopwatch pollingTimer = new Stopwatch();                       /// ポーリング用タイマー
		protected bool isPolling = false;											// ポーリング処理中か？
   
        /// <summary>
        /// 通信スレッド処理
        /// </summary>
        protected void machineConnectorThread()
        {
            while (isThreadValid)
            {
                // 切断状態の場合は何もしない
                if (status != Status.Uninitialized &&
                    status != Status.Offline)
                {
                    // 受信処理
                    recvTask();

                    // 送信処理
                    sendTask();

                    // ポーリングタイマ処理
                    pollingTask();

                    // キューが空の場合は、1ミリ秒待つ
                    if (sendQueue.Count == 0)
                    {
                        Thread.Sleep(1);
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// 受信処理
        /// </summary>
        protected void recvTask()
        {
			int bytesToRead = 0;

			switch (recvStatus)
            {
                case RecvStatus.Ready:

                    // バイト数チェック
					try {
						bytesToRead = serialPort.BytesToRead;
						if (bytesToRead < 1)
						{
							return;
						}

						// ヘッダチェック
						read(buffer, 0, 1);
					}
					catch {
						ts.TraceInformation("readで例外発生");
						return;
					}

					// 受信ヘッダチェック
                    if (CommandUtil.CheckHeader(buffer) == false)
                    {
                        ts.TraceInformation("受信ヘッダチェック異常");
                        return;
                    }
					recvStatus = RecvStatus.HeaderReading;
					break;

				case RecvStatus.HeaderReading : 

                    // バイト数チェック
					try {
						bytesToRead = serialPort.BytesToRead;
						if (bytesToRead < 1)
						{
							return;
						}

						// ヘッダチェック
						read(buffer, 1, 1);
					}
					catch {
						ts.TraceInformation("readで例外発生");
						return;
					}

                    recvStatus = RecvStatus.CommandProcessing;
                    break;

                case RecvStatus.CommandProcessing:
                    // 本文チェック
                    byte bodyLength = buffer[1];
					try
					{
						// 本文の長さ以上のデータが読み込めれば続行する。
						bytesToRead = serialPort.BytesToRead;
						if (bytesToRead < bodyLength+2)
						{
							return;
						}

						// 本文読み込み
                        read(buffer, 2, bodyLength + 2);

						// コマンドをデコードする
						byte[] byteImage = new byte[bodyLength + 4];
                        Array.Copy(buffer, byteImage, bodyLength + 4);
                        recvCommand = CommandUtil.decodeCommand(byteImage);

                        // Ackを受信した場合
                        if (recvCommand.Cmd == 'w' ||
                            recvCommand.Cmd == 'r')
                        {
							// 状態異常時
							if (sendStatus != SendStatus.AckWaiting) {
								ts.TraceInformation("AckWaiting以外でAckを受け取ったため無視します。");
							}
							// 正常時
							else {
								sendStatus = SendStatus.CommandProcessing;
								sendAckCommand = recvCommand.Clone();
							}
                        }
						// Ack以外のコマンドを受信した場合
                        else {
							// コマンド実行
							execCommand(recvCommand);
							
							// Ackの返信
							respondAck(recvCommand);
						}
                    }
                    catch 
                    {
                        ts.TraceInformation("コマンド受信異常");
                    }

                    // シーケンスを終了
                    recvStatus = RecvStatus.Ready;
                    break;
            }
        }

        /// <summary>
        /// 送信処理
        /// </summary>
        protected void sendTask()
        {
			switch (sendStatus)
            {
                case SendStatus.Ready : /* 送信予約待ち */
					// 送信データありの場合
					if (sendQueue.Count > 0)
                    {
						// コマンドを取り出す
						sendCommand = ((CommandStruct)sendQueue.Peek()).Clone();
						isPolling = false;
					}
					// ポーリングタイムアウト発生
					else if (isPolling)
					{
						sendCommand = new CommandStruct();
						sendCommand.Cmd = 'R';
						sendCommand.Address = 0x000A;
						sendCommand.size = 1;
					}
					// それ以外
					else {
						return;
					}

                    // リトライ回数を初期化
                    sendRetryCount = 0;
					sendStatus = SendStatus.Sending;
                    break;

                case SendStatus.Sending :   /* 送信中 */
                    // コマンドをバイト列に変換する
                    byte[] cmd = sendCommand.Encode();

					// コマンドを送信する。
                    try {
                        write(cmd, 0, cmd.Length);
						sendTimer.Restart();
                        sendStatus = SendStatus.AckWaiting;
                    }
                    catch 
                    {
                        ts.TraceInformation("Write処理失敗");
                        execSendRetry();
                        return;
                    }

                    break;

                case SendStatus.AckWaiting :    /* Ack待ち */
                    // タイムアウト発生の場合はリトライ処理を行う。
                    if (sendTimer.ElapsedMilliseconds > timeout)
                    {
                        ts.TraceInformation("Ack待ちタイムアウトが発生");
                        execSendRetry();
                        return;
                    }
					break;

				case SendStatus.CommandProcessing :		/* コマンド実行 */
                    // 不正Ackの場合は、リトライを行う。
                    if (CommandUtil.CheckAckCommand(sendCommand, sendAckCommand) == false)
                    {
                        ts.TraceInformation("不正Ack受信");
                        execSendRetry();
                        return;
                    }

                    // Online状態でなければ、Onlineに切り替え
                    if (status != Status.Online)
                    {
                        // 切断理由をリセットする
                        comError = SystemConstants.COM_ERROR_NORMAL;

                        // StatusをOnlineにする。
                        status = Status.Online;

                        // 接続イベント発生
                        if (connstatusChange != null)
                        {
                            ConnStatusEventArgs e = new ConnStatusEventArgs();
                            e.EventCode = SystemConstants.EVENT_CONNECT;

                            ctl.Invoke(connstatusChange, new object[] { this, e });
                        }
                    }
 
					// コマンド実行
					execCommand(sendAckCommand, sendCommand);

                    // ポーリング通信の場合
					if (isPolling) {
                        // ポーリング状態の解除
						isPolling = false;
					}
                    // キューからのデータ指示の場合
					else {
                        // キューから1件データを削除する
						sendQueue.Dequeue();
					}

					sendStatus = SendStatus.Ready;
                    break;
            }
        }

		/// <summary>
		/// ポーリング監視処理
		/// </summary>
		protected void pollingTask() 
		{
			// ポーリング状態でない場合
			if (!isPolling) {

				// ポーリング間隔を経過していたら、ポーリングフラグを立てる。
				// 実際の処理は、送信処理が行う。
				if (pollingTimer.ElapsedMilliseconds > pollingInterval)
				{
					isPolling = true;
				}
			}
		}

		/// <summary>
		/// 監視を1回行う
		/// </summary>
		protected void sendAliveCommand()
		{
			isPolling = true;
		}

		/// <summary>
		/// シリアルポートから読み込みを行う。
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		protected void read(byte[] buffer, int offset, int count)
		{
			// 読み込みを行う。
			serialPort.Read(buffer, offset, count);

			// 読み込みが成功したら、タイマーをリセットする。
			pollingTimer.Restart();
		}

		/// <summary>
		/// シリアルポートへ書き込みを行う。
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		protected void write(byte[] buffer, int offset, int count)
		{
			serialPort.Write(buffer, offset, count);

			// 書き込みが成功したら、タイマーをリセットする。
			pollingTimer.Restart();
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="cmd"></param>
		protected void execCommand(CommandStruct cmd, CommandStruct cmd2 = null)
		{
			CommandStruct res = new CommandStruct();

			// 共用メモリの更新
			switch (cmd.Cmd)
			{
				case 'r' : /* 読み込み要求Ack処理 */
				case 'W':  /* 書き込み要求処理 */
					// 機械メモリに書き込み
					Array.Copy(cmd.Data, 0, machineMem, cmd.Address, cmd.size);
					Array.Copy(cmd.Data, 0, pcMem, cmd.Address, cmd.size); 
					
					// メモリ書き換えイベント発生
					if (memoryDataChange != null)
					{
						MemChangeEventArgs e = new MemChangeEventArgs();
						e.Address = sendAckCommand.Address;
						e.Count = sendAckCommand.size;

						ctl.Invoke(memoryDataChange, new object[] { this, e });
						ts.TraceInformation("メモリ書き換えイベント");
					}
					ts.TraceInformation(cmd.ToString());

					break;

				case 'w': /* 書き込み要求Ack処理 */
					Array.Copy(pcMem, cmd2.Address, machineMem, cmd2.Address, cmd2.Data.Length);
					cmd2.Cmd = 'w';
					ts.TraceInformation(cmd2.ToString());
					break;

				case 'R': /* 読み込み要求処理 */
					ts.TraceInformation(cmd.ToString());
					break;
			}
		}

		/// <summary>
		/// Ack返信処理
		/// </summary>
		/// <param name="cmd"></param>
		protected void respondAck(CommandStruct cmd)
		{
			byte[] commandImage = null;

			// レスポンス処理
			if (cmd.Cmd == 'W')
			{
				commandImage = CommandUtil.EncodeCommand('w');
			}
			else if (cmd.Cmd == 'R')
			{
				ushort[] data = new ushort[cmd.size];

				//Array.Copy(machineMem, cmd.Address, data, 0, cmd.size);
				for (int i = 0; i < cmd.size; i++) {
					data[i] = (ushort)machineMem[cmd.Address+i];
				}

				commandImage = CommandUtil.EncodeCommand('r', cmd.Address, data);
			}
			
			// データ送信
			try {
				write(commandImage, 0, commandImage.Length);
			}
			catch 
            {
				ts.TraceInformation("Ack返信処理で例外発生");
			}
		}

        /// <summary>
        /// 送信リトライ処理
        /// </summary>
        protected void execSendRetry()
        {

            // リトライ回数内
			if (sendRetryCount < SystemConstants.MAX_RETRY_COUNT)
            {
				status = Status.Suspended;
				sendStatus = SendStatus.Sending;
				ts.TraceInformation("リトライ{0}回目", new object[] { sendRetryCount + 1 });
				sendRetryCount++;
            }
            // リトライ回数オーバー
            else
            {
				// 通信途絶
                PortClose();
                ts.TraceInformation("リトライ回数をオーバーしました。");

                // 切断理由を変更
                comError = SystemConstants.COM_ERROR_RETRYERROR;

                // 切断イベントの通知
                if (connstatusChange != null)
                {
                    ConnStatusEventArgs args = new ConnStatusEventArgs();
                    args.EventCode = SystemConstants.EVENT_DISCONNECT;

                    ctl.Invoke(connstatusChange, new object[] {this, args});
                    ts.TraceInformation("切断イベントが発生しました。");
                }
            }
        }
    }
}
