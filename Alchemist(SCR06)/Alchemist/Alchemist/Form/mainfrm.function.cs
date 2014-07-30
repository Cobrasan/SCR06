using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

namespace Alchemist
{
	partial class mainfrm : Form
	{
		// テキストボックスのイベントを設定する
		public void SetTextBoxEvent(int workIDType, int WorkID, CustomTextBox customtextBox)
		{
			customtextBox.EnterKeyDown += delegate(EventArgs e) {
                EnterTextBox(workIDType, WorkID, customtextBox);

                // フォーカスアウトする
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;           
			};
		}

        // テキストボックスでエンターキー押されたときの処理
        public void EnterTextBox(int workIDType, int WorkID, CustomTextBox customtextBox)
        {
            double value;
            string message = "";

            // テキストボックスの入力チェック
            if (checkTextBoxValue(workIDType, WorkID, customtextBox.Text, out value, out message) == false)
            {
                Utility.ShowErrorMsg(message);
                return;
            }

            // ワークデータを書き込む
            mainfrm.WriteWorkData(WorkID, value);
        }

        // テキストボックスのイベントを設定する：目標値
        // ItemType= 1:wirelength, 2:strip1, 3:strip2
        // 該当の加工値から範囲チェックと値の更新をし、入力された値を整形してBANK属性に追加する
        public void SetTextBoxEventToBank(int workIDType, int WorkID, CustomTextBox customtextBox, int ItemType)
        {
            customtextBox.EnterKeyDown += delegate(EventArgs e)
            {
                EnterTextBoxToBank(workIDType, WorkID, customtextBox, ItemType);

                // フォーカスアウトする
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;
            };
        }

        // テキストボックスでエンターキー押されたときの処理：目標値
        public void EnterTextBoxToBank(int workIDType, int WorkID, CustomTextBox customtextBox, int ItemType)
        {
            double value;
            string message = "";
            int selectedNo = 0;
            string newvalue = "";

            // テキストボックスの入力チェック
            if (checkTextBoxValue(workIDType, WorkID, customtextBox.Text, out value, out message) == false)
            {
                Utility.ShowErrorMsg(message);
                return;
            }

            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return;

            // ワークデータを書き込む
            mainfrm.WriteWorkData(WorkID, value);

            // 現在選択されているバンクNoを取得
            Program.DataController.BankNoRead(ref selectedNo);

            Program.DataController.GetFormatWorkDataStr(WorkID, value, ref newvalue);
            mainfrm.BankDataItemWrite(selectedNo, newvalue, ItemType);
        }
        
        // コントロール内のテキストボックスの長さを設定する
        public void SetTextBoxLength(Control ctl, int Length)
        {
            foreach (Control control in ctl.Controls)
            {
                // テキストボックスならばテキスト長を設定する
                if (control is TextBox)
                {
                    (control as TextBox).MaxLength = Length;
                }

                // 子コントロールを含む場合は再帰的に処理を行う。
                else if (control.HasChildren)
                {
                    SetTextBoxLength(control, Length);
                }
            }
        }

		// テキストボックスの値をチェックする
		public bool checkTextBoxValue(int Type, int workID, object inValue, out double outValue, out string errMessage)
		{
			int ret = 0;
			string workName = "";
			string strValue = "";
			double min = 0;
			double max = 0;
			string strMin = "";
			string strMax = "";

			string format = "";
			
			// 範囲を取得
			switch (Type) {
			case SystemConstants.WORKID_TYPE_WORKDATA :
				// 範囲を取得
				ret = Program.DataController.GetWorkDataRange(workID, ref min, ref max);
                Program.DataController.GetWorkDataRangeStr(workID, ref strMin, ref strMax);
				workName = Utility.GetMessageString(SystemConstants.WORK_MSG, workID);

				break;

			case SystemConstants.WORKID_TYPE_CORRECTDATA :
				// 範囲を取得
                ret = Program.DataController.GetCorrectDataRange(workID, ref min, ref max);
                Program.DataController.GetCorrectDataRangeStr(workID, ref strMin, ref strMax);
				workName = Utility.GetMessageString(SystemConstants.CORRECT_MSG, workID);
				break;

			case SystemConstants.WORKID_TYPE_TIMINGDATA :
				// 範囲を取得
                ret = Program.DataController.GetTimingDataRange(workID, ref min, ref max);
                Program.DataController.GetTimingDataRangeStr(workID, ref strMin, ref strMax);
				workName = Utility.GetMessageString(SystemConstants.TIMMING_MSG, workID);
				break;
			}

			// ワークIDが見つからない場合
			if (SystemConstants.ERR_NO_WORK_ID == ret) {
				errMessage = Utility.GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG023);
				outValue = 0;
				return false;
			}

			// nullチェック
			if (inValue == null) {
				format = Utility.GetMessageString(
					SystemConstants.SYSTEM_MSG, 
					SystemConstants.SYSTEM_MSG024
				);
				outValue = 0;
				errMessage = string.Format(format, workName);

				return false;
			}

			strValue = inValue.ToString();

			// 必須チェック
			if (strValue.Trim() == "") {
				format = Utility.GetMessageString(
					SystemConstants.SYSTEM_MSG, 
					SystemConstants.SYSTEM_MSG024
				);
				outValue = 0;
				errMessage = string.Format(format, workName);

				return false;
			}

			// フォーマットチェック
			if (double.TryParse(strValue, out outValue) == false) {
				format = Utility.GetMessageString(
					SystemConstants.SYSTEM_MSG, 
					SystemConstants.SYSTEM_MSG025
				);
				errMessage = string.Format(format, workName);
				return false;
			}


			// 範囲チェック
			if (Utility.CheckRange(outValue, min, max) == false) {
				format = Utility.GetMessageString(
					SystemConstants.SYSTEM_MSG,
					SystemConstants.SYSTEM_MSG026
				);
				errMessage = string.Format(format, workName, strMin, strMax);
				return false;
			}

			errMessage = "";
			return true;
		}

        // WorkIDを持つコントロールを最新の値に更新する
        public void refreshControl(int WorkID, Control ctl, string addText="")
        {
            string value = "";

            if (!ctl.Focused)
            {
                Program.DataController.ReadWorkDataStr(WorkID, ref value);

                if (ctl.Text != value)
                {
                    ctl.Text = value + addText;
                }
            }
        }

        // WorkIDを持つボタンのイベントを設定する
        public void SetBtnEvent(int WorkID, int btnAction, Button btn)
        {
            btn.Click += new EventHandler(delegate(object sender, EventArgs args)
            {
                mainfrm.WritePushBtn(WorkID, btnAction);
            });
        }


        static public void CheckBtnAnd_ChangeColor(int BtnID, Button Btn)
        {
            int status = 0;
            Program.DataController.ReadPushBtn(BtnID, ref status);
            //前回と変更がない場合は、処理なし
            if ((status == SystemConstants.BTN_OFF && Btn.BackColor == Color.Gray) || (status == SystemConstants.BTN_ON && Btn.BackColor == Color.Red))
            {
                return;

            }
            else
            {
                if (status == SystemConstants.BTN_OFF)
                {
                    Btn.BackColor = Color.Gray;
                }
                else
                {
                    Btn.BackColor = Color.Red;
                }
            }

        }

        static public void CheckBtnAnd_ChangeColor_Label(int BtnID, Label Lbl, bool ReverseSwitch = false)
        {
            int status = 0;
            Color onColor, offColor;

            //反転時は色を逆転させる
            if (ReverseSwitch == true)
            {
                onColor = Color.Gray;
                offColor = Color.Red;
            }
            else
            {
                onColor = Color.Red;
                offColor = Color.Gray;
            }
            Program.DataController.ReadPushBtn(BtnID, ref status);
            //前回と変更がない場合は、処理なし
            if ((status == SystemConstants.BTN_OFF && Lbl.BackColor == offColor) || (status == SystemConstants.BTN_ON && Lbl.BackColor == onColor))
            {
                return;
            }
            else
            {
                if (status == SystemConstants.BTN_OFF)
                {
                    Lbl.BackColor = offColor;
                }
                else
                {
                    Lbl.BackColor = onColor;
                }
            }

        }

        static public void CheckBtnAnd_ChangePicture(int BtnID, Button Btn, Image PictureOn, Image PictureOff)
        {
            int status = 0;
            Program.DataController.ReadPushBtn(BtnID, ref status);
       
            if (status == SystemConstants.BTN_OFF)
            {
                Btn.Image = PictureOff;
            }
            else
            {
                Btn.Image = PictureOn;
            }
        }

        static public int CheckBtnOnOff(int BtnID)
        {
            int status = 0;
            Program.DataController.ReadPushBtn(BtnID, ref status);

            return status;
        }

        // 以下、データコントローラー書き込み用関数のエラー表示版
        static public int WriteCorrectData(int WorkID, double WorkData, bool WriteFileFlag = true)
        {   
            if (!Program.Initialized)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG028);
                return SystemConstants.ERR_SYNC_CANCELLED;
            }

            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return SystemConstants.ERR_MACHINE_RUN_CANCELLED;

            int ret = Program.DataController.WriteCorrectData(WorkID, WorkData, WriteFileFlag);

            Utility.ShowErrorCode(ret);

            return ret;
        }

        static public int WriteWorkData(int WorkID, double WorkData, bool initFlag = true, bool feedFlag = false)
        {
            if (!Program.Initialized)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG028);
                return SystemConstants.ERR_SYNC_CANCELLED;
            }

            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return SystemConstants.ERR_MACHINE_RUN_CANCELLED;

            int ret = Program.DataController.WriteWorkData(WorkID, WorkData, initFlag, feedFlag);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int WritePushBtn(int BtnID, int BtnStatus, bool execRelated = true, bool initFlag = true)
        {
            if (!Program.Initialized)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG028);
                return SystemConstants.ERR_SYNC_CANCELLED;
            }

            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return SystemConstants.ERR_MACHINE_RUN_CANCELLED;

            int ret = Program.DataController.WritePushBtn(BtnID, BtnStatus, execRelated, initFlag);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int WriteTimingData(int WorkID, double WorkData, bool WriteFileFlag = true)
        {
            if (!Program.Initialized)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG028);
                return SystemConstants.ERR_SYNC_CANCELLED;
            }

            // 運転中は操作不可
            int machineStatus = Program.DataController.GetMachineStatus();
            if ((machineStatus & SystemConstants.BIT_RUN) != 0) return SystemConstants.ERR_MACHINE_RUN_CANCELLED;

            int ret = Program.DataController.WriteTimingData(WorkID, WorkData, WriteFileFlag);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int BankDataCommentWrite(int BankNo, string BankComment)
        {
            int ret = Program.DataController.BankDataCommentWrite(BankNo, BankComment);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int BankDataItemWrite(int BankNo, string BankItem, int ItemType)
        {
            int ret = Program.DataController.BankDataItemWrite(BankNo, BankItem, ItemType);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int BankNoWrite(int SelectedNo)
        {
            int ret = Program.DataController.BankNoWrite(SelectedNo);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int BankDataLoad(int SelectedNo)
        {
            int ret = Program.DataController.BankDataLoad(SelectedNo);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int BankDataSave(int SelectedNo)
        {
            int ret = Program.DataController.BankDataSave(SelectedNo);

            Utility.ShowErrorCode(ret);
            return ret;
        }

        static public int BankNoRead(BankAttributeStruct BankAtrribute, ref int BankNo)
        {
            int ret = Program.DataController.BankNoReadFromAttributes(BankAtrribute, ref BankNo);
            return ret;
        }

        static public int BankLastNoRead(ref int BankNo)
        {
            int ret = Program.DataController.BankNoLast(ref BankNo);
            return ret;
        }

        private int GetRGBColor(string rgbTxt)
        {
            int rgbColor;
            rgbColor = Convert.ToInt32(rgbTxt, 16);
            return rgbColor;
        }

        static public void WriteBankDataItem(int BankNo, int WorkID, double Value, int ItemType)
        {
            string newvalue = "";
            // ワークデータを書き込む
            WriteWorkData(WorkID, Value);
            Program.DataController.GetFormatWorkDataStr(WorkID, Value, ref newvalue);
            BankDataItemWrite(BankNo, newvalue, ItemType);
        }

        static public void ReadBankDataItem(int BankNo, ref string BankItem, int ItemType)
        {
            Program.DataController.BankDataItemRead(BankNo, ref BankItem, ItemType);
        }

        static public void GetBankDataFormat(int WorkID, double Value, ref string ValueStr)
        {
            Program.DataController.GetFormatWorkDataStr(WorkID, Value, ref ValueStr);
        }

        static public void setCounter(int WorkID, double SetCount)
        {
            WriteWorkData(WorkID, SetCount);
        }

        // テンキーから入力完了イベント
        public void TenKeyEnterEvent(TenKeyData td)
        {
            switch (td.obj.GetType().Name)
            {
                case "CustomTextBox":
                    CustomTextBox ct = (CustomTextBox)td.obj;
                    ct.Text = td.val.ToString();
                    if (td.itemtype == 0)
                    {
                        EnterTextBox(td.workidtype, td.workid, ct);
                    }
                    else
                    {
                        EnterTextBoxToBank(td.workidtype, td.workid, ct, td.itemtype);                    
                    }
                    break;
            }
        }

        // テキストボックスのクリックイベントを設定する
        public void ClickTextBoxEvent(int workIDType, int WorkID, CustomTextBox customtextBox)
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

        // テキストボックスのクリックイベントを設定する
        public void ClickTextBoxEventToBank(int workIDType, int WorkID, CustomTextBox customtextBox, int ItemType)
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
                tc.tenKeyData.itemtype = ItemType;
                tc.tenkeyFormShow();

                // フォーカスアウトする
                Form frm = customtextBox.FindForm();
                frm.ActiveControl = null;
            };
        }

        static public int BankDataDelete(int SelectedNo)
        {
            int ret = Program.DataController.BankDataDelete(SelectedNo);

            Utility.ShowErrorCode(ret);
            return ret;
        }

	}
}
