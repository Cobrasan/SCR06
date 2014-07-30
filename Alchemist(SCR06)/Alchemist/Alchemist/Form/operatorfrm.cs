using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class operatorfrm : Form
    {
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);
        }

        public operatorfrm()
        {
            InitializeComponent();

            SetTextBoxEvent(txtBARCODE);
        }

        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            Program.MainForm.Close();
        }

        private void operatorfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void SetTextBoxEvent(CustomTextBox customtextBox)
        {
            customtextBox.EnterKeyDown += delegate(EventArgs e)
            {
                string operatorcode = customtextBox.Text.Remove(0, 1); //先頭のA削除
                string operatorname = "";
                int result = Program.SCR06DB.dbGetOperatorName(operatorcode, ref operatorname);
                if (result == SystemConstants.ERR_OPERATOR_NAME)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG035);
                    customtextBox.Text = "";
                    return;
                }

                T_Operator tempop = new T_Operator();
                tempop.OperatorCode = operatorcode;
                tempop.OperatorName = operatorname;
                tempop.WorkDate = DateTime.Now;

                if (Program.SCR06DB.dbAddTemporaryOperator(tempop) == SystemConstants.ERR_TEMP_OPERATOR_ADD)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG036);
                    Program.MainForm.Close();
                }
                Visible = false;
            };
        }

        private void operatorfrm_Activated(object sender, EventArgs e)
        {
            txtBARCODE.Text = "";
        }
    }
}
