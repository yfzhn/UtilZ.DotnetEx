using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Factory;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase.Connection
{
    internal class DbConnectionPoolManager
    {
        /// <summary>
        /// 连接池实例字典集合
        /// </summary>
        private readonly static ConcurrentDictionary<int, DbConnectionPool> _dbConnectionPoolDic = new ConcurrentDictionary<int, DbConnectionPool>();

        /// <summary>
        /// 连接池实例字典集合线程锁
        /// </summary>
        private readonly static object _dbConnectionPoolDicMonitor = new object();

        /// <summary>
        /// 连接池状态[true:正常;false:已释放]
        /// </summary>
        private static bool _status = true;

        /// <summary>
        /// 添加数据库连接池
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="dbInteraction">数据库交互实例</param>
        internal static void AddDbConnectionPool(DatabaseConfig config, IDBInteraction dbInteraction)
        {
            int dbid = config.DBID;
            lock (_dbConnectionPoolDicMonitor)
            {
                if (!_status)
                {
                    throw new ApplicationException("连接池已释放");
                }

                DbConnectionPool dbConnectionPool;
                if (_dbConnectionPoolDic.ContainsKey(dbid))
                {
                    //连接池已经创建,不再重复创建,直接返回
                    return;
                }

                dbConnectionPool = new DbConnectionPool(config, dbInteraction);
                _dbConnectionPoolDic.TryAdd(dbid, dbConnectionPool);
            }
        }

        /// <summary>
        /// 从数据库连接池中移除指定编号的数据库连接池实例
        /// </summary>
        /// <param name="dbid">数据库编号ID</param>
        internal static void RemoveDbConnectionPool(int dbid)
        {
            lock (_dbConnectionPoolDicMonitor)
            {
                DbConnectionPool dbConnectionPool;
                if (_dbConnectionPoolDic.ContainsKey(dbid))
                {
                    if (_dbConnectionPoolDic.TryRemove(dbid, out dbConnectionPool))
                    {
                        dbConnectionPool.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 根据DBID获取数据库访问连接池对象
        /// </summary>
        /// <param name="dbid">数据库编号ID</param>
        /// <returns>数据库访问连接对象</returns>
        internal static DbConnectionPool GetConnectionPoolByDBID(int dbid)
        {
            DbConnectionPool dbConnectionPool;
            if (!_dbConnectionPoolDic.TryGetValue(dbid, out dbConnectionPool))
            {
                var config = DatabaseConfigManager.GetConfig(dbid);
                IDBFactory dbFactory = DBFactoryManager.GetDBFactory(config);
                IDBInteraction dbInteraction = dbFactory.GetDBInteraction();
                AddDbConnectionPool(config, dbInteraction);

                if (!_dbConnectionPoolDic.TryGetValue(dbid, out dbConnectionPool))
                {
                    throw new ApplicationException(string.Format("连接池中不包含数据库编号ID为:{0}的连接信息", dbid));
                }
            }

            return dbConnectionPool;
        }

        /// <summary>
        /// 释放连接池内所有的连接
        /// </summary>
        internal static void ReleaseDbConnection()
        {
            lock (_dbConnectionPoolDicMonitor)
            {
                if (!_status)
                {
                    return;
                }

                _status = false;
                try
                {
                    foreach (var dbConnectionPool in _dbConnectionPoolDic.Values)
                    {
                        dbConnectionPool.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
        }
    }
}
