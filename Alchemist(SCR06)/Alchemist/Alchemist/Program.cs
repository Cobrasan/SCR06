using System;
using System.Threading;
using System.Windows.Forms;

namespace Alchemist
{
    static class Program
    {
		// グローバル変数
		static public mainfrm MainForm;												/* メインフォーム */
		static public SystemDataStorage SystemData = new SystemDataStorage();		/* システムデータ */
        static public DataController DataController = null;                         /* データコントローラ */
        static public bool Initialized = false;                                     /* 同期済みかどうか？ */
        static public SCR06DBController SCR06DB;                                    /* SQLサーバーコントローラ */                                                               

        // メモリ割当データをチェックする
        private static bool checkMemAllocFile()
        {
            MemoryAllocationData memalloc = new MemoryAllocationData();

            try
            {
                memalloc.Load();
            }
            catch
            {
                // 読めない場合→Initializeでエラーとなる。
                return true;
            }

            // 初回起動時
            if (SystemData.machineid == "")
            {
                return true;
            }

            // 2回目以降
            // 前回と機種が同じ
            if (memalloc.GetMachineName() == SystemData.machineid)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// COM設定の初期化
        /// </summary>
        /// <returns></returns>
        private static int ComInit()
        {
            int portno = 0;
            try
            {
                portno = Int32.Parse(Program.SystemData.comport.Substring(3));
            }
            catch
            {
                /* 無視する */
            }

            // COM設定
            return Program.DataController.ComConfigure(
                portno,
                Program.SystemData.borate,
                Program.SystemData.dataBits,
                Program.SystemData.stopBits,
                Program.SystemData.parity,
                Program.SystemData.handshake
            );
        }

        /// <summary>
        /// 接続同期処理
        /// </summary>
        /// <returns></returns>
        static public int Connect()
        {
            int ret;


            // 初期化済みの場合
            if (Initialized)
            {
                // COMの初期化
                ret = ComInit();
                if (ret != SystemConstants.DCPF_SUCCESS)
                {
                    return ret;
                }

                // PortOpenを行う
                // シリアル接続する
                ret = Program.DataController.MachineConnect(SystemConstants.MACHINE_CONNECT);
                if (ret != SystemConstants.DCPF_SUCCESS)
                {
                    return ret;
                }
            }
            // 初期化済みでない場合
            else
            {
                // キューの初期化
                ret = Program.DataController.Initialize();
                if (ret != SystemConstants.DCPF_SUCCESS)
                {
                    Utility.ShowErrorCode(ret);
                    Application.Exit();
                    return ret;
                }

                // COMの初期化
                ret = ComInit();
                if (ret != SystemConstants.DCPF_SUCCESS)
                {
                    return ret;
                }

                // PortOpenを行う
                // シリアル接続する
                ret = Program.DataController.MachineConnect(SystemConstants.MACHINE_CONNECT);
                if (ret != SystemConstants.DCPF_SUCCESS)
                {
                    return ret;
                }

                // データ同期を行う
                syncroprogressfrm dialog = new syncroprogressfrm();
                try
                {
                    DialogResult result = dialog.ShowDialog(MainForm);

                    // キャンセルされた場合
                    if (result == DialogResult.No)
                    {
                        return dialog.Result;
                    }

                    Initialized = true;
                }
                finally
                {
                    dialog.Dispose();
                }
            }

            return SystemConstants.DCPF_SUCCESS;
        }

        private static bool checkConnectSCR06DB()
        {
            try
            {
                SQLServerConnectionStruct cs = new SQLServerConnectionStruct();
                cs.MachineName = SystemData.sqlserver_machinename;
                cs.DataBaseName = SystemData.sqlserver_databasename;
                cs.UserId = SystemData.sqlserver_userid;
                cs.Password = SystemData.sqlserver_password;
                SCR06DB = new SCR06DBController(cs);

                if (SCR06DB.dbConnectTest() == SystemConstants.ERR_SQL_CONNECT)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG037);
                    cs.MachineName = Environment.MachineName;                                      
                    SCR06DB = new SCR06DBController(cs);
                    if (SCR06DB.dbConnectTest() == SystemConstants.ERR_SQL_CONNECT)
                    {
                        return true;
                    }                    
                }
            }
            catch
            {
                return true;
            }

            SCR06DB.ServerOnline = true;
            return false;
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US", true);

                // アプリの多重起動を防止
                using (Mutex mutex = new Mutex(false, SystemConstants.MUTEX_NAME))
                {
                    // 既に起動している場合は、アプリの起動を中断
                    if (mutex.WaitOne(0, false) == false)
                    {
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG004);
                        return;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // System.xmlの読み取り
                    try
                    {
                        SystemData.Load();
                    }
                    catch
                    {
                        // 読み取れぬ場合は、エラー終了
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG013);
                        return;
                    }

                    // カルチャ情報の設定
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(SystemData.culture, true);

                    // リモートセッションの場合は、アプリケーションを終了する
                    if (Utility.IsRemoteSession())
                    {
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG005);
                        return;
                    }

                    // メモリ割当の機種チェック
                    if (checkMemAllocFile() == false)
                    {
                        // ダイアログを表示し、NOが選択された場合は、アプリを終了する。
                        if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG006) == false)
                        {
                            return;
                        }
                    }

                    // SCR06DBのデータセットを作成
                    if (checkConnectSCR06DB() == true)
                    {
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG049);
                        return;
                    }

                    // メインフォームの作成
                    MainForm = new mainfrm();

                    // データコントローラを初期化する。
                    DataController = new DataController(MainForm);

                    // データコントローラの初期化に失敗した場合は、異常終了する。
                    int ret = DataController.Initialize();
                    if (ret != SystemConstants.DCPF_SUCCESS)
                    {
                        Utility.ShowErrorCode(ret);
                        DataController.Dispose();
                        return;
                    }

                    // バージョン情報の取得→ソフト名と自動機名の表示
                    //MainForm.Text = string.Format("{0} V{1} for {2}", Utility.AssemblyProduct, Utility.AssemblyVersion, DataController.GetMachineName());
                    MainForm.Text = string.Format("{0} for {1}", Utility.AssemblyProduct, DataController.GetMachineName());

                    // 接続
                    ret = Connect();
                    if (ret == SystemConstants.ERR_SYNC_CANCELLED)
                    {
                        DataController.Dispose();
                        return;
                    }
                    // 補正値データに書き込みが行えなかった場合
                    else if (ret == SystemConstants.ERR_CORRECT_FILE_ERROR)
                    {
                        Utility.ShowErrorCode(ret);
                        DataController.Dispose();
                        return;
                    }

                    // アプリケーション起動
                    Application.Run(MainForm);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // マルチセッション中の多重起動の場合
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG004);
            }
        }
    }
}