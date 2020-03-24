using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DBPostgreSQL.Core
{
    /// <summary>
    /// PostgreSQLDBAccess
    /// </summary>
    internal partial class PostgreSQLDBAccess : DBAccessAbs
    {
        /// <summary>
        /// sql语句最大长度
        /// </summary>
        private const long SQL_MAX_LENGTH = 1048576;
        public PostgreSQLDBAccess(IDBInteraction dbInteraction, DatabaseConfig config, string databaseTypeName)
            : base(dbInteraction, config, databaseTypeName, SQL_MAX_LENGTH)
        {

        }

        /// <summary>
        /// 创建数据库对象
        /// </summary>
        /// <returns></returns>
        protected override IDatabase CreateDatabase()
        {
            return new PostgreSQLDatabase(this);
        }
    }
}
