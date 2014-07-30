using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemist
{
    public struct SQLServerConnectionStruct
    {
        public string MachineName;
        public string DataBaseName;
        public string UserId;
        public string Password;
    };

    class SQLServerController
    {

        protected string stConnectionString = string.Empty;

        public SQLServerController(SQLServerConnectionStruct cs)
        {
            setConnctionString(cs);
        }

        public void setConnctionString(SQLServerConnectionStruct cs)
        {
            // 接続文字列の作成
            stConnectionString += "Data Source         = " + cs.MachineName + ";";
            stConnectionString += "Initial Catalog     = " + cs.DataBaseName + ";";
            stConnectionString += "Integrated Security = False;";
            stConnectionString += "uid = " + cs.UserId + ";";
            stConnectionString += "pwd = " + cs.Password + ";";
        }
    }
}
