using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBSQLite.Core
{
    internal class SQLiteDBInteraction : DBInteractionAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SQLiteDBInteraction()
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

        private string CreateSQLiteDBConStr(SQLiteConnectionStringBuilder scsb)
        {
            //if (visitType == DBVisitType.R)
            //{
            //    scsb.ReadOnly = true;
            //}
            //else
            //{
            //    scsb.ReadOnly = false;
            //}

            string dbDir = Path.GetDirectoryName(scsb.DataSource);
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }

            return scsb.ConnectionString;
        }

        /// <summary>
        /// 创建数据库拼接连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        protected override string CreateDBSpliceConStr(DatabaseConfig config, DBVisitType visitType)
        {
            var scsb = new SQLiteConnectionStringBuilder();
            scsb.Pooling = true;
            scsb.DataSource = DirectoryInfoEx.GetFullPath(config.DatabaseName);
            if (!string.IsNullOrEmpty(config.Password))
            {
                scsb.Password = config.Password;
            }

            return this.CreateSQLiteDBConStr(scsb);
        }

        /// <summary>
        /// 创建原生连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        protected override string CreateDBOriginConnectionString(DatabaseConfig config, DBVisitType visitType)
        {
            var scsb = new SQLiteConnectionStringBuilder(config.ConStr);
            return this.CreateSQLiteDBConStr(scsb);
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        public override DbProviderFactory GetProviderFactory()
        {
            return SQLiteFactory.Instance;
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
                dstSqlStr = string.Format("{0} limit {1},{2}", sqlStr, startIndex, pageSize);
            }
            else
            {
                dstSqlStr = string.Format("{0} ORDER BY {1} limit {2},{3}", sqlStr, orderStr, startIndex, pageSize);
            }

            return dstSqlStr;
        }

        private readonly SQLiteSqlFieldValueFormator _sqlFieldValueFormator = new SQLiteSqlFieldValueFormator();
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
