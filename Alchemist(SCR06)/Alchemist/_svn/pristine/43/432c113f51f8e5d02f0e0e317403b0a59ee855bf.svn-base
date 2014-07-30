using System;
using System.Linq;
using System.Diagnostics;

namespace Alchemist
{
    /// <summary>
    /// コマンドイメージクラス
    /// </summary>
    public class CommandStruct
    {
        public char Cmd;                                /// コマンド
        public ushort Address;                          /// アドレス
        public ushort[] Data;                           /// 書き込みデータ
        public byte size;                          /// 読み出しバイト数

        public CommandStruct Clone()
        {
            CommandStruct image = new CommandStruct();

            image.Cmd = this.Cmd;
            image.Address = this.Address;
            image.size = this.size;

            if (this.Data != null)
            {
                image.Data = new ushort[this.Data.Length];
                for (int i = 0; i < Data.Length; i++)
                {
                    image.Data[i] = this.Data[i];
                }
            }

            return image;
        }

		/// <summary>
		/// コマンドをエンコードする。
		/// </summary>
		/// <returns></returns>
		public byte[] Encode() {
			if (Cmd == 'R') {
				return CommandUtil.EncodeCommand(Cmd, Address, size);
			}
			else if (Cmd == 'W' || Cmd == 'r') {
				return CommandUtil.EncodeCommand(Cmd, Address, Data); 
			}
			else if (Cmd == 'w') {
				return CommandUtil.EncodeCommand(Cmd);
			}
			else {
				return null;
			}
		}

		public override string ToString() {
			if (Data == null) {
				Data = new ushort[0];
			}

			return String.Format(@"
##############################################################
Cmd     : {0}
Address : 0x{1,0:X4}
Size    : {2}
Data    : {3}
##############################################################
", new object [] {Cmd, Address, size, string.Join(",", Data) });
		}
    }
    
    /// <summary>
    /// 通信コマンドのエンコード/デコードモジュール
    /// </summary>
    static public class CommandUtil
    {
        private const byte STB = 0x01;                      /* 電文開始コード(STX) */
        private const byte ENB = 0x04;                      /* 電文終了コード(CR)  */

        /// <summary>
        /// チェックサムを計算します。
        /// </summary>
        /// <param name="data">チェックサムを計算する対象の配列を指定します。</param>
        /// <param name="count">チェックサムを計算する要素数を指定します。</param>
        /// <returns>チェックサム結果を返却します。</returns>
        public static byte CalcChecksum(byte[] data, int count)
        {
            byte sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += data[i];
            }

            return sum;
        }

        /// <summary>
        /// ヘッダをチェックします。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckHeader(byte[] data)
        {
            if (data[0] == STB)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 送ったコマンドに対応するAckかチェックします。
        /// </summary>
        /// <param name="sent"></param>
        /// <param name="ack"></param>
        /// <returns></returns>
        public static bool CheckAckCommand(CommandStruct sent, CommandStruct ack)
        {
            if (sent.Cmd.ToString().ToLower() != ack.Cmd.ToString())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 発行するコマンドを作成（エンコード）します。
        /// </summary>
        /// <param name="cmd">'R'のいずれかを指定します。</param>
        /// <param name="address">アドレスを指定します。</param>
        /// <param name="count">読み込みまたは、書き出しバイト数を指定します。</param>
        /// <returns></returns>
        public static byte[] EncodeCommand(char cmd, ushort address, byte count)
        {
            byte []byteImage = new byte[8];
            
            // コマンド種別チェック
            Debug.Assert(cmd == 'R');

            byte[] arrAddress = BitConverter.GetBytes(address);

            /* 電文作成 */
            int n = 0;
            byteImage[n++] = STB;                               /* STB */
            byteImage[n++] = 4;                                 /* サイズ */
            byteImage[n++] = (byte)cmd;                         /* コマンド */
            byteImage[n++] = arrAddress[0];                     /* addr low */
            byteImage[n++] = arrAddress[1];                     /* addr hi  */
            byteImage[n++] = (byte)(count);                     /* count */
            byteImage[n++] = CalcChecksum(byteImage, 6);        /* チェックサム */
            byteImage[n++] = ENB;                               /* ENB */

            return byteImage;
        }

        /// <summary>
        /// 発行するコマンドを作成（エンコード）します。
        /// </summary>
        /// <param name="cmd">'R'のいずれかを指定します。</param>
        /// <param name="address">アドレスを指定します。</param>
        /// <param name="count">読み込みまたは、書き出しバイト数を指定します。</param>
        /// <returns></returns>
        public static byte[] EncodeCommand(char cmd)
        {
            byte[] byteImage = new byte[5];

            // コマンド種別チェック
            Debug.Assert(cmd == 'w');

            /* 電文作成 */
            int n = 0;
            byteImage[n++] = STB;                               /* STB */
            byteImage[n++] = 1;                                 /* サイズ */
            byteImage[n++] = (byte)cmd;                         /* コマンド */
            byteImage[n++] = CalcChecksum(byteImage, 3);        /* チェックサム */
            byteImage[n++] = ENB;                               /* ENB */

            return byteImage;
        }


        /// <summary>
        /// 発行するコマンドを作成（エンコード）します。
        /// </summary>
        /// <param name="cmd">'W', 'r'のいずれかを指定します。</param>
        /// <param name="address">アドレスを指定します。</param>
        /// <param name="count">読み込みまたは、書き出しバイト数を指定します。</param>
        /// <returns></returns>
        public static byte[] EncodeCommand(char cmd, int address, ushort[] values)
        {
            // コマンド種別チェック
            Debug.Assert(cmd == 'W' || cmd == 'r');

            Debug.Assert(values != null);

            // 一度に書き込める許容量
            Debug.Assert(values.Count() <= 256);

            if (values.Count() > 256)
            {
                return null;
            }

            // 書き込みバイト数を取得
            byte count = (byte)values.Count();

            // 7+2nバイトだけメモリを確保する
            byte[] byteImage = new byte[7 + 2 * count];

            // アドレスを取得
            byte[] arrAddress = BitConverter.GetBytes(address);

            int n = 0;
            byteImage[n++] = STB;                                       /* STB */
            byteImage[n++] = (byte)(3 + 2 * count);                     /* サイズ */
            byteImage[n++] = (byte)cmd;
            byteImage[n++] = arrAddress[0];                             /* アドレスLo */
            byteImage[n++] = arrAddress[1];                             /* アドレスHi */

            for (int i = 0; i < count; i++)
            {
                byte[] data = BitConverter.GetBytes(values[i]);
                byteImage[n++] = data[0];                               /* データLo */
                byteImage[n++] = data[1];                               /* データHi */
            }

            byteImage[n++] = CalcChecksum(byteImage, 5 + 2 * count);    /* チェックサム */
            byteImage[n++] = ENB;                                       /* 電文終了コード */

            return byteImage;
        }

        /// <summary>
        /// バイトデータをデコードしてコマンドを識別します。
        /// デコードできない場合、または、チェックサムが異なる場合は、
        /// 例外が発生します。
        /// </summary>
        /// <param name="byteImage">コマンドデータ</param>
        /// <returns>コマンドイメージクラスを返却</returns>
        public static CommandStruct decodeCommand(byte []byteImage){
            CommandStruct cmdImage = new CommandStruct();

            Debug.Assert(byteImage != null);

            // STBチェック
            if (byteImage[0] != STB)
            {
                throw new ArgumentException("STB異常");
            }

            // コマンド種別チェック
            char cmd = (char)byteImage[2];
            if (cmd != 'r' && cmd != 'R' &&
                cmd != 'w' && cmd != 'W')
            {
                throw new ArgumentException("コマンド異常");
            }

            // 受信バイト数とデータ数が合わない場合は、エラー
            if ((byteImage.Length - 4) != byteImage[1])
            {
                throw new ArgumentException("サイズ異常");
            }

            // ENBチェック
            if (byteImage.Last() != ENB)
            {
                throw new ArgumentException("ENB異常");
            }

            // データを取得
            cmdImage.Cmd = cmd;

            switch (cmd)
            {
                case 'R': /* 読み込み要求 */
                    {
						cmdImage.Address = BitConverter.ToUInt16(byteImage, 3);

						// チェックサムチェック
                        byte orgFcs = byteImage[6];
                        byte calcFcs = CalcChecksum(byteImage, 6);

                        // チェックサムエラー
                        if (orgFcs != calcFcs)
                        {
                            throw new ArgumentException("チェックサム異常(R)");
                        }

                        cmdImage.size = byteImage[5];
                    }
                    break;

                case 'w': /* 書き込みAck  */
                    {
                        // チェックサムチェック
                        byte orgFcs = byteImage[3];
                        byte calcFcs = CalcChecksum(byteImage, 3);

                        // チェックサムエラー
                        if (orgFcs != calcFcs)
                        {
                            throw new ArgumentException("チェックサム異常(w)");
                        }
                    }
                    break;

                case 'W': /* 書き込み要求 */
                case 'r': /* 読み込みAck */
                    {
						cmdImage.Address = BitConverter.ToUInt16(byteImage, 3);

						byte count = (byte)((byteImage[1] - 3) / 2);       /* データ数 */
                        int n = 5;                              /* データ開始位置  */

                        // バイト数をセット
                        cmdImage.size = count;

                        // チェックサムチェック
                        byte orgFcs = byteImage[byteImage.Length - 2];
                        byte calcFcs = CalcChecksum(byteImage, byteImage.Length - 2);

                        // チェックサムエラー
                        if (orgFcs != calcFcs)
                        {
                            throw new ArgumentException("チェックサム異常(W)");
                        }

                        // データ部分をデコード
                        cmdImage.Data = new ushort[count];
                        for (int i = 0; i < count; i++)
                        {
                            cmdImage.Data[i] = BitConverter.ToUInt16(byteImage, n);
                            n += 2;
                        }
                    }
                    break;
            }

 
            return cmdImage;
        }
    }
}
