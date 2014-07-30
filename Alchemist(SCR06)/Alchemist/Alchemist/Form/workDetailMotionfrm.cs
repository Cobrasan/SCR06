using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class workDetailMotionfrm : Form
    {
        public workDetailMotionfrm()
        {
            InitializeComponent();

        }

        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            //シース剥ぎA
            Program.MainForm.SetBtnEvent(SystemConstants.STRIP1_BTN, SystemConstants.BTN_PUSH, btnSTRIP1);
            //シース剥ぎB
            Program.MainForm.SetBtnEvent(SystemConstants.STRIP2_BTN, SystemConstants.BTN_PUSH, btnSTRIP2);
        }


        public void refresh()
        {
            //シース剥ぎA
            mainfrm.CheckBtnAnd_ChangePicture(SystemConstants.STRIP1_BTN, btnSTRIP1, Alchemist.Properties.Resources.StripAON, Alchemist.Properties.Resources.StripAOFF);
            //シース剥ぎB
            mainfrm.CheckBtnAnd_ChangePicture(SystemConstants.STRIP2_BTN, btnSTRIP2, Alchemist.Properties.Resources.StripBON, Alchemist.Properties.Resources.StripBOFF);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void workDetailMotionfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }
    }
}