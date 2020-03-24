using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Factory;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DBMySql.Core
{
    /// <summary>
    /// MySql数据访问对象创建工厂
    /// </summary>
    public class MySqlDBFactory : DBFactoryAbs
    {
        private readonly MySqlDBInteraction _dbInteraction;
        private readonly string _databaseTypeName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MySqlDBFactory()
            : base()
        {
            this._dbInteraction = new MySqlDBInteraction();
            //this._databaseTypeName = typeof(MySqlConnection).Assembly.FullName;
            this._databaseTypeName = "MySql";
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
            return new MySqlDBAccess(this._dbInteraction, config, this._databaseTypeName);
        }

        /// <summary>
        /// 附加EF配置
        /// </summary>
        public override void AttatchEFConfig()
        {
            string providerInvariantName = typeof(MySqlConnection).Namespace;
            EFDbConfiguration.AddProviderServices(providerInvariantName, new MySqlProviderServices());
            EFDbConfiguration.AddProviderFactory(providerInvariantName, MySqlClientFactory.Instance);
        }
    }
}
