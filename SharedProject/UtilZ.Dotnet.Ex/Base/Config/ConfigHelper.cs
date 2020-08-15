using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 配置辅助类
    /// </summary>
    public sealed class ConfigHelper
    {

        /// <summary>
        /// 写配置对象到xml文件
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="configFilePath">配置文件存放路径</param>
        public static void WriteConfigToXmlFile(object config, string configFilePath)
        {
            if (configFilePath == null)
            {
                throw new ArgumentNullException(nameof(configFilePath));
            }

            XDocument xdoc = WriteConfigToXDocument(config);
            DirectoryInfoEx.CheckFilePathDirectory(configFilePath);
            xdoc.Save(configFilePath);
        }

        /// <summary>
        /// 写配置到XDocument
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>配置XDocument</returns>
        public static XDocument WriteConfigToXDocument(object config)
        {
            var configCore = new ConfigCore();
            return configCore.WriteConfigToXDocument(config);
        }









        /// <summary>
        /// 从文件读取配置
        /// </summary>
        /// <typeparam name="T">配置对象类型</typeparam>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>配置对象</returns>
        public static T ReadConfigFromFile<T>(string configFilePath) where T : new()
        {
            object config = new T();
            XDocument xdoc = XDocument.Load(configFilePath);
            ReadConfigFromXDocument(xdoc, ref config);
            return (T)config;
        }

        /// <summary>
        /// 从文件读取配置到指定的配置对象
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <param name="config">指定的配置对象</param>
        public static void ReadConfigFromFile(string configFilePath, object config)
        {
            XDocument xdoc = XDocument.Load(configFilePath);
            ReadConfigFromXDocument(xdoc, ref config);
        }

        /// <summary>
        /// 从xml配置中读取配置
        /// </summary>
        /// <typeparam name="T">配置对象类型</typeparam>
        /// <param name="xmlStr">xml字符串</param>
        /// <returns>配置对象</returns>
        public static T ReadConfigFromXml<T>(string xmlStr) where T : new()
        {
            object config = new T();
            XDocument xdoc = XDocument.Parse(xmlStr);
            ReadConfigFromXDocument(xdoc, ref config);
            return (T)config;
        }

        /// <summary>
        /// 从xml配置中读取配置到指定的配置对象中
        /// </summary>
        /// <param name="xmlStr">xml字符串</param>
        /// <param name="config">指定的配置对象</param>
        public static void ReadConfigFromXml(string xmlStr, object config)
        {
            XDocument xdoc = XDocument.Parse(xmlStr);
            ReadConfigFromXDocument(xdoc, ref config);
        }

        /// <summary>
        /// 从XDocument配置中读取配置
        /// </summary>
        /// <typeparam name="T">配置对象类型</typeparam>
        /// <param name="xdoc">XDocument配置</param>
        /// <returns>配置对象</returns>
        public static T ReadConfigFromXml<T>(XDocument xdoc) where T : new()
        {
            object config = new T();
            ReadConfigFromXDocument(xdoc, ref config);
            return (T)config;
        }

        /// <summary>
        /// 从xml配置中读取配置到指定的配置对象中
        /// </summary>
        /// <param name="xdoc">xml配置</param>
        /// <param name="config">指定的配置对象</param>
        public static void ReadConfigFromXml(XDocument xdoc, object config)
        {
            var configCore = new ConfigCore();
            ReadConfigFromXDocument(xdoc, ref config);
        }

        private static void ReadConfigFromXDocument(XDocument xdoc, ref object config)
        {
            var configCore = new ConfigCore();
            configCore.ReadConfigFromXDocument(xdoc, ref config);
        }









        #region 读写配置辅助方法
        /// <summary>
        /// 默认配置文件名称
        /// </summary>
        public const string DEFAULT_CONFIGFILE_NAME = "config.xml";

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>配置对象</returns>
        public static T Load<T>(string configFilePath = null) where T : IConfig, new()
        {
            GeneratConfigFilePath(typeof(T), ref configFilePath);

            T config;
            if (File.Exists(configFilePath))
            {
                try
                {
                    config = DeserializerFromFile<T>(configFilePath);
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                    config = new T();
                    config.InitDefaultValue();
                    SerializeToFile(config, configFilePath);
                }
            }
            else
            {
                config = new T();
                config.InitDefaultValue();
                SerializeToFile(config, configFilePath);
            }

            return config;
        }

        private static void SerializeToFile(object config, string configFilePath)
        {
            GeneratConfigFilePath(config.GetType(), ref configFilePath);
            DirectoryInfoEx.CheckFilePathDirectory(configFilePath);
            WriteConfigToXmlFile(config, configFilePath);
        }

        private static T DeserializerFromFile<T>(string configFilePath) where T : IConfig, new()
        {
            return ReadConfigFromFile<T>(configFilePath);
        }

        private static void GeneratConfigFilePath(Type type, ref string configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                configFilePath = Path.Combine(DirectoryInfoEx.GetAssemblyDirectory(type), DEFAULT_CONFIGFILE_NAME);
            }
        }


        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config">要保存的配置</param>
        /// <param name="configFilePath">配置保存路径</param>
        public static void Save(object config, string configFilePath = DEFAULT_CONFIGFILE_NAME)
        {
            SerializeToFile(config, configFilePath);
        }
        #endregion
    }
}
