using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBSQLite.Core
{
    internal class SQLiteDatabase : DatabaseAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbAccess">数据库访问对象</param>
        public SQLiteDatabase(IDBAccess dbAccess)
            : base(dbAccess)
        {

        }

        #region 判断表或字段是否存在
        /// <summary>
        /// 判断表是否存在[存在返回true,不存在返回false]
        /// </summary>
        /// <param name="con">IDbConnection</param>
        /// <param name="tableName">表名[表名区分大小写的数据库:Oracle,SQLite]</param>
        /// <returns>存在返回true,不存在返回false</returns>
        protected override bool PrimitiveExistTable(IDbConnection con, string tableName)
        {
            //string sqlStr = @"SELECT COUNT(0) FROM sqlite_master where type='table' and name=@TABLENAME";
            //var paras = new NDbParameterCollection();
            //paras.Add("TABLENAME", tableName);
            //object value = this.ExecuteScalar(sqlStr, paras);
            //return Convert.ToInt32(value) > 0;
            const string sqlStr = @"select tbl_name,'' from sqlite_master where type='table'";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            string dbTableName;
            foreach (DataRow row in dt.Rows)
            {
                dbTableName = row[0].ToString();
                if (tableName.Equals(dbTableName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断表中是否存在字段[存在返回true,不存在返回false]
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>存在返回true,不存在返回false</returns>
        protected override bool PrimitiveExistField(IDbConnection con, string tableName, string fieldName)
        {
            string sqlStr = string.Format(@"select * from {0} LIMIT 0,0", tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            if (dt == null)
            {
                return false;
            }

            return dt.Columns.Contains(fieldName);
        }
        #endregion

        #region 获取表的字段信息
        /// <summary>
        /// 获取表的字段信息
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="tableName">表名</param>
        /// <returns>字段信息集合</returns>
        protected override List<DBFieldInfo> PrimitiveGetTableFieldInfo(IDbConnection con, string tableName)
        {
            //因为sqlite没有记录列信息的表,所以此处采用的是查询空表数据,然后从DataTable中获取列的信息
            string sqlStr = string.Format(@"select * from {0} LIMIT 0,0", tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            var dicFieldDbClrFieldType = base.GetFieldDbClrFieldType(dt.Columns);//字段的公共语言运行时类型字典集合
            var priKeyCols = this.PrimitiveQueryPriKeyField(con, tableName);//主键列名集合
            List<DBFieldInfo> colInfos = new List<DBFieldInfo>();
            string fieldName;
            string dbTypeName;
            bool allowNull;
            object defaultValue;
            Type type;
            DBFieldType fieldType;

            foreach (DataColumn col in dt.Columns)
            {
                fieldName = col.ColumnName;
                dbTypeName = col.DataType.Name;
                allowNull = col.AllowDBNull; ;
                defaultValue = col.DefaultValue;
                //comments:sqlite没有这一项
                type = col.DataType;
                fieldType = dicFieldDbClrFieldType[fieldName];
                colInfos.Add(new DBFieldInfo(tableName, fieldName, dbTypeName, type, null, defaultValue, allowNull, fieldType, priKeyCols.Contains(fieldName)));
            }

            return colInfos;
        }

        /// <summary>
        /// 查询主键列名集合
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="tableName">表名</param>
        /// <returns>主键列名集合</returns>
        protected override List<string> PrimitiveQueryPriKeyField(IDbConnection con, string tableName)
        {
            string sqlStr = string.Format(@"pragma table_info ('{0}')", tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            List<string> priKeyCols = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["pk"]) == 1)
                {
                    priKeyCols.Add(row[1].ToString());
                }
            }

            return priKeyCols;
        }
        #endregion

        #region 获取表信息
        /// <summary>
        /// 获取当前用户有权限的所有表集合
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="getFieldInfo">是否获取字段信息[true:获取字段信息;false:不获取;默认不获取]</param>
        /// <returns>当前用户有权限的所有表集合</returns>
        protected override List<DBTableInfo> PrimitiveGetTableInfoList(IDbConnection con, bool getFieldInfo)
        {
            const string queryTableNameSqlStr = @"select name from sqlite_master where type = 'table'";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, queryTableNameSqlStr);

            if (getFieldInfo)
            {
                return this.GetTableInfoListHasGetFieldInfo(dt, con);
            }
            else
            {
                return this.GetTableInfoListHasNotGetFieldInfo(dt, con);
            }
        }

        private List<DBTableInfo> GetTableInfoListHasGetFieldInfo(DataTable dt, IDbConnection con)
        {
            string queryIndexSqlStr = this.GetQueryIndexSql(null);
            DataTable dtIndex = base.PrimitiveQueryDataToDataTable(con, queryIndexSqlStr);
            var indexTupleRowArr = new Tuple<string, DataRow>[dtIndex.Rows.Count];
            for (int i = 0; i < dtIndex.Rows.Count; i++)
            {
                indexTupleRowArr[i] = new Tuple<string, DataRow>(DBAccessEx.ConvertObject<string>(dtIndex.Rows[i]["tbl_name"]), dtIndex.Rows[i]);
            }

            DataRow[] indexArr;
            var tableInfoList = new List<DBTableInfo>();
            string tableName;
            DBIndexInfoCollection indexInfoCollection = null;

            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                if (this.IgnorTable(tableName))
                {
                    continue;
                }

                indexArr = indexTupleRowArr.Where(t => { return string.Equals(t.Item1, tableName); }).Select(t => { return t.Item2; }).ToArray();
                indexInfoCollection = this.ConvertTableIndexs(indexArr, tableName);
                tableInfoList.Add(this.PrimitiveGetTableInfoByName(con, tableName, true, indexInfoCollection));
            }

            return tableInfoList;
        }

        private List<DBTableInfo> GetTableInfoListHasNotGetFieldInfo(DataTable dt, IDbConnection con)
        {
            var tableInfoList = new List<DBTableInfo>();
            string tableName;

            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                if (this.IgnorTable(tableName))
                {
                    continue;
                }

                tableInfoList.Add(this.PrimitiveGetTableInfoByName(con, tableName, false, null));
            }

            return tableInfoList;
        }

        private bool IgnorTable(string tableName)
        {
            return string.Equals("sqlite_sequence", tableName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取表信息[表不存在返回null]
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="getFieldInfo">是否获取字段信息[true:获取字段信息;false:不获取;默认不获取]</param>
        /// <param name="indexInfoCollection">索引集合</param>
        /// <returns>表信息</returns>
        protected override DBTableInfo PrimitiveGetTableInfoByName(IDbConnection con, string tableName, bool getFieldInfo, DBIndexInfoCollection indexInfoCollection)
        {
            const string sqlStr = @"select tbl_name from sqlite_master where type='table'";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            string dbTableName = null;
            foreach (DataRow row in dt.Rows)
            {
                dbTableName = row[0].ToString();
                if (tableName.Equals(dbTableName, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    dbTableName = null;
                }
            }

            if (string.IsNullOrWhiteSpace(dbTableName))
            {
                throw new ArgumentException($"表[{tableName}]不存在");
            }

            List<DBFieldInfo> colInfos = null;//字段集合
            IEnumerable<DBFieldInfo> priKeyColInfos = null;//主键列字段集合

            if (getFieldInfo)
            {
                colInfos = this.PrimitiveGetTableFieldInfo(con, tableName);//获取表所有字段集合
                priKeyColInfos = from col in colInfos where col.IsPriKey select col;//获取主键列字段集合
            }

            return new DBTableInfo(tableName, string.Empty, new DBFieldInfoCollection(colInfos),
                new DBFieldInfoCollection(priKeyColInfos), indexInfoCollection);
        }
        #endregion

        /// <summary>
        /// 获取表索引信息集合
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="tableName">表名</param>
        /// <returns>表索引信息集合</returns>
        protected override DBIndexInfoCollection PrimitiveGetTableIndexs(IDbConnection con, string tableName)
        {
            string queryIndexSqlStr = this.GetQueryIndexSql(tableName);
            DataTable dtIndex = base.PrimitiveQueryDataToDataTable(con, queryIndexSqlStr);
            return this.ConvertTableIndexs(dtIndex.Rows, tableName);
        }

        private string GetQueryIndexSql(string tableName)
        {
            string queryIndexSqlStr;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                queryIndexSqlStr = "select tbl_name,name,rootpage,sql from sqlite_master where type='index'";
            }
            else
            {
                queryIndexSqlStr = $"select name,rootpage,sql from sqlite_master where type='index' AND tbl_name='{tableName}'";
            }

            return queryIndexSqlStr;
        }

        private DBIndexInfoCollection ConvertTableIndexs(IEnumerable rows, string tableName)
        {
            var indexinfoList = new List<DBIndexInfo>();
            if (rows != null)
            {
                string indexName, fieldName, sql;
                int lastLeftParenthesisIndex, lastRightParenthesisIndex;
                StringBuilder sbDetail = new StringBuilder();

                foreach (DataRow row in rows)
                {
                    sbDetail.Clear();
                    indexName = DBAccessEx.ConvertObject<string>(row["name"]);//Stu_Index_Name
                    sbDetail.AppendLine($"[rootpage:{DBAccessEx.ConvertObject<string>(row["rootpage"])}]");//63
                    sql = DBAccessEx.ConvertObject<string>(row["sql"]);//CREATE INDEX "Stu_Index_Name" ON "Stu"("Name")

                    lastLeftParenthesisIndex = sql.LastIndexOf('(');
                    lastRightParenthesisIndex = sql.LastIndexOf(')');
                    fieldName = sql.Substring(lastLeftParenthesisIndex + 1, lastRightParenthesisIndex - lastLeftParenthesisIndex - 1);
                    sbDetail.AppendLine($"[sql:{sql}]");

                    indexinfoList.Add(new DBIndexInfo(tableName, indexName, fieldName, sbDetail.ToString()));
                }
            }

            return new DBIndexInfoCollection(indexinfoList);
        }

        /// <summary>
        /// 获取数据库版本信息
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <returns>数据库版本信息</returns>
        protected override DataBaseVersionInfo PrimitiveGetDataBaseVersion(IDbConnection con)
        {
            const string sqlStr = @"select sqlite_version()";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            string dataBaseVersion = DBAccessEx.ConvertObject<string>(value);//dataBaseVersion:3.8.2
            string verStr = dataBaseVersion.Substring(0, dataBaseVersion.IndexOf('.'));
            int version;
            if (!int.TryParse(verStr, out version))
            {
                version = 3;
            }

            return new DataBaseVersionInfo(version, dataBaseVersion);
        }

        /// <summary>
        /// 获取数据库系统时间
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <returns>数据库系统时间</returns>
        protected override DateTime PrimitiveGetDataBaseSysTime(IDbConnection con)
        {
            const string sqlStr = @"select datetime('now','localtime')";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            return DBAccessEx.ConvertObject<DateTime>(value);
        }

        /// <summary>
        /// 获取当前登录用户名
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <returns>当前登录用户名</returns>
        protected override string PrimitiveGetLoginUserName(IDbConnection con)
        {
            return base._dbAccess.Config.Account;
        }

        /// <summary>
        /// 获取数据库名称
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <returns>数据库名称</returns>
        protected override string PrimitiveGetDatabaseName(IDbConnection con)
        {
            string databaseName;
            if (string.IsNullOrWhiteSpace(base._dbAccess.Config.DatabaseName))
            {
                var scsb = new SQLiteConnectionStringBuilder(con.ConnectionString);
                databaseName = scsb.DataSource;
                base._dbAccess.Config.DatabaseName = databaseName;
            }
            else
            {
                databaseName = base._dbAccess.Config.DatabaseName;
            }

            return databaseName;
        }

        #region 获取数据库属性信息
        /// <summary>
        /// 获取数据库属性信息
        /// </summary>
        /// <param name="lastDatabasePropertyInfo">前一次获取到的数据库属性信息</param>
        /// <returns>数据库属性信息</returns>
        protected override DatabasePropertyInfo PrimitiveGetDatabasePropertyInfo(DatabasePropertyInfo lastDatabasePropertyInfo)
        {
            using (var con = base.CreateConnection())
            {
                var dbConnection = con.DbConnection;
                long memorySize = this.PrimitiveGetMemorySize(dbConnection);
                long diskSize = this.PrimitiveGetDiskSize(dbConnection);
                int maxConnectCount = this.PrimitiveGetMaxConnectCount(dbConnection);
                int totalConnectCount, concurrentConnectCount;
                this.PrimitiveGetTotalConnectCountAndConcurrentConnectCount(dbConnection, out totalConnectCount, out concurrentConnectCount);
                int activeConnectCount = concurrentConnectCount;

                DateTime startTime, createtTime;
                if (lastDatabasePropertyInfo == null)
                {
                    startTime = this.PrimitiveGetStartTime(dbConnection);
                    createtTime = this.PrimitiveGetCreatetTime(dbConnection);
                }
                else
                {
                    startTime = lastDatabasePropertyInfo.StartTime;
                    createtTime = lastDatabasePropertyInfo.CreatetTime;
                }

                List<string> allUserNameList = this.GetAllUserNameList(dbConnection);
                float cpuRate = 0f;

                return new DatabasePropertyInfo(memorySize, diskSize, maxConnectCount,
                    totalConnectCount, concurrentConnectCount, activeConnectCount, allUserNameList, startTime, createtTime, cpuRate);
            }
        }

        /// <summary>
        /// 获取内存占用大小，单位/字节
        /// </summary>
        /// <returns>内存占用大小</returns>
        private long PrimitiveGetMemorySize(DbConnection dbConnection)
        {
            return 0L;
        }

        /// <summary>
        /// 获取磁盘空间占用大小，单位/字节
        /// </summary>
        /// <returns>磁盘空间占用大小</returns>
        private long PrimitiveGetDiskSize(DbConnection dbConnection)
        {
            string databaseFilePath = this.PrimitiveGetDatabaseName(dbConnection);
            var fileInfo = new FileInfo(databaseFilePath);
            if (fileInfo.Exists)
            {
                return fileInfo.Length;
            }
            else
            {
                return 0L;
            }
        }

        /// <summary>
        /// 获取最大连接数
        /// </summary>
        /// <returns>最大连接数</returns>
        private int PrimitiveGetMaxConnectCount(DbConnection dbConnection)
        {
            var config = base._dbAccess.Config;
            int maxConnectCount = 0;
            if (config.ReadConCount < 0)
            {
                return int.MaxValue;
            }
            else
            {
                maxConnectCount += config.ReadConCount;
            }

            if (config.WriteConCount > 0)
            {
                maxConnectCount += config.WriteConCount;
            }

            return maxConnectCount;
        }

        /// <summary>
        /// 获取连接数和并发连接数
        /// </summary>
        /// <returns>连接数</returns>
        private void PrimitiveGetTotalConnectCountAndConcurrentConnectCount(DbConnection dbConnection, out int totalConnectCount, out int concurrentConnectCount)
        {
            totalConnectCount = -1;
            concurrentConnectCount = -1;
        }

        private List<string> GetAllUserNameList(DbConnection dbConnection)
        {
            return new List<string>();
        }

        /// <summary>
        /// 获取数据库启动时间
        /// </summary>
        /// <returns>数据库启动时间</returns>
        private DateTime PrimitiveGetStartTime(DbConnection dbConnection)
        {
            return Process.GetCurrentProcess().StartTime;
        }

        /// <summary>
        /// 获取数据库创建时间
        /// </summary>
        /// <returns>数据库创建时间</returns>
        private DateTime PrimitiveGetCreatetTime(DbConnection dbConnection)
        {
            string databaseFilePath = this.PrimitiveGetDatabaseName(dbConnection);
            var fileInfo = new FileInfo(databaseFilePath);
            if (fileInfo.Exists)
            {
                return fileInfo.CreationTime;
            }
            else
            {
                return Process.GetCurrentProcess().StartTime;
            }
        }
        #endregion
    }
}
