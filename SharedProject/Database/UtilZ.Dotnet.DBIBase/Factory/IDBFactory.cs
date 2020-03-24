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
    /// 数据库工厂接口
    /// </summary>
    public interface IDBFactory
    {
        /// <summary>
        /// 获取IDBInteraction对象
        /// </summary>
        /// <returns></returns>
        IDBInteraction GetDBInteraction();

        /// <summary>
        /// 创建数据库访问实例
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>数据库访问实例</returns>
        IDBAccess CreateDBAccess(DatabaseConfig config);

        /// <summary>
        /// 附加EF配置
        /// </summary>
        void AttatchEFConfig();       
    }
}
