using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;
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

namespace UtilZ.Dotnet.DBOracle.Core
{
    /// <summary>
    /// Oracle数据访问对象创建工厂
    /// </summary>
    public class OracleDBFactory : DBFactoryAbs
    {
        private readonly OracleDBInteraction _dbInteraction;
        private readonly string _databaseTypeName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OracleDBFactory() :
            base()
        {
            this._dbInteraction = new OracleDBInteraction();
            //this._databaseTypeName = typeof(OracleConnection).Assembly.FullName;
            this._databaseTypeName = "Oracle";
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
            return new OracleDBAccess(this._dbInteraction, config, this._databaseTypeName);
        }

        /// <summary>
        /// 附加EF配置
        /// </summary>
        public override void AttatchEFConfig()
        {
            string providerInvariantName = typeof(OracleConnection).Namespace;
            EFDbConfiguration.AddProviderServices(providerInvariantName, EFOracleProviderServices.Instance);
            EFDbConfiguration.AddProviderFactory(providerInvariantName, OracleClientFactory.Instance);
        }
    }
}
