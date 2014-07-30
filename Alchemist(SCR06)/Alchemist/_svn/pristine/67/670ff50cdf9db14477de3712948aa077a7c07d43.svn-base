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
    public partial class wireConfirmfrm : Form
    {
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);
        }

        public wireConfirmfrm()
        {
            InitializeComponent();

            SetTextBoxEvent(txtBARCODE);
        }

        private void SetTextBoxEvent(CustomTextBox customtextBox)
        {
            customtextBox.EnterKeyDown += delegate(EventArgs e)
            {
                R_Work workdata = new R_Work();
                SCR06DBController db = Program.SCR06DB;
                db.dbGetStartResultWorkData(ref workdata);
                
                string barcord = customtextBox.Text;

                // スーパーバイザー作業者コード入力でも照合
                // 電線バーコード先頭には「P」が付いている
                if ("P" + workdata.Densen_code == barcord ||
                db.dbGetSuperVisorOperator(barcord) == SystemConstants.SQL_SUCCESS)
                {
                    customtextBox.Text = "";
                    Visible = false;
                }
                else
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG047);
                    customtextBox.Text = "";
                    return;
                }                
            };
        }

        private void wireconfirmfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;

            //画面を閉じたら運転禁止解除
            mainfrm.WritePushBtn(SystemConstants.WIRE_CHANGE, SystemConstants.BTN_OFF);
        }

        private void wireconfirmfrm_Activated(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            R_Work workdata = new R_Work();
            db.dbGetStartResultWorkData(ref workdata);

            lblWIRECODE.Text = workdata.Densen_code;

            //画面表示中運転禁止
            mainfrm.WritePushBtn(SystemConstants.WIRE_CHANGE, SystemConstants.BTN_ON);
        }

    }
}
