using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Config
{
    /// <summary>
    /// 数据库配置类
    /// </summary>
    [Serializable]
    public class DatabaseConfig
    {
        /// <summary>
        /// 数据库编号,int.MinValue无效编号
        /// </summary>
        [XmlElement("数据库编号", Namespace = "int.MinValue无效编号")]
        public int DBID { get; set; } = int.MinValue;

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        [XmlElement("数据库连接名称")]
        public string ConName { get; set; } = string.Empty;

        /// <summary>
        /// 数据库连接信息类型[0:字符串;1:ip端口号等分散信息;2:自定义]
        /// </summary>
        [XmlElement("数据库连接信息类型", Namespace = "0:字符串;1:ip端口号等分散信息;2:自定义")]
        public int DBConInfoType { get; set; } = 0;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [XmlElement("数据库连接字符串")]
        public string ConStr { get; set; } = null;

        /// <summary>
        /// 主机名或IP
        /// </summary>
        [XmlElement("主机名或IP")]
        public string Host { get; set; } = null;

        /// <summary>
        /// 端口号
        /// </summary>
        [XmlElement("端口号")]
        public int Port { get; set; } = 0;

        /// <summary>
        /// 数据库名称
        /// </summary>
        [XmlElement("数据库名称")]
        public string DatabaseName { get; set; } = null;

        /// <summary>
        /// 帐号
        /// </summary>
        [XmlElement("帐号")]
        public string Account { get; set; } = null;

        /// <summary>
        /// 密码
        /// </summary>
        [XmlElement("密码")]
        public string Password { get; set; } = null;

        /// <summary>
        /// 连接超时时长,单位:毫秒.DBConstant.CommandTimeout为默认值
        /// </summary>
        [XmlElement("连接超时时长", Namespace = "单位:毫秒")]
        public int ConnectionTimeout { get; set; } = DBConstant.CommandTimeout;

        /// <summary>
        /// SQL语句执行超时时长,单位:毫秒.DBConstant.CommandTimeout为默认值
        /// </summary>
        [XmlElement("SQL语句执行超时时长", Namespace = "单位:毫秒,-1使用默认值")]
        public int CommandTimeout { get; set; } = DBConstant.CommandTimeout;

        /// <summary>
        /// 数据库访问工厂类型名称
        /// </summary>
        [XmlElement("数据库访问工厂类型名称")]
        public string DBFactory { get; set; } = null;

        /// <summary>
        /// sql语句最大长度,DBConstant.SqlMaxLength为数制库默认值
        /// </summary>
        [XmlElement("sql语句最大长度")]
        public long SqlMaxLength { get; set; } = DBConstant.SqlMaxLength;

        /// <summary>
        /// 扩展参数
        /// </summary>
        [XmlElement("扩展参数")]
        public string Extend { get; set; } = null;

        /// <summary>
        /// 数据库写连接数,小于1为无限制
        /// </summary>
        [XmlElement("数据库写连接数", Namespace = "小于1无限制")]
        public int WriteConCount { get; set; } = DBConstant.WriteConCount;

        /// <summary>
        /// 数据库读连接数,小于1为无限制
        /// </summary>
        [XmlElement("数据库读连接数", Namespace = "小于1无限制")]
        public int ReadConCount { get; set; } = DBConstant.ReadConCount;

        /// <summary>
        /// 获取连接超时时长,单位:毫秒
        /// </summary>
        [XmlElement("获取连接超时时长", Namespace = "单位:毫秒")]
        public int GetConTimeout { get; set; } = DBConstant.GetConTimeout;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DatabaseConfig()
        {

        }

        /// <summary>
        /// 返回表示当前对象的String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = this.ConName;
            if (string.IsNullOrWhiteSpace(str))
            {
                str = this.DatabaseName;
            }

            return str;
        }
    }
}
