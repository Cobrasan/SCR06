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
    public partial class wireselectfrm : Form
    {
        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // 電線名の取得
            Program.SCR06DB.dbGetItemWireTypeComboBox(ref comboBox1);
            Program.SCR06DB.dbGetItemWireSizeComboBox(ref comboBox2);
            Program.SCR06DB.dbGetItemColorCharComboBox(ref comboBox3);
            Program.SCR06DB.dbGetItemColorNoComboBox(ref comboBox4);
        }

        public wireselectfrm()
        {
            InitializeComponent();
        }

        private void wireselectfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void btn_WIRE_Select_Click(object sender, EventArgs e)
        {
            int selectedNo = 0;
            string wirename = "";
            string wirecode = "";

            // 現在選択されているバンクNoを取得
            Program.DataController.BankNoRead(ref selectedNo);

            wirename = comboBox1.Text + comboBox2.Text + comboBox3.Text + comboBox4.Text;
            wirecode = Program.SCR06DB.dbGetWireCode(comboBox1.Text, comboBox2.Text, comboBox3.Text, comboBox4.Text);

            M_WireDetail wireinfo = new M_WireDetail();
            // 登録されている電線か確認
            if (Program.SCR06DB.dbGetWireInfomation(wirecode, ref wireinfo) == SystemConstants.ERR_NO_WIRE_INFO)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG038);
                return;
            }

            // 選択された電線名をbankdata.xmlに書き込む
            mainfrm.BankDataItemWrite(selectedNo, wirename, SystemConstants.BANKITEM_TYPE_WIRENAME);

            // メインフォームのバンクコメントに電線コードをbankdata.xmlに書き込む
            mainfrm.BankDataCommentWrite(selectedNo, wirecode);

            Visible = false;
        }
    }
}
