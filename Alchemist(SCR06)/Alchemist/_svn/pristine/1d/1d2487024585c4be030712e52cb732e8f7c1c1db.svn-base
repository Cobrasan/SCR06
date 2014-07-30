using System;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class workDetailSpeedfrm : Form
    {
        private TenkeyControl tenkey;

        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            // クリックイベント
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED_THRES1, textFEED_SPEED_THRES1);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED_THRES2, textFEED_SPEED_THRES2);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED1, textFEED_SPEED1);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED2, textFEED_SPEED2);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED3, textFEED_SPEED3);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_ACCEL1, textFEED_ACCEL1);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_ACCEL2, textFEED_ACCEL2);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_ACCEL3, textFEED_ACCEL3);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH_CORRECT1, textWIRE_LENGTH_CORRECT11);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH_CORRECT2, textWIRE_LENGTH_CORRECT12);
            ClickTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH_CORRECT3, textWIRE_LENGTH_CORRECT13);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_WORKDATA, workDetailSpeedView);

            // テンキーからの入力完了イベント
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);

        }

        public workDetailSpeedfrm()
        {
            InitializeComponent();
        }

        private void workDetailSpeedfrm_Shown(object sender, EventArgs e)
        {
            int[] ID = null;

            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED1, textFEED_SPEED1);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_ACCEL1, textFEED_ACCEL1);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH_CORRECT1, textWIRE_LENGTH_CORRECT11);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED2, textFEED_SPEED2);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_ACCEL2, textFEED_ACCEL2);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH_CORRECT2, textWIRE_LENGTH_CORRECT12);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED3, textFEED_SPEED3);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_ACCEL3, textFEED_ACCEL3);
            Program.MainForm.SetTextBoxEvent(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH_CORRECT3, textWIRE_LENGTH_CORRECT13);
            Program.MainForm.SetTextBoxLength(this, 10);

            workDetailSpeedView.CurrentCell = null;
            Program.DataController.GetMemryDataGroupList(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WORK_GROUP_SPEED1, ref ID);

            string min = "";
            string max = "";
            string value = "";
            string name = "";
            string unit = "";

            foreach (var workid in ID)
            {
                // 名称を取得する(どうやるか？）
                name = Utility.GetMessageString(SystemConstants.WORK_MSG, workid);

                // 範囲を取得する
                Program.DataController.GetWorkDataRangeStr(workid, ref min, ref max);

                // 値を取得する
                Program.DataController.ReadWorkDataStr(workid, ref value);

                //単位を取得する。
                Program.DataController.GetWorkDataUnit(workid, ref unit);

                // 値を設定する
                workDetailSpeedView.Rows.Add(new Object[] { workid, name, string.Format("{0} - {1}", new object[] { min, max }), value, unit });
            }
        }

        private void workDetailSpeedfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        public void refresh()
        {
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED_THRES1, textFEED_SPEED_THRES1);
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED_THRES2, textFEED_SPEED_THRES2);
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED_THRES1, lblFEED_SPEED_THRES2, " -");
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED_THRES2, lblFEED_SPEED_THRES3, " -");
            //Program.MainForm.refreshControl(SystemConstants.FEED_SPEED_THRES1, textFEED_SPEED_THRES1);
            //Program.MainForm.refreshControl(SystemConstants.FEED_SPEED_THRES2, textFEED_SPEED_THRES2);
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED1, textFEED_SPEED1);
            Program.MainForm.refreshControl(SystemConstants.FEED_ACCEL1, textFEED_ACCEL1);
            Program.MainForm.refreshControl(SystemConstants.WIRE_LENGTH_CORRECT1, textWIRE_LENGTH_CORRECT11);
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED2, textFEED_SPEED2);
            Program.MainForm.refreshControl(SystemConstants.FEED_ACCEL2, textFEED_ACCEL2);
            Program.MainForm.refreshControl(SystemConstants.WIRE_LENGTH_CORRECT2, textWIRE_LENGTH_CORRECT12);
            Program.MainForm.refreshControl(SystemConstants.FEED_SPEED3, textFEED_SPEED3);
            Program.MainForm.refreshControl(SystemConstants.FEED_ACCEL3, textFEED_ACCEL3);
            Program.MainForm.refreshControl(SystemConstants.WIRE_LENGTH_CORRECT3, textWIRE_LENGTH_CORRECT13);

            int rowCount = workDetailSpeedView.Rows.Count;
            string value = "";

            //GridViewの更新ｓ
            for (int y = 0; y < rowCount; y++)
            {
                int workid = Int32.Parse(workDetailSpeedView.Rows[y].Cells[0].Value.ToString());

                // 値を取得する
                Program.DataController.ReadWorkDataStr(workid, ref value);

                var cell = workDetailSpeedView.Rows[y].Cells[3];

                // 値が編集中でなければ、値を変更する
                if (!cell.IsInEditMode)
                {
                    workDetailSpeedView.Rows[y].Cells[3].Value = value;
                }
            }
        }

        private void workDetailSpeedView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (Program.SystemData.tachpanel == true) return;
            if (e.ColumnIndex != 3) return;

            DataGridView view = workDetailSpeedView;
            int workidtype = SystemConstants.WORKID_TYPE_WORKDATA;
            int workid = Int32.Parse(view.Rows[e.RowIndex].Cells[0].Value.ToString());
            int rowindex = e.RowIndex;
            object value = e.FormattedValue;

            EnterDataGridView(workidtype, workid, rowindex, value);

            /*int workID = 0;				/* WorkID */
            /*object value;				/* 値 */
            /*double outValue = 0;		/* 値をdoubleに変換したもの */
            /*string errMessage = "";		/* エラーメッセージ */

            /*DataGridView view = workDetailSpeedView;
            if (e.ColumnIndex == 3)
            {
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

        // 閾値２の確定処理
        private void textFEED_SPEED_THRES2_EnterKeyDown(EventArgs e)
        {
            /*double workdata = 0;
            string errMessage;
            double outValue;

            // 形式チェック
            if (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED_THRES2, textFEED_SPEED_THRES2.Text, out outValue, out errMessage) == false)
            {
                Utility.ShowErrorMsg(errMessage);
                return;
            }

            // 閾値１を読む
            Program.DataController.ReadWorkData(SystemConstants.FEED_SPEED_THRES1, ref workdata);

            // 範囲チェック
            if (outValue <= workdata)
            {
                string workname = Utility.GetMessageString(SystemConstants.WORK_MSG, SystemConstants.FEED_SPEED_THRES2);
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG022, workname, lblFEED_SPEED_THRES2.Text);
                return;
            }

            // ワークデータを書き込む
            mainfrm.WriteWorkData(SystemConstants.FEED_SPEED_THRES2, outValue);
            */

            EnterTextBox(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED_THRES2, textFEED_SPEED_THRES2);

            // フォーカスアウトする
            Form frm = textFEED_SPEED_THRES2.FindForm();
            frm.ActiveControl = null;
        }

        // 閾値１の確定処理
        private void textFEED_SPEED_THRES1_EnterKeyDown_1(EventArgs e)
        {
            /*double outValue;
            string errMessage;
            double workdata = 0;

            // 形式チェック
            if (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED_THRES1, textFEED_SPEED_THRES1.Text, out outValue, out errMessage) == false)
            {
                Utility.ShowErrorMsg(errMessage);
                return;
            }

            // 閾値２を読む
            Program.DataController.ReadWorkData(SystemConstants.FEED_SPEED_THRES2, ref workdata);

            // 適正範囲チェック
            if (textFEED_SPEED_THRES1.Text == "0" || workdata <= outValue)
            {
                string workname = Utility.GetMessageString(SystemConstants.WORK_MSG, SystemConstants.FEED_SPEED_THRES2);
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG027, workname, "0", textFEED_SPEED_THRES2.Text);
                return;
            }

            // ワークデータを書き込む
            mainfrm.WriteWorkData(SystemConstants.FEED_SPEED_THRES1, outValue);
            */

            EnterTextBox(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.FEED_SPEED_THRES1, textFEED_SPEED_THRES1);

            // フォーカスアウトする
            Form frm = textFEED_SPEED_THRES1.FindForm();
            frm.ActiveControl = null;
        }

        // テキストボックスでエンターキー押されたときの処理
        public void EnterTextBox(int workIDType, int WorkID, CustomTextBox customtextBox)
        {
            double value;
            string message;
            double workdata = 0;

            // 形式チェック
            if (Program.MainForm.checkTextBoxValue(workIDType, WorkID, customtextBox.Text, out value, out message) == false)
            {
                Utility.ShowErrorMsg(message);
                return;
            }

            switch (WorkID)
            {
                case SystemConstants.FEED_SPEED_THRES1:

                    // 閾値２を読む
                    Program.DataController.ReadWorkData(SystemConstants.FEED_SPEED_THRES2, ref workdata);

                    // 適正範囲チェック
                    if (customtextBox.Text == "0" || workdata <= value)
                    {
                        string workname = Utility.GetMessageString(SystemConstants.WORK_MSG, SystemConstants.FEED_SPEED_THRES2);
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG027, workname, "0", textFEED_SPEED_THRES2.Text);
                        return;
                    }

                    break;
                case SystemConstants.FEED_SPEED_THRES2:

                    // 閾値１を読む
                    Program.DataController.ReadWorkData(SystemConstants.FEED_SPEED_THRES1, ref workdata);

                    // 範囲チェック
                    if (value <= workdata)
                    {
                        string workname = Utility.GetMessageString(SystemConstants.WORK_MSG, SystemConstants.FEED_SPEED_THRES2);
                        Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG022, workname, lblFEED_SPEED_THRES2.Text);
                        return;
                    }

                    break;
            }

            // ワークデータを書き込む
            mainfrm.WriteWorkData(WorkID, value);
        }

        // Ｅｎｔｅｒキーを押されデータを更新する
        private void EnterDataGridView(int WorkIDType, int WorkID, int RowIndex, object value)
        {
            double outValue = 0;		/* 値をdoubleに変換したもの */
            string errMessage = "";		/* エラーメッセージ */
            DataGridView view = workDetailSpeedView;

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
                case "CustomTextBox":
                    CustomTextBox ct = (CustomTextBox)td.obj;
                    ct.Text = td.val.ToString();
                    EnterTextBox(td.workidtype, td.workid, ct);
                    break;
                case "DataGridView":
                    DataGridView dg = (DataGridView)td.obj;
                    td.value = td.val;
                    EnterDataGridView(td.workidtype, td.workid, td.rowindex, td.value);
                    dg.Rows[td.rowindex].Cells[td.columindex].Value = td.val;
                    string a = dg.Name;
                    break;

            }
        }

        // テキストボックスのクリックイベントを設定する
        private void ClickTextBoxEvent(int workIDType, int WorkID, CustomTextBox customtextBox)
        {
            customtextBox.Click += delegate(Object sender, EventArgs e)
            {
                if (Program.SystemData.tachpanel == false) return;

                CustomTextBox ct = (CustomTextBox)sender;
                TenkeyControl tc = tenkey;
                tc.tenKeyData.obj = sender;
                tc.tenKeyData.val = double.Parse(ct.Text);
                tc.tenKeyData.workid = WorkID;
                tc.tenKeyData.workidtype = workIDType;
                tc.tenKeyData.itemtype = 0;
                tc.tenkeyFormShow();

                // フォーカスアウトする
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;
            };
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
}