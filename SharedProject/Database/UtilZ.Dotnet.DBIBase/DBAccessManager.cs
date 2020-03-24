using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Factory;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase
{
    /// <summary>
    /// 数据库访问类管理器
    /// </summary>
    public class DBAccessManager
    {
        /// <summary>
        /// 数据库访问对象实例字典[key:数据库编号ID;value:数据库访问实例]
        /// </summary>
        private static readonly ConcurrentDictionary<int, IDBAccess> _dbAccessDic = new ConcurrentDictionary<int, IDBAccess>();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object _dicDBAccessLock = new object();

        /// <summary>
        /// 获取数据库访问实例
        /// </summary>
        /// <param name="dbid">数据库编号ID</param>
        /// <returns>数据库访问实例</returns>
        public static IDBAccess GetDBAccessInstance(int dbid)
        {
            IDBAccess dbAccess;
            if (!_dbAccessDic.TryGetValue(dbid, out dbAccess))
            {
                lock (_dicDBAccessLock)
                {
                    if (!_dbAccessDic.TryGetValue(dbid, out dbAccess))
                    {
                        var dbBConfigItem = DatabaseConfigManager.GetConfig(dbid);
                        IDBFactory dbFactory = DBFactoryManager.GetDBFactory(dbBConfigItem);
                        dbAccess = dbFactory.CreateDBAccess(dbBConfigItem);

                        if (!_dbAccessDic.TryAdd(dbid, dbAccess))
                        {
                            Loger.Warn(string.Format("添加数据库编号ID为{0}数据库访问实例失败", dbid), null);
                        }
                    }
                }
            }

            return dbAccess;
        }

        /// <summary>
        /// 移除数据库实例
        /// </summary>
        /// <param name="dbid">数据库编号ID</param>
        /// <returns>已移除的数据库访问实例</returns>
        public static IDBAccess RemoveDBAccessInstance(int dbid)
        {
            IDBAccess dbAccess;
            if (_dbAccessDic.TryRemove(dbid, out dbAccess))
            {
                dbAccess.Dispose();
                DbConnectionPoolManager.RemoveDbConnectionPool(dbid);
            }

            return dbAccess;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Release()
        {
            lock (_dicDBAccessLock)
            {
                foreach (var dbAccess in _dbAccessDic.Values)
                {
                    dbAccess.Dispose();
                }
            }

            DbConnectionPoolManager.ReleaseDbConnection();
        }

        /// <summary>
        /// 获取字段值格式化对象
        /// </summary>
        /// <param name="config">数据配置</param>
        /// <returns></returns>
        public static ISqlFieldValueFormator GetFieldValueFormator(DatabaseConfig config)
        {
            return DBFactoryManager.GetSqlFieldValueFormator(config);
        }

        /// <summary>
        /// 加载指定路径数据库插件
        /// </summary>
        /// <param name="pluginAssemblyPath">数据库插件Assembly路径</param>
        public static void LoadDBPlugin(string pluginAssemblyPath)
        {
            DBFactoryManager.LoadDBPlugin(pluginAssemblyPath);
        }
    }
}
