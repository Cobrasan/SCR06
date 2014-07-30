using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Alchemist
{
    public class Utility
    {
        /// <summary>
        /// ファイルのバックアップを作成する
        /// 同じディレクトリに日付をつけてバックアップします。
        /// </summary>
        /// <param name="filename">バックアップするファイルの名前</param>
        public static string CreateBackupFile(string filename)
        {
            // バックアップファイル名を取得
            string backupFileName = filename + "." + DateTime.Now.ToString("yyyyMMddHHmmss");

            // 同じディレクトリにファイルをコピーする
            try
            {
                File.Copy(filename, backupFileName);
            }
            catch (Exception)
            {
                /* 例外が発生した場合は、何もしない */
                return "";
            }

            return backupFileName;
        }

        /// <summary>
        /// バックアップファイルを削除します。
        /// バックアップファイルが存在しないか、
        /// １世代しか存在しない場合は、何も行いません。
        /// ２世代以上存在する場合は、作成日時が古いものを削除します。
        /// </summary>
        /// <param name="filename">元ファイルの名前をフルパスで指定します。</param>
        /// <returns></returns>
        public static void DeleteBackupFile(string filename)
        {
            string path = Path.GetDirectoryName(filename);
            string basefilename = Path.GetFileName(filename);

            // バックアップファイルの一覧を列挙する
            // http://msdn.microsoft.com/ja-jp/library/bb546159.aspx
            DirectoryInfo dir = new DirectoryInfo(path);
            try
            {
                IEnumerable<FileInfo> files = dir.GetFiles(basefilename + ".*");

                // 削除ファイルを取得する
                IEnumerable<FileInfo> deleteFiles = from file in files
                                                    orderby file.CreationTime descending
                                                    where file.Name != basefilename
                                                    select file;

                // 2世代以上古いファイルを取得
                deleteFiles = deleteFiles.SkipWhile(
                    (file, index) => index < 2
                );

                // 削除する
                foreach (var file in deleteFiles)
                {
                    file.Delete();
                }
            }
            catch
            {
                /* アクセスできない場合は、何もしない */
            }
        }
        /// <summary>
        /// セッションがリモートセッションの場合は、true
        /// コンソールセッションの場合は、falseを返します。
        /// </summary>
        /// <returns></returns>
        public static bool IsRemoteSession()
        {
            return System.Windows.Forms.SystemInformation.TerminalServerSession;
        }

        /// <summary>
        /// 範囲チェックを行う。
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool CheckRange(int val, int min, int max)
        {
            if (val < min || val > max)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
		/// <summary>
		/// 範囲チェックを行う。
		/// </summary>
		/// <param name="val"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool CheckRange(double val, double min, double max)
		{
			if (val < min || val > max)
			{
				return false;
			}
			else
			{
				return true;
			}
		}


        /// <summary>
        /// 文字列の範囲チェックを行う。
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool CheckStringRange(string  val, double  min, double max)
        {
            if (Convert.ToDouble(val) < min || Convert.ToDouble(val) > max)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		/// <summary>
		/// WorkDataをMemWriteに渡せる整数値の配列に変換します。
		/// </summary>
		/// <param name="Alloc">メモリ割当データの構造体を指定します。</param>
		/// <param name="WorkData">加工値、補正値、タイミング値等を指定します。</param>
		/// <param name="result">結果を返却します。</param>
		public static void ConvertWokdDataToInt(MemAllocStruct Alloc, double WorkData, out int[] result)
		{
			// 16bitの場合
			if (Alloc.DoubleWord == 0) {
				result = new int[1];

				// ValueFactor計算する
				result[0] = Convert.ToInt32(WorkData * Alloc.ValueFactor);
			}
			// 32bitの場合
			else {
				result = new int[2];

				// ValueFactor計算する
				int temp = Convert.ToInt32(WorkData * Alloc.ValueFactor);

				// byteデータに分割する
				byte[] bytes = BitConverter.GetBytes(temp);

				result[0] = BitConverter.ToInt16(new byte[] {bytes[0], bytes[1]}, 0);
				result[1] = BitConverter.ToInt16(new byte[] {bytes[2], bytes[3]}, 0);
			}
		}

		/// <summary>
		/// MemReadで読み込んだWorkDataをdoubleに変換します。
		/// </summary>
		/// <param name="Alloc"></param>
		/// <param name="WorkData"></param>
		/// <param name="result"></param>
		public static double ConvertWorkDataToDouble(MemAllocStruct Alloc, int[] WorkData)
		{
			// 16bitの場合
			if (Alloc.DoubleWord == 0) {
				return Convert.ToDouble((short)WorkData[0]) / Convert.ToDouble(Alloc.ValueFactor);
			}
			// 32bitの場合
			else {
				byte[] bytes1 = BitConverter.GetBytes(WorkData[0]);
				byte[] bytes2 = BitConverter.GetBytes(WorkData[1]);

				int ret = BitConverter.ToInt32(new byte[] {bytes1[0], bytes1[1], bytes2[0], bytes2[1]}, 0);

				return Convert.ToDouble(ret) / Convert.ToDouble(Alloc.ValueFactor);

			}
		}

		/// <summary>
		/// WorkDataにValueFactorを適用した時の値を文字列として取得します。
		/// </summary>
		/// <param name="WorkData">WorkDataを指定します。</param>
		/// <param name="ValueFactor">ValueFactorを指定します。</param>
		/// <returns></returns>
		public static string GetWorkDataString(double WorkData, int ValueFactor)
		{
			string format = "";
			switch (ValueFactor) {
			case 1 : 
				format = "{0:f0}";
				break;

			case 10 : 
				format = "{0:f1}";
				break;

			case 100 : 
				format = "{0:f2}";
				break;

			case 1000 : 
				format = "{0:f3}";
				break;
			}

			return string.Format(format, WorkData);
		}

		public static String GetReturnCodeString(int ret)
		{
			switch (ret) {
				case SystemConstants.ERR_NO_WORK_ID:
					return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG008);

				case SystemConstants.ERR_ADDRESS_RANGE:
					return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG009);

				case SystemConstants.ERR_TRANSFAR_RANGE :
					return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG010);

				case SystemConstants.ERR_WORK_RANGE :
					return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG011);

                case SystemConstants.ERR_NO_BANK_DATA:
                case SystemConstants.ERR_BANK_PARTS_BREKE:
                case SystemConstants.ERR_BANK_PARTS_RANGE :
                    return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG012);

                case SystemConstants.ERR_MEMALLOC_FILE_ERROR :
                    return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG014);

                case SystemConstants.ERR_BANK_FILE_ERROR :
                    return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG015);

                case SystemConstants.ERR_CORRECT_FILE_ERROR :
                    return GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG016);
			}

			return "";
		}

		/// <summary>
		/// 言語データのメッセージから、現在の言語のMessageType, MessageIDに対応する
		/// メッセージを返します。
		/// 
		/// MessageTypeの意味は次の通りです。
		/// 
		/// ERROR_MSG : エラーメッセージ
		/// SYSTEM_MSG : システムメッセージ
		/// CORRECT_MSG : 補正値メッセージ
		/// TIMING_MSG : タイミングメッセージ
		/// 該当メッセージIDが存在しない場合、ID + IDの数字を返します。
		/// </summary>
		/// <param name="MessageType"></param>
		/// <param name="MessageID"></param>
		/// <returns></returns>
		public static string GetMessageString(int MessageType, int MessageID)
		{
			string key = "";
			string value = "";
            ResourceManager man = Alchemist.Properties.ScreenMsg.ResourceManager;

			switch (MessageType) {
			case SystemConstants.ERROR_MSG :
				key = "ERR_" + string.Format("{0:X4}", MessageID);
                man = Alchemist.Properties.ScreenMsg.ResourceManager;
				break;

			case SystemConstants.SYSTEM_MSG :
				key = "SYS_" + string.Format("{0:X4}", MessageID);
                man = Alchemist.Properties.ScreenMsg.ResourceManager;
				break;

			case SystemConstants.WORK_MSG : 
				key = "WORK_" + string.Format("{0:X4}", MessageID);
                man = Alchemist.Properties.WorkDataMsg.ResourceManager;
				break;


			case SystemConstants.CORRECT_MSG :
				key = "CORR_" + string.Format("{0:X4}", MessageID);
                man = Alchemist.Properties.CorrectDataMsg.ResourceManager;
				break;


			case SystemConstants.TIMMING_MSG :
				key = "TIMM_" + string.Format("{0:X4}", MessageID);
                man = Alchemist.Properties.TimingDataMsg.ResourceManager;
				break;
			}

			// リソースを取得
			value =	man.GetString(key);

			// 存在しない場合は、key値を返す
			if (null == value) {
				value = key;
			}

			return value;
		}

        /// <summary>
        /// エラーメッセージを取得する
        /// </summary>
        /// <param name="MessageID"></param>
        /// <returns></returns>
        public static string GetErrorMessage(int MessageID)
        {
            string message = GetMessageString(SystemConstants.ERROR_MSG, MessageID);

            // 先頭2文字を削除する
            if (!string.IsNullOrEmpty(message) && message.Length >= 2)
            {
                message = message.Remove(0, 2);
            }

            return message;
        }

        // 情報ダイアログを出す
        public static void ShowInfoMsg(int MessageID, params object[] args)
        {
            string caption = GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG001);
            string format = GetMessageString(SystemConstants.SYSTEM_MSG, MessageID);

            string text = string.Format(format, args);
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        // 確認ダイアログを出す
        public static bool ShowConfirmMsg(int MessageID, params object[] args)
        {
            string caption = GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG002);
            string format = GetMessageString(SystemConstants.SYSTEM_MSG, MessageID);

            string text = string.Format(format, args);
            if (MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // エラーダイアログを出す
        public static void ShowErrorMsg(string text)
        {
            string caption = GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG003);
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // エラーダイアログを出す
        public static void ShowErrorMsg(int MessageID, params object[] args)
        {
            string caption = GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG003);
            string format = GetMessageString(SystemConstants.SYSTEM_MSG, MessageID);

            string text = string.Format(format, args);

            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // エラーダイアログを出す
        public static void ShowErrorCode(int ret)
        {
            // 正常値の場合は、エラーを出さない
            if (ret == SystemConstants.DCPF_SUCCESS || ret == SystemConstants.MCPF_SUCCESS)
            {
                return;
            }

            string caption = "";
            string text = "";

            // バンクデータに欠損や範囲外の値があった場合は、情報としてメッセージを表示
            if (ret == SystemConstants.ERR_BANK_PARTS_BREKE || 
                ret == SystemConstants.ERR_BANK_PARTS_RANGE ||
                ret == SystemConstants.ERR_NO_BANK_DATA)
            {
                caption = GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG001);
                text = GetReturnCodeString(ret);
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // エラーの場合
            else
            {
                caption = GetMessageString(SystemConstants.SYSTEM_MSG, SystemConstants.SYSTEM_MSG003);
                text = GetReturnCodeString(ret);
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ハッシュ値を計算する
        public static string ComputeHash(string input)
        {
            // テキストをUTF-8エンコードでバイト配列化
            byte[] byteValue = Encoding.UTF8.GetBytes(input);

            // SHA256のハッシュ値を取得する
            SHA1 crypto = new SHA1CryptoServiceProvider();
            byte[] hashValue = crypto.ComputeHash(byteValue);

            // バイト配列をUTF8エンコードで文字列化
            StringBuilder hashedText = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                hashedText.AppendFormat("{0:X2}", hashValue[i]);
            }

            return hashedText.ToString();
        }

        public static string AssemblyTitle
        {
            get
            {
                // このアセンブリ上のタイトル属性をすべて取得します
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // 少なくとも 1 つのタイトル属性がある場合
                if (attributes.Length > 0)
                {
                    // 最初の項目を選択します
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // 空の文字列の場合、その項目を返します
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // タイトル属性がないか、またはタイトル属性が空の文字列の場合、.exe 名を返します
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                // このアセンブリ上の説明属性をすべて取得します
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // 説明属性がない場合、空の文字列を返します
                if (attributes.Length == 0)
                    return "";
                // 説明属性がある場合、その値を返します
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // このアセンブリ上の製品属性をすべて取得します
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // 製品属性がない場合、空の文字列を返します
                if (attributes.Length == 0)
                    return "";
                // 製品属性がある場合、その値を返します
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // このアセンブリ上の著作権属性をすべて取得します
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // 著作権属性がない場合、空の文字列を返します
                if (attributes.Length == 0)
                    return "";
                // 著作権属性がある場合、その値を返します
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // このアセンブリ上の会社属性をすべて取得します
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // 会社属性がない場合、空の文字列を返します
                if (attributes.Length == 0)
                    return "";
                // 会社属性がある場合、その値を返します
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        public static string AssemblyMachineVersion
        {
            get
            {
                int[] revision = new int[5];
                string revisionStr = "";

                // 機械バージョンを取得
                Program.DataController.GetMachineRevision(ref revision);

                for (int i = 0; i < SystemConstants.MACHINE_REVISION_COUNT; i++)
                {
                    // 文字列機械バージョンの中身が空だった場合
                    if (revisionStr == "")
                    {
                        // 文字列の機械バージョンを代入
                        revisionStr = revisionStr + revision[i];
                    }
                    // 文字列機械バージョンの中身が空以外だった場合
                    else
                    {
                        // ピリオド+文字列の機械バージョンを代入
                        revisionStr = revisionStr + "." + revision[i];
                    }
                }

                return revisionStr;
            }
        }
    }
}
