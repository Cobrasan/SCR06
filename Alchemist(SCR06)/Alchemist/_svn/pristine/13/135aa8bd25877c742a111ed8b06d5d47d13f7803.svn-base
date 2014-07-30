using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class mainfrm : Form
    {
        // スレッド
        private Thread thread = null;
        delegate void RefreshDelegate();
        // テンキー
        private TenkeyControl tenkey = null;

        // 表示更新スレッド
        private void monitorRefreshThread()
        {
            while (true)
            {
                // UIスレッドでrefresh関数を実行させる
                Invoke(new RefreshDelegate(refresh));

                // カウンタフォームの表示更新
                if (counterForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(counterForm.refresh));
                }

                // 段取り操作の表示更新
                if (setupOperationForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(setupOperationForm.refresh));
                }

                // 通信画面の表示更新
                if (connectOperationForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(connectOperationForm.refresh));
                }

                // 情報画面の表示更新
                Invoke(new RefreshDelegate(errInfoMsgForm.refresh));

                // 加工詳細設定画面(電線切断)の表示更新
                if (workDetailItemFormwire1.Visible == true)
                {
                    Invoke(new RefreshDelegate(workDetailItemFormwire1.refresh));
                }

                // 加工詳細設定画面(ストリップ1)の表示更新
                if (workDetailItemFormstrip1.Visible == true)
                {
                    Invoke(new RefreshDelegate(workDetailItemFormstrip1.refresh));
                }

                // 加工詳細設定画面(ストリップ2)の表示更新
                if (workDetailItemFormstrip2.Visible == true)
                {
                    Invoke(new RefreshDelegate(workDetailItemFormstrip2.refresh));
                }

                // 速度設定画面の表示更新
                if (workDetailSpeedForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(workDetailSpeedForm.refresh));
                }

                // 加工動作詳細設定画面の表示更新
                if (workDetailMotionForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(workDetailMotionForm.refresh));
                }

                // 段取り操作画面の表示更新
                if (setupOperationForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(setupOperationForm.refresh));
                }

                // システム操作画面の表示更新
                if (systemConfigrationForm.Visible == true)
                {
                    Invoke(new RefreshDelegate(systemConfigrationForm.refresh));
                }
               
                Thread.Sleep(100);
            }
        }

        // 子フォーム群
        private counterfrm counterForm = new counterfrm();			                	            /* カウンタ詳細画面 */
        private setupOperationfrm setupOperationForm = new setupOperationfrm();	    	            /* 段取り操作画面 */
        private workDetailItemfrmWIRE1 workDetailItemFormwire1 = new workDetailItemfrmWIRE1();      /* 加工値詳細設定画面(電線切断) */
        private workDetailItemfrmSTRIP1 workDetailItemFormstrip1 = new workDetailItemfrmSTRIP1();   /* 加工値詳細設定画面(1側ストリップ) */
        private workDetailItemfrmSTRIP2 workDetailItemFormstrip2 = new workDetailItemfrmSTRIP2();   /* 加工値詳細設定画面(2側ストリップ) */
        private workDetailSpeedfrm workDetailSpeedForm = new workDetailSpeedfrm();                  /* 速度設定画面 */
        private workDetailMotionfrm workDetailMotionForm = new workDetailMotionfrm();               /* 加工動作詳細画面 */
        private machineOperationfrm machineOperationForm = new machineOperationfrm();               /* 機械操作画面 */
        private bankOperationfrm bankOperationForm = new bankOperationfrm();                        /* バンク操作画面 */
        private systemConfigurationfrm systemConfigrationForm = new systemConfigurationfrm();       /* システム設定画面 */
        private connectOperationfrm connectOperationForm = new connectOperationfrm();			    /* 接続・切断画面 */
        private errInfoMsgfrm errInfoMsgForm = new errInfoMsgfrm();                                 /* 情報・エラーメッセージ画面 */
        private passwordCollationfrm passwordcollationForm = new passwordCollationfrm();            /* パスワード照合画面 */
        private AboutBox1 aboutboxForm = new AboutBox1();                                           /* 情報ボックス画面 */
        private iocheckfrm iocheckForm = new iocheckfrm();                                          /* メモリモニタ画面 */
        private wireselectfrm wireselectForm = new wireselectfrm();                                 /* 電線選択画面 */
        private operatorfrm operatorForm = new operatorfrm();                                       /* 作業者照会画面 */
        private fdatafrm fdataForm = new fdatafrm();                                                /* エフデータ照会画面 */
        private qualityRecordfrm qualityRecordForm = new qualityRecordfrm();                        /* 品質記録画面 */
        private wireConfirmfrm wireConfirmForm = new wireConfirmfrm();                              /* 電線照合画面 */

        // 加工動作画面設定table
        private Dictionary<int, workMotionStruct> map = new Dictionary<int, workMotionStruct>();
      
        // 加工動作Visible設定構造体
        private struct workMotionStruct
        {
            public Image image;
            public bool wire_Length;
            public bool strip_Length;
            public bool strip_Hogusi;
        };

        // 1側のボタンIDの配列
        private int[] btnIdArray1 = new int[]{
                SystemConstants.STRIP1_BTN,
            };

        // 2側のボタンIDの配列
        private int[] btnIdArray2 = new int[]{
                SystemConstants.STRIP2_BTN,
            };

        // コンストラクタ
        public mainfrm()
        {
            InitializeComponent();
        }

        // 初期化処理
        private void Initialize()
        {
            workMotionStruct[] workMtnStruct = new workMotionStruct[14];

            // フォームの初期化
            aboutboxForm.Initialize();
            bankOperationForm.Initialize();
            connectOperationForm.Initialize();
            counterForm.Initialize();
            errInfoMsgForm.Initialize();
            iocheckForm.Initialize();
            machineOperationForm.Initialize();
            passwordcollationForm.Initialize();
            setupOperationForm.Initialize();
            systemConfigrationForm.Initialize();
            workDetailItemFormwire1.Initialize();
            workDetailItemFormstrip1.Initialize();
            workDetailItemFormstrip2.Initialize();
            workDetailMotionForm.Initialize();
            workDetailSpeedForm.Initialize();
            wireselectForm.Initialize();
            operatorForm.Initialize();
            qualityRecordForm.Initialize();
            wireConfirmForm.Initialize();

            // イベントの初期化
            SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.QTY_SET_COUNTER1, textQTY);
            SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.LOT_SET_COUNTER1, textLOT);
            SetTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH1, textWIRE_LENGTH_VALUE, SystemConstants.BANKITEM_TYPE_WIRELENGTH);
            SetTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH1, textSTRIP1_VALUE, SystemConstants.BANKITEM_TYPE_STRIP1);
            SetTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH2, textSTRIP2_VALUE, SystemConstants.BANKITEM_TYPE_STRIP2);

            SetBtnEvent(SystemConstants.STRIP1_BTN, SystemConstants.BTN_PUSH, btnSTRIP1);
            SetBtnEvent(SystemConstants.STRIP2_BTN, SystemConstants.BTN_PUSH, btnSTRIP2);
            SetBtnEvent(SystemConstants.NORMAL_BTN, SystemConstants.BTN_ON, btnNORMAL);
            SetBtnEvent(SystemConstants.EJECT_BTN, SystemConstants.BTN_ON, btnEJECT);
            SetBtnEvent(SystemConstants.SAMPLE_BTN, SystemConstants.BTN_ON, btnSAMPLE);
            SetBtnEvent(SystemConstants.TEST_BTN, SystemConstants.BTN_ON, btnTEST);
            SetBtnEvent(SystemConstants.FREE_BTN, SystemConstants.BTN_ON, btnFREE);
            SetBtnEvent(SystemConstants.JOG_BTN, SystemConstants.BTN_ON, btnJOG);
            SetBtnEvent(SystemConstants.CYCLE_BTN, SystemConstants.BTN_ON, btnCYCLE);
            SetBtnEvent(SystemConstants.AUTO_BTN, SystemConstants.BTN_ON, btnAUTO);
            SetBtnEvent(SystemConstants.LOT_INTERVAL1_BTN, SystemConstants.BTN_PUSH, btnAUTOEXIT);

            // タッチパネル使用時
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.QTY_SET_COUNTER1, textQTY);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.LOT_SET_COUNTER1, textLOT);
            ClickTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH1, textWIRE_LENGTH_VALUE, SystemConstants.BANKITEM_TYPE_WIRELENGTH);
            ClickTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH1, textSTRIP1_VALUE, SystemConstants.BANKITEM_TYPE_STRIP1);
            ClickTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH2, textSTRIP2_VALUE, SystemConstants.BANKITEM_TYPE_STRIP2);
            
            // テンキーからの入力完了イベント
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);         

            // 加工動作が1側のぶつ切りの場合の設定
            workMtnStruct[0].image = Alchemist.Properties.Resources.Fig1_0;
            workMtnStruct[0].wire_Length = true;
            workMtnStruct[0].strip_Length = false;
            workMtnStruct[0].strip_Hogusi = false;

            // 加工動作が2側のぶつ切りの場合の設定
            workMtnStruct[1].image = Alchemist.Properties.Resources.Fig2_0;
            workMtnStruct[1].wire_Length = true;
            workMtnStruct[1].strip_Length = false;
            workMtnStruct[1].strip_Hogusi = false;

            // 加工動作が1側のストリップの場合の設定
            workMtnStruct[2].image = Alchemist.Properties.Resources.Fig1_1;
            workMtnStruct[2].wire_Length = true;
            workMtnStruct[2].strip_Length = true;
            workMtnStruct[2].strip_Hogusi = true;

            // 加工動作が2側のストリップの場合の設定
            workMtnStruct[3].image = Alchemist.Properties.Resources.Fig2_1;
            workMtnStruct[3].wire_Length = true;
            workMtnStruct[3].strip_Length = true;
            workMtnStruct[3].strip_Hogusi = true;

            map.Add(0, workMtnStruct[0]);       // 1側のぶつ切りのテーブルを設定
            map.Add(1, workMtnStruct[1]);       // 2側のぶつ切りのテーブルを設定
            map.Add(2, workMtnStruct[2]);       // 1側のストリップのテーブルを設定
            map.Add(3, workMtnStruct[3]);       // 2側のストリップのテーブルを設定

            // フォーム内のテキストボックスの長さを10にする。
            SetTextBoxLength(this, 10);

            // 電線エラー発生イベント
            errInfoMsgForm.ErrEventHandler += new EventHandler(errInfoMsgfrm_OnWireError);
            errInfoMsgForm.InfoEventHandler += new EventHandler(errInfoMsgfrm_OnProductComplete);

            // 描画スレッドを開始する
            thread = new Thread(new ThreadStart(monitorRefreshThread));
            thread.Start();
        }


        // メインフォーム初期化
        private void mainfrm_Load(object sender, EventArgs e)
        {
            Initialize();

            // 起動時、作業中の作業者の有無を確認して登録画面を表示
            RegisterOperator();
        }

        // フォームが閉じられた時の処理
        private void mainfrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 表示処理を先に止める
            if (thread != null)
            {
                // スレッドを停止する
                thread.Abort();
                thread.Join();
            }

            // 機種名を保存する
            Program.SystemData.machineid = Program.DataController.GetMachineName();

            try
            {
                Program.SystemData.Save();
            }
            catch
            {
                /* 例外を無視 */
            }

            // フォームを破棄する
            counterForm.Dispose();
            setupOperationForm.Dispose();
            workDetailItemFormwire1.Dispose();
            workDetailItemFormstrip1.Dispose();
            workDetailItemFormstrip2.Dispose();
            workDetailSpeedForm.Dispose();
            workDetailMotionForm.Dispose();
            machineOperationForm.Dispose();
            bankOperationForm.Dispose();
            systemConfigrationForm.Dispose();
            connectOperationForm.Dispose();
            errInfoMsgForm.Dispose();
            passwordcollationForm.Dispose();
            wireselectForm.Dispose();
            operatorForm.Dispose();
            aboutboxForm.Dispose();
            fdataForm.Dispose();
            qualityRecordForm.Dispose();

            // データコントローラを解放する
            Program.DataController.Dispose();
            Dispose();
        }

        // 描画処理
        private void refresh()
        {
            int selectedNo = 0;
            string bankComment = "";
            string bankWireName = "";
            string bankWireLength = "";
            string bankStrip1 = "";
            string bankStrip2 = "";

            // カウンタのWorkIDを取得
            refreshControl(SystemConstants.TOTAL_COUNTER1, lblTOTAL2);

            // QTYのWorkIDを取得
            refreshControl(SystemConstants.QTY_COUNTER1, lblQTY2);

            // LOTのWorkIDを取得
            refreshControl(SystemConstants.LOT_COUNTER1, lblLOT2);

            // タクト1のWorkIDを取得
            refreshControl(SystemConstants.MACHINE_TACT1, lblTact4);

            // 切断長のWorkIDを取得
            //refreshControl(SystemConstants.WIRE_LENGTH1, textWIRE_LENGTH_VALUE);          

            // シース剥ぎAのWorkIDを取得
            //refreshControl(SystemConstants.STRIP_LENGTH1, textSTRIP1_VALUE);

            // シース剥ぎBのWorkIDを取得
            //refreshControl(SystemConstants.STRIP_LENGTH2, textSTRIP2_VALUE);

            // QTY設定のWorkIDを取得
            refreshControl(SystemConstants.QTY_SET_COUNTER1, textQTY);

            // LOT設定のWorkIDを取得
            refreshControl(SystemConstants.LOT_SET_COUNTER1, textLOT);

#if DEBUG
            lblSetQuant.Visible = true;
            lblSetLot.Visible = true;

            // ↓デバッグ用↓
            // QTY設定のWorkIDを取得
            refreshControl(SystemConstants.D_QTY_SET_COUNTER, lblSetQuant);

            // LOT設定のWorkIDを取得
            refreshControl(SystemConstants.D_LOT_SET_COUNTER, lblSetLot);
            //　↑デバッグ用↑
#endif

            // 接続状態の取得
            if (Program.DataController.IsConnect() == true)
            {
                // true: 背景Green 文字色 White 文字 ONLINE
                lblOFFLINE.Text = Utility.GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG029);
                lblOFFLINE.BackColor = System.Drawing.Color.Green;
                lblOFFLINE.ForeColor = System.Drawing.Color.White;
                panelOFFLINE.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                // false: 背景 Red 文字色 Black 文字 OFFLINE
                lblOFFLINE.Text = Utility.GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG030);
                lblOFFLINE.BackColor = System.Drawing.Color.Red;
                lblOFFLINE.ForeColor = System.Drawing.Color.Black;
                panelOFFLINE.BackColor = System.Drawing.Color.Red;
            }

            // ステータスによってシース剥ぎAボタンの画像を変更
            CheckBtnAnd_ChangePicture(SystemConstants.STRIP1_BTN, btnSTRIP1, Alchemist.Properties.Resources.StripAON, Alchemist.Properties.Resources.StripAOFF);

            // ステータスによってシース剥ぎBボタンの画像を変更
            CheckBtnAnd_ChangePicture(SystemConstants.STRIP2_BTN, btnSTRIP2, Alchemist.Properties.Resources.StripBON, Alchemist.Properties.Resources.StripBOFF);

            // 加工動作の画面設定を行う
            workMotionDisplay();

            // 動作表示
            CheckBtnAnd_ChangeColor_Label(SystemConstants.STRIP1_SENSOR_LOCK, lblSTRIPMISS1, SystemConstants.BTN_ONOFF_REVERSE);
            CheckBtnAnd_ChangeColor_Label(SystemConstants.PERMIT_COUNTUP_BTN, lblPERMIT_COUNTUP);
            CheckBtnAnd_ChangeColor_Label(SystemConstants.WIRE_DISENTANGLE_BTN, lblDISENTANGLE);
            CheckBtnAnd_ChangeColor_Label(SystemConstants.CUT_WIRETOP_BTN, lblCUTWIRETOP);

            // 動作モード・サイクルモード、ティーチング、自動復帰ボタン
            CheckBtnAnd_ChangeColor(SystemConstants.NORMAL_BTN, btnNORMAL);
            CheckBtnAnd_ChangeColor(SystemConstants.EJECT_BTN, btnEJECT);
            CheckBtnAnd_ChangeColor(SystemConstants.SAMPLE_BTN, btnSAMPLE);
            CheckBtnAnd_ChangeColor(SystemConstants.TEST_BTN, btnTEST);
            CheckBtnAnd_ChangeColor(SystemConstants.FREE_BTN, btnFREE);
            CheckBtnAnd_ChangeColor(SystemConstants.JOG_BTN, btnJOG);
            CheckBtnAnd_ChangeColor(SystemConstants.CYCLE_BTN, btnCYCLE);
            CheckBtnAnd_ChangeColor(SystemConstants.AUTO_BTN, btnAUTO);
            CheckBtnAnd_ChangeColor(SystemConstants.LOT_INTERVAL1_BTN, btnAUTOEXIT);

            // 現在選択されているBankNoを取得
            Program.DataController.BankNoRead(ref selectedNo);

            // バンクコメントを取得
            Program.DataController.BankDataCommentRead(selectedNo, ref bankComment);

            lblWIRECODE.Text = bankComment;
            /*if (!textBankComment.Focused)
            {
                // メイン画面のbankcommentを設定
                textBankComment.Text = bankComment;
            }*/

            // バンク：電線名を取得
            ReadBankDataItem(selectedNo, ref bankWireName, SystemConstants.BANKITEM_TYPE_WIRENAME);
            lblWIRENAME.Text = bankWireName;
            /*if (!textWIRE_NAME.Focused)
            {
                // メイン画面のtextWIRE_NAMEを設定
                textWIRE_NAME.Text = bankWireName;
            }*/

            // バンク：切断長を取得
            ReadBankDataItem(selectedNo, ref bankWireLength, SystemConstants.BANKITEM_TYPE_WIRELENGTH);

            if (!textWIRE_LENGTH_VALUE.Focused)
            {
                // メイン画面のtextWIRE_LENGTH_VALUEを設定
                textWIRE_LENGTH_VALUE.Text = bankWireLength;
            }

            // バンク：シース剥ぎAを取得
            ReadBankDataItem(selectedNo, ref bankStrip1, SystemConstants.BANKITEM_TYPE_STRIP1);

            if (!textSTRIP1_VALUE.Focused)
            {
                // メイン画面のtextWIRE_LENGTH_VALUEを設定
                textSTRIP1_VALUE.Text = bankStrip1;
            }

            // バンク：シース剥ぎBを取得
            ReadBankDataItem(selectedNo, ref bankStrip2, SystemConstants.BANKITEM_TYPE_STRIP2);

            if (!textSTRIP2_VALUE.Focused)
            {
                // メイン画面のtextWIRE_LENGTH_VALUEを設定
                textSTRIP2_VALUE.Text = bankStrip2;
            }

            // マシン操作設定が本体の場合は、操作ボタンは非表示にする。
            /*if (Program.SystemData.machineoperation == "machine")
            {
                // 操作画面を消す
                if (machineOperationForm.Visible != false)
                {
                    machineOperationForm.Visible = false;
                }

                // パネルも消す
                if (panel28.Visible != false)
                {
                    panel28.Visible = false;
                }
            }
            else
            {
                if (panel28.Visible != true) 
                {
                    panel28.Visible = true;
                }
            }*/

            // 起動時オンラインなら設定本数を再設定
            InitialQtyLotSet();

            // 電線イメージを更新
            WireImageDisplay();

            // 切断長確認
            WireLengthCheck();

            // ストリップショートモード中確認
            ShortStripModeCheck();

            // オンライン情報の表示
            OnlineInfomationDisplay();

            // ガイド・ブレード情報の表示
            GuideBladeInfomationDisplay();

            // 運転中操作不可制御
            OperationControlMachineStatusRun();
        }

        private void VersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutboxForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // WritePushBtnnにて、QTY_COUNTER_RESET1_BTNのBTN_PUSHを実行
            mainfrm.WritePushBtn(SystemConstants.QTY_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH);
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            counterForm.Show();
        }

        private void mainfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
        }

        private void btnSETUPOPERATION_Click(object sender, EventArgs e)
        {
            setupOperationForm.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            workDetailItemFormwire1.Show();
        }

        private void btnSTRIP1_Detail_Click(object sender, EventArgs e)
        {
            workDetailItemFormstrip1.Show();
        }

        private void btnSTRIP2_Detail_Click(object sender, EventArgs e)
        {
            workDetailItemFormstrip2.Show();
        }

        private void btnSpeedsetting_Click(object sender, EventArgs e)
        {
            workDetailSpeedForm.Show();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            workDetailMotionForm.Show();
        }

        private void button38_Click(object sender, EventArgs e)
        {
            machineOperationForm.Show();
        }

        private void btnBANK_Click(object sender, EventArgs e)
        {
            bankOperationForm.Show();
        }

        private void btnManagement_setting_Click(object sender, EventArgs e)
        {
            // パスワードが一致したら、システム設定を開く
            passwordCollationfrm pass = new passwordCollationfrm();
            SCR06DBController db = Program.SCR06DB;

            // パスワードが空の場合表示
            if (string.IsNullOrEmpty(Program.SystemData.password))
            {
                systemConfigrationForm.Show();
            }
            // パスワードが設定されている場合
            else
            {
                // パスワード入力画面で照合を選んだ場合
                if (pass.ShowDialog() == DialogResult.OK)
                {
                    // パスワードが一致 又は、管理者作業者のバーコードが入力
                    if (pass.CheckPassword() == Program.SystemData.password || 
                        db.dbGetSuperVisorOperator(pass.CheckPassword().Remove(0,1)) == SystemConstants.SQL_SUCCESS) //作業者コードの先頭の[A]削除
                    {
                        systemConfigrationForm.Show();
                    }
                    // パスワードが不一致
                    else
                    {
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG017);
                    }
                }
            }
        }

        private void lblOFFLINE_Click(object sender, EventArgs e)
        {
            connectOperationForm.Show();
        }

        private void btnLOTReset_Click(object sender, EventArgs e)
        {
            // WritePushBtnnにて、LOT_COUNTER_RESET1_BTNのBTN_PUSHを実行
            mainfrm.WritePushBtn(SystemConstants.LOT_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH);
        }

        private void workMotionDisplay()
        {
            int key1 = 0;
            int key2 = 0;
            int btnStatus = 0;

            //ハッシュテーブルのkey値算出方法
            //下記の表に対応した値を取得
            //
            //  10000       1000    100        10              1
            //　ハーフ　 ｜ 　　 ｜ 　　｜ 　　　　　 ｜ 1側or　　　　  ｜　　　｜
            //ストリップ ｜ 防水 ｜ 圧着｜ ストリップ ｜ 2側(2側だと○) ｜2進数 ｜10進数
            // ―――――｜―――｜ ――｜――――――｜――――――――｜―――｜―――
            //　　　　　 ｜ 　　 ｜ 　　｜ 　　　　　 ｜　　　　　　　　｜00000 ｜  0
            //　　　　　 ｜ 　　 ｜ 　　｜ 　　　　　 ｜  　　 ○ 　　　｜00001 ｜  1
            //　　　　　 ｜ 　　 ｜ 　　｜ 　　○　　 ｜  　　　　　　　｜00010 ｜  2
            //　　　　　 ｜ 　　 ｜ 　　｜ 　　○　　 ｜  　　 ○ 　　　｜00011 ｜  3
            //　　　　　 ｜ 　　 ｜  ○ ｜ 　　○　　 ｜  　　　　　　　｜00110 ｜  6
            //　　　　　 ｜ 　　 ｜  ○ ｜ 　　○　　 ｜  　　 ○ 　　　｜00111 ｜  7
            //　　　　　 ｜  ○  ｜ 　　｜ 　　　　　 ｜  　　　　　　　｜01000 ｜  8
            //　　　　　 ｜  ○  ｜ 　　｜ 　　　　　 ｜  　　 ○ 　　　｜01001 ｜  9
            //　　　　　 ｜  ○  ｜ 　　｜ 　　○　　 ｜  　　　　　　　｜01010 ｜ 10
            //　　　　　 ｜  ○  ｜ 　　｜ 　　○　　 ｜  　　 ○ 　　　｜01011 ｜ 11
            //　　　　　 ｜  ○  ｜  ○ ｜ 　　○　　 ｜  　　　　　　　｜01110 ｜ 14
            //　　　　　 ｜  ○  ｜  ○ ｜ 　　○　　 ｜  　　 ○ 　　　｜01111 ｜ 15
            //　　○　　 ｜ 　　 ｜ 　　｜ 　　○　　 ｜  　　　　　　　｜10010 ｜ 18
            //　　○　　 ｜ 　　 ｜ 　　｜ 　　○　　 ｜  　　 ○ 　　　｜10011 ｜ 19
            //

            // 1側のkey値算出
            for (int i = 0; i < btnIdArray1.Length; i++)
            {
                Program.DataController.ReadPushBtn(btnIdArray1[i], ref btnStatus);
                if (btnStatus == SystemConstants.BTN_ON)
                {
                    key1 = key1 + (2 << i);
                }
            }

            // 2側のkey値算出
            for (int i = 0; i < btnIdArray2.Length; i++)
            {
                Program.DataController.ReadPushBtn(btnIdArray2[i], ref btnStatus);
                if (btnStatus == SystemConstants.BTN_ON)
                {
                    key2 = key2 + (2 << i);
                }
            }
            // key2に+1する(2側のkey値は1側の+1)
            key2 = key2 + 1;

            try
            {
                pictureBoxSIDE1.Image = map[key1].image;                                    // 1側の画像を設定
                lblWIRE_LENGTH_VALUE.Visible = map[key1].wire_Length;                       // 切断長ラベルのVisibleを設定
                textWIRE_LENGTH_VALUE.Visible = map[key1].wire_Length;                      // 切断長テキストのVisibleを設定
                lblSTRIP1_VALUE.Visible = map[key1].strip_Length;                           // 1側のストリップラベルのVisibleを設定
                textSTRIP1_VALUE.Visible = map[key1].strip_Length;                          // 1側のストリップテキストのVisibleを設定
                btnSTRIP1_Detail.Visible = map[key1].strip_Length;                          // 1側のストリップ詳細ボタンのVisibleを設定

                pictureBoxSIDE2.Image = map[key2].image;                                    // 2側の画像を設定
                lblWIRE_LENGTH_VALUE.Visible = map[key2].wire_Length;                       // 切断長ラベルのVisibleを設定
                textWIRE_LENGTH_VALUE.Visible = map[key2].wire_Length;                      // 切断長テキストのVisibleを設定
                lblSTRIP2_VALUE.Visible = map[key2].strip_Length;                        // 2側のストリップラベルのVisibleを設定
                textSTRIP2_VALUE.Visible = map[key2].strip_Length;                          // 2側のストリップテキストのVisibleを設定
                btnSTRIP2_Detail.Visible = map[key2].strip_Length;                          // 2側のストリップ詳細ボタンのVisibleを設定
            }
            catch
            {
                /* 例外を無視（データの取得タイミングで本来とりうるはずのない組み合わせがきた場合の対策） */
            }

        }

        // メインフォーム表示が切り替わったときの処理
        private void mainfrm_VisibleChanged(object sender, EventArgs e)
        {            
            /*if (Visible == true)
            {
                // QTY設定のテキストボックスに範囲チェック等をイベントを設定
                Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.QTY_SET_COUNTER1, textQTY);

                // LOT設定のテキストボックスに範囲チェック等をイベントを設定
                Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.LOT_SET_COUNTER1, textLOT);

                // 目標値：切断長のテキストボックスに範囲チェック等をイベントを設定
                Program.MainForm.SetTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH1, textWIRE_LENGTH_VALUE, SystemConstants.BANKITEM_TYPE_WIRELENGTH);

                // シース剥ぎAのテキストボックスに範囲チェック等をイベントを設定
                Program.MainForm.SetTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH1, textSTRIP1_VALUE, SystemConstants.BANKITEM_TYPE_STRIP1);

                // シース剥ぎBのテキストボックスに範囲チェック等をイベントを設定
                Program.MainForm.SetTextBoxEventToBank(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH2, textSTRIP2_VALUE, SystemConstants.BANKITEM_TYPE_STRIP2);
            }*/
        }

        private void panel8_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void pictureBoxSIDE1_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void pictureBoxSIDE2_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void textBankComment_EnterKeyDown(EventArgs e)
        {
            int selectedNo = 0;

            // 現在選択されているバンクNoを取得
            Program.DataController.BankNoRead(ref selectedNo);

            // メインフォームのバンクコメントをbankdata.xmlに書き込む
            //mainfrm.BankDataCommentWrite(selectedNo, textBankComment.Text);
                        
            // フォーカスを外す
            ActiveControl = null;
        }

        private void btnWIRE_Select_Click(object sender, EventArgs e)
        {
            wireselectForm.ShowDialog();
        }

        private void btnOPERATOR_Click(object sender, EventArgs e)
        {
            operatorForm.Show();
        }

        private void btnOPERATOR_OUT_Click(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;

            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG033) == true)
            {
                if (db.dbDelTemporaryOperator() == SystemConstants.ERR_TEMP_OPERATOR_DELETE)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG034);
                }
                RegisterOperator();
            }
        }

        /// <summary>
        /// ストリップ長40mmより短い場合、機械側でほぐしと先端カットをロックする（ショートモード）
        /// マシンステータスのモード状態をラベル色で表示する（ON：赤、OFF：灰）
        /// </summary>
        /// <returns></returns>
        private void ShortStripModeCheck()
        {
            int machineStatus = Program.DataController.GetMachineStatus();

            if ((machineStatus & SystemConstants.BIT_SHORT_STRIP1) == 0)
            {
                lblSHORT1.BackColor = Color.Gray;
            }
            else
            {
                lblSHORT1.BackColor = Color.Red;
            }

            if ((machineStatus & SystemConstants.BIT_SHORT_STRIP2) == 0)
            {
                lblSHORT2.BackColor = Color.Gray;
            }
            else
            {
                lblSHORT2.BackColor = Color.Red;
            }
    
        }

        /// <summary>
        /// 切断長の長さ≦A端ストリップ＋B端ストリップ＋330mmのときは警報を発し機械はスタートしない
        /// </summary>
        /// <returns></returns>
        private void WireLengthCheck()
        {            
            double wirelength = 0;
            double strip1 = 0;
            double strip2 = 0;

            // オフライン中と初期化前に行うとエラー発生
            if (Program.DataController.IsConnect() == false || !Program.Initialized) return;

            // データの読み込み
            Program.DataController.ReadWorkData(SystemConstants.WIRE_LENGTH1, ref wirelength);
            Program.DataController.ReadWorkData(SystemConstants.STRIP_LENGTH1, ref strip1);
            Program.DataController.ReadWorkData(SystemConstants.STRIP_LENGTH2, ref strip2);

            // 比較
            if (wirelength <= strip1 + strip2 + 330)
            {
                mainfrm.WritePushBtn(SystemConstants.ALARM_WIRELENGTH, SystemConstants.BTN_ON);

                textWIRE_LENGTH_VALUE.BackColor = Color.Red;
                textSTRIP1_VALUE.BackColor = Color.Red;
                textSTRIP2_VALUE.BackColor = Color.Red;
            }
            else
            {
                mainfrm.WritePushBtn(SystemConstants.ALARM_WIRELENGTH, SystemConstants.BTN_OFF);

                textWIRE_LENGTH_VALUE.BackColor = Color.Black;
                textSTRIP1_VALUE.BackColor = Color.Black;
                textSTRIP2_VALUE.BackColor = Color.Black;
            }
        }

        // 色変換
        private Color StrToColor(string colorstr)
        {
            return Color.FromArgb(0xFF, Color.FromArgb(GetRGBColor(colorstr)));
        }

        // 電線イメージの表示
        private void WireImageDisplay()
        {
            M_WireDetail wireinfo = new M_WireDetail();
            SCR06DBController db = Program.SCR06DB;
            bool strip1on, strip2on;

            if (lblWIRECODE.Text == "") return;

            if (CheckBtnOnOff(SystemConstants.STRIP1_BTN) == SystemConstants.BTN_ON)
                strip1on = true;
            else
                strip1on = false;

            if (CheckBtnOnOff(SystemConstants.STRIP2_BTN) == SystemConstants.BTN_ON)
                strip2on = true;
            else
                strip2on = false;

            if(sieldWireImagePanel.StripAOn == strip1on &&
               sieldWireImagePanel.StripBOn == strip2on &&
               sieldWireImagePanel.WireName == lblWIRENAME.Text) return;

            if(db.dbGetWireInfomation(lblWIRECODE.Text, ref wireinfo) == SystemConstants.ERR_NO_WIRE_INFO) return;

            sieldWireImagePanel.WireName = lblWIRENAME.Text;
            sieldWireImagePanel.CoreWireNumber = wireinfo.Core_Num;
            sieldWireImagePanel.SieldColor = StrToColor(db.dbGetColorRGBCode(wireinfo.Wire_Color));
            sieldWireImagePanel.SetCoreColor(1, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col1_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col1_2)));
            if (wireinfo.Core_Num > 1)
                sieldWireImagePanel.SetCoreColor(2, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col2_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col2_2)));
            if (wireinfo.Core_Num > 2)
                sieldWireImagePanel.SetCoreColor(3, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col3_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col3_2)));
            if (wireinfo.Core_Num > 3)
                sieldWireImagePanel.SetCoreColor(4, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col4_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col4_2)));
            if (wireinfo.Core_Num > 4)
                sieldWireImagePanel.SetCoreColor(5, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col5_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col5_2)));
            if (wireinfo.Core_Num > 5)
                sieldWireImagePanel.SetCoreColor(6, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col6_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col6_2)));
            if (wireinfo.Core_Num > 6)
                sieldWireImagePanel.SetCoreColor(7, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col7_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col7_2)));
            if (wireinfo.Core_Num > 7)
                sieldWireImagePanel.SetCoreColor(8, StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col8_1)), StrToColor(db.dbGetColorRGBCode(wireinfo.C_Col8_2)));
            sieldWireImagePanel.StripAOn = strip1on;
            sieldWireImagePanel.StripBOn = strip2on;            
        }

        private void btnEFU_Click(object sender, EventArgs e)
        {
            fdataForm.ShowDialog();
        }

        // 作業者照会
        private void RegisterOperator()
        {
            //マシンとオフライン起動したときは行わない
            if (Program.DataController.IsConnect() == false)            
                return;            

            SCR06DBController db = Program.SCR06DB;

            // 登録済み作業者の有無を確認
            //  いない場合と登録日付が違う場合は照会
            T_Operator tempop = new T_Operator();
            switch (db.dbGetTemporaryOperator(ref tempop))
            {
                case SystemConstants.ERR_TEMP_OPERATOR_NOBODY:
                    operatorForm.ShowDialog();
                    db.dbGetTemporaryOperator(ref tempop);
                    break;
                case SystemConstants.ERR_TEMP_OPERATOR_TODAY:
                    if (db.dbDelTemporaryOperator() == SystemConstants.ERR_TEMP_OPERATOR_DELETE)
                    {
                        Utility.ShowInfoMsg(SystemConstants.SYSTEM_MSG034);
                        break;
                    }
                    operatorForm.ShowDialog();
                    db.dbGetTemporaryOperator(ref tempop);
                    break;
            }
            // 作業者名表示
            lblOPERATORNAME.Text = tempop.OperatorName;
            
            // 作業実績がある場合は、担当者名を更新
            R_Work workdata = new R_Work();
            if (db.dbGetStartResultWorkData(ref workdata) == SystemConstants.SQL_SUCCESS)
            {
                workdata.Sagyosya_code = tempop.OperatorCode;
                workdata.Sagyosya_name = tempop.OperatorName;

                if (db.dbUpdateResultWorkData(workdata) == SystemConstants.ERR_RESULT_WORKDATA_UPDATE)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG044);
                }
            }
        }

        // オンライン情報の表示
        private void OnlineInfomationDisplay()
        {
            //マシンとオフライン起動したときは行わない
            if (Program.DataController.IsConnect() == false)
            {
                btnSERVER_OFFLINE.Visible =
                btnWIRE_SELECT.Visible =
                pnlONLINE.Visible =
                btnSERVER_ONLINE.Visible = false;
                return;
            }
            else
            {
                btnSERVER_OFFLINE.Visible =
                btnSERVER_ONLINE.Visible = true;
            }

            SCR06DBController db = Program.SCR06DB;
            
            // 連番、エフ照会ボタン、中断ボタン表示
            R_Work workdata = new R_Work();
            if (db.dbGetStartResultWorkData(ref workdata) == SystemConstants.ERR_NO_START_RESULT_WORKDATA)
            {
                btnEFU.Visible = true;
                btnCHUDAN.Visible = false;
                btnWIRE_SELECT.Visible = true;

                lblRENBAN.Text = "";
            }
            else
            {
                btnEFU.Visible = false;
                btnCHUDAN.Visible = true;
                btnWIRE_SELECT.Visible = false;

                lblRENBAN.Text = workdata.Renban;
            } 

            // サーバーオンライン・オフライン表示
            if (db.ServerOnline == true)
            {
                btnWIRE_SELECT.Visible = false;
                pnlONLINE.Visible = true;
            }
            else
            {
                btnWIRE_SELECT.Visible = true;
                pnlONLINE.Visible = false;
            }

        }

        private void btnCHUDAN_Click(object sender, EventArgs e)
        {
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG045) == true)
            {
                InterruptWorkData();
            }
        }

        // 中断処理
        private int InterruptWorkData()
        {
            R_Work workdata = new R_Work();
            L_Work l_workdata = new L_Work();
            SCR06DBController db = Program.SCR06DB;

            if (db.dbGetStartResultWorkData(ref workdata) == SystemConstants.ERR_NO_START_RESULT_WORKDATA)
            {
                return SystemConstants.ERR_NO_START_RESULT_WORKDATA;
            }

            workdata.Sagyou_status = 'C';
            workdata.Chudan_setsu = workdata.Setsu - Int32.Parse(lblQTY2.Text);
            workdata.Chudan_tabadorisu = workdata.Tabadorisu;

            if (db.dbUpdateResultWorkData(workdata) == SystemConstants.ERR_RESULT_WORKDATA_UPDATE)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG044);
                return SystemConstants.ERR_RESULT_WORKDATA_UPDATE;
            }

            // 前回エフに登録
            RegisterLastWorkData(workdata);
            return SystemConstants.SQL_SUCCESS;
        }

        // 満了処理
        private int CompleteWorkData()
        {
            R_Work workdata = new R_Work();
            SCR06DBController db = Program.SCR06DB;
            DataController dataCtrl =Program.DataController;
            string value = "";

            if (db.dbGetStartResultWorkData(ref workdata) == SystemConstants.ERR_NO_START_RESULT_WORKDATA)
            {
                return SystemConstants.ERR_NO_START_RESULT_WORKDATA;
            }

            workdata.Sagyou_status = 'E';
            workdata.Syuryo_time = DateTime.Now;

            dataCtrl.ReadWorkDataStr(SystemConstants.WIRE_LENGTH1, ref value);
            workdata.Setsudancho_set = Int32.Parse(value);
            dataCtrl.ReadWorkDataStr(SystemConstants.STRIP_LENGTH1, ref value);
            workdata.A_kawamukinagasa_set = Double.Parse(value);
            dataCtrl.ReadWorkDataStr(SystemConstants.STRIP_LENGTH2, ref value);
            workdata.B_kawamukinagasa_set = Double.Parse(value);
            dataCtrl.ReadWorkDataStr(SystemConstants.STRIP_HOGUSI1, ref value);
            workdata.A_hogushiryo_set = Double.Parse(value);
            dataCtrl.ReadWorkDataStr(SystemConstants.STRIP_HOGUSI2, ref value);
            workdata.B_hogushiryo_set = Double.Parse(value);

            if (db.dbUpdateResultWorkData(workdata) == SystemConstants.ERR_RESULT_WORKDATA_UPDATE)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG044);
                return SystemConstants.ERR_RESULT_WORKDATA_UPDATE;
            }

            // 前回エフに登録
            RegisterLastWorkData(workdata);
            return SystemConstants.SQL_SUCCESS;
        }

        // 前回の作業データの登録
        private void RegisterLastWorkData(R_Work WorkData)
        {
            L_Work l_workdata = new L_Work();
            SCR06DBController db = Program.SCR06DB;
            
            l_workdata.Renban = WorkData.Renban;
            l_workdata.Densen_code = WorkData.Densen_code;

            if (db.dbUpdateLastWorkData(l_workdata) == SystemConstants.ERR_LAST_WORKDATA_UPDATE)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG048);
            }
        }

        // 電線エラーが発生しエラーがリセットされたら電線照会画面表示
        private void errInfoMsgfrm_OnWireError(object sender, EventArgs e)
        {
            R_Work workdata = new R_Work();
            SCR06DBController db = Program.SCR06DB;

            if (db.F_Dandori == false)
            {
                if (db.dbGetStartResultWorkData(ref workdata) != SystemConstants.ERR_NO_START_RESULT_WORKDATA)
                {
                    wireConfirmForm.ShowDialog();
                }
            }
        }

        // 生産本数満了が発生しリセットされたら生産中のエフを満了更新
        private void errInfoMsgfrm_OnProductComplete(object sender, EventArgs e)
        {
            CompleteWorkData();
        }

        private void btnSERVER_ONLINE_Click(object sender, EventArgs e)
        {
            Program.SCR06DB.ServerOnline = true;
        }

        private void btnSERVER_OFFLINE_Click(object sender, EventArgs e)
        {
            R_Work workdata = new R_Work();
            SCR06DBController db = Program.SCR06DB;

            Utility.ShowInfoMsg(SystemConstants.SYSTEM_MSG050);

            if (db.dbGetStartResultWorkData(ref workdata) == SystemConstants.SQL_SUCCESS)
            {
                InterruptWorkData();
                db.dbDeleteteLastWorkData();
            }
            Program.SCR06DB.ServerOnline = false;
        }

        // ガイド・ブレードの情報を表示
        private void GuideBladeInfomationDisplay()
        {
            string wirecode = "";
            string wirename = "";
            string wiretype = "";
            string wiresize = "";
            string guide1 = "";
            string guide2 = "";
            string blade = "";
            SCR06DBController db = Program.SCR06DB;

            wirename = lblWIRENAME.Text;
            wirecode = lblWIRECODE.Text;
            if (wirename != "" && wirecode.Length == 10)
            {
                wiretype = wirename.Substring(0, 5);
                wiresize = wirename.Substring(5, 6);

                if (db.dbGetGuideName(wiretype, wiresize, ref guide1, ref guide2) == SystemConstants.SQL_SUCCESS)
                {
                    lblGUIDENAME1.Text = guide1;
                    lblGUIDENAME2.Text = guide2;
                }

                if (db.dbGetBladeName(wiretype, wiresize, ref blade) == SystemConstants.SQL_SUCCESS)
                {
                    lblBLADENAME.Text = blade;
                }               
            }

        }

        // 起動時に本数を設定
        private void InitialQtyLotSet()
        {
            int qty, lot;
            R_Work workdata = new R_Work();
            SCR06DBController db = Program.SCR06DB;

            if (textQTY.Text != "0") return;

            // オフライン中と初期化前に行うとエラー発生
            if (Program.DataController.IsConnect() == false || !Program.Initialized) return;

            // 連番、エフ照会ボタン、中断ボタン表示
            if (db.dbGetStartResultWorkData(ref workdata) == SystemConstants.SQL_SUCCESS)
            {
                qty = Int32.Parse(workdata.Setsu.ToString());
                lot = Int32.Parse(workdata.Tabadorisu.ToString());
                setCounter(SystemConstants.QTY_SET_COUNTER1, qty);
                setCounter(SystemConstants.LOT_SET_COUNTER1, lot);
            }
        }

        private void btnQUALITY_RECORD_Click(object sender, EventArgs e)
        {
            qualityRecordForm.ShowDialog();
        }

        private void ProgramEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.MainForm.Close();
        }

        // 運転中はボタン操作不可
        private void OperationControlMachineStatusRun()
        {
            bool value = false;

            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) 
                value = false;
            else
                value = true;

            btnEFU.Enabled = value;
            btnCHUDAN.Enabled = value;
            btnOPERATOR_OUT.Enabled = value;
            btnQUALITY_RECORD.Enabled = value;
            btnWIRE_SELECT.Enabled = value;
            btnBANK.Enabled = value;
        }
    }
}