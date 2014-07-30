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
    // エンターキー入力デリゲート
    public delegate void enterKeyDelegate(double value);

    public partial class tenKeyfrm : Form
    {
        const string BTN0 = "0";
        const string BTN1 = "1"; 
        const string BTN2 = "2";
        const string BTN3 = "3";
        const string BTN4 = "4";
        const string BTN5 = "5";
        const string BTN6 = "6";
        const string BTN7 = "7";
        const string BTN8 = "8";
        const string BTN9 = "9";
        const string BTND = ".";
        const string BTNM = "-";
        const string BTNC = "C";
        const string BTNB = "B";
        const string BTNE = "E";

        // エンターキー入力イベント
        public event enterKeyDelegate enterKeyEvent;

        // 入力前の値取得用（書き込み専用）
        public double val { private get; set; }

        public tenKeyfrm()
        {
            InitializeComponent();

            // 数字ボタンクリックイベント
            setClickNumberButtonEvent(button0, BTN0);
            setClickNumberButtonEvent(button1, BTN1);
            setClickNumberButtonEvent(button2, BTN2);
            setClickNumberButtonEvent(button3, BTN3);
            setClickNumberButtonEvent(button4, BTN4);
            setClickNumberButtonEvent(button5, BTN5);
            setClickNumberButtonEvent(button6, BTN6);
            setClickNumberButtonEvent(button7, BTN7);
            setClickNumberButtonEvent(button8, BTN8);
            setClickNumberButtonEvent(button9, BTN9);
            setClickNumberButtonEvent(buttonC, BTNC);
            setClickNumberButtonEvent(buttonB, BTNB);
            setClickNumberButtonEvent(buttonM, BTNM);
            setClickNumberButtonEvent(buttonD, BTND);
            setClickNumberButtonEvent(buttonE, BTNE);
        }

        // 数字ボタンクリックイベント
        private void setClickNumberButtonEvent(Button btn, string strNum)
        {
            btn.Click += new EventHandler(delegate(object sender, EventArgs e)
            {
                inputString(textBox1, strNum);
            });
        }
        
        // キー入力アクション
        private void inputString(TextBox text, string strNum)
        {
            switch (strNum)
            {
                case BTN0:
                case BTN1:
                case BTN2:
                case BTN3:
                case BTN4:
                case BTN5:
                case BTN6:
                case BTN7:
                case BTN8:
                case BTN9:
                    text.Text += strNum;
                    break;
                case BTND:
                    if (text.Text != "" && text.Text.Contains(BTND) == false)
                        text.Text += strNum;
                    break;
                case BTNM:
                    if (text.Text == "")
                        text.Text += strNum;
                    break;
                case BTNC:
                    text.Clear();
                    break;
                case BTNB:
                    if(text.Text != "")
                        text.Text = text.Text.Remove(text.Text.Length - 1, 1);
                    break;
                case BTNE:
                    try
                    {
                        enterKeyEvent(Double.Parse(text.Text));
                        this.Close();
                    }
                    catch
                    {
                        text.Text = "";
                    }
                    break;
            }
        }

        // フォーカス時変更前の値を表示
        private void tenKeyfrm_Activated(object sender, EventArgs e)
        {
            textBox2.Text = val.ToString();
            textBox1.Text = "";
            textBox1.Focus();
        }

    }
}
