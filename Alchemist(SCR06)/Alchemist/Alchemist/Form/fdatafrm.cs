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
    public partial class fdatafrm : Form
    {
        private wireConfirmfrm wireConfirmForm = new wireConfirmfrm();
        private wireChangeInfofrm wireChangeInfoForm = new wireChangeInfofrm();

        private int barcode_read;
        private string efu_first;
        private char AB_change;

        public fdatafrm()
        {
            InitializeComponent();
            SetTextBoxEvent(txtBARCODE);
        }
        public void Initialize()
        {
            Program.MainForm.AddOwnedForm(this);
        }

        private void SetTextBoxEvent(CustomTextBox customtextBox)
        {
            customtextBox.EnterKeyDown += delegate(EventArgs e)
            {
                ReadBarcode(customtextBox.Text);
                customtextBox.Text = "";
            };
        }

        private void fdatafrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void fdatafrm_Activated(object sender, EventArgs e)
        {
            barcode_read = 0;
            efu_first = "";
            AB_change = 'N';
            lblAB_CHANGE.Visible = false;

            lblHINBAN.Text = "";
            lblSEIBAN.Text = "";
            lblRENBAN.Text = "";
            lblDEKIDAKA1.Text = ""; 
            lblSETSU.Text = "";
            lblTABADORISU.Text = "";
            lblSENSYU.Text = "";
            lblSIZE.Text = "";
            lblIRO.Text = "";           
            lblDENSEN_CODE.Text = "";
            lblA_KAIROKIGOU.Text = "";
            lblB_KAIROKIGOU.Text = "";
            lblSETSUDANCHO.Text = "";
            lblA_KAWAMUKINAGASA.Text = "";
            lblB_KAWAMUKINAGASA.Text = "";
        }

        private void ReadBarcode(string Barcode)
        {
            D_Work workdata = new D_Work();
            R_Work r_workdata = new R_Work();
            int result;
            bool interruptflg = false;
            double value;
            string message = "";
            CustomTextBox dumy = new CustomTextBox();            

            // 完了したデータを検索            
            result = Program.SCR06DB.dbGetCompleteResultWorkData(Barcode);
            if (result != SystemConstants.ERR_NO_COMPLETE_RESULT_WORKDATA)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG046);
                return;
            }

            // 中断されたデータを検索
            result = Program.SCR06DB.dbGetInterruptResultWorkData(Barcode, ref r_workdata);
            if (result == SystemConstants.ERR_NO_INTERRUPT_RESULT_WORKDATA)
            {
                // 新規データが登録されているか
                result = Program.SCR06DB.dbGetWorkData(Barcode, ref workdata);
                if (result == SystemConstants.ERR_NO_WORKDATA)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG039);
                    return;
                }
                
                // 登録データの切断長が設定範囲内か
                if  (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.WIRE_LENGTH1, workdata.Setsudancho, out value, out message) == false)
                {
                    Utility.ShowErrorMsg(message);
                    return;
                }

                // 登録データのストリップ長１が設定範囲内か
                if (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH1, workdata.A_kawamukinagasa, out value, out message) == false)
                {
                    Utility.ShowErrorMsg(message);
                    return;
                }

                // 登録データのストリップ長２が設定範囲内か
                if (Program.MainForm.checkTextBoxValue(SystemConstants.WORKID_TYPE_WORKDATA, SystemConstants.STRIP_LENGTH2, workdata.B_kawamukinagasa, out value, out message) == false)
                {
                    Utility.ShowErrorMsg(message);
                    return;
                }
            }

            barcode_read += 1;

            // 確認のためバーコードを二回読む
            if (barcode_read == 1)
            {
                if (result == SystemConstants.GET_INTERRUPT_RESULT_WORKDATA)
                {
                    efu_first = Barcode;
                    displyRegisterWorkData(r_workdata);
                }
                else
                {
                    if (checkWorkData(workdata) == SystemConstants.SQL_SUCCESS)
                    {
                        efu_first = Barcode;
                    }
                }
            }
            if (barcode_read == 2)
            {
                if (efu_first == Barcode)
                {
                    if (result == SystemConstants.GET_INTERRUPT_RESULT_WORKDATA)
                    {
                        // エフ確定処理
                        if (registerInterruptWorkData(r_workdata) == SystemConstants.ERR_RESULT_WORKDATA_UPDATE)
                        {
                            Program.MainForm.Close();
                        }
                        interruptflg = true;                       
                    }
                    else
                    {
                        // エフ確定処理
                        if (registerWorkData(workdata) == SystemConstants.ERR_RESULT_WORKDATA_INSERT)
                        {
                            Program.MainForm.Close();
                        }
                        interruptflg = false;                        
                    }
                    // 更新されたデータを取得
                    result = Program.SCR06DB.dbGetStartResultWorkData(ref r_workdata);
                    if (result == SystemConstants.ERR_NO_START_RESULT_WORKDATA)
                    {
                        Program.MainForm.Close();
                    }

                    // カウンターのリセット
                    mainfrm.WritePushBtn(SystemConstants.QTY_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH);
                    mainfrm.WritePushBtn(SystemConstants.LOT_COUNTER_RESET1_BTN, SystemConstants.BTN_PUSH);

                    // 作業データへ反映
                    setWorkData(r_workdata, interruptflg);

                    Visible = false;

                    //段取り中
                    Program.SCR06DB.F_Dandori = true;
                    
                    // 前回エフと比較し電線照会
                    confirmEfuWire(r_workdata);

                    //段取り終了
                    Program.SCR06DB.F_Dandori = false;
                }
                else
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG041);
                }
            }

        }

        private int checkWorkData(D_Work WorkData)
        {
            // 電線の登録有無を確認
            M_WireDetail wireinfo = new M_WireDetail();
            // 登録されている電線か確認
            if (Program.SCR06DB.dbGetWireInfomation(WorkData.Densen_Code, ref wireinfo) == SystemConstants.ERR_NO_WIRE_INFO)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG038);
                return SystemConstants.SYSTEM_MSG038;
            }

            // 切断長を確認
            if (WorkData.A_kawamukinagasa + WorkData.B_kawamukinagasa + 330 >= WorkData.Setsudancho)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG040);
                return SystemConstants.SYSTEM_MSG040;
            }

            // ＡＢ入れ替え有無確認
            if (WorkData.A_kawamukinagasa > WorkData.B_kawamukinagasa)
            {
                AB_change = 'Y';
                lblAB_CHANGE.Visible = true;
            }

            // 照会内容を表示
            lblHINBAN.Text = WorkData.Hinban;
            lblSEIBAN.Text = WorkData.Seiban;
            lblRENBAN.Text = WorkData.Renban;
            lblDEKIDAKA1.Text = WorkData.Dute1.ToString();
            lblSETSU.Text = WorkData.Setsu.ToString();
            lblTABADORISU.Text = WorkData.Tabadorisu.ToString();
            lblSENSYU.Text = WorkData.Sensyu;
            lblSIZE.Text = WorkData.Size;
            lblIRO.Text = WorkData.Iro;
            lblDENSEN_CODE.Text = WorkData.Densen_Code;
            lblA_KAIROKIGOU.Text = WorkData.A_kairokigou;
            lblB_KAIROKIGOU.Text = WorkData.B_kairokigou;
            lblSETSUDANCHO.Text = WorkData.Setsudancho.ToString();
            lblA_KAWAMUKINAGASA.Text = WorkData.A_kawamukinagasa.ToString();
            lblB_KAWAMUKINAGASA.Text = WorkData.B_kawamukinagasa.ToString();

            return SystemConstants.SQL_SUCCESS;
        }

        private void displyRegisterWorkData(R_Work WorkData)
        {
            lblHINBAN.Text = WorkData.Hinban;
            lblSEIBAN.Text = WorkData.Seiban;
            lblRENBAN.Text = WorkData.Renban;
            lblDEKIDAKA1.Text = WorkData.Dute1.ToString();
            lblSENSYU.Text = WorkData.Sensyu;
            lblSIZE.Text = WorkData.Size;
            lblIRO.Text = WorkData.Iro;
            lblDENSEN_CODE.Text = WorkData.Densen_code;
            lblA_KAIROKIGOU.Text = WorkData.A_kairokigou;
            lblB_KAIROKIGOU.Text = WorkData.B_kairokigou;
            lblSETSUDANCHO.Text = WorkData.Setsudancho.ToString();
            lblA_KAWAMUKINAGASA.Text = WorkData.A_kawamukinagasa.ToString();
            lblB_KAWAMUKINAGASA.Text = WorkData.B_kawamukinagasa.ToString();
            // 中断した本数でセット
            lblSETSU.Text = WorkData.Chudan_setsu.ToString();
            lblTABADORISU.Text = WorkData.Chudan_tabadorisu.ToString();

            // ＡＢ入れ替え有無確認
            if (WorkData.AB_change == 'Y')
            {
                AB_change = 'Y';
                lblAB_CHANGE.Visible = true;
            }
        }

        private int registerWorkData(D_Work WorkData)
        {
            try
            {
                R_Work r_workdata = new R_Work();
                T_Operator t_op = new T_Operator();

                r_workdata.Hinban = WorkData.Hinban;
                r_workdata.Seiban = WorkData.Seiban;
                r_workdata.Renban = WorkData.Renban;
                r_workdata.Dute1 = WorkData.Dute1;
                r_workdata.Setsu = WorkData.Setsu;
                r_workdata.Tabadorisu = WorkData.Tabadorisu;
                r_workdata.Sensyu = WorkData.Sensyu;
                r_workdata.Size = WorkData.Size;
                r_workdata.Iro = WorkData.Iro;
                r_workdata.Densen_code = WorkData.Densen_Code;
                r_workdata.A_kairokigou = WorkData.A_kairokigou;
                r_workdata.B_kairokigou = WorkData.B_kairokigou;
                r_workdata.Setsudancho = WorkData.Setsudancho;
                if (AB_change == 'N')
                {
                    r_workdata.A_kawamukinagasa = WorkData.A_kawamukinagasa;
                    r_workdata.B_kawamukinagasa = WorkData.B_kawamukinagasa;
                }
                else
                {
                    r_workdata.A_kawamukinagasa = WorkData.B_kawamukinagasa;
                    r_workdata.B_kawamukinagasa = WorkData.A_kawamukinagasa;
                }
                r_workdata.AB_change = AB_change;
                r_workdata.Kaishi_time = DateTime.Now;
                r_workdata.Sagyou_status = 'S';

                Program.SCR06DB.dbGetTemporaryOperator(ref t_op);

                r_workdata.Sagyosya_code = t_op.OperatorCode;
                r_workdata.Sagyosya_name = t_op.OperatorName;

                if (Program.SCR06DB.dbInsertResultWorkData(r_workdata) == SystemConstants.ERR_RESULT_WORKDATA_INSERT)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG043);
                    return SystemConstants.ERR_RESULT_WORKDATA_INSERT;
                }
            }
            catch
            {
                return SystemConstants.ERR_RESULT_WORKDATA_INSERT;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        private int registerInterruptWorkData(R_Work WorkData)
        {
            try
            {
                T_Operator t_op = new T_Operator();
                Program.SCR06DB.dbGetTemporaryOperator(ref t_op);

                WorkData.Sagyou_status = 'S';
                WorkData.Sagyosya_code = t_op.OperatorCode;
                WorkData.Sagyosya_name = t_op.OperatorName;

                if (Program.SCR06DB.dbUpdateResultWorkData(WorkData) == SystemConstants.ERR_RESULT_WORKDATA_UPDATE)
                {
                    Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG043);
                    return SystemConstants.ERR_RESULT_WORKDATA_UPDATE;
                }
            }
            catch
            {
                return SystemConstants.ERR_RESULT_WORKDATA_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 指定バンク番号のバンクを読み込む
        /// </summary>
        private void setWorkData(R_Work WorkData, bool InterruptFlg)
        {
            int qty, lot;
            
            // バンクから該当データを検索、ない場合は新規登録
            searchBankData(WorkData);
           
            // 設定本数の設定
            if (InterruptFlg == true)
            {
                qty = Int32.Parse(WorkData.Chudan_setsu.ToString());
                lot = Int32.Parse(WorkData.Chudan_tabadorisu.ToString());
            }
            else
            {
                qty = Int32.Parse(WorkData.Setsu.ToString());
                lot = Int32.Parse(WorkData.Tabadorisu.ToString());
            }

            mainfrm.setCounter(SystemConstants.QTY_SET_COUNTER1, qty);
            mainfrm.setCounter(SystemConstants.LOT_SET_COUNTER1, lot);
        }

        /// <summary>
        /// 指定バンク番号のバンクを読み込む
        /// </summary>
        private void loadBank(int BankNo)
        {
            // selectednoを設定する
            mainfrm.BankNoWrite(BankNo);

            // バンクデータをロードする
            int result = mainfrm.BankDataLoad(BankNo);

            // バンクデータをセーブする
            result = mainfrm.BankDataSave(BankNo);

            Utility.ShowErrorCode(result);
        }

        /// <summary>
        /// 新規にバンクを登録し読み込む
        /// </summary>
        private void loadNewBank(BankAttributeStruct BankData)
        {
            int lastno = 0;
            int result = mainfrm.BankLastNoRead(ref lastno);
            int selectedNo = 0;

            // バンク範囲外
            if (result == SystemConstants.ERR_BANKNO_RANGE)
            {
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG058);
                return;
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                //+1して追加
                loadBank(lastno + 1);

                // 現在選択されているバンクNoを取得
                Program.DataController.BankNoRead(ref selectedNo);

                mainfrm.BankDataCommentWrite(selectedNo, BankData.WireCode);
                mainfrm.BankDataItemWrite(selectedNo, BankData.WireName, SystemConstants.BANKITEM_TYPE_WIRENAME);                
                mainfrm.WriteBankDataItem(selectedNo, SystemConstants.WIRE_LENGTH1, double.Parse(BankData.WireLength), SystemConstants.BANKITEM_TYPE_WIRELENGTH);
                mainfrm.WriteBankDataItem(selectedNo, SystemConstants.STRIP_LENGTH1, double.Parse(BankData.Strip1Length), SystemConstants.BANKITEM_TYPE_STRIP1);
                mainfrm.WriteBankDataItem(selectedNo, SystemConstants.STRIP_LENGTH2, double.Parse(BankData.Strip2Length), SystemConstants.BANKITEM_TYPE_STRIP2);
            }
        }

        /// <summary>
        /// 電線コード、電線長、皮むき長さが一致するバンクデータを検索し読み込む
        /// 該当するものがなければ新規登録する
        /// </summary>        
        private void searchBankData(R_Work WorkData)
        {
            BankAttributeStruct BankData = new BankAttributeStruct();
            BankData.WireName = WorkData.Sensyu + WorkData.Size + WorkData.Iro;
            BankData.WireCode = WorkData.Densen_code;
            BankData.WireLength = WorkData.Setsudancho.ToString();         
            BankData.Strip1Length = WorkData.A_kawamukinagasa.ToString();
            BankData.Strip2Length = WorkData.B_kawamukinagasa.ToString();
            mainfrm.GetBankDataFormat(SystemConstants.WIRE_LENGTH1, double.Parse(BankData.WireLength), ref BankData.WireLength);
            mainfrm.GetBankDataFormat(SystemConstants.STRIP_LENGTH1, double.Parse(BankData.Strip1Length), ref BankData.Strip1Length);
            mainfrm.GetBankDataFormat(SystemConstants.STRIP_LENGTH2, double.Parse(BankData.Strip2Length), ref BankData.Strip2Length);
            
            int bankno = 0;
            int result = mainfrm.BankNoRead(BankData, ref bankno);

            if (result == SystemConstants.DCPF_SUCCESS)
            {
                loadBank(bankno);
            }
            else
            {
                loadNewBank(BankData);
            }
        }

        /// <summary>
        /// 前回のエフの電線と今回のを比較して違っていたら段取り確認画面を表示
        /// ※電線照合は交換したとき電線エラーが出るので自動的に照合になる
        /// </summary>
        private void confirmEfuWire(R_Work Workdata)
        {       
            wireChangeInfofrm wireChangeInfoForm = new wireChangeInfofrm();
            L_Work l_workdata = new L_Work();            

            if (Program.SCR06DB.dbGetLastWorkData(ref l_workdata) == SystemConstants.ERR_NO_LAST_WORKDATA ||
                l_workdata.Densen_code != Workdata.Densen_code
                )
            {
                wireChangeInfoForm.ShowDialog();                
                wireConfirmForm.ShowDialog();
            }
        }

    }
}
