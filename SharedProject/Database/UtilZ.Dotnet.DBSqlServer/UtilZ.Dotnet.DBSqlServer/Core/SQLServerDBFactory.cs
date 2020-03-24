using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Factory;

namespace UtilZ.Dotnet.DBSqlServer.Core
{
    public class SQLServerDBFactory : DBFactoryAbs
    {
        private readonly SQLServerDBInteraction _dbInteraction;
        private readonly string _databaseTypeName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLServerDBFactory() :
            base()
        {
            this._dbInteraction = new SQLServerDBInteraction();
            //this._databaseTypeName = typeof(SqlConnection).Assembly.FullName;
            this._databaseTypeName = "SQLServer";
        }

        /// <summary>
        /// 获取IDBInteraction对象
        /// </summary>
        /// <returns></returns>
        public override IDBInteraction GetDBInteraction()
        {
            return this._dbInteraction;
        }

        /// <summary>
        /// 创建数据库访问实例
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>数据库访问实例</returns>
        public override IDBAccess CreateDBAccess(DatabaseConfig config)
        {
            return new SQLServerDBAccess(this._dbInteraction, config, this._databaseTypeName);
        }

        /// <summary>
        /// 附加EF配置
        /// </summary>
        public override void AttatchEFConfig()
        {
            string providerInvariantName = typeof(SqlConnection).Namespace;
            EFDbConfiguration.AddProviderServices(providerInvariantName, SqlProviderServices.Instance);
            EFDbConfiguration.AddProviderFactory(providerInvariantName, SqlClientFactory.Instance);
        }
    }
}
