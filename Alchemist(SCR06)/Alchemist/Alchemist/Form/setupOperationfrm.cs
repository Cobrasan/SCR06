using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class setupOperationfrm : Form
    {
        public setupOperationfrm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            //�d������
            Program.MainForm.SetBtnEvent(SystemConstants.FEED1_BTN, SystemConstants.BTN_PUSH, btnFEED);
            //�J�E���g�A�b�v
            Program.MainForm.SetBtnEvent(SystemConstants.PERMIT_COUNTUP_BTN, SystemConstants.BTN_PUSH, btnCOUNTUP);
            //�d���ق������b�N
            Program.MainForm.SetBtnEvent(SystemConstants.WIRE_DISENTANGLE_BTN, SystemConstants.BTN_PUSH, btnDISENTANGLE);
            //��[�J�b�g���b�N
            Program.MainForm.SetBtnEvent(SystemConstants.CUT_WIRETOP_BTN, SystemConstants.BTN_PUSH, btnCUTWIRETOP);
        }

       

        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void setupOperationfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        public void refresh()
        {
            //�t�B�[�h
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.FEED1_BTN, btnFEED);
            //�J�E���g�A�b�v
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.PERMIT_COUNTUP_BTN, btnCOUNTUP);
            //�d������
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.WIRE_DISENTANGLE_BTN, btnDISENTANGLE);
            //��[�J�b�g���b�N
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.CUT_WIRETOP_BTN, btnCUTWIRETOP);
        }
    }
}