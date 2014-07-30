using System.IO;
using System.Text;
using System.Windows.Forms;
using System;

namespace Alchemist
{
    public class CsvFileOutDefine
    {
        //分類
        public const string CATE_PRM = "Parameter";
        public const string CATE_TIM = "Timming";
 
        //エラー
        public const int FILEOUT_NO_ERROR = 0x0;
        public const int FILEOUT_OPEN_ERROR = 0x10;
        public const int FILROUT_ADD_DATA_ERROR = 0x11;
    }
 
    public class CsvFileOutException : Exception
    {
        public int ErrorCode
        {
            get;
            set;
        }

        public CsvFileOutException(int ErrorCode)
        {
            this.ErrorCode = ErrorCode;
        }

        public static void ThrowException(int ErrorCode)
        {
            throw new CsvFileOutException(ErrorCode);
        }
    }

    class CsvFileOut
    {
        // <summary>
        // CSVファイル作成
        // </summary>
        // <param name ="FileName">ファイル名</param>
        // <returns>リターン値</returns>
        public int CsvFileOpen(string FileName)
        {
            StreamWriter csvFile;
            DateTime dt = DateTime.Now;

            try
            {
                csvFile = new StreamWriter(FileName, false, Encoding.Default);
                csvFile.WriteLine("Machine Parameter List");
                csvFile.WriteLine("Create Date," + dt.ToString());
                csvFile.Close();
            }
            catch
            {
                CsvFileOutException.ThrowException(CsvFileOutDefine.FILEOUT_OPEN_ERROR);
                return CsvFileOutDefine.FILEOUT_OPEN_ERROR;
            }

            return CsvFileOutDefine.FILEOUT_NO_ERROR;
        }

        // <summary>
        // CSVファイルにDataGridViewの内容を追加
        // </summary>
        // <param name ="FileName">ファイル名</param>
        // <param name ="Category">パラメター／タイミング</param>
        // <param name ="Title">パラメタータイトル</param>
        // <returns>リターン値</returns>
        public int CsvFileDataGridViewAdd(string FileName, string Category, string Title, DataGridView Data)
        {
            StreamWriter csvFile;
            string csvLine = "";

            try
            {
                csvFile = new StreamWriter(FileName, true, Encoding.Default);
                csvFile.WriteLine("");
                csvFile.WriteLine(Category + " ," + Title);
                csvFile.WriteLine("");

                for (int i = 1; i <= Data.ColumnCount - 1; i++)
                {
                    if (i != 1) 
                        csvLine += ",";
                    csvLine += Data.Columns[i].HeaderText;                    
                }
                
                csvFile.WriteLine(csvLine);
                csvLine = "";

                for (int i = 0; i <= Data.RowCount - 1; i++)
                {
                    for (int j = 1; j <= Data.ColumnCount - 1; j++)
                    {
                        if (j != 1) 
                            csvLine += ",";
                        csvLine += Data.Rows[i].Cells[j].Value.ToString();
                    }

                    csvFile.WriteLine(csvLine);
                    csvLine = "";
                }

                csvFile.Close();
            }
            catch
            {
                CsvFileOutException.ThrowException(CsvFileOutDefine.FILROUT_ADD_DATA_ERROR);
                return CsvFileOutDefine.FILROUT_ADD_DATA_ERROR;
            }            

            return CsvFileOutDefine.FILEOUT_NO_ERROR;
        }
    }
}
