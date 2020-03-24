using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBMySql.Core
{
    internal class MySqlDBInteraction : DBInteractionAbs
    {
        private readonly MySqlClientFactoryZ _mySqlClientFactory;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MySqlDBInteraction()
            : base()
        {
            this._mySqlClientFactory = new MySqlClientFactoryZ();
        }

        private const string PARASIGN = "?";
        /// <summary>
        /// 数据库参数字符
        /// </summary>
        public override string ParaSign
        {
            get { return PARASIGN; }
        }

        /// <summary>
        /// 创建数据库拼接连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        protected override string CreateDBSpliceConStr(DatabaseConfig config, DBVisitType visitType)
        {
            //MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

            if (config.Port == 0)
            {
                config.Port = 3306;
            }

            return string.Format(@"database={0};data source={1};Port={2};user id={3};password={4}", config.DatabaseName, config.Host, config.Port, config.Account, config.Password);
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        public override DbProviderFactory GetProviderFactory()
        {
            //6.10.8版本中居然没有重写CreateDataAdapter()方法,不晓得Oracle这群SB在干啥.这种低级BUG都搞得出来,没办法,自己写一个吧
            //return MySqlClientFactory.Instance;

            return this._mySqlClientFactory;
        }

        /// <summary>
        /// 转换原始查询SQL语句为分页查询SQL语句
        /// </summary>
        /// <param name="sqlStr">原始查询SQL语句</param>
        /// <param name="orderStr">排序字符串</param>
        /// <param name="pageIndex">目标页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dataBaseVersion">数据库版本号信息</param>
        /// <param name="pagingAssistFieldName">分页字段名称</param>
        /// <returns>分页查询SQL语句</returns>
        protected override string ConvertSqlToPagingQuerySql(string sqlStr, string orderStr, long pageIndex, long pageSize, DataBaseVersionInfo dataBaseVersion, out string pagingAssistFieldName)
        {
            //dataBaseVersion:3.8.2
            //eg:SELECT * from person WHERE ID < 100 ORDER by ID DESC limit 0,10
            pagingAssistFieldName = null;
            var startIndex = (pageIndex - 1) * pageSize;
            string dstSqlStr = null;
            if (string.IsNullOrWhiteSpace(orderStr))
            {
                dstSqlStr = $@"{sqlStr} limit {startIndex},{pageSize}";
            }
            else
            {
                dstSqlStr = $@"{sqlStr} ORDER BY {orderStr} limit {startIndex},{pageSize}";
            }

            return dstSqlStr;
        }

        private readonly MySqlSqlFieldValueFormator _sqlFieldValueFormator = new MySqlSqlFieldValueFormator();
        /// <summary>
        /// 获取sql字段值格式化对象
        /// </summary>
        /// <returns>sql字段值格式化对象</returns>
        protected override ISqlFieldValueFormator GetSqlFieldValueFormator()
        {
            return this._sqlFieldValueFormator;
        }
    }
}
