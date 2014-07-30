using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class bankOperationfrm : Form
    {
        const int GRID_ROW = 10;

        private TenkeyControl tenkey;

        // �������ݒ�
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // �e���L�[����̓��͊����C�x���g
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);
        }

        public bankOperationfrm()
        {
            InitializeComponent();

            ClickTextBoxEvent(textCopy2);
        }

        // �e���L�[������͊����C�x���g
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

        // �e�L�X�g�{�b�N�X�̃N���b�N�C�x���g��ݒ肷��
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

                // �t�H�[�J�X�A�E�g����
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;
            };
        }

        /// <summary>
        /// �o���N�I������
        /// </summary>
        private void selectBank()
        {
            //int selectno = bankOperationView.CurrentRow.Index;
            int selectno = currentBankNo();

            loadBank(selectno);
        }

        /// <summary>
        /// �o���N�ǂݍ��ݏ���
        /// </summary>
        private void loadBank(int BankNo)
        {
            // selectedno��ݒ肷��
            mainfrm.BankNoWrite(BankNo);

            // �o���N�f�[�^�����[�h����
            int result = mainfrm.BankDataLoad(BankNo);

            // �o���N�f�[�^���Z�[�u����
            result = mainfrm.BankDataSave(BankNo);

            // �\�����X�V����
            lblNowBankNo2.Text = (BankNo + 1).ToString();
            Utility.ShowErrorCode(result);

            // �t�H�[�������
            Visible = false;
        }

        // �I���{�^���������̏���
        private void button1_Click(object sender, EventArgs e)
        {
            // �^�]���͑���s��
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return;
            
            selectBank();
        }

        // �R�s�[�{�^���������̏���
        private void btnCopy_Click(object sender, EventArgs e)
        {
            string bankComment = "";
            string bankWireName = "";
            string bankWireLength = "";
            string bankStrip1 = "";
            string bankStrip2 = "";

            // �R�s�[��̃o���N�i���o�[��0�ȉ��A101�ȏ�̐��������͂��ꂽ�ꍇ
            if (textCopy2.Text != "")
            {
                //�R�s�[��o���N�i���o�[�擾
                int bankToCopyNo = Int32.Parse(textCopy2.Text);

                //���ёւ����Ă��w��̃o���N�ԍ�����C���f�b�N�X���擾�i+1�͋������ɍ��킹�āj
                bankToCopyNo = bankNoIndex(bankToCopyNo.ToString()) + 1;

                // �R�s�[���ƃR�s�[�悪�����������ꍇ
                //if (bankOperationView.CurrentRow.Index == (bankToCopyNo - 1))
                if (currentBankNo() == (bankToCopyNo - 1))
                {
                    // �����Ȃ�
                    return;
                }

                if (bankToCopyNo <= 0 || bankToCopyNo >= SystemConstants.BANK_MAX + 2)
                {
                    // �͈͊O�ł���|�y�ѓ��͔͈͂̃��b�Z�[�W��\������
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG026, lblCopy1.Text, 1, SystemConstants.BANK_MAX + 1);
                }
                else
                {                    
                    // �I������Ă���o���NNo���w�肳�ꂽ�o���NNo�ɃR�s�[
                    //Program.DataController.CopyBankData(bankOperationView.CurrentRow.Index, (bankToCopyNo - 1));
                    Program.DataController.CopyBankData(currentBankNo(), (bankToCopyNo - 1));

                    // �R�s�[��̃o���N�f�[�^����o���N�R�����g���擾
                    Program.DataController.BankDataCommentRead((bankToCopyNo - 1), ref bankComment);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankWireName, 0);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankWireLength, 1);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankStrip1, 2);
                    Program.DataController.BankDataItemRead((bankToCopyNo - 1), ref bankStrip2, 3);

                    // �R�s�[��̃o���N�R�����g�̕\�����X�V
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

        // ����{�^���������̏���
        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        // �t�H�[�����鏈��
        private void bankOperationfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }


        // Visible��ԕύX���̏���
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

                // ���݂���Ă���o���N�i���o�[���擾
                Program.DataController.BankNoRead(ref selectedno);
                
                // ���ݑI������Ă���o���N�i���o�[�����݃o���NNo�ɐݒ�
                lblNowBankNo2.Text = (selectedno + 1).ToString();

                this.SuspendLayout();

                // �O���b�h�r���[�̒��g��S�ċ�ɂ���
                bankOperationView.RowCount = 0;

                for (int i = 0; i < SystemConstants.BANK_MAX + 1; i++)
                {
                    // i�Ԗڂ̃o���N�R�����g���擾
                    int result = Program.DataController.BankDataCommentRead(i, ref bankComment);
                        result = Program.DataController.BankDataItemRead(i, ref bankWireName, 0);
                        result = Program.DataController.BankDataItemRead(i, ref bankWireLength, 1);
                        result = Program.DataController.BankDataItemRead(i, ref bankStrip1, 2);
                        result = Program.DataController.BankDataItemRead(i, ref bankStrip2, 3);

                    // BankDataCommentRead����ERR_NO_BANK_DATA���Ԃ��Ă����ꍇ
                    if (result == SystemConstants.ERR_NO_BANK_DATA)
                    {
                        bankOperationView.Rows.Add(new Object[] { (i + 1).ToString(), "", "", "", "", "[No Bank Data]" });
                    }
                    // BankDataCommentRead�œǂݍ��߂��ꍇ
                    else
                    {
                        bankOperationView.Rows.Add(new Object[] { (i + 1).ToString(), bankWireName, bankWireLength, bankStrip1, bankStrip2, bankComment });
                    }
                }

                // �I������������
                bankOperationView.ClearSelection();

                // �s��I��
                bankOperationView.CurrentCell = bankOperationView[1, selectedno];
                bankOperationView.Rows[selectedno].Selected = true;                

                this.ResumeLayout();
            }
        }

        // �Z�����_�u���N���b�N�������̏����i�I�������j
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
            // �e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            if (checkTextBox(SystemConstants.WIRE_LENGTH1, textBox1) == false)
            {
                return;
            }

            // �e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            if (checkTextBox(SystemConstants.STRIP_LENGTH1, textBox2) == false)
            {
                return;
            }

            // �e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            if (checkTextBox(SystemConstants.STRIP_LENGTH2, textBox3) == false)
            {
                return;
            }

            int lastno = 0;
            int result = mainfrm.BankLastNoRead(ref lastno);
            int selectedNo = 0;

            // ����ɏ������s��ꂽ�ꍇ
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                button2.Text = lastno.ToString();

                //+1���Ēǉ�
                loadBank(lastno + 1);

                // ���ݑI������Ă���o���NNo���擾
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
            
            // �e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            if (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, WorkID, refTextBox.Text, out value, out message) == false)
            {
                Utility.ShowErrorMsg(message);
                return false;
            }
            return true;
        }

        // �O���b�h�y�[�W�A�b�v
        private void button3_Click(object sender, EventArgs e)
        {
            int i = bankOperationView.FirstDisplayedScrollingRowIndex - GRID_ROW;
            if (i < 0) i = 0;
            bankOperationView.FirstDisplayedScrollingRowIndex = i;
        }

        // �O���b�h�y�[�W�_�E��
        private void button4_Click(object sender, EventArgs e)
        {
            int i = bankOperationView.FirstDisplayedScrollingRowIndex + GRID_ROW;
            if (i <= bankOperationView.RowCount - 1)
                bankOperationView.FirstDisplayedScrollingRowIndex = i;
        }

        /// <summary>
        /// ���݂̃o���N�ԍ��擾
        /// ����U��ꂽ�ԍ���Ԃ�
        /// </summary>
        private int currentBankNo()
        {
            string bankno = bankOperationView.Rows[bankOperationView.CurrentRow.Index].Cells[0].Value.ToString();
            
            return Int16.Parse(bankno) - 1;
        }

        /// <summary>
        /// �w��̃o���N�ԍ��̃C���f�b�N�X���擾
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
        /// �\�[�g����
        /// �����A�~�����w�肵�ēd�����Ń\�[�g
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
        /// �f�[�^�O���b�h�\�[�g����
        ///     �󗓂̓\�[�g���Ȃ�
        ///     �d�����͕�����r�A�d�����ƃo���NNo�͐��l��r
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

                // �󗓂𖳎����Č���
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

        // ���ёւ������\�������ɖ߂�
        private void btnReset_Click(object sender, EventArgs e)
        {
            bankOperationfrm_VisibleChanged(sender, e);
        }

        // �o���N�f�[�^�폜�{�^��
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // �^�]���͑���s��
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return;

            // �폜�m�F���b�Z�[�W
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG059) == true)
            {
                selectDeleteBank();

                //�\���̍X�V
                bankOperationfrm_VisibleChanged(sender, e);
            }
        }

        /// <summary>
        /// �폜�o���N�I������
        /// </summary>
        private void selectDeleteBank()
        {
            int selectno = currentBankNo();
            int selectedno = -1;
            
            // ���݂���Ă���o���N�i���o�[���擾
            Program.DataController.BankNoRead(ref selectedno);

            if (selectno == selectedno)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG060);
                return;
            }

            deleteBank(selectno);
        }

        /// <summary>
        /// �o���N�폜����
        /// </summary>
        private void deleteBank(int BankNo)
        {
            mainfrm.BankDataDelete(BankNo);
        }

    }
}