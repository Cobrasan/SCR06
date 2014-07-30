using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class iocheckfrm : Form
    {

        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);
        }

        public iocheckfrm()
        {
            InitializeComponent();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {

            int memVal = 0;

            PictureBox[] pictureBoxArray = new PictureBox[]{
                pictureBox0,
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4,
                pictureBox5,
                pictureBox6,
                pictureBox7,
                pictureBox8,
                pictureBox9,
                pictureBox10,
                pictureBox11,
                pictureBox12,
                pictureBox13,
                pictureBox14,
                pictureBox15,
            };

            // 指定されたアドレスの値をMemReadで取得
            if (textAddress.Text != "")
            {
                int result = Program.DataController.MemRead(int.Parse(textAddress.Text, System.Globalization.NumberStyles.HexNumber), ref memVal);
            }
            else
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG026, lblAddress.Text, "0000", "FFFF");
            }

            // INTラベルに10進数で出力
            lblInt2.Text = memVal.ToString();

            // HEXラベルに16進数で出力
            lblHex2.Text = String.Format("{0:X}", memVal);

            // LEDグラフィックをビットに合わせてON、OFFグラフィックを切り替える
            for (int i = 0; i < 16; i++)
            {
                // iビット目が0だった場合
                if (0 == (memVal & (1 << i)))
                {
                    // LEDグラフィックをOFFにする
                    pictureBoxArray[i].Image = Alchemist.Properties.Resources.LedGreenOff;
                }
                // iビット目が0以外だった場合
                else
                {
                    // LEDグラフィックをONにする
                    pictureBoxArray[i].Image = Alchemist.Properties.Resources.LedGreenOn;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void iocheckfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }
    }
}