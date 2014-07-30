using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class workDetailItemfrm : Form
    {
        protected int Group = SystemConstants.WORK_GROUP_WIRE1;
        private TenkeyControl tenkey;

        public workDetailItemfrm()
        {
            InitializeComponent();
        }

        // �������ݒ�
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // �Z���̃N���b�N�C�x���g
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_WORKDATA, workDetailItemView);

            // �e���L�[����̓��͊����C�x���g
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);
        }

        public void insertData(int Group)
        {
            int[] workIDs = null;

            workDetailItemView.Rows.Clear();

            DataController dataController = Program.DataController;
            dataController.GetMemryDataGroupList(SystemConstants.WORKID_TYPE_WORKDATA, Group, ref workIDs);

            string min = "";
            string max = "";
            string value = "";
            string name = "";
            string unit = "";

            foreach (var workid in workIDs)
            {
                // ���̂��擾����(�ǂ���邩�H�j
                name = Utility.GetMessageString(SystemConstants.WORK_MSG, workid);

                // �͈͂��擾����
                dataController.GetWorkDataRangeStr(workid, ref min, ref max);

                // �l���擾����
                dataController.ReadWorkDataStr(workid, ref value);

                //�P�ʂ��擾����B
                Program.DataController.GetWorkDataUnit(workid, ref unit);

                // �l��ݒ肷��
                workDetailItemView.Rows.Add(new Object[] { workid, name, string.Format("{0} - {1}", new object[] { min, max }), value, unit });
            }
        }

        public void refresh()
        {
            int rowCount = workDetailItemView.Rows.Count;
            string value = "";

            for (int y = 0; y < rowCount; y++)
            {
                int workid = Int32.Parse(workDetailItemView.Rows[y].Cells[0].Value.ToString());

                // �l���擾����
                Program.DataController.ReadWorkDataStr(workid, ref value);

                var cell = workDetailItemView.Rows[y].Cells[3];

                // �l���ҏW���łȂ���΁A�l��ύX����
                if (!cell.IsInEditMode)
                {
                    workDetailItemView.Rows[y].Cells[3].Value = value;
                }
            }
        }

        private void workDetailItemfrm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < workDetailItemView.Columns.Count; i++)
            {
                workDetailItemView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            insertData(Group);
        }

        private void workDetailItemfrm_Shown(object sender, EventArgs e)
        {
            workDetailItemView.CurrentCell = null;
        }

        // ����{�^���������������̏���
        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void workDetailItemfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void workDetailItemView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //�\������Ă���R���g���[����DataGridViewTextBoxEditingControl�����ׂ�
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridView dgv = (DataGridView)sender;

                //�ҏW�̂��߂ɕ\������Ă���R���g���[�����擾
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //�C�x���g�n���h�����폜
                tb.KeyPress -=
                    new KeyPressEventHandler(dataGridViewTextBox_KeyPress);

                //�Y������񂩒��ׂ�
                if (dgv.CurrentCell.OwningColumn.Name == "Value")
                {
                    //KeyPress�C�x���g�n���h����ǉ�
                    tb.KeyPress +=
                        new KeyPressEventHandler(dataGridViewTextBox_KeyPress);
                }
            }
        }

        //DataGridView�ɕ\������Ă���e�L�X�g�{�b�N�X��KeyPress�C�x���g�n���h��
        private void dataGridViewTextBox_KeyPress(object sender,
            KeyPressEventArgs e)
        {
            //�����������͂ł��Ȃ��悤�ɂ���
            if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar != '.') && (e.KeyChar != '\b'))
            {
                e.Handled = true;
            }
        }

        // �Z���̓��̓`�F�b�N���\�b�h
        private void workDetailItemView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (Program.SystemData.tachpanel == true) return;
            if (e.ColumnIndex != 3) return;

            DataGridView view = workDetailItemView;
            int workidtype = SystemConstants.WORKID_TYPE_WORKDATA;
            int workid = Int32.Parse(view.Rows[e.RowIndex].Cells[0].Value.ToString());
            int rowindex = e.RowIndex;
            object value = e.FormattedValue;

            EnterDataGridView(workidtype, workid, rowindex, value);

            /*int workID = 0;				/* WorkID */
            /*object value;				/* �l */
            /*double outValue = 0;		/* �l��double�ɕϊ��������� */
            /*string errMessage = "";		/* �G���[���b�Z�[�W */

            /*DataGridView view = workDetailItemView;
            if (e.ColumnIndex == 3) {
                // WorkID�ƒl���O���b�h�r���[����擾����
                workID = Int32.Parse(view.Rows[e.RowIndex].Cells[0].Value.ToString());
                value = e.FormattedValue;

                // Enter�������ꂽ��
                bool checkResult = Program.MainForm.checkTextBoxValue(
                    SystemConstants.WORKID_TYPE_WORKDATA,
                    workID,
                    value,
                    out outValue,
                    out errMessage
                );

                // �`�F�b�N�G���[�̏ꍇ�A���b�Z�[�W��\������
                if (checkResult == false)
                {
                    Utility.ShowErrorMsg(errMessage);
                    return;
                }

                // �ύX�𑗐M����
                mainfrm.WriteWorkData(workID, outValue);             
            }*/
        }

        // �d���������L�[��������f�[�^���X�V����
        private void EnterDataGridView(int WorkIDType, int WorkID, int RowIndex, object value)
        {
            double outValue = 0;		/* �l��double�ɕϊ��������� */
            string errMessage = "";		/* �G���[���b�Z�[�W */
            DataGridView view = workDetailItemView;

            // Enter�������ꂽ��
            bool checkResult = Program.MainForm.checkTextBoxValue(
                SystemConstants.WORKID_TYPE_WORKDATA,
                WorkID,
                value,
                out outValue,
                out errMessage
            );

            // �`�F�b�N�G���[�̏ꍇ�A���b�Z�[�W��\������
            if (checkResult == false)
            {
                Utility.ShowErrorMsg(errMessage);
                return;
            }

            // �ύX�𑗐M����
            mainfrm.WriteWorkData(WorkID, outValue);             
        }

        // �e���L�[������͊����C�x���g
        private void TenKeyEnterEvent(TenKeyData td)
        {
            switch (td.obj.GetType().Name)
            {
                case "DataGridView":
                    DataGridView dg = (DataGridView)td.obj;
                    td.value = td.val;
                    EnterDataGridView(td.workidtype, td.workid, td.rowindex, td.value);
                    dg.Rows[td.rowindex].Cells[td.columindex].Value = td.val;
                    string a = dg.Name;
                    break;

            }
        }

        // �f�[�^�O���b�h�̃N���b�N�C�x���g��ݒ肷��
        private void ClickDataGridViewCell(int workIDType, DataGridView dataGridView)
        {
            dataGridView.CellClick += delegate(object sender, DataGridViewCellEventArgs e)
            {
                if (Program.SystemData.tachpanel == false) return;
                if (e.ColumnIndex != 3) return;
                if (e.RowIndex < 0) return;

                DataGridView dg = (DataGridView)sender;
                TenkeyControl tc = tenkey;

                if (dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                }

                tc.tenKeyData.obj = sender;
                tc.tenKeyData.val = double.Parse(dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                tc.tenKeyData.rowindex = e.RowIndex;
                tc.tenKeyData.columindex = e.ColumnIndex;
                tc.tenKeyData.workid = Int32.Parse(dg.Rows[e.RowIndex].Cells[0].Value.ToString()); ;
                tc.tenKeyData.workidtype = workIDType;
                tc.tenkeyFormShow();

                // �t�H�[�J�X�A�E�g����
                dg.CurrentCell = null;
            };
        }

    }

	// WORK_GROUP_WIRE1
	public class workDetailItemfrmWIRE1 : workDetailItemfrm
	{
		public workDetailItemfrmWIRE1() : base() {
			Group = SystemConstants.WORK_GROUP_WIRE1;
		}
	}

	// WORK_GROUP_STRIP1
	public class workDetailItemfrmSTRIP1 : workDetailItemfrm
	{
		public workDetailItemfrmSTRIP1()
			: base()
		{
			Group = SystemConstants.WORK_GROUP_STRIP1;
		}
	}
	// WORK_GROUP_STRIP2
	public class workDetailItemfrmSTRIP2 : workDetailItemfrm
	{
		public workDetailItemfrmSTRIP2()
			: base()
		{
			Group = SystemConstants.WORK_GROUP_STRIP2;
		}
	}

}