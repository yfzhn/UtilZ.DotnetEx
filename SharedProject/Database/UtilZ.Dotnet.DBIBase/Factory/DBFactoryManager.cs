using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase.Factory
{
    /// <summary>
    /// 数据库访问对象创建工厂
    /// </summary>
    internal class DBFactoryManager
    {
        /// <summary>
        /// 数据库工厂集合[key:数据库访问工厂类型(Type),value:数据库访问工厂实例(DBFactoryBase)]
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IDBFactory> _dbFactoryDic = new ConcurrentDictionary<Type, IDBFactory>();

        static DBFactoryManager()
        {
            var assemblyArr = AppDomain.CurrentDomain.GetAssemblies();
            Dictionary<string, Assembly> assembliyDic = new Dictionary<string, Assembly>();
            foreach (var assemblyItem in assemblyArr)
            {
                assembliyDic[assemblyItem.GetName().Name] = assemblyItem;
            }

            Assembly currentAssembly = typeof(DBFactoryManager).Assembly;
            var ignoreAssemblyNames = new List<string>();
            ignoreAssemblyNames.Add(currentAssembly.GetName().Name);
            ignoreAssemblyNames.Add(typeof(UtilZ.Dotnet.Ex.Base.ObjectEx).Assembly.GetName().Name);
            ignoreAssemblyNames.AddRange(assembliyDic.Keys);

            string dbPluginRootFullDir = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "DBPlugins");
            Type idbFactoryType = typeof(IDBFactory);
            string[] dllFullPathArr = Directory.GetFiles(dbPluginRootFullDir, "UtilZ.Dotnet.DB*.dll", SearchOption.AllDirectories);
            Assembly assembly;

            foreach (var dllFullPath in dllFullPathArr)
            {
                try
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFullPath);
                    if (ignoreAssemblyNames.Contains(an.Name))
                    {
                        continue;
                    }

                    if (assembliyDic.ContainsKey(an.Name))
                    {
                        assembly = assembliyDic[an.Name];
                    }
                    else
                    {
                        assembly = Assembly.LoadFrom(dllFullPath);
                    }

                    ignoreAssemblyNames.Add(assembly.GetName().Name);
                    PrimitiveLoadDBPlugin(idbFactoryType, assembly);
                }
                catch (Exception ex)
                {
                    Loger.Error($"加载{dllFullPath}异常", ex);
                }
            }

            PluginLoadCompleted();
        }

        private static void PrimitiveLoadDBPlugin(Type idbFactoryType, Assembly assembly)
        {
            foreach (Type type in assembly.ExportedTypes)
            {
                if (!type.IsClass ||
                    type.IsAbstract ||
                    type.GetInterface(idbFactoryType.FullName) == null ||
                    !type.GetConstructors().Where(c => { return c.GetParameters().Length == 0; }).Any())
                {
                    //不是类或抽象类或没有实现IDBInteraction接口或没有无参构造函数,忽略
                    continue;
                }

                var dbFactory = Activator.CreateInstance(type) as IDBFactory;
                if (dbFactory == null)
                {
                    continue;
                }

                dbFactory.AttatchEFConfig();
                _dbFactoryDic[type] = dbFactory;
            }
        }

        /// <summary>
        /// 插件加载完成
        /// </summary>
        private static void PluginLoadCompleted()
        {
            //注:插件加载完成后,在此实例化一次EFDbContext,.NET的一个诡异BUG,应用域中已加载程序集却要报找不着,
            //执行下面代码手动加载EF依赖程序集。当该BUG解决后,删除本函数以及TestEFDbContextDbConnectionInfo类
            if (_dbFactoryDic.Count == 0)
            {
                return;
            }

            var enable = Ex.Base.AssemblyEx.Enable;
            try
            {
                Ex.Base.AssemblyEx.Enable = true;
                var dbFactory = _dbFactoryDic.Values.ElementAt(0);
                using (var efDbContext = new EF.EFDbContext(new TestEFDbContextDbConnectionInfo(dbFactory.GetDBInteraction().GetProviderFactory().CreateConnection()), null))
                {

                }
            }
            finally
            {
                Ex.Base.AssemblyEx.Enable = enable;
            }
        }

        /// <summary>
        /// 手动加载数据库插件
        /// </summary>
        /// <param name="pluginAssemblyPath">数据库插件Assembly路径</param>
        internal static void LoadDBPlugin(string pluginAssemblyPath)
        {
            if (string.IsNullOrWhiteSpace(pluginAssemblyPath))
            {
                throw new ArgumentNullException(nameof(pluginAssemblyPath));
            }

            if (string.IsNullOrWhiteSpace(Path.GetPathRoot(pluginAssemblyPath)))
            {
                pluginAssemblyPath = Path.GetFullPath(pluginAssemblyPath);
            }

            if (!File.Exists(pluginAssemblyPath))
            {
                throw new FileNotFoundException("数据库插件程序集不存在", pluginAssemblyPath);
            }

            AssemblyName an = AssemblyName.GetAssemblyName(pluginAssemblyPath);
            Dictionary<string, Assembly> assembliyDic = AppDomain.CurrentDomain.GetAssemblies().ToDictionary(p => { return p.GetName().Name; });
            Assembly assembly;
            if (assembliyDic.ContainsKey(an.Name))
            {
                assembly = assembliyDic[an.Name];
            }
            else
            {
                assembly = Assembly.LoadFrom(pluginAssemblyPath);
            }

            PrimitiveLoadDBPlugin(typeof(IDBFactory), assembly);
            PluginLoadCompleted();
        }

        /// <summary>
        /// 获取数据库访问工厂实例
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>数据库访问工厂实例</returns>
        internal static IDBFactory GetDBFactory(DatabaseConfig config)
        {
            return GetDBFactoryByConfig(config);
        }

        /// <summary>
        /// [key:dbid;value:IDBFactory]
        /// </summary>
        private static readonly ConcurrentDictionary<int, IDBFactory> _dbFactoryDic2 = new ConcurrentDictionary<int, IDBFactory>();
        private static readonly object _dbFactoryDicLock = new object();
        /// <summary>
        /// 根据数据库配置获取数据库访问工厂实例
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>数据库访问工厂实例</returns>
        private static IDBFactory GetDBFactoryByConfig(DatabaseConfig config)
        {
            int dbid = config.DBID;
            IDBFactory dbFactory;
            if (_dbFactoryDic2.TryGetValue(dbid, out dbFactory))
            {
                return dbFactory;
            }

            lock (_dbFactoryDicLock)
            {
                if (_dbFactoryDic2.TryGetValue(dbid, out dbFactory))
                {
                    return dbFactory;
                }

                dbFactory = GetDBFactoryByDBFactoryName(config.DBFactory);
                _dbFactoryDic2[dbid] = dbFactory;
                return dbFactory;
            }
        }

        /// <summary>
        /// 获取数据库访问工厂实例
        /// </summary>
        /// <param name="dbFactoryName">数据库工厂类型名称</param>
        /// <returns>数据库访问工厂实例</returns>
        private static IDBFactory GetDBFactoryByDBFactoryName(string dbFactoryName)
        {
            Type type = TypeEx.ConvertTypeByTypeFullName(dbFactoryName, true);
            if (type == null)
            {
                throw new ApplicationException(string.Format("获取名称为{0}数据库创建工厂实例失败", dbFactoryName));
            }

            IDBFactory dbFactory;
            if (_dbFactoryDic.TryGetValue(type, out dbFactory))
            {
                return dbFactory;
            }
            else
            {
                throw new ApplicationException(string.Format("获取名称为{0}数据库创建工厂实例失败", dbFactoryName));
            }
        }

        /// <summary>
        /// 获取Sql字段值格式化对象
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>Sql字段值格式化对象</returns>
        internal static ISqlFieldValueFormator GetSqlFieldValueFormator(DatabaseConfig config)
        {
            return GetDBFactoryByConfig(config).GetDBInteraction().SqlFieldValueFormator;
        }
    }
}
