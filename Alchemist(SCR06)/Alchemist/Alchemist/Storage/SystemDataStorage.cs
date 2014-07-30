using System;
using System.Linq;
using System.Xml.Linq;

namespace Alchemist
{
    public class SystemDataException : Exception
    {
        public SystemDataException()
            : base()
        {

        }
    }

    public class SystemDataStorage
    {
        private static XMLAccessor xmlAccessor = new SystemXMLAccessor();

        public int borate = SystemConstants.BORATE_19200;               // ボーレート
        public int dataBits = SystemConstants.DATABIT_8;                // データビット
        public int stopBits = SystemConstants.STOPBIT_2;                // ストップビット
        public int parity = SystemConstants.PARITY_NONE;                // パリティ
        public int handshake = SystemConstants.FLOW_NONE;               // フロー制御
		public string culture = "ja-JP";								// カルチャ情報
		public string machineoperation = "machine";						// 機械操作情報
		private string _password = "";									// パスワード情報
		public string comport = "COM1";                                 // COMポート
        public string machineid = "";                                   // ID
        public bool tachpanel = false;                                  // タッチパネル使用(0:OFF/1:ON)
        public string sqlserver_machinename = "";                       // SQLサーバー接続先PC名
        public string sqlserver_databasename = "";                      // SQLサーバーデータベース名
        public string sqlserver_userid = "";                            // SQLサーバーユーザー名
        public string sqlserver_password = "";                          // SQLサーバーパスワード

        // パスワードプロパティ
        public string password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        /// <summary>
        /// クラスに保持されているシステムデータ値を”system.xml”XMLファイルにセーブします。
        /// ファイルにアクセスする際は、世代管理を行います。
        /// ファイルに書き込みができない場合、ファイルが見つからない場合は、例外を発生させます。
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            string str_borate = "";                 // ボーレート
            string str_dataBits = "";               // データビット
            string str_stopBits = "";               // ストップビット
            string str_parity = "";                 // パリティ
            string str_handshake = "";              // フロー制御
            string str_tachpanel = "";              // タッチパネル使用有無

            // ファイルパス取得
            string xmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\JAM\\Alchemist\\system.xml";

            // 通信設定を読み込む
            // XMLファイルを読む
            var nodes = from n in xmlAccessor.document.Elements("system").Elements("communication")
                        select n;
            XElement communication;

            communication = nodes.First();

            //定数の値を数値に戻して、文字列で格納
            //ボーレートの置き換え
            switch (borate)
            {
                case SystemConstants.BORATE_4800:
                    str_borate = "4800";
                    break;

                case SystemConstants.BORATE_9600:
                    str_borate = "9600";
                    break;

                case SystemConstants.BORATE_14400:
                    str_borate = "14400";
                    break;

                case SystemConstants.BORATE_19200:
                    str_borate = "19200";
                    break;

                case SystemConstants.BORATE_38400:
                    str_borate = "38400";
                    break;

                case SystemConstants.BORATE_57600:
                    str_borate = "57600";
                    break;

                case SystemConstants.BORATE_115200:
                    str_borate = "115200";
                    break;
            }
            //データビットの置き換え
            switch (dataBits)
            {
                case SystemConstants.DATABIT_7:
                    str_dataBits = "7";
                    break;

                case SystemConstants.DATABIT_8:
                    str_dataBits = "8";
                    break;
            }
            //ストップビットの置き換え
            switch (stopBits)
            {
                case SystemConstants.STOPBIT_1:
                    str_stopBits = "1";
                    break;

                case SystemConstants.STOPBIT_2:
                    str_stopBits = "2";
                    break;
            }
            //パリティの置き換え
            switch (parity)
            {
                case SystemConstants.PARITY_NONE:
                    str_parity = "none";
                    break;

                case SystemConstants.PARITY_EVEN:
                    str_parity = "even";
                    break;

                case SystemConstants.PARITY_ODD:
                    str_parity = "odd";
                    break;
            }
            //フロー制御の置き換え
            switch (handshake)
            {
                case SystemConstants.FLOW_NONE:
                    str_handshake = "none";
                    break;

                case SystemConstants.FLOW_HARD:
                    str_handshake = "hard";
                    break;

                case SystemConstants.FLOW_XONXOFF:
                    str_handshake = "xonxoff";
                    break;
            }          

			// COMポートを設定する
			communication.SetAttributeValue("comport", comport);

            // ボーレートを設定する
            communication.SetAttributeValue("borate", str_borate);
            // データビットを設定する
            communication.SetAttributeValue("databit", str_dataBits);
            // ストップビットを設定する
            communication.SetAttributeValue("stopbit", str_stopBits);
            // パリティを設定する
            communication.SetAttributeValue("parity", str_parity);
            // フロー制御を設定する
            communication.SetAttributeValue("flow", str_handshake);

			// カルチャ情報を設定する
			xmlAccessor.document.Root.Element("locale").Attribute("culture").Value = culture;

			// 機械操作情報を設定する
			xmlAccessor.document.Root.Element("machineoperation").Attribute("type").Value = machineoperation;

			// パスワードを設定する
			xmlAccessor.document.Root.Element("password").Attribute("value").Value = password;

            // マシンIDを設定する
            xmlAccessor.document.Root.Element("machineid").Attribute("value").Value = machineid;

            // タッチパネル使用有無設定
            switch (tachpanel)
            {
                case SystemConstants.TACHPANEL_OFF:
                    str_tachpanel = "disable";
                    break;
                case SystemConstants.TACHPANEL_ON:
                    str_tachpanel = "enable";
                    break;　                   
            }
            xmlAccessor.document.Root.Element("tachpanel").Attribute("value").Value = str_tachpanel;

            // SQLサーバー接続先PC名を設定する
            xmlAccessor.document.Root.Element("sqlserver").Attribute("machinename").Value = sqlserver_machinename;
            // SQLサーバーデータベース名を設定する
            xmlAccessor.document.Root.Element("sqlserver").Attribute("databasename").Value = sqlserver_databasename;
            // SQLサーバーユーザーIDを設定する
            xmlAccessor.document.Root.Element("sqlserver").Attribute("userid").Value = sqlserver_userid;
            // SQLサーバーパスワードを設定する
            xmlAccessor.document.Root.Element("sqlserver").Attribute("password").Value = sqlserver_password;

            // XMLファイルにセーブする
            try
            {
                // XMLファイルを開く
                xmlAccessor.SaveXmlFile(xmlFileName);

            }
            finally {
                Utility.DeleteBackupFile(xmlFileName);
            }
        }

        /// <summary>
        /// system.xmlからシステムデータを読み込み、クラス内に値を保持します。
        /// ファイルにアクセスする際は、世代管理を行います。
        /// ファイルが見つからない場合、読み込みができない場合は、例外を発生させます。
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            string str_borate = "";                 // ボーレート
            string str_dataBits = "";               // データビット
            string str_stopBits = "";               // ストップビット
            string str_parity = "";                 // パリティ
            string str_handshake = "";              // フロー制御
            string str_tachpanel = "";              // タッチパネル使用有無

            // ファイルパス取得
            string xmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\JAM\\Alchemist\\system.xml";

            try
            {
                // XMLファイルを開く
                xmlAccessor.LoadXmlFile(xmlFileName);

            }
            catch (Exception)
            {
                // ファイルが見つからなかった場合例外を発生させる
                throw;
            }

            // 通信設定を読み込む
            // XMLファイルを読む
            var nodes = from n in xmlAccessor.document.Elements("system").Elements("communication")
                        select n;
            XElement communication;

            communication = nodes.First();

			// COMポートを取得する
			comport = communication.Attribute("comport").Value;

            // ボーレートを読み出し、変数に代入する
            str_borate = communication.Attribute("borate").Value;
            // データビットを読み出し、変数に代入する
            str_dataBits = communication.Attribute("databit").Value;
            // ストップビットを読み出し、変数に代入する
            str_stopBits = communication.Attribute("stopbit").Value;
            // パリティを読み出し、変数に代入する
            str_parity = communication.Attribute("parity").Value;
            // フロー制御を読み出し、変数に代入する
            str_handshake = communication.Attribute("flow").Value;


            //各種通信設定変数の内容に応じて定数に置き換える
            //ボーレートの置き換え
            switch (str_borate)
            {
                case "4800":
                    borate = SystemConstants.BORATE_4800;
                    break;

                case "9600":
                    borate = SystemConstants.BORATE_9600;
                    break;

                case "14400":
                    borate = SystemConstants.BORATE_14400;
                    break;

                case "19200":
                    borate = SystemConstants.BORATE_19200;
                    break;

                case "38400":
                    borate = SystemConstants.BORATE_38400;
                    break;

                case "57600":
                    borate = SystemConstants.BORATE_57600;
                    break;

                case "115200":
                    borate = SystemConstants.BORATE_115200;
                    break;
            }
            //データビットの置き換え
            switch (str_dataBits)
            {
                case "7":
                    dataBits = SystemConstants.DATABIT_7;
                    break;

                case "8":
                    dataBits = SystemConstants.DATABIT_8;
                    break;
            }
            //ストップビットの置き換え
            switch (str_stopBits)
            {
                case "1":
                    stopBits = SystemConstants.STOPBIT_1;
                    break;

                case "2":
                    stopBits = SystemConstants.STOPBIT_2;
                    break;
            }
            //パリティの置き換え
            switch (str_parity)
            {
                case "none":
                    parity = SystemConstants.PARITY_NONE;
                    break;

                case "even":
                    parity = SystemConstants.PARITY_EVEN;
                    break;

                case "odd":
                    parity = SystemConstants.PARITY_ODD;
                    break;
            }
            //フロー制御の置き換え
            switch (str_handshake)
            {
                case "none":
                    handshake = SystemConstants.FLOW_NONE;
                    break;

                case "hard":
                    handshake = SystemConstants.FLOW_HARD;
                    break;

                case "xonxoff":
                    handshake = SystemConstants.FLOW_XONXOFF;
                    break;
            }

			// カルチャ情報の取得
			culture = xmlAccessor.document.Root.Element("locale").Attribute("culture").Value;

			// 機械同期情報の取得
			machineoperation = xmlAccessor.document.Root.Element("machineoperation").Attribute("type").Value;

			// パスワード情報の取得
			password = xmlAccessor.document.Root.Element("password").Attribute("value").Value;

            // マシンID情報の取得
            machineid = xmlAccessor.document.Root.Element("machineid").Attribute("value").Value;

            // タッチパネル使用有無情報の取得
            str_tachpanel = xmlAccessor.document.Root.Element("tachpanel").Attribute("value").Value;
            switch (str_tachpanel)
            {
                case "enable":
                    tachpanel = true;
                    break;
                default:
                    tachpanel = false;
                    break;
            }

            // SQLサーバー接続先PC名情報の取得
            sqlserver_machinename = xmlAccessor.document.Root.Element("sqlserver").Attribute("machinename").Value;
            // SQLサーバーデータベース名情報の取得
            sqlserver_databasename = xmlAccessor.document.Root.Element("sqlserver").Attribute("databasename").Value;
            // SQLサーバーユーザーID情報の取得
            sqlserver_userid = xmlAccessor.document.Root.Element("sqlserver").Attribute("userid").Value;
            // SQLサーバーパスワード情報の取得
            sqlserver_password = xmlAccessor.document.Root.Element("sqlserver").Attribute("password").Value;
        }
    }
}
