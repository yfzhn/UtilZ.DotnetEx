using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DBSqlServer.Core
{
    internal partial class SQLServerDBAccess : DBAccessAbs
    {
        /// <summary>
        /// sql语句最大长度
        /// 65,536 * 网络数据包大小,网络数据包大小指的是用于在应用程序和关系 数据库引擎 之间进行通信的表格格式数据流(TDS) 数据包的大小。 默认的数据包大小为 4 KB，由“网络数据包大小”配置选项控制。
        /// </summary>
        private const long SQL_MAX_LENGTH = 268435456;
        public SQLServerDBAccess(IDBInteraction dbInteraction, DatabaseConfig config, string databaseTypeName)
            : base(dbInteraction, config, databaseTypeName, SQL_MAX_LENGTH)
        {

        }

        /// <summary>
        /// 创建数据库对象
        /// </summary>
        /// <returns></returns>
        protected override IDatabase CreateDatabase()
        {
            return new SqlServerDatabase(this);
        }
    }
}
