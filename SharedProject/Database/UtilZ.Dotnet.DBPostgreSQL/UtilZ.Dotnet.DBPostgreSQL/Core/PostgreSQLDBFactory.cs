using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Factory;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DBPostgreSQL.Core
{
    /// <summary>
    /// PostgreSQ数据访问对象创建工厂
    /// </summary>
    public class PostgreSQLDBFactory : DBFactoryAbs
    {
        private readonly PostgreSQLDBInteraction _dbInteraction;
        private readonly string _databaseTypeName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PostgreSQLDBFactory()
            : base()
        {
            this._dbInteraction = new PostgreSQLDBInteraction();
            //this._databaseTypeName = typeof(NpgsqlConnection).Assembly.FullName;
            this._databaseTypeName = "PostgreSQL";
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
            return new PostgreSQLDBAccess(this._dbInteraction, config, this._databaseTypeName);
        }

        /// <summary>
        /// 附加EF配置
        /// </summary>
        public override void AttatchEFConfig()
        {
            string providerInvariantName = typeof(NpgsqlConnection).Namespace;
            EFDbConfiguration.AddProviderServices(providerInvariantName, NpgsqlServices.Instance);
            EFDbConfiguration.AddProviderFactory(providerInvariantName, NpgsqlFactory.Instance);
        }
    }
}
