using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.DBIBase.EF
{
    /// <summary>
    /// EF配置类
    /// </summary>
    public class EFDbConfiguration : DbConfiguration
    {
        /// <summary>
        /// [key:providerInvariantName;value:DbProviderServices]
        /// </summary>
        private readonly static ConcurrentDictionary<string, DbProviderServices> _providerServicesDic = new ConcurrentDictionary<string, DbProviderServices>();

        /// <summary>
        /// [key:providerInvariantName;value:DbProviderFactory]
        /// </summary>
        private readonly static ConcurrentDictionary<string, DbProviderFactory> _providerFactoryDic = new ConcurrentDictionary<string, DbProviderFactory>();

        /// <summary>
        /// 添加Call this method from the constructor of a class derived from System.Data.Entity.DbConfiguration to register an ADO.NET provider.
        /// </summary>
        /// <param name="providerInvariantName">The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this provider will be used.</param>
        /// <param name="providerFactory">The provider instance</param>
        public static void AddProviderFactory(string providerInvariantName, DbProviderFactory providerFactory)
        {
            _providerFactoryDic[providerInvariantName] = providerFactory;
        }

        /// <summary>
        /// Call this method from the constructor of a class derived from System.Data.Entity.DbConfiguration to register an Entity Framework provider.
        /// </summary>
        /// <param name="providerInvariantName">The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this provider will be used</param>
        /// <param name="provider">The provider instance.</param>
        public static void AddProviderServices(string providerInvariantName, DbProviderServices provider)
        {
            _providerServicesDic[providerInvariantName] = provider;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EFDbConfiguration()
        {
            base.SetDefaultConnectionFactory(new EFConnectionFactory());
            foreach (var kv in _providerFactoryDic)
            {
                base.SetProviderFactory(kv.Key, kv.Value);
            }

            foreach (var kv in _providerServicesDic)
            {
                base.SetProviderServices(kv.Key, kv.Value);
            }

            //base.AddInterceptor(new ZDbInterceptor());//添加自定义数据读取器,以替换默认的读取器
        }
    }
}
