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

        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // セルのクリックイベント
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_WORKDATA, workDetailItemView);

            // テンキーからの入力完了イベント
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
                // 名称を取得する(どうやるか？）
                name = Utility.GetMessageString(SystemConstants.WORK_MSG, workid);

                // 範囲を取得する
                dataController.GetWorkDataRangeStr(workid, ref min, ref max);

                // 値を取得する
                dataController.ReadWorkDataStr(workid, ref value);

                //単位を取得する。
                Program.DataController.GetWorkDataUnit(workid, ref unit);

                // 値を設定する
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

                // 値を取得する
                Program.DataController.ReadWorkDataStr(workid, ref value);

                var cell = workDetailItemView.Rows[y].Cells[3];

                // 値が編集中でなければ、値を変更する
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

        // 閉じるボタンを押下した時の処理
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
            //表示されているコントロールがDataGridViewTextBoxEditingControlか調べる
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集のために表示されているコントロールを取得
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //イベントハンドラを削除
                tb.KeyPress -=
                    new KeyPressEventHandler(dataGridViewTextBox_KeyPress);

                //該当する列か調べる
                if (dgv.CurrentCell.OwningColumn.Name == "Value")
                {
                    //KeyPressイベントハンドラを追加
                    tb.KeyPress +=
                        new KeyPressEventHandler(dataGridViewTextBox_KeyPress);
                }
            }
        }

        //DataGridViewに表示されているテキストボックスのKeyPressイベントハンドラ
        private void dataGridViewTextBox_KeyPress(object sender,
            KeyPressEventArgs e)
        {
            //数字しか入力できないようにする
            if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar != '.') && (e.KeyChar != '\b'))
            {
                e.Handled = true;
            }
        }

        // セルの入力チェックメソッド
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
            /*object value;				/* 値 */
            /*double outValue = 0;		/* 値をdoubleに変換したもの */
            /*string errMessage = "";		/* エラーメッセージ */

            /*DataGridView view = workDetailItemView;
            if (e.ColumnIndex == 3) {
                // WorkIDと値をグリッドビューから取得する
                workID = Int32.Parse(view.Rows[e.RowIndex].Cells[0].Value.ToString());
                value = e.FormattedValue;

                // Enterが押された時
                bool checkResult = Program.MainForm.checkTextBoxValue(
                    SystemConstants.WORKID_TYPE_WORKDATA,
                    workID,
                    value,
                    out outValue,
                    out errMessage
                );

                // チェックエラーの場合、メッセージを表示する
                if (checkResult == false)
                {
                    Utility.ShowErrorMsg(errMessage);
                    return;
                }

                // 変更を送信する
                mainfrm.WriteWorkData(workID, outValue);             
            }*/
        }

        // Ｅｎｔｅｒキーを押されデータを更新する
        private void EnterDataGridView(int WorkIDType, int WorkID, int RowIndex, object value)
        {
            double outValue = 0;		/* 値をdoubleに変換したもの */
            string errMessage = "";		/* エラーメッセージ */
            DataGridView view = workDetailItemView;

            // Enterが押された時
            bool checkResult = Program.MainForm.checkTextBoxValue(
                SystemConstants.WORKID_TYPE_WORKDATA,
                WorkID,
                value,
                out outValue,
                out errMessage
            );

            // チェックエラーの場合、メッセージを表示する
            if (checkResult == false)
            {
                Utility.ShowErrorMsg(errMessage);
                return;
            }

            // 変更を送信する
            mainfrm.WriteWorkData(WorkID, outValue);             
        }

        // テンキーから入力完了イベント
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

        // データグリッドのクリックイベントを設定する
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

                // フォーカスアウトする
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