using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBSqlServer.Core
{
    internal class SQLServerDBInteraction : DBInteractionAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLServerDBInteraction()
            : base()
        {

        }

        private const string PARASIGN = "@";
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
            //SqlConnectionStringBuilder
            if (config.Port == 0)
            {
                config.Port = 1433;
            }

            return string.Format(@"data source={0},{1};initial catalog={2};user id={3};password={4}", config.Host, config.Port, config.DatabaseName, config.Account, config.Password);
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        public override DbProviderFactory GetProviderFactory()
        {
            return SqlClientFactory.Instance;
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
            //dataBaseVersion.Version:2008
            var startIndex = (pageIndex - 1) * pageSize;
            string pagingSql;
            if (string.IsNullOrWhiteSpace(orderStr))
            {
                if (dataBaseVersion != null && dataBaseVersion.Version >= 2012)
                {
                    //SQLServer2012+新特性,在高并发时性能强于ROW_NUMBER方式
                    pagingSql = $"{sqlStr} OFFSET {startIndex} ROW FETCH NEXT {pageSize} rows only";
                    pagingAssistFieldName = null;
                }
                else
                {
                    pagingAssistFieldName = base.GetPagingAssistColName(sqlStr);
                    string lowerSqlStr = sqlStr.ToLower();
                    string selectStr = "SELECT";
                    string ronumSqlStr = sqlStr.Insert(lowerSqlStr.IndexOf(selectStr) + selectStr.Length + 1, $" ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS {pagingAssistFieldName},");
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM (");
                    sb.Append(ronumSqlStr);
                    sb.Append(string.Format(") AS WQS_T WHERE {0} BETWEEN ", pagingAssistFieldName));
                    sb.Append(startIndex);
                    sb.Append(" AND ");
                    sb.Append(startIndex + pageSize);
                    pagingSql = sb.ToString();
                }
            }
            else
            {
                if (dataBaseVersion != null && dataBaseVersion.Version >= 2012)
                {
                    /*
               declare @pageIndex int
               declare @pageSize int
               set @pageIndex = 1
               set @pageSize = 10
               select * from person order by ID asc OFFSET (@pageSize * (@pageIndex-1)) ROW FETCH NEXT @pageSize rows only;
                */

                    //SQLServer2012+新特性,在高并发时性能强于ROW_NUMBER方式
                    pagingSql = $"{sqlStr} ORDER BY {orderStr} OFFSET {startIndex} ROW FETCH NEXT {pageSize} rows only";
                    pagingAssistFieldName = null;
                }
                else
                {
                    pagingAssistFieldName = base.GetPagingAssistColName(sqlStr);
                    string lowerSqlStr = sqlStr.ToLower();
                    string selectStr = "SELECT";
                    string ronumSqlStr = sqlStr.Insert(lowerSqlStr.IndexOf(selectStr) + selectStr.Length + 1, $" ROW_NUMBER() OVER (ORDER BY {orderStr}) AS {pagingAssistFieldName},");
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM (");
                    sb.Append(ronumSqlStr);
                    sb.Append(string.Format(") AS WQS_T WHERE {0} BETWEEN ", pagingAssistFieldName));
                    sb.Append(startIndex);
                    sb.Append(" AND ");
                    sb.Append(startIndex + pageSize);
                    pagingSql = sb.ToString();
                }
            }

            return pagingSql;
        }

        private readonly SQLServerSqlFieldValueFormator _sqlFieldValueFormator = new SQLServerSqlFieldValueFormator();
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
