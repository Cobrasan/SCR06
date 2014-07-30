using System;
using System.Collections;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace Alchemist
{
    /// <summary>
    /// 機械からの書き込み通信が発生した時、または通信モジュール
    /// から読み出し通信後、機会からのackによってメモリの値が変化したときに
    /// 発生します。
    /// </summary>
    public class MemChangeEventArgs : EventArgs
    {
        public int Address = 0x00;
        public int Count = 0;
    }

    /// <summary>
    /// 通信クラス
    /// </summary>
    public partial class MachineConnector : IDisposable
    {
        // ハンドル
		protected Control ctl;
        protected TraceSource ts = new TraceSource("MachineConnector");                 /* ログ */
		protected Thread thread;                                                        /* スレッド */
		protected SerialPort serialPort = new SerialPort();                             /* シリアルポートクラス */

		// 状態
		protected enum Status
		{
			Uninitialized,      /* 未初期化 */
			Offline,            /* オフライン */
            Connecting,         /* 接続試行中 */
			Online,             /* オンライン */
			Suspended           /* ネットワーク切断 */
		};
        protected Status status;                                                        /* 状態 */
        protected bool isThreadValid = true;                                            /* スレッド有効フラグ */
        public int comError = SystemConstants.COM_ERROR_NORMAL;                         /* 通信の異常状態 */

        // 共用メモリ
        protected int[] pcMem = new int[65536+256];                                     /* 共用メモリ(PC側) */
        protected int[] machineMem = new int[65536+256];                                /* 共用メモリ(自動機側) */
        
		// 通信設定
        protected string portName;                                                      /// ポート名
        protected int borate;                                                           /// ボーレート
        protected int dataBits;                                                         /// データビット
        protected StopBits stopBits;                                                    /// ストップビット
        protected Parity parity;                                                        /// パリティ
        protected Handshake handshake;                                                  /// フロー制御
        protected int timeout = SystemConstants.SERIAL_ACK_TIMEOUT;                     // タイムアウト値  
        protected int pollingInterval = SystemConstants.SERIAL_POLLING_INTERVAL;        // ポーリング間隔

        // イベントハンドラ
        public event EventHandler<ConnStatusEventArgs> connstatusChange = null;         /// 通信接続、切断状態に変化があった時に発生します。
        public event EventHandler<MemChangeEventArgs> memoryDataChange = null;          /// 機械からの書き込み通信が発生した時、または通信モジュールから読み出し通信後
                                                                                        /// 

		protected MachineConnector() {
		}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MachineConnector(Control mainForm)
        {
            ctl = mainForm;

            // スレッドの作成
            thread = new Thread(new ThreadStart(this.machineConnectorThread));
            thread.Name = "MachineConnectorThread";
            isThreadValid = true;

            // スレッドを開始する
            ts.TraceInformation("スレッドを開始します");
            thread.Start();
            ts.TraceInformation("スレッドを開始しました");
        }

        public void Dispose() 
        {
            ts.TraceInformation("スレッドを終了します");
            isThreadValid = false;
            thread.Join();
            ts.TraceInformation("スレッドを終了しました");
        }

        /// <summary>
        /// 通信クラスの各変数を初期設定値に戻す。
        /// </summary>
        /// <returns>正常に処理が行われた場合、MCPF_SUCCESSを返す。
        /// 通信状態時に呼ばれた場合、エラーとなり、ERR_ILLEGALSTATUSを返します。</returns>
        public int Initialize()
        {
            // 通信中に呼ばれた場合は、エラー
            if (status == Status.Online ||
                status == Status.Suspended)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // 共用メモリクリア
            Array.Clear(pcMem, 0, pcMem.Length);
            Array.Clear(machineMem, 0, machineMem.Length);

            // 送受信バッファクリア
            sendQueue.Clear();
 
            // COM設定初期化
            ComConfigure(
                1,
                SystemConstants.BORATE_19200,
                SystemConstants.DATABIT_8,
                SystemConstants.STOPBIT_2,
                SystemConstants.PARITY_NONE,
                SystemConstants.FLOW_NONE
            );

            // オフライン状態へ遷移
            status = Status.Offline;

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// 通信タイムアウト時間（秒）をTimeに設定します。
        /// 
        /// </summary>
        /// <param name="Time"></param>
        /// <returns>正常に処理が行われた場合、MCPF_SUCCESSを返す。
        /// Timeが0 ～ 600の範囲を外れた場合、ERR_TIMEOUTRANGEを返す。
        /// </returns>
        public int SetTimeOut(int Time)
        {
            // 範囲チェック
            if (Utility.CheckRange(Time, 0, 600) == false)
            {
                return SystemConstants.ERR_TIMEOUTRANGE;
            }

            timeout = Time;

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// ポーリング送信間隔（秒）を、Timeに設定します。
        /// 
        /// </summary>
        /// <param name="Time"></param>
        /// <returns>正常に処理が行われた場合、MCPF_SUCCESSを返す。
        /// Timeが0 ～ 600の範囲を外れた場合、ERR_POLLINGRANGEを返します。
        /// </returns>
        public int SetPollingInterval(int Time)
        {
            // 範囲チェック
            if (Utility.CheckRange(Time, 0, 600) == false)
            {
                return SystemConstants.ERR_POLLINGRANGE;
            }

            pollingInterval = Time;

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// 機械と通信を行っているかを返します。
        /// </summary>
        /// <returns>
        /// 通信状態の場合、trueを返します。
        /// 切断状態の場合、falseを返します。
        /// </returns>
        public bool IsConnection()
        {
            if (status == Status.Online ||
                status == Status.Suspended)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 機械との通信条件を設定する。
        /// </summary>
        /// <param name="COMPort"></param>
        /// <param name="Borate"></param>
        /// <param name="DataBit"></param>
        /// <param name="StopBit"></param>
        /// <param name="Parity"></param>
        /// <param name="FlowControl"></param>
        /// <returns></returns>
        public int ComConfigure(int COMPort, int Borate, int DataBit, int StopBit, int Parity, int FlowControl)
        {
            int ret = SystemConstants.MCPF_SUCCESS;
            bool comportFound = false;

            string strComport = "COM" + COMPort;
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    // ポート発見
                    if (strComport == port)
                    {
                        comportFound = true;
                        break;
                    }
                }

                // comportが見つからなかった場合
                if (!comportFound)
                {
                    ret |= 0x01;
                }
            }
            catch (Exception)
            {
                // ポート列挙関数が失敗した場合
                ret |= 0x01;
            }

            // ボーレートのチェック
            if (Borate != SystemConstants.BORATE_4800 &&
                Borate != SystemConstants.BORATE_9600 &&
                Borate != SystemConstants.BORATE_14400 &&
                Borate != SystemConstants.BORATE_19200 &&
                Borate != SystemConstants.BORATE_38400 &&
                Borate != SystemConstants.BORATE_57600 &&
                Borate != SystemConstants.BORATE_115200)
            {
                ret |= 0x02;
            }

            // データビットチェック
            if (DataBit != SystemConstants.DATABIT_7 &&
                DataBit != SystemConstants.DATABIT_8)
            {
                ret |= 0x04;
            }

            // ストップビットチェック
            if (StopBit != SystemConstants.STOPBIT_1 &&
                StopBit != SystemConstants.STOPBIT_2)
            {
                ret |= 0x08;
            }

            // パリティビットチェック
            if (Parity != SystemConstants.PARITY_NONE &&
                Parity != SystemConstants.PARITY_EVEN &&
                Parity != SystemConstants.PARITY_ODD)
            {
                ret |= 0x10;
            }

            // フローコントロールチェック
            if (FlowControl != SystemConstants.FLOW_NONE &&
                FlowControl != SystemConstants.FLOW_HARD &&
                FlowControl != SystemConstants.FLOW_XONXOFF)
            {
                ret |= 0x20;
            }

            // エラーなしの場合、設定を保存する。
            if (ret == SystemConstants.MCPF_SUCCESS)
            {
                portName = strComport;
                switch (Borate)
                {
                    case SystemConstants.BORATE_4800:
                        borate = 4800;
                        break;

                    case SystemConstants.BORATE_9600:
                        borate = 9600;
                        break;

                    case SystemConstants.BORATE_14400:
                        borate = 14400;
                        break;

                    case SystemConstants.BORATE_19200:
                        borate = 19200;
                        break;

                    case SystemConstants.BORATE_38400:
                        borate = 38400;
                        break;

                    case SystemConstants.BORATE_57600:
                        borate = 57600;
                        break;

                    case SystemConstants.BORATE_115200:
                        borate = 115200;
                        break;
                }

                switch (DataBit)
                {
                    case SystemConstants.DATABIT_7:
                        dataBits = 7;
                        break;

                    case SystemConstants.DATABIT_8:
                        dataBits = 8;
                        break;
                }

                switch (StopBit)
                {
                    case SystemConstants.STOPBIT_1:
                        stopBits = StopBits.One;
                        break;

                    case SystemConstants.STOPBIT_2:
                        stopBits = StopBits.Two;
                        break;
                }

                switch (Parity)
                {
                    case SystemConstants.PARITY_NONE:
                        parity = System.IO.Ports.Parity.None;
                        break;

                    case SystemConstants.PARITY_EVEN:
                        parity = System.IO.Ports.Parity.Even;
                        break;

                    case SystemConstants.PARITY_ODD:
                        parity = System.IO.Ports.Parity.Odd;
                        break;
                }

                switch (FlowControl)
                {
                    case SystemConstants.FLOW_NONE:
                        handshake = Handshake.None;
                        break;

                    case SystemConstants.FLOW_HARD:
                        handshake = Handshake.RequestToSend;
                        break;

                    case SystemConstants.FLOW_XONXOFF:
                        handshake = Handshake.XOnXOff;
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// COMポートを開き通信を開始する。
        /// </summary>
        /// <returns>
        /// 正常にポートが開かれ、テスト通信の送信予約が行われた場合、MCPF_SUCCESS
        /// ポートオープンに失敗した場合、ERR_PORTOPEN
        /// ステータス異常の場合、ERR_ILLEGALSTATUS
        /// </returns>
        public int PortOpen()
        {
            // ステータスチェック
            if (status != Status.Offline)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // ポートオープン
            try
            {
                // シリアルポートの設定
                serialPort.PortName = portName;
                serialPort.BaudRate = borate;
                serialPort.DataBits = dataBits;
                serialPort.StopBits = stopBits;
                serialPort.Parity = parity;
                serialPort.Handshake = handshake;
                serialPort.ReadTimeout = SystemConstants.SERIAL_READ_TIMEOUT;
                serialPort.WriteTimeout = SystemConstants.SERIAL_WRITE_TIMEOUT;
                
                // ポートオープン
                serialPort.Open();

				sendRetryCount = 0;
				sendStatus = SendStatus.Ready;
				recvStatus = RecvStatus.Ready;
            }
            catch (Exception)
            {
                return SystemConstants.ERR_PORTOPEN;
            }

            // テスト通信予約
            sendAliveCommand();
            status = Status.Connecting;

            // OnlineかOfflineになるまで待つ
            while ((status != Status.Online) && (status != Status.Offline))
            {
                Thread.Sleep(10);
            }

            // オフラインになっていたら、ポートオープン失敗を返す
            if (status == Status.Offline)
            {
                return SystemConstants.ERR_PORTOPEN;
            }

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// COMポートを閉じ、通信を終了する
        /// </summary>
        /// <returns>
        /// 正常に処理が行われた場合 MCPF_SUCCESS
        /// 状態異常の場合 ERR_ILLEGALSTATUS
        /// </returns>
        public int PortClose()
        {
            // 状態チェック
            if (status == Status.Uninitialized ||
                status == Status.Offline)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // ポートのクローズ
            try
            {
                serialPort.Close();
            }
            catch (Exception)
            {
                ts.TraceInformation("PortClose()で例外発生");
            }
            finally
            {
                status = Status.Offline;
                //comError = SystemConstants.COM_ERROR_NORMAL;

                // 切断イベント発生
                if (connstatusChange != null)
                {
                    ConnStatusEventArgs e = new ConnStatusEventArgs();
                    e.EventCode = SystemConstants.EVENT_DISCONNECT;

                    ctl.Invoke(connstatusChange, new object[] {this, e});
                }


            }

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// Address番地の共有メモリの値をMemValに設定する。
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="MemVal"></param>
        /// <returns>正常に終了した場合、MCPF_SUCCESS
        /// Address番地が、0x0000 - 0xFFFFの範囲外の場合、ERR_ADDRESS_RANGE
        /// MemValが、-32768 - 32767の場合、ERR_MEMORY_RANGE
        /// </returns>
        public int MemWrite(int Address, int MemVal)
        {
            // 状態チェック
            if (status == Status.Uninitialized)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // address範囲チェック
            if (Utility.CheckRange(Address, 0x0000, 0xFFFF) == false)
            {
                return SystemConstants.ERR_ADDRESS_RANGE;
            }
            // MemVal範囲チェック
            if (Utility.CheckRange(MemVal, -32768, 32767) == false)
            {
                return SystemConstants.ERR_MEMORY_RANGE;
            }

            pcMem[Address] = MemVal;


            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// 共有メモリのAddress番地の値をMemValに入れて返す。
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="MemVal"></param>
        /// <returns>Address番地が0x0000 - 0xFFFFの範囲外の場合ERR_ADDRESS_RANGE
        /// 値が共有メモリから正常に取得できた場合、MCPF_SUCCESS
        /// </returns>
        public int MemRead(int Address, ref int MemVal)
        {
            // 状態チェック
            if (status == Status.Uninitialized)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // address範囲チェック
            if (Utility.CheckRange(Address, 0x0000, 0xFFFF) == false)
            {
                return SystemConstants.ERR_ADDRESS_RANGE;
            }

            MemVal = machineMem[Address];
            


            return SystemConstants.MCPF_SUCCESS;
        }

		/// <summary>
		/// 共有メモリのPC版のAddress番地の値をMemValに入れて返す。
		/// </summary>
		/// <param name="Address"></param>
		/// <param name="MemVal"></param>
		/// <returns>Address番地が0x0000 - 0xFFFFの範囲外の場合ERR_ADDRESS_RANGE
		/// 値が共有メモリから正常に取得できた場合、MCPF_SUCCESS
		/// </returns>
		public int MemReadPC(int Address, ref int MemVal)
		{
			// 状態チェック
			if (status == Status.Uninitialized)
			{
				return SystemConstants.ERR_ILLEGALSTATUS;
			}

			// address範囲チェック
			if (Utility.CheckRange(Address, 0x0000, 0xFFFF) == false)
			{
				return SystemConstants.ERR_ADDRESS_RANGE;
			}

			MemVal = pcMem[Address];

			return SystemConstants.MCPF_SUCCESS;
		}

        /// <summary>
        /// ForceFlgがfalseの場合、送信対象（Address番地からSendCount分）のメモリ値をチェックし、いずれかが
        /// 異なっていた場合、機械にメモリの値を送信します。
        /// 送信対象のメモリ値が同じ場合は、正常に送信されたものとして返り値を返します。
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="SendCount"></param>
        /// <param name="ForceFlg"></param>
        /// <returns>
        /// 正常に処理された場合、MCPF_SUCCESSを返す。
        /// 送信対象が、0x00 - 0xFFFFの範囲外の場合、エラーとし、ERR_ADDRESS_RANGEを返す。
        /// SendCountが1-255を超えた場合、エラーとし、ERR_TRANSFAR_RANGEを返す。
        /// </returns>
        public int MemSend(int Address, int SendCount, bool ForceFlg)
        {
            // 状態チェック
            if (status == Status.Uninitialized)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // address範囲チェック
            if (Utility.CheckRange(Address, 0x0000, 0xFFFF) == false)
            {
                return SystemConstants.ERR_ADDRESS_RANGE;
            }

            // SendCount範囲チェック
            if (Utility.CheckRange(SendCount, 1, 255) == false)
            {
                return SystemConstants.ERR_TRANSFAR_RANGE;
            }

            /*
            // Address+SendCountが範囲外の場合
            if (Utility.CheckRange(Address+SendCount, 0x0000, 0xFFFF) == false)
            {
                return SystemConstants.ERR_ADDRESS_RANGE;
            }
            */

            // ForceFlgがfalseの場合、共有メモリ上の差異をチェックする。
            if (!ForceFlg)
            {
                bool diff = false;
                for (int i = 0; i < SendCount; i++)
                {
                    if (pcMem[Address + i] != machineMem[Address + i])
                    {
                        diff = true;
                        break;
                    }
                }

                // 違いがなければ、正常終了とする。
                if (!diff)
                {
                    return SystemConstants.MCPF_SUCCESS;
                }
            }

            // 送信処理
            ushort[] values = new ushort[SendCount];
            for (int i = 0; i < SendCount; i++) {
                values[i] = (ushort)pcMem[Address+i];
            }

            // コマンド送信キューに予約する
            CommandStruct image = new CommandStruct();

            image.Cmd = 'W';
            image.Address = (ushort)Address;
            image.Data = values;

            sendQueue.Enqueue(image);

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// 取得対象（Address番地からGetCount分）の読み込みコマンドを機械に送出する。
        /// 送信予約された時点で本ファンクションを終了します。（送信または、Ack応答は待たない）
        /// 
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="GetCount"></param>
        /// <returns>
        /// 正常に処理された場合、MCPF_SUCCESSを返す。
        /// 送信対象が、0x0000 - 0xFFFFの範囲外の場合、エラーとし、ERR_ADDRESS_RANGEを返す。
        /// GetCountが1-255の範囲外の場合、エラーとし、ERR_TRANSFAR_RANGEを返す。
        /// </returns>
        public int MemGet(int Address, int GetCount)
        {
            // 状態チェック
            if (status == Status.Uninitialized)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // address範囲チェック
            if (Utility.CheckRange(Address, 0x0000, 0xFFFF) == false)
            {
                return SystemConstants.ERR_ADDRESS_RANGE;
            }

            // GetCount範囲チェック
            if (Utility.CheckRange(GetCount, 1, 255) == false)
            {
                return SystemConstants.ERR_TRANSFAR_RANGE;
            }

            // コマンド送信キューに予約する
            CommandStruct image = new CommandStruct();
            image.Cmd = 'R';
            image.Address = (ushort)Address;
            image.size = (byte)GetCount;

            sendQueue.Enqueue(image);

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// Ack待ちがある場合、送信予約済みの数をReservedCountに
        /// 入れて、AckWaitCountに１を入れて返す。
        /// Ack待ちがない場合、送信予約済みの数をReservedCountに入れて返す。
        /// 
        /// </summary>
        /// <param name="ReservedCount"></param>
        /// <param name="AckWaitCount"></param>
        /// <returns>処理が正常に行われた場合、MCPF_SUCCESSを返す。</returns>
        public int SyncWaitCount(ref int ReservedCount, ref int AckWaitCount)
        {
            // 状態チェック
            if (status == Status.Uninitialized)
            {
                return SystemConstants.ERR_ILLEGALSTATUS;
            }

            // 送信予約済みの数
            ReservedCount = sendQueue.Count;
			if (sendStatus == SendStatus.AckWaiting)
			{
				AckWaitCount = 1;
			}
			else {
				AckWaitCount = 0;
			}

            return SystemConstants.MCPF_SUCCESS;
        }

        /// <summary>
        /// 通信の切断理由を取得します。
        /// </summary>
        /// <returns></returns>
        public int GetComError()
        {
            return comError;
        }
    }
}
