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

            //電線送り
            Program.MainForm.SetBtnEvent(SystemConstants.FEED1_BTN, SystemConstants.BTN_PUSH, btnFEED);
            //カウントアップ
            Program.MainForm.SetBtnEvent(SystemConstants.PERMIT_COUNTUP_BTN, SystemConstants.BTN_PUSH, btnCOUNTUP);
            //電線ほぐしロック
            Program.MainForm.SetBtnEvent(SystemConstants.WIRE_DISENTANGLE_BTN, SystemConstants.BTN_PUSH, btnDISENTANGLE);
            //先端カットロック
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
            //フィード
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.FEED1_BTN, btnFEED);
            //カウントアップ
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.PERMIT_COUNTUP_BTN, btnCOUNTUP);
            //電線送り
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.WIRE_DISENTANGLE_BTN, btnDISENTANGLE);
            //先端カットロック
            mainfrm.CheckBtnAnd_ChangeColor(SystemConstants.CUT_WIRETOP_BTN, btnCUTWIRETOP);
        }
    }
}