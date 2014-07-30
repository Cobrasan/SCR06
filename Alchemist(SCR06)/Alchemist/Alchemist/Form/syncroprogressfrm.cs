using System;
using System.Windows.Forms;

namespace Alchemist
{


    public partial class syncroprogressfrm : Form
    {
        private int allQueue = 0;               // �S�̗v���̑����i���̃c�[���o�[�̍ő�l�j       
        private int workDataQueue = 0;          // ���H�l�̑���
        private int correctDataQueue = 0;       // �␳�l�̑���
        private int timingDataQueue = 0;        // �^�C�~���O�l�̑���
        private int bankDataQueue = 0;          // �o���N�f�[�^�̑���
        private int formatType;                 // �t�H�[�}�b�g�ς݂��ۂ�

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

        // �����\��
        private void syncroprogressfrm_Load(object sender, EventArgs e)
        {
            // �����l��\��
            progressBarEach.Value = 0;
            progressBarTOTAL.Value = 0;
            lblProgress.Text = "";
        }

        /// <summary>
        /// �L���[�̃f�[�^��҂�
        /// </summary>
        private void waitForQueue(bool refresh_flag, Phase phase=Phase.CorrectReading)
        {
            //�L���[���M�҂�
            while (Program.DataController.IsCommWait())
            {
                // �ؒf���ꂽ��A�������L�����Z������
                if (!Program.DataController.IsConnect())
                {
                    throw new TimeoutException();
                }

                // �v���O���X�o�[��`��
                if (refresh_flag)
                {
                    refresh(phase);
                }

                //�t�H�[�}�b�g�f�[�^�擾�����܂Ń��[�v
                Application.DoEvents();
            }
        }


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void syncroprogressfrm_Shown(object sender, EventArgs e)
        {
            try
            {
                //�t�H�[�}�b�g�f�[�^�擾
                Program.DataController.MemorySynchro(SystemConstants.ADDR_FORMAT_CHECK, SystemConstants.FORMAT_CHECK_COUNT);
                waitForQueue(false);

                // �t�H�[�}�b�g�`�F�b�N
                formatType = Program.DataController.FomatCheck();

                // bankno
                int bankno = 0;
                Program.DataController.BankNoRead(ref bankno);
                BankDataStruct[] bankdataStruct = new BankDataStruct[0];
                Program.DataController.BankDataRead(bankno, ref bankdataStruct);
                bankDataQueue = bankdataStruct.Length;

                workDataQueue = bankdataStruct.Length;

                //�t�H�[�}�b�g�ς݂̏ꍇ
                if (formatType == SystemConstants.FORMAT_DONE)
                {
                    // ���M����l�̐������߂�
                    correctDataQueue = Program.DataController.CorrectDataRead(false);
                    timingDataQueue = Program.DataController.TimingDataRead(false);
                    workDataQueue = Program.DataController.WorkDataRead(false);
                    allQueue = correctDataQueue + timingDataQueue + workDataQueue + bankDataQueue;

                    // �␳�l�̎�M
                    Program.DataController.CorrectDataRead();
                    waitForQueue(true, Phase.CorrectReading);

                    // �^�C�~���O�l�̎�M
                    Program.DataController.TimingDataRead();
                    waitForQueue(true, Phase.TimingReading);

                    // ���H�l�̓���
                    Program.DataController.WorkDataRead();
                    waitForQueue(true, Phase.WorkdataReading);

                    // �o���N�f�[�^�̑��M
                    mainfrm.BankDataLoad(bankno);
                    waitForQueue(true, Phase.BankWriting);

                }
                // �t�H�[�}�b�g�ς݂łȂ��ꍇ
                else
                {
                    // ���M����l�̐������߂�
                    correctDataQueue = Program.DataController.CorrectDataSend(false);
                    timingDataQueue = Program.DataController.TimingDataSend(false);
                    workDataQueue = Program.DataController.WorkDataRead(false);
                    allQueue = correctDataQueue + timingDataQueue + workDataQueue + bankDataQueue;

                    // �A�v���P�[�V�����̑��s�m�F
                    if (!Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG007))
                    {
                        DialogResult = DialogResult.No;
                        return;
                    }

                    // �␳�l�𑗂�
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

                    // ���H�l�̓���
                    Program.DataController.WorkDataRead();
                    waitForQueue(true, Phase.WorkdataReading);

                    // �o���N�f�[�^�̑��M
                    mainfrm.BankDataLoad(bankno);
                    waitForQueue(true, Phase.BankWriting);

                    // �t�H�[�}�b�g�������������݂܂��B
                    Program.DataController.FormatWrite();
                }

                // ���H�l�A�␳�l��ۑ�����
                mainfrm.BankDataSave(bankno);
                Program.DataController.CorrectDataSave();
            }
            catch (TimeoutException)
            {
                /* �^�C���A�E�g�����������ꍇ�́A�����փW�����v */
                DialogResult = DialogResult.No;
                Result = SystemConstants.ERR_SYNC_TIMEOUT;
                return;
            }

            //�v���O���X�o�[�t�H�[�������B
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
        /// �v���O���X�o�[�̕`��̍X�V���s��
        /// </summary>
        private void refresh(Phase phase)
        {
            int ReservedCount = 0;
            int AckWaitCount = 0;
            int nowQueue = 0;

            // �L���[�̎c���������߂�
            Program.DataController.CommWaitCount(ref ReservedCount, ref AckWaitCount);

            // �t�F�[�Y���Ƃɕ\���𕪊�
            switch (phase)
            {
                case Phase.CorrectReading : /* �␳�l�ǂݍ��ݒ� */
                    labelSet(lblCorrectDataRead);
                    progressBarEach.Maximum = correctDataQueue;
                    progressBarEach.Value = correctDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value;
                    break;

                case Phase.TimingReading :  /* �^�C�~���O�ǂݍ��ݒ� */
                    labelSet(lblTimingDataRead);
                    progressBarEach.Maximum = timingDataQueue;
                    progressBarEach.Value = timingDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue;
                    break;

                case Phase.CorrectWriting : /* �␳�l�������ݒ� */
                    labelSet(lblCorrectDataSend);
                    progressBarEach.Maximum = correctDataQueue;
                    progressBarEach.Value = correctDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value;
                    break;

                case Phase.TimingWriting :  /* �^�C�~���O�������ݒ� */
                    labelSet(lblTimingDataSend);
                    progressBarEach.Maximum = timingDataQueue;
                    progressBarEach.Value = timingDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue;
                    break;

                case Phase.WorkdataReading: /* ���H�l�ǂݍ��ݒ� */
                    labelSet(lblWorkDataRead);
                    progressBarEach.Maximum = workDataQueue;
                    progressBarEach.Value = workDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue + timingDataQueue;
                    break;

                case Phase.BankWriting :    /* �o���N�f�[�^�������ݒ� */
                    labelSet(lblworkdataSend);
                    progressBarEach.Maximum = bankDataQueue;
                    progressBarEach.Value = bankDataQueue - ReservedCount;
                    nowQueue = progressBarEach.Value + correctDataQueue + timingDataQueue + workDataQueue;
                    break;
            }

            //�i���󋵂��X�V����B
            lblProgress.Text = "(" + nowQueue + "/" + allQueue + ")";
            progressBarTOTAL.Maximum = allQueue;
            progressBarTOTAL.Value = nowQueue;
        }
    }
}