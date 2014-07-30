using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Alchemist
{
    // チェック関数デリゲート
    public delegate bool BoolDelegate(ButtonActionStruct actionStruct, int WorkID, int Status);

    // WritePushBtn用の例外クラス
    public class WritePushBtnException : Exception
    {
        public int ErrorCode
        {
            get;
            set;
        }

        public WritePushBtnException(int ErrorCode)
        {
            this.ErrorCode = ErrorCode;
        }

        public static void ThrowException(int ErrorCode)
        {
            throw new WritePushBtnException(ErrorCode);
        }
    }

    // ボタン動作構造体
    public struct ButtonActionStruct
    {
        public int WorkID;
        public BoolDelegate ActionDelegate;
        public BoolDelegate OnOffCheck;
        public BoolDelegate OffOnCheck;
        public int Digit;
        public BoolDelegate RelatedDelegate;
        public int Address;
        public int BankStore;
    };

    // ボタンの挙動をコントロールするクラス
    public class ButtonControl
    {
        private DataController dataController;
        private MachineConnector machineConnector;
        private WorkDataMemory workDataMemory;

        public ButtonControl(DataController dataController, MachineConnector machineConnector, WorkDataMemory workDataMemory)
        {
            this.dataController = dataController;
            this.machineConnector = machineConnector;
            this.workDataMemory = workDataMemory;

            // 動作モード
            /*
            Add(SystemConstants.NORMAL_BTN, bitControlAction, 0, null, null, modeGroupAction, 0x0001, 0);				// NORMAL_BTN
            Add(SystemConstants.EJECT_BTN, bitControlAction, 1, null, null, modeGroupAction, 0x0001, 0);				// EJECT_BTN
            Add(SystemConstants.TEST_BTN, bitControlAction, 2, null, null, modeGroupAction, 0x0001, 0);					// TEST_BTN
            Add(SystemConstants.FREE_BTN, bitControlAction, 3, null, null, modeGroupAction, 0x0001, 0);					// FREE_BTN
            Add(SystemConstants.SAMPLE_BTN, bitControlAction, 4, null, null, modeGroupAction, 0x0001, 0);				// SAMPLE_BTN
            */
            Add(SystemConstants.NORMAL_BTN, valueSetAction, 1, null, null, modeGroupAction, 0x0001, 0);				// NORMAL_BTN
            Add(SystemConstants.EJECT_BTN, valueSetAction, 2, null, null, modeGroupAction, 0x0001, 0);				// EJECT_BTN
            Add(SystemConstants.TEST_BTN, valueSetAction, 3, null, null, modeGroupAction, 0x0001, 0);				// TEST_BTN
            Add(SystemConstants.FREE_BTN, valueSetAction, 4, null, null, modeGroupAction, 0x0001, 0);				// FREE_BTN
            Add(SystemConstants.SAMPLE_BTN, valueSetAction, 5, null, null, modeGroupAction, 0x0001, 0);				// SAMPLE_BTN

            // サイクルモード
            /*
            Add(SystemConstants.JOG_BTN, bitControlAction, 0, null, null, cycleModeAction, 0x0000, 0);				                // JOG_BTN
            Add(SystemConstants.CYCLE_BTN, bitControlAction, 1, null, null, cycleModeAction, 0x0000, 0);		                		// CYCLE_BTN
            Add(SystemConstants.AUTO_BTN, bitControlAction, 2, null, null, cycleModeAction, 0x0000, 0);				                // AUTO_BTN
            */
            Add(SystemConstants.JOG_BTN, valueSetAction, 0, null, null, cycleModeAction, 0x0000, 0);				 // JOG_BTN
            Add(SystemConstants.CYCLE_BTN, valueSetAction, 1, null, null, cycleModeAction, 0x0000, 0);		         // CYCLE_BTN
            Add(SystemConstants.AUTO_BTN, valueSetAction, 2, null, null, cycleModeAction, 0x0000, 0);				 // AUTO_BTN

            // 加工モード1
            Add(SystemConstants.STRIP1_BTN, bitControlAction, 0, null, null, strip1Action, 0x0002, 1);					// STRIP1_BTN

            // 加工モード2
            Add(SystemConstants.STRIP2_BTN, bitControlAction, 0, null, null, strip2Action, 0x0003, 1);					// STRIP2_BTN

            // 加工モード3

            // 加工モード4
            Add(SystemConstants.LOT_INTERVAL1_BTN, valueSetAction, 1, null, null, null, 0x000B, 0);						// LOT_INTERVAL1_BTN

            // 段取り
            Add(SystemConstants.FEED1_BTN, valueSetAction, 1, null, null, autoOffAction, 0x000D, 0);					// FEED1_BTN
            Add(SystemConstants.PERMIT_COUNTUP_BTN, valueSetAction, 1, null, null, null, 0x001E, 0);					// PERMIT_COUNTUP_BTN
            Add(SystemConstants.WIRE_DISENTANGLE_BTN, valueSetAction, 1, null, null, null, 0x0015, 1);                  // WIRE_DISENTANGLE_BTN
            Add(SystemConstants.CUT_WIRETOP_BTN, valueSetAction, 1, null, null, null, 0x0016, 1);                       // CUT_WIRETOP_BTN

            // 操作
            Add(SystemConstants.MACHINE_START1_BTN, valueSetAction, 1, null, null, autoOffAction, 0x0010, 0);			// MACHINE_START1_BTN
            Add(SystemConstants.MACHINE_STOP_BTN, valueSetAction, 1, null, null, autoOffAction, 0x0011, 0);				// MACHINE_STOP_BTN
            Add(SystemConstants.MACHINE_RESET_BTN, valueSetAction, 1, null, null, autoOffAction, 0x0012, 0);			// MACHINE_RESET_BTN

            // カウンタ
            Add(SystemConstants.TOTAL_COUNTER_RESET1_BTN, valueSetAction, 1, null, null, autoOffAction, 0x001A, 0);	    // TOTAL_COUNTER_RESET1_BTN
            Add(SystemConstants.LOT_COUNTER_RESET1_BTN, valueSetAction, 1, null, null, autoOffAction, 0x001C, 0);		// LOT_COUNTER_RESET1_BTN
            Add(SystemConstants.QTY_COUNTER_RESET1_BTN, valueSetAction, 1, null, null, autoOffAction, 0x001B, 0);		// QTY_COUNTER_RESET1_BTN
            Add(SystemConstants.COUNT_UP_BTN, bitControlAction, 0, null, null, autoOffAction, 0x001F, 0);				// COUNT_UP_BTN
            Add(SystemConstants.COUNT_DOWN_BTN, bitControlAction, 1, null, null, autoOffAction, 0x001F, 0);				// COUNT_DOWN_BTN

            // 検出ロック
            Add(SystemConstants.STRIP1_SENSOR_LOCK, bitControlAction, 0, null, null, null, 0x0004, 0);					// STRIP1_SENSOR_LOCK

            // 警報
            Add(SystemConstants.ALARM_WIRELENGTH, valueSetAction, 1, null, null, null, 0x000F, 0);                      // 切断長確認            
            Add(SystemConstants.WIRE_CHANGE, valueSetAction, 1, null, null, null, 0x000E, 0);                           // 電線照合中インターロック            
        }

        // ボタン動作設定table
        private Dictionary<int, ButtonActionStruct> map = new Dictionary<int, ButtonActionStruct>();

        // ボタン動作設定登録関数
        private void Add(int WorkID, BoolDelegate Action, int Digit, BoolDelegate OnOffCheck,
            BoolDelegate OffOnCheck, BoolDelegate RelatedDelegate, int Address, int BankStore)
        {
            ButtonActionStruct actionStruct = new ButtonActionStruct();
            actionStruct.WorkID = WorkID;
            actionStruct.ActionDelegate = Action;
            actionStruct.OnOffCheck = OnOffCheck;
            actionStruct.OffOnCheck = OffOnCheck;
            actionStruct.RelatedDelegate = RelatedDelegate;
            actionStruct.Digit = Digit;
            actionStruct.Address = Address;
            actionStruct.BankStore = BankStore;

            map.Add(
                WorkID,
                actionStruct
            );
        }

        /// <summary>
        /// ボタン動作の構造体を取得します。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <returns></returns>
        public ButtonActionStruct Get(int WorkID)
        {
            return map[WorkID];
        }

        /// <summary>
        /// ボタン動作のWorkIDをすべて取得します。
        /// </summary>
        /// <param name="WorkID"></param>
        public void GetAllBtnWorkID(ref int[] BtnWorkID)
        {
            int i = 0;

            foreach (int btnworkID in map.Keys)
            {
                // 配列の要素を1つ増やす
                Array.Resize(ref BtnWorkID, BtnWorkID.Length + 1);

                // ボタンのworkIDを追加していく
                BtnWorkID[i] = btnworkID;
                i++;
            }
        }

        /// <summary>
        /// WorkIDのアドレスのBitPosで指定されたビット位置を取得し、
        /// BitMemValに入れて返します。
        /// BitMemValは0か0以外が返されます。
        /// 0以外が返された場合、BitPosビット目が1であることを示しています。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="BitPos"></param>
        /// <param name="BitMemVal"></param>
        public void bitRead(int WorkID, int BitPos, ref int BitMemVal)
        {
            // WorkIDのアドレスを取得
            ButtonActionStruct action = Get(WorkID);

            // WorkIDのアドレスの値を取得
            int result = machineConnector.MemRead(action.Address, ref BitMemVal);

            // BitPosビット目を取得
            BitMemVal = BitMemVal & (1 << (BitPos));
        }

        /// <summary>
        /// ボタンの設定値を取得する
        /// </summary>
        /// <param name="WorkID"></param>
        /// <returns></returns>
        private int GetBtnStatus(int WorkID)
        {
            int BitMemVal = 0;

            // WorkIDのアドレスを取得
            ButtonActionStruct action = Get(WorkID);

            // ビット操作の場合
            if (action.ActionDelegate == bitControlAction)
            {
                // WorkIDのアドレスの値を取得
                int result = machineConnector.MemReadPC(action.Address, ref BitMemVal);

                // BitPosビット目を取得
                BitMemVal = BitMemVal & (1 << (action.Digit));

                if (BitMemVal == 0)
                {
                    return SystemConstants.BTN_OFF;
                }
                else
                {
                    return SystemConstants.BTN_ON;
                }
            }
            // 値設定の場合
            else if (action.ActionDelegate == valueSetAction)
            {
                // データを読む
                machineConnector.MemReadPC(action.Address, ref BitMemVal);

                // 読んだ値＝設定値の場合ON
                if (BitMemVal == action.Digit)
                {
                    return SystemConstants.BTN_ON;
                }
                // 読んだ値！＝設定値の場合OFF
                else
                {
                    return SystemConstants.BTN_OFF;
                }
            }

            return SystemConstants.BTN_OFF;
        }

        /// <summary>
        /// 操作ボタンIDの状態を、BtnStatusに入れて返します。
        /// </summary>
        /// <param name="BtnID"></param>
        /// <param name="BtnStatus"></param>
        /// <returns></returns>
        public int ReadPushBtn(int BtnID, ref int BtnStatus)
        {
            ButtonActionStruct actionStruct = Get(BtnID);
            int BitMem = 0;

            // ビット操作
            if (actionStruct.ActionDelegate == bitControlAction)
            {

                // ビットを読む
                bitRead(BtnID, actionStruct.Digit, ref BitMem);

                // 値を変換する
                if (BitMem == 0)
                {
                    BtnStatus = SystemConstants.BTN_OFF;
                }
                else
                {
                    BtnStatus = SystemConstants.BTN_ON;
                }
            }
            // 値セットの場合
            else if (actionStruct.ActionDelegate == valueSetAction) 
            {
                // データを読む
                machineConnector.MemRead(actionStruct.Address, ref BitMem);

                // 読んだ値＝設定値の場合ON
                if (BitMem == actionStruct.Digit) 
                {
                    BtnStatus = SystemConstants.BTN_ON;
                }
                // 読んだ値！＝設定値の場合OFF
                else  
                {
                    BtnStatus = SystemConstants.BTN_OFF;
                }
            }

            return SystemConstants.DCPF_SUCCESS;
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
        public int WritePushBtn(int BtnID, int BtnStatus, bool execRelated = true, bool initFlag = true)
        {
            int ret = 0;

            ButtonActionStruct actionStruct;
            try
            {
                actionStruct = Get(BtnID);
            }
            catch
            {
                return SystemConstants.ERR_NO_WORK_ID;
            }
            
            //ボタンの状態を取得する
            int currentBtnStatus = 0;

            // 加工値メモリからボタンの状態を取得する
            currentBtnStatus = GetBtnStatus(BtnID);
            
            // BTN_PUSHの場合、BtnStatusを逆にする
            if (BtnStatus == SystemConstants.BTN_PUSH)
            {
                if (currentBtnStatus == SystemConstants.BTN_ON)
                {
                    BtnStatus = SystemConstants.BTN_OFF;
                }
                else
                {
                    BtnStatus = SystemConstants.BTN_ON;
                }
            }

            // 状態の変化がない場合は、何もしない
            if (initFlag == true)
            {
                if (((currentBtnStatus == SystemConstants.BTN_OFF) && (BtnStatus == SystemConstants.BTN_OFF)) ||
                    ((currentBtnStatus == SystemConstants.BTN_ON) && (BtnStatus == SystemConstants.BTN_ON)))
                {
                    return SystemConstants.DCPF_SUCCESS;
                }
            }

            try
            {
                // 関連動作を実行が指定されていれば関連動作を実行する
                if (execRelated)
                {
                    // OFF->ONの場合
                    if ((currentBtnStatus == SystemConstants.BTN_OFF) &&
                        (BtnStatus == SystemConstants.BTN_ON))
                    {

                        // チェックNGの場合は、終了
                        if (actionStruct.OffOnCheck != null)
                        {
                            if (actionStruct.OffOnCheck(actionStruct, BtnID, BtnStatus) == false)
                            {
                                return SystemConstants.DCPF_SUCCESS;
                            }
                        }
                    }
                    // ON->OFFの場合
                    else
                    {
                        if (actionStruct.OnOffCheck != null)
                        {
                            // チェックNGの場合は、終了
                            if (actionStruct.OnOffCheck(actionStruct, BtnID, BtnStatus) == false)
                            {
                                return SystemConstants.DCPF_SUCCESS;
                            }
                        }
                    }
                    if (actionStruct.RelatedDelegate != null)
                    {
                        if (actionStruct.RelatedDelegate(actionStruct, BtnID, BtnStatus) == false)
                        {
                            return SystemConstants.DCPF_SUCCESS;
                        }
                    }

                    // 自分の動作を実行
                    actionStruct.ActionDelegate(actionStruct, BtnID, BtnStatus);


                }
                else
                {
                    // 自分の動作を実行
                    actionStruct.ActionDelegate(actionStruct, BtnID, BtnStatus);
                }
            }
            // WritePushBtnで例外が発生した場合、エラーコードに変換する
            catch (WritePushBtnException ex)
            {
                // 関連動作の場合
                if (execRelated == false)
                {
                    throw;
                }
                // 関連動作ではない場合
                else
                {
                    ret = ex.ErrorCode;
                    return ret;
                }
            }

            // initFlagがtrueの場合は、強制的にメモリを送る
            machineConnector.MemSend(actionStruct.Address, 1, initFlag);

            // 初期化フラグが立っている場合のみファイルにセーブする。
            if (initFlag)
            {
                // バンク保存対象の場合保存を行う
                if (actionStruct.BankStore == 1)
                {
                    ret = dataController.BankDataSave();
                    if (ret != SystemConstants.DCPF_SUCCESS)
                    {
                        // 関連動作の場合
                        if (!execRelated)
                        {
                            WritePushBtnException.ThrowException(ret);
                        }
                        else
                        {
                            return ret;
                        }
                    }
                }
            }

            return SystemConstants.DCPF_SUCCESS;
        }

        /// <summary>
        /// ビット操作をします。
        /// On_offが0だった場合、WorkIDのアドレスのBitPosで指定されたビット位置を反転します。
        /// On_offが1だった場合、WorkIDのアドレスのBitPosで指定されたビット位置を1にします。
        /// On_offが2だった場合、WorkIDのアドレスのBitPosで指定されたビット位置を0にします。
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="BitPos"></param>
        /// <param name="On_off"></param>
        private void bitControl(int WorkID, int BitPos, int On_off)
        {
            int memVal = 0;
            // WorkIDのアドレスを取得
            ButtonActionStruct action = Get(WorkID);

            // WorkIDのvalueを取得する。
            machineConnector.MemReadPC(action.Address, ref memVal);

            // On_offがPushだった場合
            if (On_off == SystemConstants.BTN_PUSH)
            {
                // ON->OFF、OFF->ONにする
                memVal = memVal ^ (1 << BitPos);
            }
            // On_offがOnだった場合
            else if (On_off == SystemConstants.BTN_ON)
            {
                // BitPosビット目を1にします
                memVal = memVal | (1 << BitPos);
            }
            // On_offがOffだった場合
            else if (On_off == SystemConstants.BTN_OFF)
            {
                // BitPosビット目を0にします
                memVal = memVal & ~(1 << BitPos);
            }

            //加工値メモリに保存する。
            workDataMemory.Set(SystemConstants.TYPE_WORKBTN, WorkID, On_off.ToString());

            // ビット演算結果をMemWriteする
            machineConnector.MemWrite(action.Address, memVal);
        }

        // ボタンの状態（PC側）を取得する
        private void ReadPushBtnPC(int BtnID, ref int BtnStatus)
        {
            BtnStatus = GetBtnStatus(BtnID);
        }

        // ビットを操作するアクション
        public bool bitControlAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            bitControl(WorkID, actionStruct.Digit, BtnStatus);

            return true;
        }

        // 値を直接セットするアクション
        public bool valueSetAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            // ONの場合
            if (BtnStatus == SystemConstants.BTN_ON)
            {
                //加工値メモリに保存する。
                workDataMemory.Set(SystemConstants.TYPE_WORKBTN, WorkID, actionStruct.Digit.ToString());

                // ビット演算結果をMemWriteする
                machineConnector.MemWrite(actionStruct.Address, actionStruct.Digit);

            }
            // OFFの場合
            else if (BtnStatus == SystemConstants.BTN_OFF)
            {
                //加工値メモリに保存する。
                workDataMemory.Set(SystemConstants.TYPE_WORKBTN, WorkID, "0");

                // ビット演算結果をMemWriteする
                machineConnector.MemWrite(actionStruct.Address, 0);
            }

            return true;
        }

        // 動作モードグループの関連アクション
        public bool modeGroupAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            // 切断対象ボタン
            int[] OffBtnIDs = new int[] {
				SystemConstants.NORMAL_BTN,
				SystemConstants.EJECT_BTN,
				SystemConstants.TEST_BTN, 
				SystemConstants.SAMPLE_BTN,
				SystemConstants.FREE_BTN
			};

            // 自分のボタン以外はOffにする
            foreach (int btnID in OffBtnIDs)
            {
                if (WorkID != btnID)
                {
                    WritePushBtn(btnID, SystemConstants.BTN_OFF, false);
                }
            }

            return true;
        }

        // サイクルモードの関連アクション
        public bool cycleModeAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            // 切断対象ボタン
            int[] OffBtnIDs = new int[] {
				SystemConstants.CYCLE_BTN,
				SystemConstants.JOG_BTN,
				SystemConstants.AUTO_BTN
			};

            // 自分のボタン以外はOffにする
            foreach (int btnID in OffBtnIDs)
            {
                if (WorkID != btnID)
                {
                    WritePushBtn(btnID, SystemConstants.BTN_OFF, false);
                }
            }

            return true;
        }

        // OFF動作禁止
        public bool forbidOffAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            // OFFの場合は、falseを返す
            if (SystemConstants.BTN_OFF == BtnStatus)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // 読み取り専用
        public bool readonlyAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            return false;
        }

        // 1秒したらOffにする。
        public bool autoOffAction(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            // ONの場合
            if (SystemConstants.BTN_ON == BtnStatus)
            {
                // 1秒後にOFFにする処理を予約する
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(delegate(object o)
                    {
                        Thread.Sleep(1000);
                        WritePushBtn(WorkID, SystemConstants.BTN_OFF, true);
                    })
                );
            }

            return true;
        }

        // ストリップ１をOFFにする場合、圧着１防水１、ハーフストリップでONになっているボタンをOFFにする。
        public bool strip1Action(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            /*if (SystemConstants.BTN_OFF == BtnStatus)
            {
                WritePushBtn(SystemConstants.CRIMP1_BTN, SystemConstants.BTN_OFF, false);
                WritePushBtn(SystemConstants.SEAL1_BTN, SystemConstants.BTN_OFF, false);
                WritePushBtn(SystemConstants.SEMISTRIP1_BTN, SystemConstants.BTN_OFF, false);
                WritePushBtn(SystemConstants.CRIMP1_SW_BTN, SystemConstants.BTN_OFF, false);
            }*/

            return true;
        }

        // ストリップ2をOFFにする場合、圧着2, 防水2, ハーフストリップ2で、
        // ONになっているボタンをOFFにして、ストリップ2をOFFにする。
        public bool strip2Action(ButtonActionStruct actionStruct, int WorkID, int BtnStatus)
        {
            /*if (SystemConstants.BTN_OFF == BtnStatus)
            {
                WritePushBtn(SystemConstants.CRIMP2_BTN, SystemConstants.BTN_OFF, false);
                WritePushBtn(SystemConstants.SEAL2_BTN, SystemConstants.BTN_OFF, false);
                WritePushBtn(SystemConstants.SEMISTRIP2_BTN, SystemConstants.BTN_OFF, false);
                WritePushBtn(SystemConstants.CRIMP2_SW_BTN, SystemConstants.BTN_OFF, false);
            }*/

            return true;
        }

    }
}
