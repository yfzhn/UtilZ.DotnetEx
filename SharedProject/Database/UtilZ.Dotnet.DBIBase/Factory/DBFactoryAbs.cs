using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DBIBase.Factory
{
    /// <summary>
    /// 数据访问对象创建工厂基类
    /// </summary>
    public abstract class DBFactoryAbs : IDBFactory
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBFactoryAbs()
        {

        }

        /// <summary>
        /// 获取IDBInteraction对象
        /// </summary>
        /// <returns></returns>
        public abstract IDBInteraction GetDBInteraction();

        /// <summary>
        /// 创建数据库访问实例
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>数据库访问实例</returns>
        public abstract IDBAccess CreateDBAccess(DatabaseConfig config);

        /// <summary>
        /// 附加EF配置
        /// </summary>
        public abstract void AttatchEFConfig();       
    }
}
