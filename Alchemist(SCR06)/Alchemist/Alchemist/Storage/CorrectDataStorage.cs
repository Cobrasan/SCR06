using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Alchemist
{
    public struct CorrectDataStruct
    {
        public int Type;
        public int ID;
        public string value;
    };
    public class CorrectDataException : Exception
    {
        public CorrectDataException()
            : base()
        {

        }
    }
     public class CorrectDataStorage
    {
        private  XMLAccessor xmlAccessor = new CorrectDataXMLAccessor();

        public void Load()
        {
            //correctdata.xmlのパスを取得
            string xmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\JAM\\Alchemist\\correctdata.xml";

            try
            {
                // XMLファイルを開く
                xmlAccessor.LoadXmlFile(xmlFileName);

            }
            catch (FileNotFoundException)
            {
                // ファイルがなければ作る
                xmlAccessor.NewDocument();
                xmlAccessor.SaveXmlFile(xmlFileName);
            }
        }

        /// <summary>
        /// CorrectData.xmlから補正値、タイミング値を取得します。
        ///ファイルが見つからない場合、
        ///ファイルに書き込みができない場合、例外を発生させます。
        /// </summary>
        /// <param name="CorrectData">読出した補正値・タイミング値を格納する構造体</param>
        /// <returns></returns>
        public  void GetCorrectData(ref CorrectDataStruct[] CorrectData)
        {
            // XMLファイルを読む
            var nodes = from n in xmlAccessor.document.Elements("correctdata")
                        select n;

            // エントリの数を数える
            var bank = nodes.First();
            int count = bank.Elements("entry").Count();


            // 構造体を割り当てる
            CorrectData = new CorrectDataStruct[count];


            // 構造体に読み込む
            int i = 0;
            foreach (var entry in bank.Elements())
            {
                CorrectData[i].Type = Int32.Parse(entry.Attribute("type").Value);
				CorrectData[i].ID = Int32.Parse(entry.Attribute("workid").Value.Remove(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                CorrectData[i].value = entry.Attribute("value").Value;
                i++;
            }
        }

        /// <summary>
        /// 補正値、タイミング値をCorrectData.xmlに書き込みます。
        ///ファイルが見つからない場合、
        ///ファイルに書き込みができない場合、例外を発生させます。
        /// </summary>
        /// <param name="CorrectData">書き込む補正値・タイミング値を格納する構造体</param>
        /// <returns></returns>
        public  void WriteCorrectData(CorrectDataStruct[] CorrectData)
        {
            string xmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\JAM\\Alchemist\\correctdata.xml";

            // XMLファイルを読む
            var nodes = from n in xmlAccessor.document.Elements("correctdata")
                        select n;

            XElement entry;
            entry = nodes.First();
            entry.RemoveNodes();

            // エントリを作る
            for (int i = 0; i < CorrectData.Length; i++)
            {
                var elem = new XElement("entry");

                elem.SetAttributeValue("type", CorrectData[i].Type);
                elem.SetAttributeValue("workid", string.Format("0x{0:X4}", CorrectData[i].ID));
                elem.SetAttributeValue("value", CorrectData[i].value);

                entry.Add(elem);
            }
            // XMLファイルにセーブする
            try
            {
                // XMLファイルを開く
                xmlAccessor.SaveXmlFile(xmlFileName);

            }
            finally
            {
                Utility.DeleteBackupFile(xmlFileName);
            }
        }
    }
}
