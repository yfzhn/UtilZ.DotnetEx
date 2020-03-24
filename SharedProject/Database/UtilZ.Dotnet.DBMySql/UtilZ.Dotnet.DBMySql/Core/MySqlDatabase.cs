using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBMySql.Core
{
    internal class MySqlDatabase : DatabaseAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbAccess">数据库访问对象</param>
        public MySqlDatabase(IDBAccess dbAccess)
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
            string sqlStr = $@"select Count(0) from INFORMATION_SCHEMA.TABLES where TABLE_NAME='{tableName}' AND TABLE_SCHEMA='{con.Database}'";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            return DBAccessEx.ConvertObject<int>(value) > 0;
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
            string sqlStr = $@"select count(0) from information_schema.columns WHERE table_name ='{tableName}' and column_name='{fieldName}'";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            return DBAccessEx.ConvertObject<int>(value) > 0;
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
            var priKeyCols = this.PrimitiveQueryPriKeyField(con, tableName);//主键列名集合
            string sqlStr = $"SELECT * FROM {tableName} limit 0,0";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            var dicFieldDbClrFieldType = this.GetFieldDbClrFieldType(dt.Columns);//字段的公共语言运行时类型字典集合

            //查询表C#中列信息,从空表中获得
            Dictionary<string, Type> colDBType = new Dictionary<string, Type>();
            foreach (DataColumn col in dt.Columns)
            {
                colDBType.Add(col.ColumnName, col.DataType);
            }

            using (IDbCommand cmd = DBAccessEx.CreateCommand(base._dbAccess, con))
            {
                cmd.CommandText = $@"SHOW FULL FIELDS FROM {tableName}";
                List<DBFieldInfo> colInfos = new List<DBFieldInfo>();

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    object value;
                    string fieldName;
                    string dbTypeName;
                    bool allowNull;
                    object defaultValue;
                    string comments;
                    Type type;
                    DBFieldType fieldType;

                    while (reader.Read())
                    {
                        fieldName = reader["Field"].ToString();
                        dbTypeName = reader["Type"].ToString();
                        allowNull = reader["Null"].ToString().ToUpper().Equals("NO") ? false : true;
                        value = reader["Default"];
                        defaultValue = DBNull.Value.Equals(value) ? null : value;
                        comments = reader["Comment"].ToString();
                        type = colDBType[fieldName];
                        fieldType = dicFieldDbClrFieldType[fieldName];
                        colInfos.Add(new DBFieldInfo(tableName, fieldName, dbTypeName, type, comments, defaultValue, allowNull, fieldType, priKeyCols.Contains(fieldName)));
                    }
                }

                return colInfos;
            }
        }

        /// <summary>
        /// 查询主键列名集合
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="tableName">表名</param>
        /// <returns>主键列名集合</returns>
        protected override List<string> PrimitiveQueryPriKeyField(IDbConnection con, string tableName)
        {
            string sqlStr = string.Format(@"select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where table_name='{0}' AND COLUMN_KEY='PRI'", tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            List<string> priKeyCols = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                priKeyCols.Add(row[0].ToString());
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
            //string sqlStr = $@"select TABLE_NAME,TABLE_COMMENT from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='{con.Database}'";
            const string sqlStr = @"SHOW TABLES";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);

            var tableInfoList = new List<DBTableInfo>();
            string tableName;
            DBIndexInfoCollection indexInfoCollection = null;

            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                if (getFieldInfo)
                {
                    indexInfoCollection = this.PrimitiveGetTableIndexs(con, tableName);
                }

                tableInfoList.Add(this.PrimitiveGetTableInfoByName(con, tableName, getFieldInfo, indexInfoCollection));
            }

            return tableInfoList;
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
            string sqlStr = $@"select TABLE_NAME,TABLE_COMMENT from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='{con.Database}' AND TABLE_NAME = '{tableName}'";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            string comments;//备注
            object tmpValue = null;//临时变量
            List<DBFieldInfo> colInfos = null;//字段集合
            IEnumerable<DBFieldInfo> priKeyColInfos = null;//主键列字段集合
            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                tmpValue = row[1];
                if (tmpValue != null && tmpValue != DBNull.Value)
                {
                    comments = tmpValue.ToString();
                }
                else
                {
                    comments = null;
                }

                if (getFieldInfo)//获取字段信息
                {
                    colInfos = this.PrimitiveGetTableFieldInfo(con, tableName);//获取表所有字段集合
                    priKeyColInfos = from col in colInfos where col.IsPriKey select col;//获取主键列字段集合
                }
                else//不获取字段信息
                {
                    colInfos = new List<DBFieldInfo>();
                    priKeyColInfos = new List<DBFieldInfo>();
                }

                return new DBTableInfo(tableName, comments, new DBFieldInfoCollection(colInfos),
                    new DBFieldInfoCollection(priKeyColInfos), indexInfoCollection);
            }

            return null;
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
            var indexinfoList = new List<DBIndexInfo>();
            string sqlStr = $"show index from {tableName}";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);

            if (dt != null && dt.Rows.Count > 0)
            {
                /*****************************************************************************************************************************
                * Table    Non_unique Key_name    Seg_in_index    Column_name Collation  Cardinality  Sub_part    Packed  Null    Index_type  Comment Index_comment
                * stu      0          PRIMARY     1               ID          A          1075                                     BTREE             
                * 
                *****************************************************************************************************************************/

                DataRow[] rowArr = new DataRow[dt.Rows.Count];
                dt.Rows.CopyTo(rowArr, 0);
                IEnumerable<IGrouping<string, DataRow>> indexGroups = rowArr.GroupBy(t => { return DBAccessEx.ConvertObject<string>(t["Key_name"]); });

                string indexName, fieldName;
                StringBuilder sbDetail = new StringBuilder();
                DataRow row;
                string[] fieldArr;

                foreach (var indexGroup in indexGroups)
                {
                    row = indexGroup.First();
                    sbDetail.Clear();
                    indexName = indexGroup.Key;

                    sbDetail.AppendLine($"[Non_unique:{DBAccessEx.ConvertObject<string>(row["Non_unique"])}];");
                    sbDetail.AppendLine($"[Seg_in_index:{DBAccessEx.ConvertObject<string>(row["Seq_in_index"])}];");
                    sbDetail.AppendLine($"[Collation:{DBAccessEx.ConvertObject<string>(row["Collation"])}];");
                    sbDetail.AppendLine($"[Cardinality:{DBAccessEx.ConvertObject<string>(row["Cardinality"])}];");

                    sbDetail.AppendLine($"[Sub_part:{DBAccessEx.ConvertObject<string>(row["Sub_part"])}];");
                    sbDetail.AppendLine($"[Packed:{DBAccessEx.ConvertObject<string>(row["Packed"])}];");
                    sbDetail.AppendLine($"[Null:{DBAccessEx.ConvertObject<string>(row["Null"])}];");
                    sbDetail.AppendLine($"[Index_type:{DBAccessEx.ConvertObject<string>(row["Index_type"])}];");
                    sbDetail.AppendLine($"[Comment:{DBAccessEx.ConvertObject<string>(row["Comment"])}];");
                    sbDetail.AppendLine($"[Index_comment:{DBAccessEx.ConvertObject<string>(row["Index_comment"])}];");

                    fieldArr = indexGroup.Select(t => { return DBAccessEx.ConvertObject<string>(t["Column_name"]); }).ToArray();
                    fieldName = string.Join(",", fieldArr);

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
            const string sqlStr = @"select version()";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            string dataBaseVersion = DBAccessEx.ConvertObject<string>(value);//5.6.25-log
            string verStr = dataBaseVersion.Substring(0, dataBaseVersion.IndexOf('.'));
            int version;
            if (!int.TryParse(verStr, out version))
            {
                version = 0;
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
            const string sqlStr = @"select CURRENT_TIMESTAMP()";
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
            string userName;
            if (string.IsNullOrWhiteSpace(base._dbAccess.Config.Account))
            {
                const string sqlStr = @"select user()";
                object obj = base.PrimitiveExecuteScalar(con, sqlStr);
                userName = obj.ToString();
                int index = userName.IndexOf('@');
                if (index > 0)
                {
                    userName = userName.Substring(0, index);
                }

                base._dbAccess.Config.Account = userName;
            }
            else
            {
                userName = base._dbAccess.Config.Account;
            }

            return userName;
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
                const string queryDatabaseNameSqlStr = @"select database()";
                object obj = base.PrimitiveExecuteScalar(con, queryDatabaseNameSqlStr);
                databaseName = obj.ToString();
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
            const string sqlStr = @"SELECT (@@key_buffer_size + @@innodb_buffer_pool_size + @@query_cache_size + @@tmp_table_size) AS result";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<long>(obj);
        }

        /// <summary>
        /// 获取磁盘空间占用大小，单位/字节
        /// </summary>
        /// <returns>磁盘空间占用大小</returns>
        private long PrimitiveGetDiskSize(DbConnection dbConnection)
        {
            string dataBaseName = this.PrimitiveGetDatabaseName(dbConnection);
            string sqlStr = $@"SELECT sum(DATA_LENGTH+INDEX_LENGTH) FROM information_schema.TABLES WHERE TABLE_SCHEMA='{dataBaseName}'";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<long>(obj);
        }

        /// <summary>
        /// 获取最大连接数
        /// </summary>
        /// <returns>最大连接数</returns>
        private int PrimitiveGetMaxConnectCount(DbConnection dbConnection)
        {
            const string sqlStr = @"show variables like 'max_connections'";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, sqlStr);
            object obj = dt.Rows[0][1];
            return DBAccessEx.ConvertObject<int>(obj);
        }

        /// <summary>
        /// 获取连接数和并发连接数
        /// </summary>
        /// <returns>连接数</returns>
        private void PrimitiveGetTotalConnectCountAndConcurrentConnectCount(DbConnection dbConnection, out int totalConnectCount, out int concurrentConnectCount)
        {
            const string sqlStr = @"show full processlist";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, sqlStr);
            totalConnectCount = dt.Rows.Count;
            concurrentConnectCount = totalConnectCount - dt.Select("Command='Sleep'").Length;
        }

        private List<string> GetAllUserNameList(DbConnection dbConnection)
        {
            const string sqlStr = @"select user from mysql.user";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, sqlStr);
            List<string> allUserNameList = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                allUserNameList.Add(row[0].ToString());
            }

            return allUserNameList;
        }

        /// <summary>
        /// 获取数据库启动时间
        /// </summary>
        /// <returns>数据库启动时间</returns>
        private DateTime PrimitiveGetStartTime(DbConnection dbConnection)
        {
            //原理:当前时间减去数据库运行时长,则得到数据库启动时间
            const string queryRunDurationSqlStr = @"show global status like 'uptime'";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, queryRunDurationSqlStr);
            object obj = dt.Rows[0][1];
            double duration = DBAccessEx.ConvertObject<double>(obj);//数据库启动后运行时长，单位为秒            

            DateTime currentTime = this.PrimitiveGetDataBaseSysTime(dbConnection);
            duration = 0 - duration;
            DateTime startTime = currentTime.AddSeconds(duration);
            return startTime;
        }

        /// <summary>
        /// 获取数据库创建时间
        /// </summary>
        /// <returns>数据库创建时间</returns>
        private DateTime PrimitiveGetCreatetTime(DbConnection dbConnection)
        {
            string dataBaseName = this.PrimitiveGetDatabaseName(dbConnection);
            string sqlStr = $@"select MIN(CREATE_TIME) from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='{dataBaseName}'";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            DateTime createtTime = DBAccessEx.ConvertObject<DateTime>(obj);
            return createtTime;
        }
        #endregion
    }
}
