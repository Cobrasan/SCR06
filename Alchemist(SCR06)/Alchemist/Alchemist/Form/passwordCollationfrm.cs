using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class passwordCollationfrm : Form
    {
        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);
        }

		// コンストラクタ
        public passwordCollationfrm()
        {
            InitializeComponent();
        }

		// ロードされたときの処理
		private void passwordCollationfrm_Load(object sender, EventArgs e)
		{
			textPassword.Clear();
			textPassword.Focus();
		}

		// パスワードをチェックする
		//public bool CheckPassword() {
		//	return textPassword.Text == Program.SystemData.password;
		//}

        // パスワードをチェックする
        public string CheckPassword() {
        	return textPassword.Text;
        }

    }
}
