using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Linq;

namespace Alchemist
{
    public partial class systemConfigurationfrm : Form
    {
        private iocheckfrm iochecForm = new iocheckfrm(); /* メモリモニタ画面 */
        private TenkeyControl tenkey;

        // 初期化設定
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);

            //イベントを設定
            Program.MainForm.SetBtnEvent(SystemConstants.STRIP1_SENSOR_LOCK, SystemConstants.BTN_PUSH, btnSTRIPMISS1);

            //補正値データグリッドのイベントを設定
            setDataGridEvent(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_BASEMACHINE1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_DISENTANGLE1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_CUT1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_PULL1View);

            //タイミングデータグリッドのイベントを設定
            setDataGridEvent(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_FEEDCUT1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_PULL1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_STRIP1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_DISENTANGLE1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_TRIMMING1View);
            setDataGridEvent(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_DISCHARGE1View);

            // セルのクリックイベント
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_BASEMACHINE1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_DISENTANGLE1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_CUT1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_CORRECTDATA, systemConfiguration_CORR_GROUP_PULL1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_FEEDCUT1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_PULL1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_STRIP1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_DISENTANGLE1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_TRIMMING1View);
            ClickDataGridViewCell(SystemConstants.WORKID_TYPE_TIMINGDATA, systemConfiguration_TIMM_GROUP_DISCHARGE1View);

            // テンキーからの入力完了イベント
            tenkey = new TenkeyControl();
            tenkey.dataEneterEvent += new dataEneterDelegate(TenKeyEnterEvent);

        }

        public systemConfigurationfrm()
        {
            InitializeComponent();
        }

        private int portNameToInt(String name)
        {
            int ret = 0;

            try {
                ret = Int32.Parse(name.Replace("COM", ""));
            }
            catch {
                /* 無視 */
            }

            return ret;
        }

        private void btnMemoryMonitor_Click(object sender, EventArgs e)
        {
            iochecForm.Show();
        }

        public void refresh()
        {
            //検出タブの表示更新
            CheckBtnAnd_ChangeColor(SystemConstants.STRIP1_SENSOR_LOCK, btnSTRIPMISS1);

            //補正値・タイミング値の表示更新

            GridUpdate(systemConfiguration_CORR_GROUP_BASEMACHINE1View, SystemConstants.WORKID_TYPE_CORRECTDATA);
            GridUpdate(systemConfiguration_CORR_GROUP_DISENTANGLE1View, SystemConstants.WORKID_TYPE_CORRECTDATA);
            GridUpdate(systemConfiguration_CORR_GROUP_CUT1View, SystemConstants.WORKID_TYPE_CORRECTDATA);
            GridUpdate(systemConfiguration_CORR_GROUP_PULL1View, SystemConstants.WORKID_TYPE_CORRECTDATA);

            // タイミングカム用
            GridUpdateTimCam(systemConfiguration_TIMM_GROUP_FEEDCUT1View, SystemConstants.WORKID_TYPE_TIMINGDATA);
            GridUpdateTimCam(systemConfiguration_TIMM_GROUP_PULL1View, SystemConstants.WORKID_TYPE_TIMINGDATA);
            GridUpdateTimCam(systemConfiguration_TIMM_GROUP_STRIP1View, SystemConstants.WORKID_TYPE_TIMINGDATA);
            GridUpdateTimCam(systemConfiguration_TIMM_GROUP_DISENTANGLE1View, SystemConstants.WORKID_TYPE_TIMINGDATA);
            GridUpdateTimCam(systemConfiguration_TIMM_GROUP_TRIMMING1View, SystemConstants.WORKID_TYPE_TIMINGDATA);
            GridUpdateTimCam(systemConfiguration_TIMM_GROUP_DISCHARGE1View, SystemConstants.WORKID_TYPE_TIMINGDATA);
        }

        /// <summary>
        /// グリッドビューにデータを表示させる。
        /// </summary>
        /// <param name="view"></param>
        /// <param name="group"></param>
        private void viewDisp(DataGridView view, int type, int group)
        {
            int[] ID = null;

            view.CurrentCell = null;
            Program.DataController.GetMemryDataGroupList(type, group, ref ID);

            int rowCount = 0;
            string min = "";
            string max = "";
            string value = "";
            string name = "";
            string unit = "";

            //登録がない場合、抜ける
            if (ID == null) return;

            foreach (var workid in ID)
            {
                //補正値
                if (type == SystemConstants.WORKID_TYPE_CORRECTDATA)
                {

                    // 名称を取得する(どうやるか？）
                    name = Utility.GetMessageString(SystemConstants.CORRECT_MSG, workid);

                    // 範囲を取得する
                    Program.DataController.GetCorrectDataRangeStr(workid, ref min, ref max);

                    // 値を取得する
                    Program.DataController.ReadCorrectDataStr(workid, ref value);

                    //単位を取得する
                    Program.DataController.GetCorrectDataUnit(workid, ref unit);
                }
                //タイミング
                else
                {
                    // 名称を取得する(どうやるか？）
                    name = Utility.GetMessageString(SystemConstants.TIMMING_MSG, workid);

                    // 範囲を取得する
                    Program.DataController.GetTimingDataRangeStr(workid, ref min, ref max);

                    // 値を取得する
                    Program.DataController.ReadTimingDataStr(workid, ref value);

                    //単位を取得する
                    Program.DataController.GetTimingDataUnit(workid, ref unit);
                }
                // 値を設定する
                view.Rows.Add(new Object[] { workid, name, string.Format("{0} - {1}", new object[] { min, max }), value, unit });

                //背景色を設定する
                if ((rowCount % 2) == 0)
                {
                    //偶数の場合、背景白色
                    view[1, rowCount].Style.BackColor = Color.White;
                }
                else
                {
                    //奇数の場合、背景薄青色
                    view[1, rowCount].Style.BackColor = Color.LightGreen;
                }

                rowCount++;
            }
        }

        /// <summary>
        /// グリッドビューにデータを表示させる。（タイミングカム専用）
        /// </summary>
        /// <param name="view"></param>
        /// <param name="group"></param>
        private void viewDispTimCam(DataGridView view, int type, int group)
        {
            int[] ID = null;

            view.CurrentCell = null;
            Program.DataController.GetMemryDataGroupList(type, group, ref ID);

            int rowCount = 0;
            string min = "";
            string max = "";
            string value1 = "";
            string value2 = "";
            string value3 = "";
            string value4 = "";
            string value5 = "";
            string value6 = "";
            string value7 = "";
            string value8 = "";
            string name = "";
            int workidtop = 0;

            int i = 1;
            foreach (var workid in ID)
            {
                //補正値
                if (type == SystemConstants.WORKID_TYPE_TIMINGDATA)
                {
                    if (i == 1)
                    {
                        workidtop = workid;

                        // 名称を取得する(どうやるか？）
                        name = Utility.GetMessageString(SystemConstants.TIMMING_MSG, workid);

                        // 範囲を取得する
                        Program.DataController.GetTimingDataRangeStr(workid, ref min, ref max);

                        // 値を取得する
                        Program.DataController.ReadTimingDataStr(workid, ref value1);
                    }
                    if (i == 2)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value2);
                    }
                    if (i == 3)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value3);
                    }
                    if (i == 4)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value4);
                    }
                    if (i == 5)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value5);
                    }
                    if (i == 6)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value6);
                    }
                    if (i == 7)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value7);
                    }
                    if (i == 8)
                    {
                        Program.DataController.ReadTimingDataStr(workid, ref value8);
                        i = 1;
                        // 値を設定する
                        view.Rows.Add(new Object[] { workidtop, name, string.Format("{0} - {1}", new object[] { min, max }), value1, value2, value3, value4, value5, value6, value7, value8 });

                        //背景色を設定する
                        if ((rowCount % 2) == 0)
                        {
                            //偶数の場合、背景白色
                            view[1, rowCount].Style.BackColor = Color.White;
                        }
                        else
                        {
                            //奇数の場合、背景薄青色
                            view[1, rowCount].Style.BackColor = Color.LightGreen;
                        }

                        rowCount++;
                    }
                    else i++;
                }
            }
        }

        /// <summary>
        /// グリッドビューのテキストボックス上でEnterが押された時の処理
        /// </summary>
        /// <param name="view"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellValidating(DataGridView view, int type, object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (Program.SystemData.tachpanel == true) return;
            if (e.ColumnIndex < 3 || e.ColumnIndex > 10) return;

            int workid = Int32.Parse(view.Rows[e.RowIndex].Cells[0].Value.ToString()) + (e.ColumnIndex - 3);
            int rowindex = e.RowIndex;
            object value = e.FormattedValue;

            EnterDataGridView(type, workid, rowindex, value, view);

            /*int workID = 0;				/* WorkID */
            /*object value;				/* 値 */
            /*double outValue = 0;		/* 値をdoubleに変換したもの */
            /*string errMessage = "";		/* エラーメッセージ */

            // タイミングカム対応のため３～１０まで対応
            /*if (e.ColumnIndex >= 3 && e.ColumnIndex <= 10)
            {
                // WorkIDと値をグリッドビューから取得する
                workID = Int32.Parse(view.Rows[e.RowIndex].Cells[0].Value.ToString()) + (e.ColumnIndex - 3);
                value = e.FormattedValue;

                // Enterが押された時
                bool checkResult = Program.MainForm.checkTextBoxValue(
                    type,
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
                if (type == SystemConstants.WORKID_TYPE_CORRECTDATA)
                {
                    mainfrm.WriteCorrectData(workID, outValue);
                }
                else
                {
                    mainfrm.WriteTimingData(workID, outValue);
                }
            }*/
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void systemConfigurationfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        /// <summary>
        /// データグリッドの設定を行う
        /// </summary>
        /// <param name="WorkIDType"></param>
        /// <param name="dataGridView"></param>
        private void setDataGridEvent(int WorkIDType, DataGridView dataGridView)
        {
            dataGridView.CellValidating += new DataGridViewCellValidatingEventHandler(delegate(object sender, DataGridViewCellValidatingEventArgs args)
            {
                CellValidating(dataGridView, WorkIDType, sender, args);
            });

        }

        /// <summary>
        /// ボタンの状態を取得し、状態に応じて背景色を変更する。
        /// ON :Gray
        /// Off:Red
        /// </summary>
        /// <param name="BtnID"></param>
        /// <param name="Btn"></param>
        private void CheckBtnAnd_ChangeColor(int BtnID, Button Btn)
        {
            int status = 0;
            Program.DataController.ReadPushBtn(BtnID, ref status);

            if (status == SystemConstants.BTN_ON)
            {
                Btn.BackColor = Color.Gray;
            }
            else
            {
                Btn.BackColor = Color.Red;
            }

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
			// パスワードと確認が異なる場合はエラー
			if (maskedTextPASSWORD.Text != maskedTextCHECK.Text) {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG018);
				return;
			}

            //パスワードの保存
			Program.SystemData.password = maskedTextPASSWORD.Text;

            //自動機操作設定の保存

			// 機械のみの場合
			if (radioButtonMAIN.Checked) {
				Program.SystemData.machineoperation = "machine";
			}
			// 自動機・PCの場合
			else if (radioButtonMAIN_PC.Checked) {
				Program.SystemData.machineoperation = "both";
			}

            //通信設定の保存
            Program.SystemData.comport = comboCOMPORT.Text;
            Program.SystemData.borate = comboBORATE.SelectedIndex + 1;
            Program.SystemData.dataBits = comboDATABIT.SelectedIndex + 1;
            Program.SystemData.stopBits = comboSTOPBIT.SelectedIndex + 1; ;
            Program.SystemData.parity = comboPARITY.SelectedIndex + 1;
            Program.SystemData.handshake = comboflow_control.SelectedIndex + 1;

            //Language設定の保存
            if (radioButtonJAPANESE.Checked == true)
            {
                Program.SystemData.culture = "ja-JP";
            }
            else if (radioButtonENGLISH.Checked == true)
            {
                Program.SystemData.culture = "en-US";
            }
            else
            {
                Program.SystemData.culture = "zh-CN";
            }

            //タッチパネル使用有無
            Program.SystemData.tachpanel = checkBoxTACHPANEL.Checked;

            //SQLサーバー設定の保存
            Program.SystemData.sqlserver_machinename = textMACHINENAME.Text;
            Program.SystemData.sqlserver_databasename = textDATABASENAME.Text;
            Program.SystemData.sqlserver_userid = textUSERID.Text;
            Program.SystemData.sqlserver_password = textPASSWORD.Text;

            //XMLに保存
            try
            {
                Program.SystemData.Save();
            }
            catch (Exception)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG013);
                return;
            }

			// システム設定を行いました。
			Utility.ShowInfoMsg(SystemConstants.SYSTEM_MSG019);
        }

        private void GridUpdate(DataGridView view, int type)
        {
            int rowCount = view.Rows.Count;
            string value = "";

            //GridViewの更新ｓ
            for (int y = 0; y < rowCount; y++)
            {
                int workid = Int32.Parse(view.Rows[y].Cells[0].Value.ToString());

                if (type == SystemConstants.WORKID_TYPE_CORRECTDATA)
                {
                    // 値を取得する
                    Program.DataController.ReadCorrectDataStr(workid, ref value);
                }
                else
                {
                    // 値を取得する
                    Program.DataController.ReadTimingDataStr(workid, ref value);
                }
                var cell = view.Rows[y].Cells[3];

                // 値が編集中でなければ、値を変更する
                if (!cell.IsInEditMode)
                {
                    view.Rows[y].Cells[3].Value = value;
                }
            }
        }

        private void GridUpdateTimCam(DataGridView view, int type)
        {
            int rowCount = view.Rows.Count;
            int colCount = view.ColumnCount;
            string value = "";

            //GridViewの更新ｓ
            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 3; x < colCount; x++)
                {
                    int workid = Int32.Parse(view.Rows[y].Cells[0].Value.ToString()) + (x - 3);

                    // 値を取得する
                    Program.DataController.ReadTimingDataStr(workid, ref value);
                    
                    var cell = view.Rows[y].Cells[x];

                    // 値が編集中でなければ、値を変更する
                    if (!cell.IsInEditMode)
                    {
                        view.Rows[y].Cells[x].Value = value;
                    }
                }
            }
        }


        private void systemConfigurationfrm_VisibleChanged(object sender, EventArgs e)
        {
            // true->falseになったときは、何もしない
            if (Visible == false)
            {
                return;
            }

            string[] com = null;

            //フォーム上に反映

            // パスワード設定
            maskedTextPASSWORD.Text = Program.SystemData.password;
            maskedTextCHECK.Text = Program.SystemData.password;

            // 自動機操作設定
            switch (Program.SystemData.machineoperation)
            {
                case "machine":
                    radioButtonMAIN.Checked = true;
                    break;

                case "both":
                    radioButtonMAIN_PC.Checked = true;
                    break;
            }


            //言語設定
            switch (Program.SystemData.culture)
            {

                case "ja-JP":
                    radioButtonJAPANESE.Checked = true;
                    break;
                case "en-US":
                    radioButtonENGLISH.Checked = true;
                    break;
                case "zh-CN":
                    radioButtonCHINESE.Checked = true;
                    break;

            }

            //通信設定
            //使用しているPCのCOMポート設定を取得
            try
            {
                com = SerialPort.GetPortNames();
            }
            catch (Exception)
            {

            }
            comboCOMPORT.Items.Clear();

            foreach (var tmp in from c in com orderby portNameToInt(c) select c)
            {
                comboCOMPORT.Items.Add(tmp);
            }

            // COMポートの値を設定する
            comboCOMPORT.SelectedIndex = comboCOMPORT.Items.IndexOf(Program.SystemData.comport);


            //ボーレート
            switch (Program.SystemData.borate)
            {
                case 1:
                    comboBORATE.SelectedIndex = 0;
                    break;

                case 2:
                    comboBORATE.SelectedIndex = 1;
                    break;

                case 3:
                    comboBORATE.SelectedIndex = 2;
                    break;

                case 4:
                    comboBORATE.SelectedIndex = 3;
                    break;

                case 5:
                    comboBORATE.SelectedIndex = 4;
                    break;

                case 6:
                    comboBORATE.SelectedIndex = 5;
                    break;

                case 7:
                    comboBORATE.SelectedIndex = 6;
                    break;
            }

            //データビット
            switch (Program.SystemData.dataBits)
            {
                case 1:
                    comboDATABIT.SelectedIndex = 0;
                    break;
                case 2:
                    comboDATABIT.SelectedIndex = 1;
                    break;
            }

            //ストップビット
            switch (Program.SystemData.stopBits)
            {
                case 1:
                    comboSTOPBIT.SelectedIndex = 0;
                    break;
                case 2:
                    comboSTOPBIT.SelectedIndex = 1;
                    break;
            }

            //パリティビット
            switch (Program.SystemData.parity)
            {
                case 1:
                    comboPARITY.SelectedIndex = 0;
                    break;
                case 2:
                    comboPARITY.SelectedIndex = 1;
                    break;
                case 3:
                    comboPARITY.SelectedIndex = 2;
                    break;
            }

            //フロー制御
            switch (Program.SystemData.handshake)
            {
                case 1:
                    comboflow_control.SelectedIndex = 0;
                    break;
                case 2:
                    comboflow_control.SelectedIndex = 1;
                    break;
                case 3:
                    comboflow_control.SelectedIndex = 2;
                    break;
            }

            //タッチパネル使用有無
            checkBoxTACHPANEL.Checked = Program.SystemData.tachpanel;

            //SQLサーバー接続先PC名
            textMACHINENAME.Text = Program.SystemData.sqlserver_machinename;
            textDATABASENAME.Text = Program.SystemData.sqlserver_databasename;
            textUSERID.Text = Program.SystemData.sqlserver_userid;
            textPASSWORD.Text = Program.SystemData.sqlserver_password;
        }

        private void systemConfigurationfrm_Shown(object sender, EventArgs e)
        {
            //グリッドビュー表示

            /* 補正値タブ */

            //ベースマシン
            viewDisp(systemConfiguration_CORR_GROUP_BASEMACHINE1View, SystemConstants.WORKID_TYPE_CORRECTDATA, SystemConstants.CORR_GROUP_BASEMACHINE1);
            //ほぐし
            viewDisp(systemConfiguration_CORR_GROUP_DISENTANGLE1View, SystemConstants.WORKID_TYPE_CORRECTDATA, SystemConstants.CORR_GROUP_DISENTANGLE1);
            //カット
            viewDisp(systemConfiguration_CORR_GROUP_CUT1View, SystemConstants.WORKID_TYPE_CORRECTDATA, SystemConstants.CORR_GROUP_CUT1);
            //引き込み
            viewDisp(systemConfiguration_CORR_GROUP_PULL1View, SystemConstants.WORKID_TYPE_CORRECTDATA, SystemConstants.CORR_GROUP_PULL1);
            
            /* タイミングタブ */

            //調尺・カット
            viewDispTimCam(systemConfiguration_TIMM_GROUP_FEEDCUT1View, SystemConstants.WORKID_TYPE_TIMINGDATA, SystemConstants.TIMM_GROUP_FEEDCUT1);
            //引き込み
            viewDispTimCam(systemConfiguration_TIMM_GROUP_PULL1View, SystemConstants.WORKID_TYPE_TIMINGDATA, SystemConstants.TIMM_GROUP_PULL1);
            //ストリップ
            viewDispTimCam(systemConfiguration_TIMM_GROUP_STRIP1View, SystemConstants.WORKID_TYPE_TIMINGDATA, SystemConstants.TIMM_GROUP_STRIP1);
            //ほぐし
            viewDispTimCam(systemConfiguration_TIMM_GROUP_DISENTANGLE1View, SystemConstants.WORKID_TYPE_TIMINGDATA, SystemConstants.TIMM_GROUP_DISENTANGLE1);
            //先端揃え
            viewDispTimCam(systemConfiguration_TIMM_GROUP_TRIMMING1View, SystemConstants.WORKID_TYPE_TIMINGDATA, SystemConstants.TIMM_GROUP_TRIMMING1);
            //排出
            viewDispTimCam(systemConfiguration_TIMM_GROUP_DISCHARGE1View, SystemConstants.WORKID_TYPE_TIMINGDATA, SystemConstants.TIMM_GROUP_DISCHARGE1);
        }

        // Ｅｎｔｅｒキーを押されデータを更新する
        private void EnterDataGridView(int WorkIDType, int WorkID, int RowIndex, object value, object DataGrid)
        {
            double outValue = 0;		/* 値をdoubleに変換したもの */
            string errMessage = "";		/* エラーメッセージ */
            DataGridView view = (DataGridView)DataGrid;

            // Enterが押された時
            bool checkResult = Program.MainForm.checkTextBoxValue(
                WorkIDType,
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
            if (WorkIDType == SystemConstants.WORKID_TYPE_CORRECTDATA)
            {
                mainfrm.WriteCorrectData(WorkID, outValue);
            }
            else
            {
                mainfrm.WriteTimingData(WorkID, outValue);
            }
        }

        // テンキーから入力完了イベント
        private void TenKeyEnterEvent(TenKeyData td)
        {
            switch (td.obj.GetType().Name)
            {
                case "DataGridView":
                    DataGridView dg = (DataGridView)td.obj;
                    td.value = td.val;
                    EnterDataGridView(td.workidtype, td.workid, td.rowindex, td.value, td.obj);
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

                if (workIDType == SystemConstants.WORKID_TYPE_CORRECTDATA)
                {
                    if (e.ColumnIndex != 3) return;
                }
                else
                {
                    if (e.ColumnIndex < 3) return;
                }
                                
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
                tc.tenKeyData.workid = Int32.Parse(dg.Rows[e.RowIndex].Cells[0].Value.ToString()) + (e.ColumnIndex - 3);
                tc.tenKeyData.workidtype = workIDType;
                tc.tenkeyFormShow();

                // フォーカスアウトする
                dg.CurrentCell = null;
            };
        }

        private void btnEditDB_Click(object sender, EventArgs e)
        {
            dbEditfrm dbEditForm = new dbEditfrm();
            dbEditForm.ShowDialog();
            dbEditForm.Dispose();
        }

        //補正値、タイミングをファイル出力する
        private void btnFileOut_Click(object sender, EventArgs e)
        {
            CsvFileOut csvfile = new CsvFileOut();            
            DialogResult ret;
            string filename = "";
            
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "CSV(*csv)|*.csv";
            saveFileDialog1.FileName = "Machine_Parameter";

            ret = saveFileDialog1.ShowDialog();
            filename = saveFileDialog1.FileName;

            if (ret == DialogResult.Cancel || filename == "")
            {
                return;
            }

            try
            {
                //ファイル作成
                FileOutErrMsg(csvfile.CsvFileOpen(filename));

                //補正値
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_PRM, tabPage11.Text, systemConfiguration_CORR_GROUP_BASEMACHINE1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_PRM, tabPage12.Text, systemConfiguration_CORR_GROUP_DISENTANGLE1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_PRM, tabPage13.Text, systemConfiguration_CORR_GROUP_CUT1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_PRM, tabPage14.Text, systemConfiguration_CORR_GROUP_PULL1View));

                //タイミング
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_TIM, tabPage21.Text, systemConfiguration_TIMM_GROUP_FEEDCUT1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_TIM, tabPage22.Text, systemConfiguration_TIMM_GROUP_PULL1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_TIM, tabPage23.Text, systemConfiguration_TIMM_GROUP_STRIP1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_TIM, tabPage24.Text, systemConfiguration_TIMM_GROUP_DISENTANGLE1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_TIM, tabPage25.Text, systemConfiguration_TIMM_GROUP_TRIMMING1View));
                FileOutErrMsg(csvfile.CsvFileDataGridViewAdd(filename, CsvFileOutDefine.CATE_TIM, tabPage26.Text, systemConfiguration_TIMM_GROUP_DISCHARGE1View));
            }
            catch
            {
                return;
            }
        }
         
        //CSVファイルアウトエラー出力
        private void FileOutErrMsg(int ret)
        {
            if (ret == CsvFileOutDefine.FILEOUT_NO_ERROR) return;
            switch (ret)
            {
                case CsvFileOutDefine.FILEOUT_OPEN_ERROR:
                    Utility.ShowErrorMsg("File Open Error");
                    break;
                case CsvFileOutDefine.FILROUT_ADD_DATA_ERROR:
                    Utility.ShowErrorMsg("File Add Error");
                    break;
            }            
        }
    }
}
