using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase.Config
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public class DatabaseConfigManager
    {
        /// <summary>
        /// 数据库配置文件名称
        /// </summary>
        public const string DBCONFIG_FILENAME = "DBConfig.xml";

        /// <summary>
        /// 配置项字典集合[key:DBID;value:配置项]
        /// </summary>
        private static readonly ConcurrentDictionary<int, DatabaseConfig> _configDic = new ConcurrentDictionary<int, DatabaseConfig>();

        /// <summary>
        /// 加载默认配置App.config
        /// </summary>
        //public static void Init()
        static DatabaseConfigManager()
        {
            try
            {
                var dir = Path.GetDirectoryName(typeof(DatabaseConfigManager).Assembly.Location);
                string dbConfigFilePath = Path.Combine(dir, DBCONFIG_FILENAME);
                //加载配置
                LoadConfig(dbConfigFilePath);
            }
            catch (Exception ex)
            {
                Loger.Error("加载数据库配置文件异常", ex);
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configFilePath">数据库配置文件路径</param>
        public static void LoadConfig(string configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath) || !File.Exists(configFilePath))
            {
                return;
            }

            var xdoc = XDocument.Load(configFilePath);
            var dbConfigItemEles = xdoc.XPathSelectElements(@"DBConfig/DBConfigItems/DBConfigItem");
            if (dbConfigItemEles.Count() == 0)
            {
                return;
            }

            IDatabaseConfigParser defaultDBConfigParser = new DefaultDatabaseConfigParser();
            IEnumerable<DatabaseConfig> configs;

            foreach (var dbConfigItemEle in dbConfigItemEles)
            {
                configs = ParseDBConfigEle(defaultDBConfigParser, dbConfigItemEle);
                if (configs != null)
                {
                    foreach (var config in configs)
                    {
                        AddConfigItem(config);
                    }
                }
            }
        }

        private static IEnumerable<DatabaseConfig> ParseDBConfigEle(IDatabaseConfigParser dbConfigParser, XElement dbConfigItemEle)
        {
            IEnumerable<DatabaseConfig> configs;
            const string DBCONFIG_PARSER_TAG = "Parser";
            var dbBConfigParserAtt = dbConfigItemEle.Attribute(DBCONFIG_PARSER_TAG);

            if (dbBConfigParserAtt == null || string.IsNullOrWhiteSpace(dbBConfigParserAtt.Value))
            {
                configs = dbConfigParser.Parse(dbConfigItemEle);
            }
            else
            {
                IDatabaseConfigParser customerParser = ActivatorEx.CreateInstance(dbConfigItemEle.Value) as IDatabaseConfigParser;
                if (customerParser == null)
                {
                    throw new ApplicationException(string.Format("创建数据库连接配置解析器{0}失败", dbConfigItemEle.Value));
                }

                configs = customerParser.Parse(dbConfigItemEle);
            }

            return configs;
        }

        /// <summary>
        /// 添加配置项
        /// </summary>
        /// <param name="config">要添加的配置项</param>
        public static void AddConfigItem(DatabaseConfig config)
        {
            if (config == null)
            {
                return;
            }

            _configDic[config.DBID] = config;
            //_configItems.TryAdd(dbBConfigItem.DBID, dbBConfigItem);
        }

        /// <summary>
        /// 移除配置项
        /// </summary>
        /// <param name="dbid">要移除的配置项的数据库编号</param>
        /// <returns>被移除的配置项</returns>
        public static DatabaseConfig RemoveConfigItem(int dbid)
        {
            DatabaseConfig dbBConfigItem;
            _configDic.TryRemove(dbid, out dbBConfigItem);
            return dbBConfigItem;
        }

        /// <summary>
        /// 移除配置项
        /// </summary>
        /// <param name="config">要移除的配置项</param>
        public static void RemoveConfigItem(DatabaseConfig config)
        {
            if (config == null)
            {
                return;
            }

            RemoveConfigItem(config.DBID);
        }

        /// <summary>
        /// 是否包含指定数据库编号的配置项
        /// </summary>
        /// <param name="dbid">指定数据库编号</param>
        public static bool ContainsDBID(int dbid)
        {
            return _configDic.ContainsKey(dbid);
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <returns>配置项</returns>
        public static DatabaseConfig GetConfig(int dbid)
        {
            DatabaseConfig dbBConfigItem;
            if (_configDic.TryGetValue(dbid, out dbBConfigItem))
            {
                return dbBConfigItem;
            }
            else
            {
                throw new ApplicationException(string.Format("配置中不包含数据库编号为{0}的配置", dbid));
            }
        }

        /// <summary>
        /// 尝试获取配置项[返回值-成功:true;失败:false]
        /// </summary>
        /// <param name="dbid">数据库编号</param>
        /// <param name="config">配置项</param>
        /// <returns>获取结果[成功:true;失败:false]</returns>
        public static bool TryGetConfigItem(int dbid, out DatabaseConfig config)
        {
            return _configDic.TryGetValue(dbid, out config);
        }

        /// <summary>
        /// 获取全部配置项列表
        /// </summary>
        /// <returns>配置项</returns>
        public static List<DatabaseConfig> GetAllConfigItems()
        {
            return _configDic.Values.ToList();
        }
    }
}
