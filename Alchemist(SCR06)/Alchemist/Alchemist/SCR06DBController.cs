using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Alchemist
{
    class SCR06DBController : SQLServerController
    {
        public bool ServerOnline = false;
        public bool F_Dandori = false;  //段取り中

        public SCR06DBController(SQLServerConnectionStruct cs) : base(cs) { }

        /// <summary>
        /// SQLServerとの接続テストを行います。
        /// </summary>
        public int dbConnectTest()
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            try
            {
                db.Connection.Open();
                db.Connection.Close();
            }
            catch
            {
                return SystemConstants.ERR_SQL_CONNECT;
            }
            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線種情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemWireTypeComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireType
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.WireType);
            }           
        }

        /// <summary>
        /// 電線サイズ情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemWireSizeComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireSize
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.WireSize);
            }
        }

        /// <summary>
        /// シース色情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemColorCharComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Color
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.ColorChar);
            }
        }

        /// <summary>
        /// 芯線色情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemColorNoComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_ColorNo
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.ColorNo);
            }
        }

        /// <summary>
        /// 電線詳細マスターを取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemWireDetailComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireDetail
                        select n;

            foreach (var m in query)
            {
                cbx.Items.Add(m.Wire_Code);
            }
        }

        /// <summary>
        /// 品質記録コード情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemQualityCodeComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Quality
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.QualityCode);
            }
        }

        /// <summary>
        /// 作業者マスターから情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemOperatorCodeComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Operator
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.OperatorCode);
            }
        }

        /// <summary>
        /// 部署マスターから部署コードを取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemSectionCodeComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Section
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.SectionCode);
            }
        }

        /// <summary>
        /// 電線種情報から電線種コード情報を返します。
        /// </summary>
        public string dbGetWireTypeCode(string WireType)
        {
            string wiretypecode = "";

            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireType
                        where n.WireType == WireType
                        select n;
            foreach (var m in query)
            {
                wiretypecode = m.WireTypeCode;
            }
            return wiretypecode;
        }

        /// <summary>
        /// 電線サイズ情報から電線サイズコード情報を返します。
        /// </summary>
        public string dbGetWireSizeCode(string WireSize)
        {
            string wiresizecode = "";

            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireSize
                        where n.WireSize == WireSize
                        select n;
            foreach (var m in query)
            {
                wiresizecode = m.WireSizeCode;
            }
            return wiresizecode;
        }

        /// <summary>
        /// シース色情報からシース色コード情報を返します。
        /// </summary>
        public string dbGetColorCode(string ColorChar)
        {
            string colorcode = "";

            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Color
                        where n.ColorChar == ColorChar
                        select n;
            foreach (var m in query)
            {
                colorcode = m.ColorCode;
            }
            return colorcode;
        }

        /// <summary>
        /// 芯線色情報から芯線色コード情報を返します。
        /// </summary>
        public string dbGetColorNoCode(string ColorNo)
        {
            string colornocode = "";

            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_ColorNo
                        where n.ColorNo == ColorNo
                        select n;
            foreach (var m in query)
            {
                colornocode = m.ColorNoCode;
            }
            return colornocode;
        }

        /// <summary>
        /// 電線色文字から電線色コード情報を返します。
        /// </summary>
        public string dbGetColorRGBCode(string ColorChar)
        {
            string colorcode = "";

            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Color
                        where n.ColorChar == ColorChar
                        select n;
            foreach (var m in query)
            {
                colorcode = m.ColorRGBCode;
            }
            return colorcode;
        }

        /// <summary>
        /// 電線情報から電線コード情報を返します。
        /// </summary>
        public string dbGetWireCode(string WireType, string WireSize, string ColorChar, string ColorNo)
        {
            string wirecode = "";

            wirecode += dbGetWireTypeCode(WireType);
            wirecode += dbGetWireSizeCode(WireSize);
            wirecode += dbGetColorCode(ColorChar);
            wirecode += dbGetColorNoCode(ColorNo);

            return wirecode;
        }

        /// <summary>
        /// 作業者コードから作業者名情報を返します。
        /// 　登録していない作業者の場合はエラーを返します。
        /// </summary>
        public int dbGetOperatorName(string OperatorCode, ref string OperatorName)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Operator
                        where n.OperatorCode == OperatorCode
                        select n;

            if (query.Count() != 1)
            {
                return SystemConstants.ERR_OPERATOR_NAME;
            }
            
            foreach (var m in query)
            {
                OperatorName = m.OperatorName;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業者コードから作業者グループ情報を返します。
        /// 　登録していない作業者の場合はエラーを返します。
        /// </summary>
        public int dbGetOperatorGroup(string OperatorCode, ref M_Operator Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Operator
                        where n.OperatorCode == OperatorCode
                        select n;

            if (query.Count() != 1)
            {
                return SystemConstants.ERR_OPERATOR_NAME;
            }

            foreach (var m in query)
            {
                Data.OperatorGroup = m.OperatorGroup;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業者コードからスーパーバイザーかどうかを返します。
        /// 　登録していない作業者の場合はエラーを返します。
        /// </summary>
        public int dbGetSuperVisorOperator(string OperatorCode)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Operator
                        where n.OperatorCode == OperatorCode && n.OperatorGroup == 3
                        select n;

            if (query.Count() != 1)
            {
                return SystemConstants.ERR_OPERATOR_NAME;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 部署コードから部署名を取得します。
        /// 　登録していない作業者の場合はエラーを返します。
        /// </summary>
        public int dbGetSectionName(int SectionCode, ref string SectionName)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Section
                        where n.SectionCode == SectionCode
                        select n;

            if (query.Count() != 1)
            {
                return SystemConstants.ERR_NO_SECTION;
            }

            foreach (var m in query)
            {
                SectionName = m.SectionName;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業中の作業者の作業者情報を返します。
        /// 　作業中の作業者いない場合はエラーを返します。
        /// 　作業中の作業者がいても作業日が今日の日付で無い場合はエラーを返します。
        /// </summary>
        public int dbGetTemporaryOperator(ref T_Operator TempOperator)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.T_Operator
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_TEMP_OPERATOR_NOBODY;
            }

            foreach (var m in query)
            {
                if (DateTime.Today.Date != m.WorkDate.Date)
                {
                    return SystemConstants.ERR_TEMP_OPERATOR_TODAY;
                };
                TempOperator = m;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業中の作業者情報を削除します。
        /// 　削除に失敗するとエラーを返します。
        /// </summary>
        public int dbDelTemporaryOperator()
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.T_Operator
                        select n;
            foreach (var m in query)
            {
                db.T_Operator.DeleteOnSubmit(m);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_TEMP_OPERATOR_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業中の作業者情報に作業者情報を追加します。
        /// 　追加に失敗するとエラーを返します。
        /// </summary>
        public int dbAddTemporaryOperator(T_Operator TempOperator)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);

            db.T_Operator.InsertOnSubmit(TempOperator);
           
            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_TEMP_OPERATOR_ADD;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線情報に電線コードが登録されているか確認します。
        /// 　電線コードが見つからないとエラーを返します。
        /// </summary>
        public int dbGetWireInfomation(string WireCode, ref M_WireDetail WireInfo)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireDetail
                        where n.Wire_Code == WireCode
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_WIRE_INFO;
            }

            foreach (var m in query)
            {
                WireInfo = m;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 連番情報から作業情報に登録されているか確認します。
        /// 　連番コードが見つからないとエラーを返します。
        /// </summary>
        public int dbGetWorkData(string Renban, ref D_Work Workdata)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.D_Work
                        where n.Renban == Renban
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_WORKDATA;
            }

            foreach (var m in query)
            {
                Workdata = m;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業実績情報に登録を行います。
        /// 　登録に失敗するとエラーを返します。
        /// </summary>
        public int dbInsertResultWorkData(R_Work WorkData)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);

            db.R_Work.InsertOnSubmit(WorkData);

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_RESULT_WORKDATA_INSERT;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業実績情報から要求された連番の中断データを返します。
        /// 　登録が無い場合はエラーを返します。
        /// </summary>
        public int dbGetInterruptResultWorkData(string Renban, ref R_Work WorkData)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.R_Work
                        where n.Renban == Renban && n.Sagyou_status == 'C'
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_INTERRUPT_RESULT_WORKDATA;
            }

            foreach (var m in query)
            {
                WorkData = m;
            }

            return SystemConstants.GET_INTERRUPT_RESULT_WORKDATA;
        }

        /// <summary>
        /// 作業実績情報から現在作業中のデータを返します。
        /// 　登録が無い場合はエラーを返します。
        /// </summary>
        public int dbGetStartResultWorkData(ref R_Work WorkData)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.R_Work
                        where n.Sagyou_status == 'S'
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_START_RESULT_WORKDATA;
            }

            foreach (var m in query)
            {
                WorkData = m;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業実績情報から要求された連番が終了しているかを返します。
        /// 　登録が無い場合はエラーを返します。
        /// </summary>
        public int dbGetCompleteResultWorkData(string Renban)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.R_Work
                        where n.Renban == Renban && n.Sagyou_status == 'E'
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_COMPLETE_RESULT_WORKDATA;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業実績情報から指定した連番のデータの内容更新します。
        /// 　登録が無い場合はエラーを返します。
        /// </summary>
        public int dbUpdateResultWorkData(R_Work WorkData)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.R_Work
                        where n.Renban == WorkData.Renban
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_RESULT_WORKDATA_UPDATE;
            }

            foreach (var m in query)
            {
                db.R_Work.DeleteOnSubmit(m);               
            }

            db.R_Work.InsertOnSubmit(WorkData);

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_RESULT_WORKDATA_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 前回のエフを返します。
        /// 　登録が無い場合はエラーを返します。
        /// </summary>
        public int dbGetLastWorkData(ref L_Work WorkData)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.L_Work
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_LAST_WORKDATA;
            }

            foreach (var m in query)
            {
                WorkData = m;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業実績変更時に前回エフとして記録します。
        /// 　登録に失敗するとエラーを返します。
        /// </summary>
        public int dbUpdateLastWorkData(L_Work WorkData)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.L_Work
                        select n;

            if (query.Count() == 0)
            {
                db.L_Work.InsertOnSubmit(WorkData);
            }
            else
            {
                foreach (var m in query)
                {
                    db.L_Work.DeleteOnSubmit(m);
                }

                db.L_Work.InsertOnSubmit(WorkData);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_LAST_WORKDATA_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 前回エフを削除します。※試行作業を行うとき
        /// 　削除に失敗するとエラーを返します。
        /// </summary>
        public int dbDeleteteLastWorkData()
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.L_Work
                        select n;

            if (query.Count() != 0)
            {
                foreach (var m in query)
                {
                    db.L_Work.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_LAST_WORKDATA_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 連番情報から電線のガイドがガイド・ブレード情報に登録されているか確認します。
        /// 　情報が見つからないとエラーを返します。
        /// </summary>
        public int dbGetGuideName(string WireType, string WireSize, ref string GuideName1, ref string GuideName2)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_GuideBlade
                        where n.WireType == WireType && n.WireSize == WireSize
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_GUIDE;
            }

            foreach (var m in query)
            {
                GuideName1 = m.Guide1;
                GuideName2 = m.Guide2;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 連番情報から電線のブレードがガイド・ブレード情報に登録されているか確認します。
        /// 　情報が見つからないとエラーを返します。
        /// </summary>
        public int dbGetBladeName(string WireType, string WireSize, ref string BladeName)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_GuideBlade
                        where n.WireType == WireType && n.WireSize == WireSize
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_BLADE;
            }

            foreach (var m in query)
            {
                BladeName = m.Blade;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 品質記録項目情報を取得し、コンボボックスに入れます。
        /// </summary>
        public void dbGetItemQualityItemComboBox(ref ComboBox cbx)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Quality
                        select n;
            foreach (var m in query)
            {
                cbx.Items.Add(m.QualityItem);
            }
        }

        /// <summary>
        /// 品質記録項目コードから品質記録項目を取得します。
        /// </summary>
        public string dbGetQualityItem(int QualityCode)
        {
            string qualityitem = "";

            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Quality
                        where n.QualityCode == QualityCode
                        select n;

            foreach (var m in query)
            {
                qualityitem = m.QualityItem;
            }

            return qualityitem;
        }

        /// <summary>
        /// 品質記録情報を取得します。
        /// 　情報が見つからない場合はエラーを返します。
        /// </summary>
        public int dbGetQualityRecord(string Renban, ref R_Quality QualityRecord)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.R_Quality
                        where n.Renban == Renban
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_NO_QUALITY_RECORD;
            }

            foreach (var m in query)
            {
                QualityRecord = m;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 品質記録実績情報に登録を行います。
        /// 　登録に失敗するとエラーを返します。
        /// </summary>
        public int dbUpdateQualityRecord(R_Quality QualityRecord)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.R_Quality
                        where n.Renban == QualityRecord.Renban
                        select n;

            if (query.Count() == 0)
            {
                db.R_Quality.InsertOnSubmit(QualityRecord);
            }
            else
            {
                foreach (var m in query)
                {
                    db.R_Quality.DeleteOnSubmit(m);
                }

                db.R_Quality.InsertOnSubmit(QualityRecord);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_QUALITY_RECORD_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 品質記録項目マスターの内容を更新します。
        /// </summary>
        public int dbUpdateQuality(M_Quality Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Quality
                        where n.QualityCode == Data.QualityCode
                        select n;

            if (query.Count() == 0)
            {
                db.M_Quality.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Quality.DeleteOnSubmit(m);
                }

                db.M_Quality.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線種マスターの内容を更新します。
        /// </summary>
        public int dbUpdateWireType(M_WireType Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireType
                        where n.WireType == Data.WireType
                        select n;

            if (query.Count() == 0)
            {
                db.M_WireType.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_WireType.DeleteOnSubmit(m);
                }

                db.M_WireType.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線サイズマスターの内容を更新します。
        /// </summary>
        public int dbUpdateWireSize(M_WireSize Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireSize
                        where n.WireSize == Data.WireSize
                        select n;

            if (query.Count() == 0)
            {
                db.M_WireSize.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_WireSize.DeleteOnSubmit(m);
                }

                db.M_WireSize.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線色マスターの内容を更新します。
        /// </summary>
        public int dbUpdateColor(M_Color Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Color
                        where n.ColorChar == Data.ColorChar
                        select n;

            if (query.Count() == 0)
            {
                db.M_Color.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Color.DeleteOnSubmit(m);
                }

                db.M_Color.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線色番号マスターの内容を更新します。
        /// </summary>
        public int dbUpdateColorNo(M_ColorNo Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_ColorNo
                        where n.ColorNo == Data.ColorNo
                        select n;

            if (query.Count() == 0)
            {
                db.M_ColorNo.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_ColorNo.DeleteOnSubmit(m);
                }

                db.M_ColorNo.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線詳細マスターの内容を更新します。
        /// </summary>
        public int dbUpdateWireDetail(M_WireDetail Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireDetail
                        where n.Wire_Code == Data.Wire_Code
                        select n;

            if (query.Count() == 0)
            {
                db.M_WireDetail.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_WireDetail.DeleteOnSubmit(m);
                }

                db.M_WireDetail.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業者マスターの内容を更新します。
        /// </summary>
        public int dbUpdateOperator(M_Operator Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Operator
                        where n.OperatorCode == Data.OperatorCode
                        select n;

            if (query.Count() == 0)
            {
                db.M_Operator.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Operator.DeleteOnSubmit(m);
                }

                db.M_Operator.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 部署マスターの内容を更新します。
        /// </summary>
        public int dbUpdateSection(M_Section Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Section
                        where n.SectionCode == Data.SectionCode
                        select n;

            if (query.Count() == 0)
            {
                db.M_Section.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Section.DeleteOnSubmit(m);
                }

                db.M_Section.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// ガイド・ブレードマスターの内容を更新します。
        /// </summary>
        public int dbUpdateGuideBlade(M_GuideBlade Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_GuideBlade
                        where n.WireType == Data.WireType && n.WireSize == Data.WireSize
                        select n;

            if (query.Count() == 0)
            {
                db.M_GuideBlade.InsertOnSubmit(Data);
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_GuideBlade.DeleteOnSubmit(m);
                }

                db.M_GuideBlade.InsertOnSubmit(Data);
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_QUALITY_RECORD_UPDATE;
            }

            return SystemConstants.SQL_SUCCESS;
        }


        /// <summary>
        /// 電線種マスターの内容を削除します。
        /// </summary>
        public int dbDeleteWireType(M_WireType Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireType
                        where n.WireType == Data.WireType
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_WireType.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線サイズマスターの内容を削除します。
        /// </summary>
        public int dbDeleteWireSize(M_WireSize Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireSize
                        where n.WireSize == Data.WireSize
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_WireSize.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線色マスターの内容を削除します。
        /// </summary>
        public int dbDeleteColor(M_Color Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Color
                        where n.ColorChar == Data.ColorChar
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Color.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線色番号マスターの内容を削除します。
        /// </summary>
        public int dbDeleteColorNo(M_ColorNo Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_ColorNo
                        where n.ColorNo == Data.ColorNo
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_ColorNo.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 電線詳細マスターの内容を削除します。
        /// </summary>
        public int dbDeleteWireDetail(M_WireDetail Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_WireDetail
                        where n.Wire_Code == Data.Wire_Code
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_WireDetail.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 品質記録項目マスターの内容を削除します。
        /// </summary>
        public int dbDeleteQuality(M_Quality Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Quality
                        where n.QualityCode == Data.QualityCode
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Quality.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 作業者マスターの内容を削除します。
        /// </summary>
        public int dbDeleteOperator(M_Operator Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Operator
                        where n.OperatorCode == Data.OperatorCode
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Operator.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// 部署マスターの内容を削除します。
        /// </summary>
        public int dbDeleteSection(M_Section Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_Section
                        where n.SectionCode == Data.SectionCode
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_Section.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

        /// <summary>
        /// ガイド・ブレードマスターの内容を削除します。
        /// </summary>
        public int dbDeleteGuideBlade(M_GuideBlade Data)
        {
            var db = new SCR06DBDataClassesDataContext(stConnectionString);
            var query = from n in db.M_GuideBlade
                        where n.WireType == Data.WireType && n.WireSize == Data.WireSize
                        select n;

            if (query.Count() == 0)
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }
            else
            {
                foreach (var m in query)
                {
                    db.M_GuideBlade.DeleteOnSubmit(m);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return SystemConstants.ERR_MASTER_DELETE;
            }

            return SystemConstants.SQL_SUCCESS;
        }

    }
}
