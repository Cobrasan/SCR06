using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Alchemist
{
    /// <summary>
    /// 通信接続、切断に変化があったときに発生します。
    /// </summary>
    public class ConnStatusEventArgs : EventArgs
    {
        public int EventCode = 0;
    }

    /// <summary>
    /// メモリ割当データ構造体
    /// </summary>
    public class MemAllocStruct
    {
        public int WorkIdType;
        public int GroupCode;
        public int WorkID;
        public int SortNo;
        public int Address;
        public int DoubleWord;
        public int ValueFactor;
        public bool BankStore;
        public double MinLimit;
        public double MaxLimit;
        public double DefaultValue;
        public string Unit;
        public int address_special;
        public string Comment;
    }

    public partial class DataController : IDisposable
    {
        // コンストラクタ
        public DataController(Control ctl)
        {
            // 通信クラスの作成
            machineConnector = new MachineConnector(ctl);
            machineConnector.Initialize();

            //加工値メモリ制御クラスの作成
            workDataMemory = new WorkDataMemory();

            // ボタン制御クラスの作成
            buttonControl = new ButtonControl(this, machineConnector, workDataMemory);

            //加工値メモリ制御クラスの初期化
            workDataMemory.Initialize(memalloc, buttonControl);

            // スレッドの作成
            thread = new Thread(new ThreadStart(this.dataControllerThread));
            thread.Name = "DataControllerThread";
            thread.Start();
        }

        /// <summary>
        /// 初期化関数。
        /// 初期化時にこの関数を必ず呼び出してください。
        /// </summary>
        public int Initialize()
        {
            //メモリ割り当てロード
            try
            {
                memalloc.Load();

                // アドレスとWORKIDの対応表を作る
                foreach (var type in new[]
                {
                    SystemConstants.WORKID_TYPE_WORKDATA,
                    SystemConstants.WORKID_TYPE_CORRECTDATA,
                    SystemConstants.WORKID_TYPE_TIMINGDATA
                })
                {
                    dupList[type] = memalloc.GetAddressDupulicatedMap(type);
                }
            }
            catch
            {
                return SystemConstants.ERR_MEMALLOC_FILE_ERROR;
            }

            // バンクデータロード
            try
            {
                bankDataStorage.Load();
            }
            catch
            {
                return SystemConstants.ERR_BANK_FILE_ERROR;
            }

            // 補正値データロード
            try
            {
                correctdata.Load();
            }
            catch
            {
                return SystemConstants.ERR_CORRECT_FILE_ERROR;
            }

            // 初期化設定
            machineConnector.Initialize();
            
            return SystemConstants.DCPF_SUCCESS;
        }

        // Dispose
        public void Dispose()
        {
            // スレッドの停止
            thread.Abort();
            thread.Join();

            // machineConnectorの破棄
            machineConnector.PortClose();
            machineConnector.Dispose();
        }

        /// <summary>
        /// AddressからMemCount分の共有メモリを通信クラスの
        /// MemGetを私用して、機械から読み出す。MemGetから戻り値が戻った
        /// 時点で、戻り値を返す。
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="MemCount"></param>
        /// <returns>MemGetの戻り値をそのまま返す。</returns>
        public int MemorySynchro(int Address, int MemCount)
        {
            return machineConnector.MemGet(Address, MemCount);
        }

        /// <summary>
        /// 通信クラスのSyncWaitCountを私用し、送信予約およびAck待ちを返す。
        /// </summary>
        /// <param name="ReservedCount"></param>
        /// <param name="AckWaitCount"></param>
        /// <returns>SyncWaitと同様</returns>
        public int CommWaitCount(ref int ReservedCount, ref int AckWaitCount)
        {
            return machineConnector.SyncWaitCount(ref ReservedCount, ref AckWaitCount);
        }

        /// <summary>
        /// CommWaitCountを私用して、通信中の有無を返す。
        /// </summary>
        /// <returns>ReservedCount > 0 || AckWaitCount > 0の場合、trueを返す。</returns>
        public bool IsCommWait()
        {
            int ReservedCount = 0;
            int AckWaitCount = 0;

            int ret = CommWaitCount(ref ReservedCount, ref AckWaitCount);

            // 通信中であれば、trueを返す。
            if (ReservedCount > 0 || AckWaitCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 通信設定関数
        public int ComConfigure(int COMPort, int Borate, int DataBit, int StopBit, int Parity, int FlowControl)
        {
            int ret = machineConnector.ComConfigure(COMPort, Borate, DataBit, StopBit, Parity, FlowControl);

            if (ret == SystemConstants.MCPF_SUCCESS)
            {
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// ConnectStatusがMACHINE_CONNECTの時、
        /// 通信クラスのPortOpenを使用して通信ポートを開き、
        /// 通信を開始する。
        /// ConnectStatusがMACHINE_DISCONNECTの時、
        /// 通信クラスのPortCloseを使用して通信ポートを閉じ、
        /// 通信を終了する。
        /// </summary>
        /// <param name="ConnectStatus"></param>
        /// <returns></returns>
        public int MachineConnect(int ConnectStatus)
        {
            int ret = 0;

            // 接続の場合
            if (ConnectStatus == SystemConstants.MACHINE_CONNECT)
            {
                ret = machineConnector.PortOpen();

                // 戻り値の変換
                if (ret == SystemConstants.MCPF_SUCCESS)
                {
                    return SystemConstants.DCPF_SUCCESS;
                }
                else
                {
                    return ret;
                }
            }
            // 切断の場合
            else if (ConnectStatus == SystemConstants.MACHINE_DISCONNECT)
            {
                ret = machineConnector.PortClose();

                // 戻り値の変換
                if (ret == SystemConstants.MCPF_SUCCESS)
                {
                    return SystemConstants.DCPF_SUCCESS;
                }
                else
                {
                    return ret;
                }
            }

            return ret;
        }

        /// <summary>
        /// 通信クラスのMemReadを使用し、指定したメモリアドレスの値を
        /// 返す。
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="MemVal"></param>
        /// <returns></returns>
        public int MemRead(int Address, ref int MemVal)
        {
            return machineConnector.MemRead(Address, ref MemVal);
        }

        /// <summary>
        /// 通信クラスのIsConnectionを使用し、
        /// 接続状態を返す。
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {
            return machineConnector.IsConnection();
        }

        /// <summary>
        /// 通信の切断理由を取得します。
        /// </summary>
        /// <returns></returns>
        public int GetComError()
        {
            return machineConnector.GetComError();
        }

        /// <summary>
        /// 加工データの管理モードを設定します。
        /// </summary>
        /// <param name="WorkMode"></param>
        /// <returns>正常に処理が行われた場合、DCPF_SUCESSを返します。</returns>
        public int SetWorkMode(int WorkMode)
        {
            workMode = WorkMode;

            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// 加工データの管理モードを返します。
        /// </summary>
        /// <returns></returns>
        public int GetWorkMode()
        {
            return workMode;
        }

        /// <summary>
        /// エラー情報データを使用して、ErrorCodeの区分を返す。
        /// </summary>
        /// <param name="ErrorCode">エラーコードを指定する。</param>
        /// <returns>ERR_MSG_INFORMATION: 情報
        /// ERR_MSG_ERROR: エラー
        /// エラー情報に該当するエラーがない場合、ERR_MSG_ERRORを返す</returns>
        public int GetErrorType(int ErrorCode)
        {
            string message = Utility.GetMessageString(SystemConstants.ERROR_MSG, ErrorCode);

            // 文字がNULLの場合
            if (string.IsNullOrEmpty(message))
            {
                return SystemConstants.ERR_MSG_ERROR;
            }
            // 文字列が2文字未満の場合
            if (message.Length < 2)
            {
                return SystemConstants.ERR_MSG_ERROR;
            }

            // 先頭2文字を見て、区分を判定する
            string type = message.Substring(0, 2);
            if (type == "I_") {
                return SystemConstants.ERR_MSG_INFORMATION;
            }
            else {
                return SystemConstants.ERR_MSG_ERROR;
            }
        }

        /// <summary>
        /// 操作ボタンIDの状態を、BtnStatusに入れて返します。
        /// </summary>
        /// <param name="BtnID"></param>
        /// <param name="BtnStatus"></param>
        /// <returns></returns>
        public int ReadPushBtn(int BtnID, ref int BtnStatus)
        {
            try
            {
                return buttonControl.ReadPushBtn(BtnID, ref BtnStatus);
            }
            catch
            {
                return SystemConstants.ERR_NO_WORK_ID;
            }
        }

        /// <summary>
        /// 操作ボタンID(BtnID)がボタンStatusに変化した時に対応する
        /// 処理を行います。
        /// </summary>
        /// <param name="BtnID">BTNのID</param>
        /// <param name="BtnStatus">BTN_PUSHの時、ボタンを押された事を意味します。
        /// BTN_OFFの時、OFFの状態にする事を意味します。
        /// BTN_ONの時、ONの状態にする事を意味します。</param>
        /// <param name="execRelated">関連動作を実行する場合は、TRUEにします。</param>
        /// <returns></returns>
        public int WritePushBtn(int BtnID, int BtnStatus, bool execRelated=true, bool initFlag=true)
        {
            return buttonControl.WritePushBtn(BtnID, BtnStatus, execRelated, initFlag);
        }

        // 機種名取得関数
        public string GetMachineName()
        {
            return memalloc.GetMachineName();
        }

        /// <summary>
        /// バンクデータ保存クラスのGetBankDataを使用し、BankNoのバンクデータを取得し、
        /// 構造体BankDataに入れて返します。BankDataは、(Type, ID, 値) の構造体です。
        /// GetBankDataから受け取ったBankDataにメモリ割当データのバンク保存対象のIDの一部が不足していた場合、
        /// BankDataへ当該不足IDを、メモリ割当データの初期値で追加を行い、ERR_BANK_PARTS_BREKEと一緒に返します。
        /// GetBankDataから、ERR_NO_BANK_DATAを受取った場合、BankDataへメモリ割当データのバンク保存対象のIDを
        /// メモリ割当データの初期値で追加を行い、ERR_NO_BANK_DATAと一緒に返します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// </summary>
        /// <param name="BankNo">読み込むバンクナンバー</param>
        /// <param name="BankData">読み込んだバンクデータを格納する構造体</param>
        /// <returns></returns>
        public int BankDataRead(int BankNo, ref BankDataStruct[] BankData)
        {

            // バンクデータ保存クラスのGetBankDataを使用し、BankNoのバンクデータを取得
            int result = bankDataStorage.GetBankData(BankNo, ref BankData);

            // 正常に処理が行われた場合
            if ((result != SystemConstants.BSPF_SUCCESS) && (result != SystemConstants.ERR_NO_BANK_DATA))
            {
                return result;
            }

            // 補完処理を行う
            WorkDataMemory memory = new WorkDataMemory();
            memory.Initialize(memalloc, buttonControl);
            result = memory.SetArray(BankData);

            BankData = new BankDataStruct[0];
            memory.GetArray(ref BankData);

            return result;
        }

        /// <summary>
        /// GetBankDataCommentを使用して、BankNoのバンクコメントを取得し、BankCommentに入れて返します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します。
        /// </summary>
        /// <param name="BankNo">読み込むバンクナンバー</param>
        /// <param name="BankComment">読み込んだバンクコメントを格納する</param>
        /// <returns></returns>
        public int BankDataCommentRead(int BankNo, ref string BankComment)
        {

            // GetBankDataCommentを使用して、BankNoのバンクコメントを取得
            int result = bankDataStorage.GetBankDataComment(BankNo, ref BankComment);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// GetBankDataWireNameを使用して、WireNameの電線情報を取得し、WireNameに入れて返します。
        /// ItemTypeで書き込むアイテムを指定する。
        /// 0:電線名
        /// 1:切断長
        /// 2:シース剥ぎA
        /// 3:シース剥ぎB
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します。
        /// </summary>
        /// <param name="BankNo">読み込むバンクナンバー</param>
        /// <param name="WireName">読み込んだバンクコメントを格納する</param>
        /// <returns></returns>
        public int BankDataItemRead(int BankNo, ref string BankItem, int ItemType)
        {

            // GetBankDataItemを使用して、BankNoのバンクアイテムを取得
            int result = bankDataStorage.GetBankDataItem(BankNo, ref BankItem, ItemType);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// GetBankNoFromAttributesを使用して、BankNoを取得し、BankNoに入れて返します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します。
        /// </summary>
        /// <param name="BankAttributes">検索する属性内容</param>
        /// <param name="BankNo">見つかったバンク番号</param>
        /// <returns></returns>
        public int BankNoReadFromAttributes(BankAttributeStruct BankAttributes, ref int BankNo)
        {

            // GetBankDataItemを使用して、BankNoのバンクアイテムを取得
            int result = bankDataStorage.GetBankNoFromAttributes(BankAttributes, ref BankNo);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// BankData[]の値をバンクデータ保存クラスの、WriteBankDataを使用して、バンクに保存します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// </summary>
        /// <param name="BankNo">書き込むバンクナンバー</param>
        /// <param name="BankData">書き込むバンクデータ</param>
        /// <returns></returns>
        public int BankDataWrite(int BankNo, BankDataStruct[] BankData)
        {

            // BankData[]の値をバンクデータ保存クラスの、WriteBankDataを使用して、バンクに保存
            int result = bankDataStorage.WriteBankData(BankNo, BankData);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // WriteBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// BankCommentの値をバンクデータ保存クラスの、WriteBankDataCommentを使用して、バンクに保存します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// </summary>
        /// <param name="BankNo">書き込むバンクナンバー</param>
        /// <param name="BankComment">書き込むバンクコメント</param>
        /// <returns></returns>
        public int BankDataCommentWrite(int BankNo, string BankComment)
        {

            // BankCommentの値をバンクデータ保存クラスの、WriteBankDataCommentを使用して、バンクに保存
            int result = bankDataStorage.WriteBankDataComment(BankNo, BankComment);
            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// BankItemの値をバンクデータ保存クラスの、WriteBankDataItemを使用して、バンクに保存します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// ItemTypeで書き込むアイテムを指定する。
        /// 0:電線名
        /// 1:切断長
        /// 2:シース剥ぎA
        /// 3:シース剥ぎB
        /// </summary>
        /// <param name="BankNo">書き込むバンクナンバー</param>
        /// <param name="BankWireName">書き込む電線名</param>
        /// <returns></returns>
        public int BankDataItemWrite(int BankNo, string BankItem, int ItemType)
        {

            // BankItemの値をバンクデータ保存クラスの、WriteBankDataItemを使用して、バンクに保存
            int result = bankDataStorage.WriteBankDataItem(BankNo, BankItem, ItemType);
            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// 選択されているバンクデータをロードする
        /// </summary>
        /// <returns></returns>
        public int BankDataLoad()
        {
            int BankNo = 0;

            // 選択されているバンクを取得する
            int ret = BankNoRead(ref BankNo);
            if (ret != SystemConstants.DCPF_SUCCESS)
            {
                return ret;
            }

            return BankDataLoad(BankNo);
        }

        /// <summary>
        /// BankDataRead、BankDataCommentReadを使用して取得したBankDataを、
        /// WriteWorkDataを使用して、共有メモリに反映させます。
        /// その際、GetWorkDataRangeを使用してメモリ割当データの上下限値と比較し、
        /// 範囲外の値があった場合は、当該項目には標準値を設定し、ERR_BANK_PARTS_RANGEを返します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// DCPF_SUCCESS以外が返った場合、その番号を返します。
        /// </summary>
        /// <param name="BankNo">ロードするバンクナンバー</param>
        /// <returns></returns>
        public int BankDataLoad(int BankNo)
        {
            BankDataStruct[] bankData = null;
            double min = 0.0;
            double max = 0.0;
            MemAllocStruct memAllocStruct = null;
            int result = SystemConstants.DCPF_SUCCESS;

            // バンクデータを読み込む
            result = BankDataRead(BankNo, ref bankData);
            if (result == SystemConstants.ERR_BANK_FILE_ERROR)
            {
                return result;
            }

            //加工値メモリに保存
            workDataMemory.SetArray(bankData);

            // 選択されているバンク番号を変える（メモリ上）
            bankDataStorage.SelectedNo = BankNo;

            // バンクデータを復元する
            for (int i = 0; i < bankData.Length; i++)
            {
                // Typeが加工値だった場合
                if (bankData[i].Type == SystemConstants.TYPE_WORKDATA)
                {
                    // GetWorkDataRangeを使用してメモリ割当データの上下限値と比較
                    int resultWDRange = GetWorkDataRange(bankData[i].ID, ref min, ref max);

                    // WorkIDがメモリ割当データに存在していない場合
                    if (resultWDRange == SystemConstants.ERR_NO_WORK_ID)
                    {
                        /* 読み飛ばす */
                        continue;
                    }

                    // 範囲外の値があった場合
                    if (Utility.CheckStringRange(bankData[i].value, min, max) == false)
                    {
                        // 当該項目には標準値を設定
                        memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_WORKDATA, bankData[i].ID);
                        bankData[i].value = memAllocStruct.DefaultValue.ToString();

                        // ERR_BANK_PARTS_RANGEを戻り値に設定
                        result = SystemConstants.ERR_BANK_PARTS_RANGE;
                    }

                    // 加工値を復元する
                    int resultWriteWD = WriteWorkData(bankData[i].ID, Convert.ToDouble(bankData[i].value), false);

                    // ファイルに書き込みが出来ない場合は、エラーを返す
                    if (resultWriteWD == SystemConstants.ERR_BANK_FILE_ERROR)
                    {
                        return resultWriteWD;
                    }
                }
                // Typeが加工ボタンだった場合
                else if (bankData[i].Type == SystemConstants.TYPE_WORKBTN)
                {
                    // WriteWorkDataを使用して、共有メモリに反映させる
                    int btnState = int.Parse(bankData[i].value);

                    // ボタンデータを復元する（関連動作は行わない）
                    int resultWriteWD = WritePushBtn(bankData[i].ID, btnState, false, false);
                    if (resultWriteWD == SystemConstants.ERR_BANK_FILE_ERROR)
                    {
                        return resultWriteWD;
                    }
                }
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            // DCPF_SUCCESS以外が返った場合
            else
            {
                // その番号を返します
                return result;
            }
        }

        /// <summary>
        /// 選択されているバンクデータをセーブします。
        /// </summary>
        /// <returns></returns>
        public int BankDataSave()
        {
            int BankNo = 0;

            // 選択されているバンクを取得する
            int ret = BankNoRead(ref BankNo);
            if (ret != SystemConstants.DCPF_SUCCESS)
            {
                return ret;
            }

            return BankDataSave(BankNo);
        }

        /// <summary>
        /// メモリ割当データのバンク保存対象の現在の値を、BankDataWrite、
        /// BankDataCommentWriteを使用して、バンクに保存します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// </summary>
        /// <param name="BankNo">セーブするバンクナンバー</param>
        /// <returns></returns>
        public int BankDataSave(int BankNo)
        {
            BankDataStruct[] bankData = new BankDataStruct[0];
            int result = 0;

            //加工値メモリからBankStoreがTrueのデータのみを取得する。
            result = workDataMemory.GetArray(ref bankData);
            if (result != SystemConstants.DCPF_SUCCESS)
            {
                return result;
            }

            // メモリ割当データのバンク保存対象の現在の値を
            // BankDataWriteを使用してバンクに保存
            result = BankDataWrite(BankNo, bankData);

            return result;
        }

        /// <summary>
        /// 補正値を同期後に保存する
        /// </summary>
        public void CorrectDataSave()
        {
            int[] groupIDList = null;
            int[] workid = null;

            List<CorrectDataStruct> correctDataStructList = new List<CorrectDataStruct>();
            CorrectDataStruct[] correctDataStruct_old = null;
            CorrectDataStruct[] correctDataStruct_new = null;
            CorrectDataStruct correctDataStruct_tmp;
            MemAllocStruct memAllocStruct = null;

            //システムデータを参照
            correctdata.GetCorrectData(ref correctDataStruct_old);

            //ArrayListにコピー
            correctDataStructList.AddRange(correctDataStruct_old);

            foreach (var type in new[] { SystemConstants.WORKID_TYPE_CORRECTDATA, SystemConstants.WORKID_TYPE_TIMINGDATA })
            {
                //group codeを全取得
                GetMemryDataGroupList(type, SystemConstants.WORKGROUP_ROOT, ref groupIDList);

                for (int i = 0; i < groupIDList.Length; i++)
                {
                    //グループごとのworkidを全取得
                    GetMemryDataGroupList(type, groupIDList[i], ref workid);

                    for (int j = 0; j < workid.Length; j++)
                    {
                        //該当workidのメモリ割り当てデータを取得
                        memAllocStruct = memalloc.GetEntry(type, workid[j]);

                        //CorrectDataにtypeがタイミングでwork[j]が存在するかチェック
                        var nodes = from n in correctDataStruct_old
                                    where n.Type == type && n.ID == workid[j]
                                    select n;



                        //システムデータに該当するIDの補正値が存在しない場合
                        if (nodes.Count() == 0)
                        {
                            //システムデータに存在しなしworkidの構造体作成
                            correctDataStruct_tmp.Type = type;
                            correctDataStruct_tmp.ID = workid[j];
                            correctDataStruct_tmp.value = memAllocStruct.DefaultValue.ToString();

                            //ArrayListに追加
                            correctDataStructList.Add(correctDataStruct_tmp);
                        }
                        // 存在する場合
                        else
                        {
                            double WorkData = 0;
                            ReadData(workid[j], ref WorkData, type);

                            CorrectDataStruct correct = nodes.First();
                            correct.ID = workid[j];
                            correct.Type = type;
                            correct.value = WorkData.ToString();
                        }
                    }
                }
            }

            //システムデータに反映
            correctDataStruct_new = new CorrectDataStruct[correctDataStructList.Count];

            for (int k = 0; k < correctDataStructList.Count; k++)
            {
                correctDataStruct_new[k] = correctDataStructList[k];
            }
            //xmlに書き込み
            correctdata.WriteCorrectData(correctDataStruct_new);
        }

        /// <summary>
        /// バンクデータ保存クラスのCopyBankDataを使用して、バンクデータのコピーを行う。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// </summary>
        /// <param name="SourceBankNo">コピー元のバンクナンバー</param>
        /// <param name="DestBankNo">コピー先のバンクナンバー</param>
        /// <returns></returns>
        public int CopyBankData(int SourceBankNo, int DestBankNo)
        {
            // バンクデータ保存クラスのCopyBankDataを使用して、バンクデータのコピーを行う
            int result = bankDataStorage.CopyBankData(SourceBankNo, DestBankNo);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // CopyBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// SelectedNoの値をバンクデータ保存クラスの、WriteSelectedNoを使用して、バンクに保存
        /// </summary>
        /// <param name="BankNo"></param>
        /// <returns></returns>
        public int BankNoWrite(int BankNo)
        {
            // SelectedNoの値をバンクデータ保存クラスの、WriteSelectedNoを使用して、バンクに保存
            int result = bankDataStorage.WriteSelectedNo(BankNo);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // WriteSelectedNoから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// 現在の選択されているバンクを取得する
        /// </summary>
        /// <param name="BankNo"></param>
        /// <returns></returns>
        public int BankNoRead(ref int BankNo)
        {
            // GetSelectedNoを使用して、selectednoを取得
            int result = bankDataStorage.GetSelectedNo(ref BankNo);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetSelectedNoから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

        /// <summary>
        /// 最終のバンク番号を取得する
        /// </summary>
        /// <param name="LastNo"></param>
        /// <returns></returns>
        public int BankNoLast(ref int LastNo)
        {
            // GetSelectedNoを使用して、selectednoを取得
            int result = bankDataStorage.GetLastNo(ref LastNo);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // GetSelectedNoから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }


        /// <summary>
        /// メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスへWorkDataを
        /// 適切な形(整数値等)に変換し、通信クラスのMemWriteを使用して書込みます。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返します。
        /// WorkDataが、メモリ割当データで定める上下限値を超えている場合、エラーとしてERR_WORK_RANGEを返します。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="WorkData"></param>
        /// <returns></returns>
        public int WriteWorkData(int WorkID, double WorkData, bool initFlag=true, bool feedFlag=false)
        {
            return writeWorkData(WorkID, WorkData, initFlag, feedFlag);
        }

        /// <summary>
        /// メモリ割当データを使用し、加工値ID (WorkID) に対応する共有メモリのアドレスから
        /// 通信クラスのMemReadを使用してデータを取得して、適切な形に変換し、WorkDataに入れて返す。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">加工値ID</param>
        /// <param name="WorkData">加工値</param>
        /// <returns></returns>
        public int ReadWorkData(int WorkID, ref double WorkData)
        {
            //共通関数呼び出し
            return ReadData(WorkID, ref WorkData, SystemConstants.WORKID_TYPE_WORKDATA);
        }

        /// <summary>
        /// ReadWorkDataを使用して、WorkIDの加工値を取得し、メモリ割当てデータのVALUE_FACTORに
        /// 対応する文字列に変換し、WorkDataStrに入れて返します。
        /// 例1: WorkData = 2, VALUE_FACTOR = 10 の時、「2.0」
        /// 例2: WorkData = 2, VALUE_FACTOR = 100 の時、「2.00」
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">加工値ID</param>
        /// <param name="WorkDataStr"></param>
        /// <returns></returns>
        public int ReadWorkDataStr(int WorkID, ref string WorkDataStr)
        {
            double workData = 0.0;
            MemAllocStruct memAllocStruct = null;

            // ReadWorkDataを使用して、WorkIDの加工値を取得
            int result = ReadWorkData(WorkID, ref workData);

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_WORKDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換
                WorkDataStr = Utility.GetWorkDataString(workData, memAllocStruct.ValueFactor);
                // DCPF_SUCCESSを返す。
                return SystemConstants.DCPF_SUCCESS;
            }

            return result;
        }

        /// <summary>
        /// RequestDataを使用して、WorkIDのメモリ割当てデータのVALUE_FACTORに
        /// 対応する文字列に変換し、WorkDataStrに入れて返します。
        /// 例1: WorkData = 2, VALUE_FACTOR = 10 の時、「2.0」
        /// 例2: WorkData = 2, VALUE_FACTOR = 100 の時、「2.00」
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">加工値ID</param>
        /// <param name="RequestData">整形要求文字列</param>
        /// <param name="WorkDataStr"></param>
        /// <returns></returns>
        public int GetFormatWorkDataStr(int WorkID, double RequestData, ref string WorkDataStr)
        {
            MemAllocStruct memAllocStruct = null;

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_WORKDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換
            WorkDataStr = Utility.GetWorkDataString(RequestData, memAllocStruct.ValueFactor);
            // DCPF_SUCCESSを返す。
            return SystemConstants.DCPF_SUCCESS;
        }


        /// <summary>
        /// メモリ割当データを使用し、加工値ID (WorkID) に対応する下限値をMinに、
        /// 上限値をMaxにいれて返します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">加工値ID</param>
        /// <param name="Min">加工値IDに対応する下限値</param>
        /// <param name="Max">加工値IDに対応する上限値</param>
        /// <returns></returns>
        public int GetWorkDataRange(int WorkID, ref double Min, ref double Max)
        {
            MemAllocStruct memAllocStruct = null;

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_WORKDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // メモリ割当データを使用し、加工値ID (WorkID) に対応する下限値をMinに入れる
            Min = memAllocStruct.MinLimit;

            // メモリ割当データを使用し、加工値ID (WorkID) に対応する上限値をMaxに入れる
            Max = memAllocStruct.MaxLimit;

            // 正常に処理が行われた場合
            // DCPF_SUCCESSを返す。
            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// GetWorkDataRangeを使用して、WorkIDの上下限値を取得し、メモリ割当てデータの
        /// VALUE_FACTORに対応する文字列に変換し、Min,Maxに入れて返します。
        /// 例1: WorkData = 2, VALUE_FACTOR = 10 の時、「2.0」
        /// 例2: WorkData = 2, VALUE_FACTOR = 100 の時、「2.00」
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">加工値ID</param>
        /// <param name="Min">加工値IDに対応する下限値</param>
        /// <param name="Max">加工値IDに対応する上限値</param>
        /// <returns></returns>
        public int GetWorkDataRangeStr(int WorkID, ref string Min, ref string Max)
        {
            double min = 0.0;
            double max = 0.0;

            MemAllocStruct memAllocStruct = null;

            // GetWorkDataRangeを使用して、WorkIDの上下限値を取得
            int result = GetWorkDataRange(WorkID, ref min, ref max);

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_WORKDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                Min = Utility.GetWorkDataString(min, memAllocStruct.ValueFactor);
                Max = Utility.GetWorkDataString(max, memAllocStruct.ValueFactor);
                // DCPF_SUCCESSを返す。
                return SystemConstants.DCPF_SUCCESS;
            }

            return result;
        }

        /// <summary>
        /// メモリ割当データを使用し、補正値ID (WorkID) に対応する共有メモリのアドレスへ
        /// WorkDataを適切な形 (整数型等) に変換し、通信クラスのMemWriteを使用して書込む。
        /// また、システムデータの補正値・タイミングデータの該当WorkIDの値をWorkDataに変更します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返します。
        /// WorkDataが、メモリ割当データで定める上下限値を超えている場合、エラーとして、ERR_WORK_RANGEを返します。
        /// </summary>
        /// <param name="WorkID">補正値ID</param>
        /// <param name="WorkData">補正値</param>
        /// <returns></returns>
        public int WriteCorrectData(int WorkID, double WorkData, bool WriteFileFlag = true)
        {
            //共通関数呼び出し
            return writeCorrectData(SystemConstants.WORKID_TYPE_CORRECTDATA, WorkID, WorkData, WriteFileFlag);
        }

        /// <summary>
        /// メモリ割当データを使用し、補正値ID (WorkID) に対応する共有メモリのアドレスから
        /// 通信クラスのMemReadを使用してデータを取得して、適切な形に変換し、WorkDataに入れて返す。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">補正値ID</param>
        /// <param name="WorkData">補正値</param>
        /// <returns></returns>
        public int ReadCorrectData(int WorkID, ref double WorkData)
        {
            //共通関数呼び出し
            return ReadData(WorkID, ref WorkData, SystemConstants.WORKID_TYPE_CORRECTDATA);
        }

        /// <summary>
        /// ReadCorrectDataを使用して、WorkIDの補正値を取得し、メモリ割当てデータの
        /// VALUE_FACTORに対応する文字列に変換し、WorkDataStrに入れて返します。
        /// 例1: WorkData = 2, VALUE_FACTOR = 10 の時、「2.0」
        /// 例2: WorkData = 2, VALUE_FACTOR = 100 の時、「2.00」
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">補正値ID</param>
        /// <param name="WorkData">補正値</param>
        /// <returns></returns>
        public int ReadCorrectDataStr(int WorkID, ref string WorkDataStr)
        {
            double workData = 0.0;
            MemAllocStruct memAllocStruct = null;

            // ReadWorkDataを使用して、WorkIDの加工値を取得
            int result = ReadCorrectData(WorkID, ref workData);

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_CORRECTDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換
                WorkDataStr = Utility.GetWorkDataString(workData, memAllocStruct.ValueFactor);
                // DCPF_SUCCESSを返す。
                return SystemConstants.DCPF_SUCCESS;
            }

            return result;
        }

        /// <summary>
        /// メモリ割当データを使用し、補正値ID (WorkID) に対応する下限値をMinに、上限値をMaxにいれて返します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">補正値ID</param>
        /// <param name="Min">WorkIDに対応する下限値</param>
        /// <param name="Max">WorkIDに対応する上限値</param>
        /// <returns></returns>
        public int GetCorrectDataRange(int WorkID, ref double Min, ref double Max)
        {
            MemAllocStruct memAllocStruct = null;

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_CORRECTDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // メモリ割当データを使用し、補正値ID (WorkID) に対応する下限値をMinに入れる
            Min = memAllocStruct.MinLimit;

            // メモリ割当データを使用し、補正値ID (WorkID) に対応する上限値をMaxに入れる
            Max = memAllocStruct.MaxLimit;

            // 正常に処理が行われた場合
            // DCPF_SUCCESSを返す。
            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// GetCorrectDataRangeを使用して、WorkIDの上下限値を取得し、メモリ割当てデータの
        /// VALUE_FACTORに対応する文字列に変換し、WorkDataStrに入れて返します。
        /// 例1: Min = 2, VALUE_FACTOR = 10 の時、「2.0」
        /// 例2: Min = 2, VALUE_FACTOR = 100 の時、「2.00」
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返す。
        /// WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID">補正値ID</param>
        /// <param name="Min">WorkIDに対応する下限値</param>
        /// <param name="Max">WorkIDに対応する上限値</param>
        /// <returns></returns>
        public int GetCorrectDataRangeStr(int WorkID, ref string Min, ref string Max)
        {

            double min = 0.0;
            double max = 0.0;

            MemAllocStruct memAllocStruct = null;

            // GetCorrectDataRangeを使用して、WorkIDの上下限値を取得
            int result = GetCorrectDataRange(WorkID, ref min, ref max);

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_CORRECTDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換
                Min = Utility.GetWorkDataString(min, memAllocStruct.ValueFactor);
                Max = Utility.GetWorkDataString(max, memAllocStruct.ValueFactor);
                // DCPF_SUCCESSを返す。
                return SystemConstants.DCPF_SUCCESS;
            }

            return result;

        }

        /// <summary>
        ///メモリ割当データを使用し、タイミングID (WorkID) に対応する共有メモリのアドレスへWorkDataを適切な形 (整数型等) に変換し、
        ///通信クラスのMemWriteを使用して書込む。
        ///また、システムデータの補正値・タイミングデータの該当WorkIDの値をWorkDataに変更します。
        ///正常に処理が行われた場合、DCPF_SUCCESSを返します。
        ///WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返します。
        ///WorkDataが、メモリ割当データで定める上下限値を超えている場合、エラーとして、ERR_WORK_RANGEを返します。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="WorkData"></param>
        /// <returns></returns>
        public int WriteTimingData(int WorkID, double WorkData, bool WriteFileFlag = true)
        {
            return writeCorrectData(SystemConstants.WORKID_TYPE_TIMINGDATA, WorkID, WorkData,WriteFileFlag);
        }

        /// <summary>
        /// メモリ割当データを使用し、タイミングID (WorkID) に対応する共有メモリのアドレスから通信クラスのMemReadを使用してデータを取得して、
        /// 適切な形に変換し、WorkDataに入れて返す。
        ///正常に処理が行われた場合、DCPF_SUCCESSを返す。
        ///WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="WorkData"></param>
        /// <returns></returns>
        public int ReadTimingData(int WorkID, ref double WorkData)
        {
            //共通関数呼び出し
            return ReadData(WorkID, ref WorkData, SystemConstants.WORKID_TYPE_TIMINGDATA);
        }

        /// <summary>
        /// ReadTimingDataを使用して、WorkIDの加工値を取得し、メモリ割当てデータのVALUE_FACTORに対応する文字列に変換し、WorkDataStrに入れて返します。
        ///例1: WorkData = 2, VALUE_FACTOR = 10 の時、「2.0」
        ///例2: WorkData = 2, VALUE_FACTOR = 100 の時、「2.00」
        ///正常に処理が行われた場合、DCPF_SUCCESSを返す。
        ///WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。

        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="WorkDataStr"></param>
        /// <returns></returns>
        public int ReadTimingDataStr(int WorkID, ref string WorkDataStr)
        {
            double workData = 0;
            MemAllocStruct memAllocStruct = null;

            // ReadWorkDataを使用して、WorkIDの加工値を取得
            int result = ReadTimingData(WorkID, ref workData);

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_TIMINGDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換
                WorkDataStr = Utility.GetWorkDataString(workData, memAllocStruct.ValueFactor);
                // DCPF_SUCCESSを返す。
                return SystemConstants.DCPF_SUCCESS;
            }

            return result;
        }

        /// <summary>
        /// メモリ割当データを使用し、タイミングID (WorkID) に対応する下限値をMinに、上限値をMaxにいれて返します。
        ///正常に処理が行われた場合、DCPF_SUCCESSを返す。
        ///WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        public int GetTimingDataRange(int WorkID, ref double Min, ref double Max)
        {

            MemAllocStruct memAllocStruct = null;

            //memalloc.xmlからデータを取得
            try
            {
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_TIMINGDATA, WorkID);
            }
            catch (MemoryAllocationDataException)
            {
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // メモリ割当データを使用し、補正値ID (WorkID) に対応する下限値をMinに入れる
            Min = memAllocStruct.MinLimit;

            // メモリ割当データを使用し、補正値ID (WorkID) に対応する上限値をMaxに入れる
            Max = memAllocStruct.MaxLimit;

            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// GetTimingDataRangeを使用して、WorkIDの上下限値を取得し、メモリ割当てデータのVALUE_FACTORに対応する文字列に変換し、
        /// Min, Maxに入れて返します。
        ///例1: Min = 2, VALUE_FACTOR = 10 の時、「2.0」
        ///例2: Min = 2, VALUE_FACTOR = 100 の時、「2.00」
        ///正常に処理が行われた場合、DCPF_SUCCESSを返す。
        ///WorkIDがメモリ割当データに存在していない場合、エラーとしてERR_NO_WORK_IDを返す。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        public int GetTimingDataRangeStr(int WorkID, ref string Min, ref string Max)
        {
            double min = 0.0;
            double max = 0.0;

            MemAllocStruct memAllocStruct = null;

            // GetWorkDataRangeを使用して、WorkIDの上下限値を取得
            int result = GetTimingDataRange(WorkID, ref min, ref max);

            try
            {
                // メモリ割当データを使用し、WorkIDに対応する共有メモリのアドレスを取得
                memAllocStruct = memalloc.GetEntry(SystemConstants.WORKID_TYPE_TIMINGDATA, WorkID);
            }
            // WorkIDがメモリ割当データに存在していない場合
            catch (MemoryAllocationDataException)
            {
                // エラーとしてERR_NO_WORK_IDを返します
                return SystemConstants.ERR_NO_WORK_ID;
            }

            // 正常に処理が行われた場合
            if (result == SystemConstants.DCPF_SUCCESS)
            {
                // メモリ割当てデータのVALUE_FACTORに対応する文字列に変換
                Min = Utility.GetWorkDataString(min, memAllocStruct.ValueFactor);
                Max = Utility.GetWorkDataString(max, memAllocStruct.ValueFactor);
                // DCPF_SUCCESSを返す。
                return SystemConstants.DCPF_SUCCESS;
            }

            return result;
        }

        /// <summary>
        /// メモリ割当データから、加工値、補正値、タイミング (GroupType), WorkIDグループ (GroupIndex) の、WorkID一覧を、IDに入れて返します。また、WorkIDグループがWORKGROUP_ROOTの場合、GroupIndexの一覧をIDに入れて返します。
        ///正常に処理が行われた場合、DCPF_SUCCESSを返します。
        ///GroupType, GroupIndexが範囲外の場合、ERR_WORKID_RANGEを返します。
        /// </summary>
        /// <param name="GroupType"></param>
        /// <param name="GroupIndex"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int GetMemryDataGroupList(int GroupType, int GroupIndex, ref int[] ID)
        {
            //GetGetMemoryDataGroupListコール
            try
            {
                memalloc.GetMemoryDataGroupList(GroupType, GroupIndex, ref ID);
            }
            catch (MemoryAllocationDataException)
            {
                return SystemConstants.ERR_WORKID_RANGE;
            }

            return SystemConstants.DCPF_SUCCESS;

        }

        /// <summary>
        /// MemReadを使用して、共有メモリから機械状態を取得し、機械状態を返します。
        /// </summary>
        /// <returns></returns>
        public int GetMachineStatus()
        {

            int machinestatus = 0;
            int machinestatus2 = 0;
            int status_adr = SystemConstants.ADDR_MACHINE_STATUS;

            MemRead(status_adr, ref machinestatus);
            MemRead(status_adr + 1, ref machinestatus2);
            machinestatus = (machinestatus2 << 16) | machinestatus;

            return machinestatus;
        }

        /// <summary>
        /// 通信クラスのMemReadを使用して、エラーコードのメモリを取得し、エラーコードは8アドレス使用している為、ErrBitの配列に入れて返す。
        /// </summary>
        /// <param name="ErrBit"></param>
        /// <returns></returns>
        public int GetErrorCode(ref int[] ErrBit)
        {
            //エラーコードのメモリ
            int Address = SystemConstants.ADDR_ERROR_STATUS;

            ErrBit = new int[8];

            for (int i = 0; i < ErrBit.Length; i++)
            {
                MemRead(Address++, ref ErrBit[i]);
            }

            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// 通信クラスのMemReadを使用して、機械バージョンのメモリを取得し、機械バージョンは4アドレス使用している為、Revisionの配列に入れて返す。
        /// </summary>
        /// <param name="Revision"></param>
        /// <returns></returns>
        public int GetMachineRevision(ref int[] Revision)
        {
            int Address = SystemConstants.ADDR_MACHINE_REVISION;

            for (int i = 0; i < SystemConstants.MACHINE_REVISION_COUNT; i++)
            {
                //該当箇所から機械バージョンを取得
                MemRead(Address++, ref Revision[i]);
            }

            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// 機械が設定済かを返します。
        ///設定済みの場合、FORMAT_DONEを返します。
        ///未設定の場合、FORMAT_NEEDを返します。
        /// </summary>
        /// <returns></returns>
        public int FomatCheck()
        {
            int check = 0;

            for (int i = 0; i < SystemConstants.FORMAT_CHECK_COUNT; i++)
            {
                machineConnector.MemRead(SystemConstants.ADDR_FORMAT_CHECK + i, ref check);
                if (check != 1)
                {
                    //該当箇所に1以外が存在した場合（未設定）
                    return SystemConstants.FORMAT_NEED;
                }
            }
            //該当箇所が全て1の場合（設定済み）
            return SystemConstants.FORMAT_DONE;
        }

        /// <summary>
        /// フォーマット領域に１を書き込みます。
        /// </summary>
        /// <returns></returns>
        public int FormatWrite()
        {
            for (int i = 0; i < SystemConstants.FORMAT_CHECK_COUNT; i++)
            {
                machineConnector.MemWrite(SystemConstants.ADDR_FORMAT_CHECK + i, 1);
            }

            machineConnector.MemSend(SystemConstants.ADDR_FORMAT_CHECK, SystemConstants.ADDR_FORMAT_CHECK, true);

            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// メモリ割当データとシステムデータを参照し、通信クラスのMemWrite, MemSendを使用して補正値を全て機械へ送信します。
        /// この際、MemSendは、Force オプションを使用します。
        ///システムデータに該当するIDの補正値が存在しない場合、標準値を機械へ送信し、システムデータに該当するIDの値を保存します。
        /// </summary>
        /// <returns></returns>
        public int CorrectDataSend(bool sendFlag=true)
        {
            //共通関数呼び出し
            return DataSend(SystemConstants.WORKID_TYPE_CORRECTDATA, sendFlag);
        }

        /// <summary>
        /// メモリ割当データとシステムデータを参照し、通信クラスのMemWrite, MemSendを使用してタイミングを全て機械へ送信します。この際、MemSendは、Force オプションを使用します。
        ///システムデータに該当するIDの補正値が存在しない場合、標準値を機械へ送信し、システムデータに該当するIDの値を保存します。

        /// </summary>
        public int TimingDataSend(bool sendFlag=true)
        {
            //共通関数呼び出し
            return DataSend(SystemConstants.WORKID_TYPE_TIMINGDATA, sendFlag);
        }

        /// <summary>
        /// メモリ割当データとシステムデータを参照し、通信クラスのMemGetを使用して、全ての補正値を機械から取得する
        /// </summary>
        public int CorrectDataRead(bool sendFlag=true)
        {
            //共通関数呼び出し
            return DataGet(SystemConstants.WORKID_TYPE_CORRECTDATA, sendFlag);
        }

        /// <summary>
        /// メモリ割当データとシステムデータを参照し、通信クラスのMemGetを使用して、全てのタイミングを機械から取得する
        /// </summary>
        public int TimingDataRead(bool sendFlag=true)
        {
            //共通関数呼び出し
            return DataGet(SystemConstants.WORKID_TYPE_TIMINGDATA, sendFlag);
        }

        /// <summary>
        /// 加工値メモリをロードする
        /// </summary>
        public int WorkDataRead(bool sendFlag=true)
        {
            int sendCount = 0;
            Dictionary<int, int> map = new Dictionary<int, int>();

            sendCount = DataGet(SystemConstants.WORKID_TYPE_WORKDATA, sendFlag);

            int[] btnIDs = new int[0];

            buttonControl.GetAllBtnWorkID(ref btnIDs);

            // 同一アドレスはまとめる
            foreach (var btnID in btnIDs)
            {
                ButtonActionStruct actionStruct = buttonControl.Get(btnID);
                if (!map.ContainsKey(actionStruct.Address))
                {
                    map.Add(actionStruct.Address, 1);
                }
            }

            // 機械へ送信する
            foreach (var address in map.Keys)
            {
                if (sendFlag)
                {
                    machineConnector.MemGet(address, 1);
                }
                sendCount++;
            }

            return sendCount;
        }

        /// <summary>
        /// 加工値の単位をメモリ割り当てデータから取得します。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="UnitStr"></param>
        /// <returns></returns>
        public int GetWorkDataUnit(int WorkID, ref string UnitStr)
        {

            int ret = 0;
            ret = GetUnit(WorkID, SystemConstants.WORKID_TYPE_WORKDATA, ref  UnitStr);
            if (ret == SystemConstants.MCPF_SUCCESS)
            {
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                return ret;
            }

        }

        /// <summary>
        /// 補正値の単位をメモリ割り当てデータから取得します。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="UnitStr"></param>
        /// <returns></returns>
        public int GetCorrectDataUnit(int WorkID, ref string UnitStr)
        {
            int ret = 0;
            ret = GetUnit(WorkID, SystemConstants.WORKID_TYPE_CORRECTDATA, ref  UnitStr);
            if (ret == SystemConstants.MCPF_SUCCESS)
            {
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                return ret;
            }

        }

        /// <summary>
        /// タイミングの単位をメモリ割り当てデータから取得します。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="UnitStr"></param>
        /// <returns></returns>
        public int GetTimingDataUnit(int WorkID, ref string UnitStr)
        {
            int ret = 0;
            ret = GetUnit(WorkID, SystemConstants.WORKID_TYPE_TIMINGDATA, ref  UnitStr);
            if (ret == SystemConstants.MCPF_SUCCESS)
            {
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                return ret;
            }

        }

        /// <summary>
        /// 指定されたメモリ割当データのバンク保存対象の削除します。
        /// 正常に処理が行われた場合、DCPF_SUCCESSを返します。
        /// </summary>
        /// <param name="BankNo">削除するバンクナンバー</param>
        /// <returns></returns>
        public int BankDataDelete(int BankNo)
        {
            int result = bankDataStorage.DeleteBankData(BankNo);

            // 正常に処理が行われた場合
            if (result == SystemConstants.BSPF_SUCCESS)
            {
                // DCPF_SUCCESSを返します
                return SystemConstants.DCPF_SUCCESS;
            }
            else
            {
                // WriteBankDataから、BSPF_SUCCESS以外を受取った場合、そのコードを返します
                return result;
            }
        }

    }
}
