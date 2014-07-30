using System.Xml.Linq;
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Alchemist
{
    public class XMLValidateException : Exception
    {
        public XMLValidateException() : base() {}
        public XMLValidateException(string message) : base(message) {}
    }

    public class XMLAccessor
    {
        // DOM
        protected XDocument doc = null;

        // DOMへの参照(読み込みのみ許可)
        public XDocument document
        {
            get
            {
                return doc;
            }
        }

        public virtual void NewDocument()
        {
            doc = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSchema()
        {
            return null;
        }

        /// <summary>
        /// documentが検証済XMLかチェックする
        /// </summary>
        /// <returns></returns>
        protected  void validate(string schema) {
            // schemaが設定されていなければ、チェックしない。
            if (schema == null)
            {
                return;
            }

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(schema)));

            bool errors = false;
            string message = "";
            document.Validate(schemas, (o, e) =>
            {
                errors = true;
                message += e.Message;
            }, true);

            // エラーがあれば、例外を発生させる
            if (errors)
            {
                throw new XMLValidateException(message);
            }
        }

        /// <summary>
        /// XMLファイルを読み込みます。
        /// 読み込み時はバックアップを行います。
        /// </summary>
        /// <param name="filename">ファイル名を指定します。</param>
        public void LoadXmlFile(string filename)
        {
            doc = XDocument.Load(filename);

            // スキーマチェック
            validate(GetSchema());
        }


        /// <summary>
        /// documentに格納されているXMLデータをファイルに書き出します。
        /// 書き出しの前にバックアップが行われます。
        /// </summary>
        /// <param name="filename">ファイル名を指定します。</param>
        public bool SaveXmlFile(string filename)
        {
            // スキーマチェック
            validate(GetSchema());

            string backupFileName = Utility.CreateBackupFile(filename);
            doc.Save(filename);

            return true;
        }
    }
}
