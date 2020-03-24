using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DBSQLite.Core
{
    internal partial class SQLiteDBAccess : DBAccessAbs
    {
        /// <summary>
        /// sql语句最大长度
        /// The maximum number of bytes in the text of an SQL statement is limited to SQLITE_MAX_SQL_LENGTH which defaults to 1000000. You can red
        /// </summary>
        private const long SQL_MAX_LENGTH = 1000000;
        public SQLiteDBAccess(IDBInteraction dbInteraction, DatabaseConfig config, string databaseTypeName)
            : base(dbInteraction, config, databaseTypeName, SQL_MAX_LENGTH)
        {

        }

        /// <summary>
        /// 创建数据库对象
        /// </summary>
        /// <returns></returns>
        protected override IDatabase CreateDatabase()
        {
            return new SQLiteDatabase(this);
        }
    }
}
