using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class counterfrm : Form
    {
        private TenkeyControl tenkey;

        public counterfrm()
        {
            InitializeComponent();

            // �e���L�[����̓��͊����C�x���g
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);
        }

		// �C�x���g�n���h���ݒ胁�\�b�h
		public void Initialize() {
			Program.MainForm.AddOwnedForm(this);

			// QTY���Z�b�g
            Program.MainForm.SetBtnEvent(SystemConstants.QTY_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH, btnQTYReset);

			// LOT���Z�b�g
            Program.MainForm.SetBtnEvent(SystemConstants.LOT_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH, btnLOTReset);

			// QTY UP�{�^��
            Program.MainForm.SetBtnEvent(SystemConstants.COUNT_UP_BTN, SystemConstants.BTN_PUSH, btnLOTUp);

			// QTY DOWN�{�^��
            Program.MainForm.SetBtnEvent(SystemConstants.COUNT_DOWN_BTN, SystemConstants.BTN_PUSH, btnLOTDown);

			// TOTAL���Z�b�g
            Program.MainForm.SetBtnEvent(SystemConstants.TOTAL_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH, btnTOTALReset);

			// LOT�ݒ�
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.LOT_SET_COUNTER1, textLOTSetNumber);

			// QTY�ݒ�
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.QTY_SET_COUNTER1, textQTYSetNumber);

            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.QTY_SET_COUNTER1, textQTYSetNumber);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.LOT_SET_COUNTER1, textLOTSetNumber);
        }

		// �\���X�V�p���\�b�h
		public void refresh() 
		{
			mainfrm mainForm = ((mainfrm)this.Owner);

			// QTY�J�E���^
			mainForm.refreshControl(SystemConstants.QTY_COUNTER1, lblQTY2);

			// QTY�ݒ�{��
			mainForm.refreshControl(SystemConstants.QTY_SET_COUNTER1, textQTYSetNumber);

			// LOT�J�E���^
			mainForm.refreshControl(SystemConstants.LOT_COUNTER1, lblLOT2);

			// LOT�ݒ�{��
			mainForm.refreshControl(SystemConstants.LOT_SET_COUNTER1, textLOTSetNumber);

			// TOTAL�J�E���^
			mainForm.refreshControl(SystemConstants.TOTAL_COUNTER1, lblTOTAL2);

			// TACT1
			mainForm.refreshControl(SystemConstants.MACHINE_TACT1, lblTact4);

		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Visible = false;
		}

		private void counterfrm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}

        public void TenKeyEnterEvent(TenKeyData td)
        {
            switch (td.obj.GetType().Name)
            {
                case "CustomTextBox":
                    CustomTextBox ct = (CustomTextBox)td.obj;
                    ct.Text = td.val.ToString();
                    Program.MainForm.EnterTextBox(td.workidtype, td.workid, ct);
                    break;
            }
        }

        public void ClickTextBoxEvent(int workIDType, int WorkID, CustomTextBox customtextBox)
        {
            customtextBox.Click += delegate(Object sender, EventArgs e)
            {
                if (Program.SystemData.tachpanel == false) return;

                CustomTextBox ct = (CustomTextBox)sender;
                TenkeyControl tc = tenkey;
                tc.tenKeyData.obj = sender;
                tc.tenKeyData.val = double.Parse(ct.Text);
                tc.tenKeyData.workid = WorkID;
                tc.tenKeyData.workidtype = workIDType;
                tc.tenKeyData.itemtype = 0;
                tc.tenkeyFormShow();

                // �t�H�[�J�X�A�E�g����
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;
            };
        }

    }
}