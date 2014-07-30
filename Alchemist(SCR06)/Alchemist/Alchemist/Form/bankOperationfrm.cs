using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class bankOperationfrm : Form
    {
        const int GRID_ROW = 10;

        private TenkeyControl tenkey;

        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // テンキーからの入力完了イベント
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);
        }

        public bankOperationfrm()
        {
            InitializeComponent();

            ClickTextBoxEvent(textCopy2);
        }

        // テンキーから入力完了イベント
        private void TenKeyEnterEvent(TenKeyData td)
        {
            switch (td.obj.GetType().Name)
            {
                case "CustomTextBox":
                    CustomTextBox ct = (CustomTextBox)td.obj;
                    ct.Text = td.val.ToString();
                    break;
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

        /// <summary>
        /// バンク選択処理
        /// </summary>
        private void selectBank()
        {
            //int selectno = bankOperationView.CurrentRow.Index;
            int selectno = currentBankNo();

            loadBank(selectno);
        }

        /// <summary>
        /// バンク読み込み処理
        /// </summary>
        private void loadBank(int BankNo)
        {
            // selectednoを設定する
            mainfrm.BankNoWrite(BankNo);

            // バンクデータをロードする
            int result = mainfrm.BankDataLoad(BankNo);

            // バンクデータをセーブする
            result = mainfrm.BankDataSave(BankNo);

            // 表示を更新する
            lblNowBankNo2.Text = (BankNo + 1).ToString();
            Utility.ShowErrorCode(result);

            // フォームを閉じる
            Visible = false;
        }

        // 選択ボタン押下時の処理
        private void button1_Click(object sender, EventArgs e)
        {
            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return;
            
            selectBank();
        }

        // コピーボタン押下時の処理
        private void btnCopy_Click(object sender, EventArgs e)
        {
            string bankComment = "";
            string bankWireName = "";
            string bankWireLength = "";
            string bankStrip1 = "";
            string bankStrip2 = "";

            // コピー先のバンクナンバーが0以下、101以上の数字が入力された場合
            if (textCopy2.Text != "")
            {
                //コピー先バンクナンバー取得
                int bankToCopyNo = Int32.Parse(textCopy2.Text);

                //並び替えしても指定のバンク番号からインデックスを取得（+1は旧処理に合わせて）
                bankToCopyNo = bankNoIndex(bankToCopyNo.ToString()) + 1;

                // コピー元とコピー先が同じだった場合
                //if (bankOperationView.CurrentRow.Index == (bankToCopyNo - 1))
                if (currentBankNo() == (bankToCopyNo - 1))
                {
                    // 処理なし
                    return;
                }

                if (bankToCopyNo <= 0 || bankToCopyNo >= SystemConstants.BANK_MAX + 2)
                {
                    // 範囲外である旨及び入力範囲のメッセージを表示する
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG026, lblCopy1.Text, 1, SystemConstants.BANK_MAX + 1);
                }
                else
                {                    
                    // 選択されているバンクNoを指定されたバンクNoにコピー
                    //Program.DataController.CopyBankData(bankOperationView.CurrentRow.Index, (bankToCopyNo - 1));
                    Program.DataController.CopyBankData(currentBankNo(), (bankToCopyNo - 1));

                    // コピー先のバンクデータからバンクコメントを取得
                    Program.DataController.BankDataCommentRead((bankToCopyNo - 1), ref bankComment);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankWireName, 0);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankWireLength, 1);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankStrip1, 2);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankStrip2, 3);

                    // コピー先のバンクコメントの表示を更新
                    bankOperationView.Rows[bankToCopyNo - 1].Cells[5].Value = bankComment;
                    bankOperationView.Rows[bankToCopyNo - 1].Cells[1].Value = bankWireName;
                    bankOperationView.Rows[bankToCopyNo - 1].Cells[2].Value = bankWireLength;
                    bankOperationView.Rows[bankToCopyNo - 1].Cells[3].Value = bankStrip1;
                    bankOperationView.Rows[bankToCopyNo - 1].Cells[4].Value = bankStrip2;

                }
            }
            else
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG026, lblCopy1.Text, 1, SystemConstants.BANK_MAX + 1);
            }
        }

        // 閉じるボタン押下時の処理
        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        // フォーム閉じる処理
        private void bankOperationfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }


        // Visible状態変更時の処理
        private void bankOperationfrm_VisibleChanged(object sender, EventArgs e)
        {

            string bankComment = "";
            string bankWireName = "";
            string bankWireLength = "";
            string bankStrip1 = "";
            string bankStrip2 = "";

            int selectedno = 0;

            if (Visible == true)
            {

                // 現在されているバンクナンバーを取得
                Program.DataController.BankNoRead(ref selectedno);
                
                // 現在選択されているバンクナンバーを現在バンクNoに設定
                lblNowBankNo2.Text = (selectedno + 1).ToString();

                this.SuspendLayout();

                // グリッドビューの中身を全て空にする
                bankOperationView.RowCount = 0;

                for (int i = 0; i < SystemConstants.BANK_MAX + 1; i++)
                {
                    // i番目のバンクコメントを取得
                    int result = Program.DataController.BankDataCommentRead(i, ref bankComment);
                        result = Program.DataController.BankDataItemRead(i, ref bankWireName, 0);
                        result = Program.DataController.BankDataItemRead(i, ref bankWireLength, 1);
                        result = Program.DataController.BankDataItemRead(i, ref bankStrip1, 2);
                        result = Program.DataController.BankDataItemRead(i, ref bankStrip2, 3);

                    // BankDataCommentReadからERR_NO_BANK_DATAが返ってきた場合
                    if (result == SystemConstants.ERR_NO_BANK_DATA)
                    {
                        bankOperationView.Rows.Add(new Object[] { (i + 1).ToString(), "", "", "", "", "[No Bank Data]" });
                    }
                    // BankDataCommentReadで読み込めた場合
                    else
                    {
                        bankOperationView.Rows.Add(new Object[] { (i + 1).ToString(), bankWireName, bankWireLength, bankStrip1, bankStrip2, bankComment });
                    }
                }

                // 選択を解除する
                bankOperationView.ClearSelection();

                // 行を選択
                bankOperationView.CurrentCell = bankOperationView[1, selectedno];
                bankOperationView.Rows[selectedno].Selected = true;                

                this.ResumeLayout();
            }
        }

        // セルをダブルクリックした時の処理（選択処理）
        private void bankOperationView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            selectBank();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            BankAttributeStruct ba = new BankAttributeStruct();
            ba.WireLength = textBox1.Text;
            ba.Strip1Length = textBox2.Text;
            ba.Strip2Length = textBox3.Text;
            ba.WireCode = textBox4.Text;
            int bankno = 0;

            int result = mainfrm.BankNoRead(ba, ref bankno);

            if (result == SystemConstants.ERR_NO_BANK_DATA)
            {
                button1.Text = "-1";
            }
            else
            {
                button1.Text = bankno.ToString();

                loadBank(bankno);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // テキストボックスの入力チェック
            if (checkTextBox(SystemConstants.WIRE_LENGTH1, textBox1) == false)
            {
                return;
            }

            // テキストボックスの入力チェック
            if (checkTextBox(SystemConstants.STRIP_LENGTH1, textBox2) == false)
            {
                return;
            }

            // テキストボックスの入力チェック
            if (checkTextBox(SystemConstants.STRIP_LENGTH2, textBox3) == false)
            {
                return;
            }

            int lastno = 0;
            int result = mainfrm.BankLastNoRead(ref lastno);
            int selectedNo = 0;

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                button2.Text = lastno.ToString();

                //+1して追加
                loadBank(lastno + 1);

                // 現在選択されているバンクNoを取得
                Program.DataController.BankNoRead(ref selectedNo);

                mainfrm.WriteBankDataItem(selectedNo, SystemConstants.WIRE_LENGTH1, double.Parse(textBox1.Text), SystemConstants.BANKITEM_TYPE_WIRELENGTH);
                mainfrm.WriteBankDataItem(selectedNo, SystemConstants.STRIP_LENGTH1, double.Parse(textBox2.Text), SystemConstants.BANKITEM_TYPE_STRIP1);
                mainfrm.WriteBankDataItem(selectedNo, SystemConstants.STRIP_LENGTH2, double.Parse(textBox3.Text), SystemConstants.BANKITEM_TYPE_STRIP2);
            }
            else
            {
                button2.Text = "-1";
            }

        }

        private bool checkTextBox(int WorkID, TextBox refTextBox)
        {
            double value;
            string message = "";
            
            // テキストボックスの入力チェック
            if (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, WorkID, refTextBox.Text, out value, out message) == false)
            {
                Utility.ShowErrorMsg(message);
                return false;
            }
            return true;
        }

        // グリッドページアップ
        private void button3_Click(object sender, EventArgs e)
        {
            int i = bankOperationView.FirstDisplayedScrollingRowIndex - GRID_ROW;
            if (i < 0) i = 0;
            bankOperationView.FirstDisplayedScrollingRowIndex = i;
        }

        // グリッドページダウン
        private void button4_Click(object sender, EventArgs e)
        {
            int i = bankOperationView.FirstDisplayedScrollingRowIndex + GRID_ROW;
            if (i <= bankOperationView.RowCount - 1)
                bankOperationView.FirstDisplayedScrollingRowIndex = i;
        }

        /// <summary>
        /// 現在のバンク番号取得
        /// 割り振られた番号を返す
        /// </summary>
        private int currentBankNo()
        {
            string bankno = bankOperationView.Rows[bankOperationView.CurrentRow.Index].Cells[0].Value.ToString();
            
            return Int16.Parse(bankno) - 1;
        }

        /// <summary>
        /// 指定のバンク番号のインデックスを取得
        /// </summary>
        private int bankNoIndex(string bankno)
        {           
            for (int i = 0; i < SystemConstants.BANK_MAX+2; i++)
            {
                if (bankOperationView.Rows[i].Cells[0].Value.ToString() == bankno)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// ソート処理
        /// 昇順、降順を指定して電線名でソート
        /// </summary>
        private void btnSort_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                bankOperationView.Sort(new RowComparer(SortOrder.Ascending));
            }
            else if (radioButton2.Checked == true)
            {
                bankOperationView.Sort(new RowComparer(SortOrder.Descending));
            }
        }

        /// <summary>
        /// データグリッドソート処理
        ///     空欄はソートしない
        ///     電線名は文字比較、電線長とバンクNoは数値比較
        /// </summary>
        private class RowComparer : System.Collections.IComparer
        {
            private static int sortOrderModifier = 1;

            public RowComparer(SortOrder sortOrder)
            {
                if (sortOrder == SortOrder.Descending)
                {
                    sortOrderModifier = -1;
                }
                else if (sortOrder == SortOrder.Ascending)
                {
                    sortOrderModifier = 1;
                }
            }

            public int Compare(object x, object y)
            {
                DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
                DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

                // 空欄を無視して検索
                int CompareResult = 0;

                if (CompareResult == 0)
                {
                    if (sortOrderModifier == 1 && DataGridViewRow1.Cells[1].Value.ToString() == "" && (DataGridViewRow2.Cells[1].Value.ToString() == ""))
                        CompareResult = 0;
                    else if (sortOrderModifier == 1 && DataGridViewRow1.Cells[1].Value.ToString() != "" && (DataGridViewRow2.Cells[1].Value.ToString() == ""))
                        CompareResult = -1;
                    else if (sortOrderModifier == 1 && DataGridViewRow1.Cells[1].Value.ToString() == "" && (DataGridViewRow2.Cells[1].Value.ToString() != ""))
                        CompareResult = 1;
                    else
                    {
                        CompareResult = System.String.Compare(
                        DataGridViewRow1.Cells[1].Value.ToString(),
                        DataGridViewRow2.Cells[1].Value.ToString());
                    }
                }

                if (CompareResult == 0)
                {
                    if (sortOrderModifier == 1 && DataGridViewRow1.Cells[2].Value.ToString() == "" && (DataGridViewRow2.Cells[2].Value.ToString() == ""))
                        CompareResult = 0;
                    else if (sortOrderModifier == 1 && DataGridViewRow1.Cells[2].Value.ToString() != "" && (DataGridViewRow2.Cells[2].Value.ToString() == ""))
                        CompareResult = -1;
                    else if (sortOrderModifier == 1 && DataGridViewRow1.Cells[2].Value.ToString() == "" && (DataGridViewRow2.Cells[2].Value.ToString() != ""))
                        CompareResult = 1;
                    else
                    {
                        if (DataGridViewRow1.Cells[2].Value.ToString() != "" && (DataGridViewRow2.Cells[2].Value.ToString() != ""))
                        {
                            if (Int16.Parse(DataGridViewRow1.Cells[2].Value.ToString()) == Int16.Parse(DataGridViewRow2.Cells[2].Value.ToString()))
                                CompareResult = 0;
                            if (Int16.Parse(DataGridViewRow1.Cells[2].Value.ToString()) > Int16.Parse(DataGridViewRow2.Cells[2].Value.ToString()))
                                CompareResult = 1;
                            if (Int16.Parse(DataGridViewRow1.Cells[2].Value.ToString()) < Int16.Parse(DataGridViewRow2.Cells[2].Value.ToString()))
                                CompareResult = -1;
                        }
                        else
                        {
                            CompareResult = System.String.Compare(
                            DataGridViewRow1.Cells[2].Value.ToString(),
                            DataGridViewRow2.Cells[2].Value.ToString());
                        }
                    }
                }
                
                if (CompareResult == 0)
                {
                    if (Int16.Parse(DataGridViewRow1.Cells[0].Value.ToString()) == Int16.Parse(DataGridViewRow2.Cells[0].Value.ToString()))
                        CompareResult = 0;
                    if (Int16.Parse(DataGridViewRow1.Cells[0].Value.ToString()) > Int16.Parse(DataGridViewRow2.Cells[0].Value.ToString()))
                        CompareResult = 1;
                    if (Int16.Parse(DataGridViewRow1.Cells[0].Value.ToString()) < Int16.Parse(DataGridViewRow2.Cells[0].Value.ToString()))
                        CompareResult = -1;                 
                }

                return CompareResult * sortOrderModifier;
            }
        }

        // 並び替えした表示を元に戻す
        private void btnReset_Click(object sender, EventArgs e)
        {
            bankOperationfrm_VisibleChanged(sender, e);
        }

        // バンクデータ削除ボタン
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return;

            // 削除確認メッセージ
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG059) == true)
            {
                selectDeleteBank();

                //表示の更新
                bankOperationfrm_VisibleChanged(sender, e);
            }
        }

        /// <summary>
        /// 削除バンク選択処理
        /// </summary>
        private void selectDeleteBank()
        {
            int selectno = currentBankNo();
            int selectedno = -1;
            
            // 現在されているバンクナンバーを取得
            Program.DataController.BankNoRead(ref selectedno);

            if (selectno == selectedno)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG060);
                return;
            }

            deleteBank(selectno);
        }

        /// <summary>
        /// バンク削除処理
        /// </summary>
        private void deleteBank(int BankNo)
        {
            mainfrm.BankDataDelete(BankNo);
        }

    }
}