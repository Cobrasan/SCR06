using System;
using System.Windows.Forms;

namespace Alchemist
{


    public partial class syncroprogressfrm : Form
    {
        private int allQueue = 0;               // 全体要求の総数（下のツールバーの最大値）       
        private int workDataQueue = 0;          // 加工値の総数
        private int correctDataQueue = 0;       // 補正値の総数
        private int timingDataQueue = 0;        // タイミング値の総数
        private int bankDataQueue = 0;          // バンクデータの総数
        private int formatType;                 // フォーマット済みか否か

        public int Result
        {
            get;
            set;
        }

        enum Phase
        {
            CorrectReading,
            TimingReading,
            CorrectWriting,
            TimingWriting,
            WorkdataReading,
            BankWriting
        };

        public syncroprogressfrm()
        {
            InitializeComponent();
        }

        // 初期表示
        private void syncroprogressfrm_Load(object sender, EventArgs e)
        {
            // 初期値を表示
            progressBarEach.Value = 0;
            progressBarTOTAL.Value = 0;
            lblProgress.Text = "";
        }

        /// <summary>
        /// キューのデータを待つ
        /// </summary>
        private void waitForQueue(bool refresh_flag, Phase phase=Phase.CorrectReading)
        {
            //キュー送信待ち
            while (Program.DataController.IsCommWait())
            {
                // 切断されたら、処理をキャンセルする
                if (!Program.DataController.IsConnect())
                {
                    throw new TimeoutException();
                }

                // プログレスバーを描画
                if (refresh_flag)
                {
                    refresh(phase);
                }

                //フォーマットデータ取得完了までループ
                Application.DoEvents();
            }
        }


        /// <summary>
        /// 同期処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void syncroprogressfrm_Shown(object sender, EventArgs e)
        {
            try
            {
                //フォーマットデータ取得
                Program.DataController.MemorySynchro(SystemConstants.ADDR_FORMAT_CHECK, SystemConstants.FORMAT_CHECK_COUNT);
                waitForQueue(false);

                // フォーマットチェック
                formatType = Program.DataController.FomatCheck();

                // bankno
                int bankno = 0;
                Program.DataController.BankNoRead(ref bankno);
                BankDataStruct[] bankdataStruct = new BankDataStruct[0];
                Program.DataController.BankDataRead(bankno, ref bankdataStruct);
                bankDataQueue = bankdataStruct.Length;

                workDataQueue = bankdataStruct.Length;

                //フォーマット済みの場合
                if (formatType == SystemConstants.FORMAT_DONE)
                {
                    // 送信する値の数を求める
                    correctDataQueue = Program.DataController.CorrectDataRead(false);
                    timingDataQueue = Program.DataController.TimingDataRead(false);
                    workDataQueue = Program.DataController.WorkDataRead(false);
                    allQueue = correctDataQueue + timingDataQueue + workDataQueue + bankDataQueue;

                    // 補正値の受信
                    Program.DataController.CorrectDataRead();
                    waitForQueue(true, Phase.CorrectReading);

                    // タイミング値の受信
                    Program.DataController.TimingDataRead();
                    waitForQueue(true, Phase.TimingReading);

                    // 加工値の同期
                    Program.DataController.WorkDataRead();
                    waitForQueue(true, Phase.WorkdataReading);

                    // バンクデータの送信
                    mainfrm.BankDataLoad(bankno);
                    waitForQueue(true, Phase.BankWriting);

                }
                // フォーマット済みでない場合
                else
                {
                    // 送信する値の数を求める
                    correctDataQueue = Program.DataController.CorrectDataSend(false);
                    timingDataQueue = Program.DataController.TimingDataSend(false);
                    workDataQueue = Program.DataController.WorkDataRead(false);
                    allQueue = correctDataQueue + timingDataQueue + workDataQueue + bankDataQueue;

                    // アプリケーションの続行確認
                    if (!Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG007))
                    {
                        DialogResult = DialogResult.No;
                        return;
                    }

                    // 補正値を送る
                    try
                    {
                        Program.DataController.CorrectDataSend();
                        waitForQueue(true, Phase.CorrectWriting);

                        Program.DataController.TimingDataSend();
                        waitForQueue(true, Phase.TimingWriting);
                    }
                    catch
                    {
                        Result = SystemConstants.ERR_CORRECT_FILE_ERROR;
                        DialogResult = System.Windows.Forms.DialogResult.No;
                        return;
                    }

                    // 加工値の同期
                    Program.DataController.WorkDataRead();
                    waitForQueue(true, Phase.WorkdataReading);

                    // バンクデータの送信
                    mainfrm.BankDataLoad(bankno);
                    waitForQueue(true, Phase.BankWriting);

                    // フォーマット完了を書き込みます。
                    Program.DataController.FormatWrite();
                }

                // 加工値、補正値を保存する
                mainfrm.BankDataSave(bankno);
                Program.DataController.CorrectDataSave();
            }
            catch (TimeoutException)
            {
                /* タイムアウトが発生した場合は、ここへジャンプ */
                DialogResult = DialogResult.No;
                Result = SystemConstants.ERR_SYNC_TIMEOUT;
                return;
            }

            //プログレスバーフォームを閉じる。
            DialogResult = DialogResult.OK;
        }


        private void labelOperation(Label lbl, bool Visible)
        {
            if (lbl.Visible != Visible)
            {
                lbl.Visible = Visible;
            }
        }

        private void labelSet(Label lbl)
        {
            Label[] labels = new Label[] {
                lblCorrectDataSend, 
                lblTimingDataSend, 
                lblCorrectDataRead, 
                lblTimingDataRead,
                lblworkdataSend, 
                lblWorkDataRead
            };

            foreach (var l in labels)
            {
                if (lbl == l)
                {
                    labelOperation(l, true);
                }
                else
                {
                    labelOperation(l, false);
                }
            }
        }


        /// <summary>
        /// プログレスバーの描画の更新を行う
        /// </summary>
        private void refresh(Phase phase)
        {
            int ReservedCount = 0;
            int AckWaitCount = 0;
            int nowQueue = 0;

            // キューの残存数を求める
            Program.DataController.CommWaitCount(ref ReservedCount, ref AckWaitCount);

            // フェーズごとに表示を分岐
            switch (phase)
            {
                case Phase.CorrectReading : /* 補正値読み込み中 */
                    labelSet(lblCorrectDataRead);
                    progressBarEach.Maximum = correctDataQueue;
                    progressBarEach.Value = correctDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value;
                    break;

                case Phase.TimingReading :  /* タイミング読み込み中 */
                    labelSet(lblTimingDataRead);
                    progressBarEach.Maximum = timingDataQueue;
                    progressBarEach.Value = timingDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue;
                    break;

                case Phase.CorrectWriting : /* 補正値書き込み中 */
                    labelSet(lblCorrectDataSend);
                    progressBarEach.Maximum = correctDataQueue;
                    progressBarEach.Value = correctDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value;
                    break;

                case Phase.TimingWriting :  /* タイミング書き込み中 */
                    labelSet(lblTimingDataSend);
                    progressBarEach.Maximum = timingDataQueue;
                    progressBarEach.Value = timingDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue;
                    break;

                case Phase.WorkdataReading: /* 加工値読み込み中 */
                    labelSet(lblWorkDataRead);
                    progressBarEach.Maximum = workDataQueue;
                    progressBarEach.Value = workDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue + timingDataQueue;
                    break;

                case Phase.BankWriting :    /* バンクデータ書き込み中 */
                    labelSet(lblworkdataSend);
                    progressBarEach.Maximum = bankDataQueue;
                    progressBarEach.Value = bankDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue + timingDataQueue + workDataQueue;
                    break;
            }

            //進捗状況を更新する。
            lblProgress.Text = "(" + nowQueue + "/" + allQueue + ")";
            progressBarTOTAL.Maximum = allQueue;
            progressBarTOTAL.Value = nowQueue;
        }
    }
}