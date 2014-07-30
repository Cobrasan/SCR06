using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Alchemist
{
    public class MemoryAllocationDataException : Exception
    {
        public MemoryAllocationDataException()
            : base()
        {

        }
    }

    public class MemoryAllocationData
    {
        private XMLAccessor xmlAccessor = new MemallocXmlAccessor();

        /// <summary>
        /// memalloc.xmlファイルからメモリ割当データを読み込みクラス内に値を保存します。
        ///ファイルにアクセスする際は、世代管理を行います。
        ///ファイルから読み込みができない場合、または、ファイルが見つからない場合は、例外が発生します。
        /// </summary>
        /// 
        public void Load()
        {
            string xmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\JAM\\Alchemist\\memalloc.xml";

            try
            {
                // XMLファイルを開く
                xmlAccessor.LoadXmlFile(xmlFileName);

            }
            catch 
            {
                throw;
            }
        }

        /// <summary>
        /// 加工値のアドレスが重複しているもののリストを取得する
        /// </summary>
        public Dictionary<int, List<int>> GetAddressDupulicatedMap(int type)
        {
            int[] groupList = null;
            int[] workidList = null;
            MemAllocStruct entry;
            Dictionary<int, List<int>> map = new Dictionary<int, List<int>>();

            // ワークIDの一覧を取得する
            GetMemoryDataGroupList(type, SystemConstants.WORKGROUP_ROOT, ref groupList);

            // アドレスとWORKIDの対応表を作る。
            foreach (var group in groupList)
            {
                // Group内のworkid一覧を取得する
                GetMemoryDataGroupList(type, group, ref workidList);

                foreach (var workid in workidList)
                {
                    entry = GetEntry(type, workid);

                    // アドレスが存在しない
                    if (!map.ContainsKey(entry.Address))
                    {
                        List<int> list = new List<int>();
                        map.Add(entry.Address, list);
                        list.Add(workid);
                    }
                    // アドレスが既に存在する
                    else
                    {
                        map[entry.Address].Add(workid);
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 機種名を取得します。
        /// </summary>
        /// <returns></returns>
        public string GetMachineName()
        {
            string machineName = null;

            // XMLファイルを読む
            var nodes = xmlAccessor.document.Elements("memalloc").Elements("machine");

            var machine = nodes.First();


            machineName = machine.Attribute("id").Value;

            return machineName;
        }

        /// <summary>
        /// ロードされたXMLデータから該当する、
        ///WorkID_Type, GroupCode, Workidを持つデータを取得し、MemAllocStruct型の値を返します。
        ///指定されたエントリが存在しない場合は、例外が発生します。
        ///ファイルがロードされていない状態で、この関数を呼び出した場合は、例外が発生します。
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="GroupCode"></param>
        /// <param name="WorkID"></param>
        /// <returns></returns>

        public MemAllocStruct GetEntry(int Type, int WorkID)
        {
            MemAllocStruct memallocstruct = new MemAllocStruct();

            int GroupCode = 0;
            int ID = 0;

            WorkIDSampler(WorkID, ref  GroupCode, ref ID);

            // XMLファイルを読む
            var nodes = from n in xmlAccessor.document.Descendants("workid_type")
                        where n.Attribute("type").Value == Type.ToString()
                        select n;

            nodes = from n in nodes.Descendants("group")
                    where n.Attribute("code").Value == GroupCode.ToString()
                    select n;


            nodes = from n in nodes.Descendants("workid")
                    where n.Attribute("id").Value == "0x" + WorkID.ToString("X4")
                    orderby n.Attribute("id")
                    select n;


            // 見つかったノードが１でなければエラーを返す。
            if (nodes.Count() != 1)
            {
                //例外発生
                throw new MemoryAllocationDataException();
            }

            var work = nodes.First();

            memallocstruct.WorkIdType = Type;
            memallocstruct.GroupCode = GroupCode;
            memallocstruct.WorkID = ID;
            memallocstruct.SortNo = Int32.Parse(work.Attribute("sortno").Value);
            memallocstruct.Address = Convert.ToInt32(work.Attribute("address").Value, 16);
            memallocstruct.DoubleWord = Int32.Parse(work.Attribute("double_word").Value);
            memallocstruct.ValueFactor = Int32.Parse(work.Attribute("value_factor").Value);
            memallocstruct.BankStore = System.Convert.ToBoolean(Int32.Parse(work.Attribute("bank_store").Value));
            memallocstruct.MinLimit = double.Parse(work.Attribute("min_limit").Value);
            memallocstruct.MaxLimit = double.Parse(work.Attribute("max_limit").Value);
            memallocstruct.DefaultValue = double.Parse(work.Attribute("default_value").Value);
			memallocstruct.Unit = work.Attribute("unit").Value;
            memallocstruct.address_special = Int32.Parse(work.Attribute("address_special").Value);
            memallocstruct.Comment = work.Attribute("comment").Value;

            return memallocstruct;
        }

        /// <summary>
        /// ロードされたXMLデータから加工値、補正値、タイミング、WorkdIDグループのWorkID一覧をIDに入れて返します。
        ///また、WorkdIDグループがWORKGROUP_ROOTの場合、GroupIndexの一覧をIDに入れて返します。
        ///指定されたGroupType, GroupIndexが存在しない場合は、例外が発生します。
        ///ファイルがロードされていない状態で、この関数を呼び出した場合は、例外が発生します。
        /// </summary>
        /// <param name="GroupType"></param>
        /// <param name="GroupIndex"></param>
        /// <param name="ID"></param>

        public void GetMemoryDataGroupList(int GroupType, int GroupIndex, ref int[] ID)
        {

            int count = 0;


            // XMLファイルを読む
            var nodes = from n in xmlAccessor.document.Elements("memalloc").Elements("workid_type")
                        where n.Attribute("type").Value == GroupType.ToString()
                        select n;

            // 見つかったノードが0の場合エラーを返す。
            if (nodes.Count() == 0)
            {
                //例外発生
                throw new MemoryAllocationDataException();
            }

            //GroupIndexがWORKGROUP_ROOTでない場合は、指定されたWorkIDグループのWorkIDの一覧をIDに入れるため
            //GroupIndexの絞り込みまで行う
            if (GroupIndex != SystemConstants.WORKGROUP_ROOT)
            {

                nodes = from n in nodes.Elements("group")
                        where n.Attribute("code").Value == GroupIndex.ToString()
                        select n;

                //昇順に並び変え
                nodes = from n in nodes.Elements("workid")
                        orderby n.Attribute("id").Value
                        select n;


                // 見つかったノードが0の場合エラーを返す。
                if (nodes.Count() == 0)
                {
                    //例外発生
                    throw new MemoryAllocationDataException();
                }
                //ノードの数を数える
                count = nodes.Count();
                //配列の割り当て
                ID = new int[count];

                int i = 0;
                //WorkIDもしくはGroupIndex一覧をIDに格納する。
                foreach (var work in nodes)
                {
					ID[i] = Int32.Parse(work.Attribute("id").Value.Remove(0,2), System.Globalization.NumberStyles.AllowHexSpecifier);

                    i++;
                }
            }
            //GroupIndexがWORKGROUP_ROOTの場合
            else
            {

                nodes = from n in nodes.Elements("group")
                        orderby n.Attribute("code").Value
                        select n;

                //ノードの数を数える
                count = nodes.Count();

                //配列の割り当て
                ID = new int[count];

                int i = 0;

                //WorkIDもしくはGroupIndex一覧をIDに格納する。
                foreach (var work in nodes)
                {
                    ID[i] = Int32.Parse(work.Attribute("code").Value);

                    i++;
                }
            }
        }
        /// <summary>
        /// 引数のWorkIDからgroup codeとworkidを抽出する
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        private void WorkIDSampler(int WorkID, ref int group, ref int id)
        {
            group = (WorkID & 0xFF00) >> 8;
            id = WorkID & 0x00FF;
        }
    }
}
