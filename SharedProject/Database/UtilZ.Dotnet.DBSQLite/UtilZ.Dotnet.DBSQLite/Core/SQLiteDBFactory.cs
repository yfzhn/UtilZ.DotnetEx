using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Factory;

namespace UtilZ.Dotnet.DBSQLite.Core
{
    /// <summary>
    /// SQLite数据访问对象创建工厂
    /// </summary>
    public class SQLiteDBFactory : DBFactoryAbs
    {
        private readonly SQLiteDBInteraction _dbInteraction;
        private readonly string _databaseTypeName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLiteDBFactory() :
            base()
        {
            this._dbInteraction = new SQLiteDBInteraction();
            //this._databaseTypeName = typeof(System.Data.SQLite.SQLiteConnection).Assembly.FullName;
            this._databaseTypeName = "SQLite";
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
            return new SQLiteDBAccess(this._dbInteraction, config, this._databaseTypeName);
        }

        /// <summary>
        /// 附加EF配置
        /// </summary>
        public override void AttatchEFConfig()
        {
            string providerInvariantName = typeof(SQLiteFactory).Namespace;
            DbProviderServices service = SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)) as DbProviderServices;
            EFDbConfiguration.AddProviderServices(providerInvariantName, service);
            EFDbConfiguration.AddProviderFactory(providerInvariantName, SQLiteFactory.Instance);

            providerInvariantName = typeof(SQLiteProviderFactory).Namespace;
            EFDbConfiguration.AddProviderFactory(providerInvariantName, SQLiteProviderFactory.Instance);
        }
    }
}
