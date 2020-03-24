using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.EF
{
    /// <summary>
    /// EF实体类型管理类
    /// </summary>
    public class EFEntityTypeManager
    {
        private readonly static ConcurrentDictionary<int, HashSet<Type>> _entityTypeDic = new ConcurrentDictionary<int, HashSet<Type>>();
        private readonly static object _entityTypeDicLock = new object();
        private readonly static Type _tableAttributeType = typeof(TableAttribute);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <returns></returns>
        private static HashSet<Type> GetEntityTypeCollection(int dbid)
        {
            HashSet<Type> entityTypes;
            if (!_entityTypeDic.TryGetValue(dbid, out entityTypes))
            {
                entityTypes = new HashSet<Type>();
                _entityTypeDic[dbid] = entityTypes;
            }

            return entityTypes;
        }

        /// <summary>
        /// 注册一个实体
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <param name="entityType">实体类型</param>
        public static void RegisterEntityType(int dbid, Type entityType)
        {
            if (entityType == null)
            {
                return;
            }

            if (!entityType.GetCustomAttributes(_tableAttributeType, true).Any())
            {
                return;
            }

            lock (_entityTypeDicLock)
            {
                var entityTypes = GetEntityTypeCollection(dbid);
                entityTypes.Add(entityType);
            }
        }

        /// <summary>
        /// 注册一个实体集合
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <param name="entityTypes">实体类型集合</param>
        public static void RegisterEntityType(int dbid, IEnumerable<Type> entityTypes)
        {
            var resultEntityTypes = entityTypes.Where(t => t != null && t.GetCustomAttributes(_tableAttributeType, true).Any());
            lock (_entityTypeDicLock)
            {
                var entityTypes2 = GetEntityTypeCollection(dbid);
                foreach (var entityType in resultEntityTypes)
                {
                    entityTypes2.Add(entityType);
                }
            }
        }

        /// <summary>
        /// 注册一个实体类型程序集
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <param name="entityAssembly">实体类型定义程序集</param>
        /// <param name="entityTypeFilterFunc">实体类型过滤回调</param>
        public static void RegisterEntityType(int dbid, Assembly entityAssembly, Func<Type, bool> entityTypeFilterFunc = null)
        {
            var resultEntityTypes = entityAssembly.ExportedTypes.Where(t => t.GetCustomAttributes(_tableAttributeType, true).Any());
            if (entityTypeFilterFunc != null)
            {
                resultEntityTypes = resultEntityTypes.Where(entityTypeFilterFunc);
            }

            lock (_entityTypeDicLock)
            {
                var entityTypes = GetEntityTypeCollection(dbid);
                foreach (var entityType in resultEntityTypes)
                {
                    entityTypes.Add(entityType);
                }
            }
        }

        /// <summary>
        /// 清空实体类型
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        public static void Clear(int dbid)
        {
            lock (_entityTypeDicLock)
            {
                var entityTypes = GetEntityTypeCollection(dbid);
                entityTypes.Clear();
            }
        }

        /// <summary>
        /// 移除满足的条件的实体类型
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <param name="entityType">要移除的实体类型</param>
        public static void RemoveEntityType(int dbid, Type entityType)
        {
            lock (_entityTypeDicLock)
            {
                var entityTypes = GetEntityTypeCollection(dbid);
                entityTypes.Remove(entityType);
            }
        }

        /// <summary>
        /// 移除满足的条件的实体类型
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <param name="match">用于定义要移除的元素应满足的条件</param>
        public static void RemoveEntityType(int dbid, Predicate<Type> match)
        {
            lock (_entityTypeDicLock)
            {
                var entityTypes = GetEntityTypeCollection(dbid);
                entityTypes.RemoveWhere(match);
            }
        }

        /// <summary>
        /// 获取实体类型集合
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <returns>实体类型集合</returns>
        internal static Type[] GetEntityTypes(int dbid)
        {
            lock (_entityTypeDicLock)
            {
                var entityTypes = GetEntityTypeCollection(dbid);
                return entityTypes.ToArray();
            }
        }
    }
}
