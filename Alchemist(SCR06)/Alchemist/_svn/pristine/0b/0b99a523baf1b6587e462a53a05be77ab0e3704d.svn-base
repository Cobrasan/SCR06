using System;
using System.Drawing;
using System.Windows.Forms;


namespace Alchemist
{
    public partial class connectOperationfrm : Form
    {
        public connectOperationfrm()
        {
            InitializeComponent();
        }

        // 初期化
        public void Initialize()
        {

            Program.MainForm.AddOwnedForm(this);
        }

        private bool prevIsConnect = true;

        // 表示更新
        public void refresh()
        {
            bool isConnect = Program.DataController.IsConnect();

            // 前回と接続状態が同じ場合は、何もしない
            if (prevIsConnect == isConnect)
            {
                return;
            }

            // 接続状態の時
            if (isConnect)
            {
                panelOnline.BackColor = Color.Green;
                lblOnline.BackColor = Color.Green;
                lblOnline.Text = Utility.GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG029);
                lblOnline.ForeColor = Color.White;
            }
            // 非接続状態の時
            else
            {
                panelOnline.BackColor = Color.Red;
                lblOnline.BackColor = Color.Red;
                lblOnline.Text = Utility.GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG030);
                lblOnline.ForeColor = Color.Black;
            }

            prevIsConnect = isConnect;
        }

        // 閉じる
        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        // 接続
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Program.Connect();
            Visible = false;
        }

        // 切断
        private void btnDisConnect_Click(object sender, EventArgs e)
        {
            Program.DataController.MachineConnect(SystemConstants.MACHINE_DISCONNECT);
        }

        // フォームを閉じる時のイベントハンドラ
        private void connectOperationfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }
    }
}