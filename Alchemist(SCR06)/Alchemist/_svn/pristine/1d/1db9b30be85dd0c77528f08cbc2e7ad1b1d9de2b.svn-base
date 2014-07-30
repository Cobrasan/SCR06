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
    public partial class qualityRecordfrm : Form
    {
        private TenkeyControl tenkey;

        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // 品質記録項目の取得
            Program.SCR06DB.dbGetItemQualityItemComboBox(ref comboBox1);
            Program.SCR06DB.dbGetItemQualityItemComboBox(ref comboBox2);
            Program.SCR06DB.dbGetItemQualityItemComboBox(ref comboBox3);
            Program.SCR06DB.dbGetItemQualityItemComboBox(ref comboBox4);
            Program.SCR06DB.dbGetItemQualityItemComboBox(ref comboBox5);
            Program.SCR06DB.dbGetItemQualityItemComboBox(ref comboBox6);

            // テンキーからの入力完了イベント
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);
        }

        public qualityRecordfrm()
        {
            InitializeComponent();
            SetTextBoxEventBarcode(txtBARCODE);
            SetTextBoxEventQty(customTextBox1);
            SetTextBoxEventQty(customTextBox2);
            SetTextBoxEventQty(customTextBox3);
            SetTextBoxEventQty(customTextBox4);
            SetTextBoxEventQty(customTextBox5);
            SetTextBoxEventQty(customTextBox6);

            ClickTextBoxEvent(customTextBox1);
            ClickTextBoxEvent(customTextBox2);
            ClickTextBoxEvent(customTextBox3);
            ClickTextBoxEvent(customTextBox4);
            ClickTextBoxEvent(customTextBox5);
            ClickTextBoxEvent(customTextBox6);
        }

        private void qualityRecordfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void qualityRecordfrm_Activated(object sender, EventArgs e)
        {     
            /*
            txtBARCODE.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
            comboBox6.Text = "";
            customTextBox1.Text = "";
            customTextBox2.Text = "";
            customTextBox3.Text = "";
            customTextBox4.Text = "";
            customTextBox5.Text = "";
            customTextBox6.Text = "";

            txtBARCODE.Focus();
            */
        }

        private void SetTextBoxEventBarcode(CustomTextBox customtextBox)
        {
            customtextBox.EnterKeyDown += delegate(EventArgs e)
            {
                if (ReadBarcode(customtextBox.Text))
                {
                    // フォーカスアウトする
                    Form frm = customtextBox.FindForm();
                    frm.ActiveControl = null;
                }
            };
        }

        // テンキーから入力完了イベント
        private void TenKeyEnterEvent(TenKeyData td)
        {
            switch (td.obj.GetType().Name)
            {
                case "CustomTextBox":
                    CustomTextBox ct = (CustomTextBox)td.obj;
                    ct.Text = td.val.ToString();
                    EnterTextBox(ct);
                    break;
            }
        }

        private void SetTextBoxEventQty(CustomTextBox customtextBox)
        {
            customtextBox.EnterKeyDown += delegate(EventArgs e)
            {
                EnterTextBox(customtextBox);
            };
        }

        // テキストボックスでエンターキー押されたときの処理
        public void EnterTextBox(CustomTextBox customtextBox)
        {
            try
            {
                if (ValueCheck(Int32.Parse(customtextBox.Text)))
                {
                    // フォーカスアウトする
                    Form frm = customtextBox.FindForm();
                    frm.ActiveControl = null;
                }
                else
                {
                    customtextBox.Text = "";
                }
            }
            catch
            {
                customtextBox.Text = "";
            }
        }

        // テキストボックスのクリックイベントを設定する
        public void ClickTextBoxEvent(CustomTextBox customtextBox)
        {
            customtextBox.Click += delegate(Object sender, EventArgs e)
            {
                if (Program.SystemData.tachpanel == false) return;

                CustomTextBox ct = (CustomTextBox)sender;
                TenkeyControl tc = tenkey;
                tc.tenKeyData.obj = sender;
                if (ct.Text != "")
                {
                    tc.tenKeyData.val = double.Parse(ct.Text);
                }
                else
                {
                    tc.tenKeyData.val = 0;
                }
                tc.tenkeyFormShow();

                // フォーカスアウトする
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;
            };
        }

        private bool ReadBarcode(string Barcode)
        {
            R_Quality qr = new R_Quality();
            SCR06DBController db = Program.SCR06DB;

            // 完了したデータを検索            
            int result = db.dbGetCompleteResultWorkData(Barcode);
            if (result == SystemConstants.ERR_NO_COMPLETE_RESULT_WORKDATA)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG051);
                txtBARCODE.Text = "";
                return false;
            }

            // 登録があったらそれを表示
            result = db.dbGetQualityRecord(Barcode, ref qr);
            if (result == SystemConstants.SQL_SUCCESS)
            {
                comboBox1.Text = qr.Item1;
                comboBox2.Text = qr.Item2;
                comboBox3.Text = qr.Item3;
                comboBox4.Text = qr.Item4;
                comboBox5.Text = qr.Item5;
                comboBox6.Text = qr.Item6;
                customTextBox1.Text = qr.Count1.ToString();
                customTextBox2.Text = qr.Count2.ToString();
                customTextBox3.Text = qr.Count3.ToString();
                customTextBox4.Text = qr.Count4.ToString();
                customTextBox5.Text = qr.Count5.ToString();
                customTextBox6.Text = qr.Count6.ToString();
            }

            return true;
        }

        private bool ValueCheck(int Value)
        {
            if (Value > 0 && Value < 100000)
            {
                return true;
            }
            return false;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (txtBARCODE.Text == "")
            {
                txtBARCODE.Focus();
                return;
            }

            // 確認メッセージ前に取らないとアクティブで消える
            R_Quality qr = new R_Quality();
            qr.Renban = txtBARCODE.Text;
            qr.Date = DateTime.Now;
            if (customTextBox1.Text != "")
            {
                qr.Item1 = comboBox1.Text;
                qr.Count1 = Int32.Parse(customTextBox1.Text);
            }
            if (customTextBox2.Text != "")
            {
                qr.Item2 = comboBox2.Text;
                qr.Count2 = Int32.Parse(customTextBox2.Text);
            }
            if (customTextBox3.Text != "")
            {
                qr.Item3 = comboBox3.Text;
                qr.Count3 = Int32.Parse(customTextBox3.Text);
            }
            if (customTextBox4.Text != "")
            {
                qr.Item4 = comboBox4.Text;
                qr.Count4 = Int32.Parse(customTextBox4.Text);
            }
            if (customTextBox5.Text != "")
            {
                qr.Item5 = comboBox5.Text;
                qr.Count5 = Int32.Parse(customTextBox5.Text);
            }
            if (customTextBox6.Text != "")
            {
                qr.Item6 = comboBox6.Text;
                qr.Count6 = Int32.Parse(customTextBox6.Text);
            }

            if(Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG052) == true)
            {
                RegisterQualityRecord(qr);
                Visible = false;
            }
        }

        private void RegisterQualityRecord(R_Quality QualityRecord)
        {
            SCR06DBController db = Program.SCR06DB;

            if (db.dbUpdateQualityRecord(QualityRecord) == SystemConstants.ERR_QUALITY_RECORD_UPDATE)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG053);
            }

        }

        private void qualityRecordfrm_Shown(object sender, EventArgs e)
        {
            txtBARCODE.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
            comboBox6.Text = "";
            customTextBox1.Text = "";
            customTextBox2.Text = "";
            customTextBox3.Text = "";
            customTextBox4.Text = "";
            customTextBox5.Text = "";
            customTextBox6.Text = "";

            txtBARCODE.Focus();
        }
    }
}
