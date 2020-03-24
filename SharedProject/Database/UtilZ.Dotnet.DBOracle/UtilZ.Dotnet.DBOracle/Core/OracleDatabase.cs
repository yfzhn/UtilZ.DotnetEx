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

namespace UtilZ.Dotnet.DBOracle.Core
{
    internal class OracleDatabase : DatabaseAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbAccess">数据库访问对象</param>
        public OracleDatabase(IDBAccess dbAccess)
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
            string sqlStr = $@"select count(0) from tabs where TABLE_NAME ='{tableName}'";
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
            string sqlStr = $@"SELECT count(0) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{fieldName}'";
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
                                                                            //string sqlStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM {0}) A WHERE ROWNUM <= 1) WHERE RN >=0", tableName);
            string sqlStr = $"SELECT * FROM \"{tableName}\" WHERE ROWNUM < 1";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            var dicFieldDbClrFieldType = base.GetFieldDbClrFieldType(dt.Columns);//字段的公共语言运行时类型字典集合
            Dictionary<string, Type> colDBType = new Dictionary<string, Type>();
            //查询表C#中列信息
            foreach (DataColumn col in dt.Columns)
            {
                colDBType.Add(col.ColumnName, col.DataType);
            }

            sqlStr = $@"select t.COLUMN_NAME,t.DATA_TYPE,t.NULLABLE,t.DATA_DEFAULT,c.COMMENTS from user_tab_columns t,user_col_comments c where t.table_name = c.table_name and t.column_name = c.column_name and t.table_name = '{tableName}'";
            using (IDbCommand cmd = DBAccessEx.CreateCommand(base._dbAccess, con, sqlStr))
            {
                List<DBFieldInfo> colInfos = new List<DBFieldInfo>();
                object value;
                string fieldName;
                string dbTypeName;
                bool allowNull;
                object defaultValue;
                string comments;
                Type type;
                DBFieldType fieldType;

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fieldName = reader.GetString(0);
                        dbTypeName = reader.GetString(1);
                        allowNull = reader.GetString(2).ToUpper().Equals("Y") ? true : false;
                        defaultValue = reader.GetValue(3);
                        value = reader[4];
                        if (value != null)
                        {
                            comments = value.ToString();
                        }
                        else
                        {
                            comments = string.Empty;
                        }

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
            //string sqlStr = @"select cu.* from user_cons_columns cu, user_constraints au where cu.constraint_name = au.constraint_name and au.constraint_type = 'P' and au.table_name = 'PERSON'";
            string sqlStr = $@"select cu.column_name from user_cons_columns cu, user_constraints au where cu.constraint_name = au.constraint_name and au.constraint_type = 'P' and au.table_name = '{tableName}'";
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
            string sqlStr = this.GetQueryTableInfoSql(null);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);

            if (getFieldInfo)
            {
                return this.GetTableInfoListHasGetFieldInfo(dt, con);
            }
            else
            {
                return this.GetTableInfoListHasNotGetFieldInfo(dt, con);
            }
        }

        private List<DBTableInfo> GetTableInfoListHasNotGetFieldInfo(DataTable dt, IDbConnection con)
        {
            var tableInfoList = new List<DBTableInfo>();
            string tableName;

            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                tableInfoList.Add(this.OracleGetTableInfoByName(con, tableName, false, null, row));
            }

            return tableInfoList;
        }

        private List<DBTableInfo> GetTableInfoListHasGetFieldInfo(DataTable dt, IDbConnection con)
        {
            string queryIndexSqlStr = this.GetQueryIndexSql(null);
            DataTable dtIndex = base.PrimitiveQueryDataToDataTable(con, queryIndexSqlStr);
            var indexTupleRowArr = new Tuple<string, DataRow>[dtIndex.Rows.Count];
            for (int i = 0; i < dtIndex.Rows.Count; i++)
            {
                indexTupleRowArr[i] = new Tuple<string, DataRow>(DBAccessEx.ConvertObject<string>(dtIndex.Rows[i]["TABLE_NAME"]), dtIndex.Rows[i]);
            }

            DataRow[] indexArr;
            var tableInfoList = new List<DBTableInfo>();
            string tableName;
            DBIndexInfoCollection indexInfoCollection = null;

            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                indexArr = indexTupleRowArr.Where(t => { return string.Equals(t.Item1, tableName); }).Select(t => { return t.Item2; }).ToArray();
                indexInfoCollection = this.ConvertTableIndexs(tableName, dtIndex.Columns, indexArr);
                tableInfoList.Add(this.OracleGetTableInfoByName(con, tableName, true, indexInfoCollection, row));
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
            string sqlStr = this.GetQueryTableInfoSql(tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return this.OracleGetTableInfoByName(con, tableName, getFieldInfo, indexInfoCollection, dt.Rows[0]);
        }

        private string GetQueryTableInfoSql(string tableName)
        {
            string sqlStr;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                sqlStr = $@"SELECT TABLE_NAME,COMMENTS FROM USER_TAB_COMMENTS WHERE TABLE_TYPE='TABLE'";
            }
            else
            {
                sqlStr = $@"SELECT TABLE_NAME,COMMENTS FROM USER_TAB_COMMENTS WHERE TABLE_TYPE='TABLE' AND TABLE_NAME='{tableName}'";
            }

            return sqlStr;
        }

        private DBTableInfo OracleGetTableInfoByName(IDbConnection con, string tableName, bool getFieldInfo, DBIndexInfoCollection indexInfoCollection, DataRow row)
        {
            string comments;//备注
            List<DBFieldInfo> colInfos = null;//字段集合
            IEnumerable<DBFieldInfo> priKeyColInfos = null;//主键列字段集合

            object tmpValue = row[1];
            if (tmpValue != null && tmpValue != DBNull.Value)
            {
                comments = tmpValue.ToString();
            }
            else
            {
                comments = null;
            }

            if (getFieldInfo)
            {
                //获取字段信息
                colInfos = this.PrimitiveGetTableFieldInfo(con, tableName);//获取表所有字段集合
                priKeyColInfos = from col in colInfos where col.IsPriKey select col;//获取主键列字段集合
            }

            return new DBTableInfo(tableName, comments, new DBFieldInfoCollection(colInfos),
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
            string sqlStr = this.GetQueryIndexSql(tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);

            DataRow[] rowArr = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rowArr, 0);
            return this.ConvertTableIndexs(tableName, dt.Columns, rowArr);
        }

        private string GetQueryIndexSql(string tableName)
        {
            string queryIndexSqlStr;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                queryIndexSqlStr = @"SELECT A.TABLE_NAME AS TABLE_NAME,A.INDEX_NAME AS INDEX_NAME,B.COLUMN_NAME AS COLUMN_NAME,B.DESCEND,A.INDEX_TYPE,A.TABLE_OWNER,A.TABLE_NAME,A.TABLE_TYPE,A.UNIQUENESS,A.COMPRESSION,A.PREFIX_LENGTH,A.TABLESPACE_NAME,A.INI_TRANS,A.MAX_TRANS,A.INITIAL_EXTENT,A.NEXT_EXTENT,A.MIN_EXTENTS,A.MAX_EXTENTS,A.PCT_INCREASE,A.PCT_THRESHOLD,A.INCLUDE_COLUMN,A.FREELISTS,A.FREELIST_GROUPS,A.PCT_FREE,A.LOGGING,A.BLEVEL,A.LEAF_BLOCKS,A.DISTINCT_KEYS,A.AVG_LEAF_BLOCKS_PER_KEY,A.AVG_DATA_BLOCKS_PER_KEY,A.CLUSTERING_FACTOR,A.STATUS,A.NUM_ROWS,A.SAMPLE_SIZE,A.LAST_ANALYZED,A.DEGREE,A.INSTANCES,A.PARTITIONED,A.TEMPORARY,A.GENERATED,A.SECONDARY,A.BUFFER_POOL,A.FLASH_CACHE,A.CELL_FLASH_CACHE,A.USER_STATS,A.DURATION,A.PCT_DIRECT_ACCESS,A.ITYP_OWNER,A.ITYP_NAME,A.PARAMETERS,A.GLOBAL_STATS,A.DOMIDX_STATUS,A.DOMIDX_OPSTATUS,A.FUNCIDX_STATUS,A.JOIN_INDEX,A.IOT_REDUNDANT_PKEY_ELIM,A.DROPPED,A.VISIBILITY,A.DOMIDX_MANAGEMENT,A.SEGMENT_CREATED
FROM user_indexes A inner join user_ind_columns B on A.INDEX_NAME=B.INDEX_NAME";
            }
            else
            {
                queryIndexSqlStr = $@"SELECT A.TABLE_NAME AS TABLE_NAME,A.INDEX_NAME AS INDEX_NAME,B.COLUMN_NAME AS COLUMN_NAME,B.DESCEND,A.INDEX_TYPE,A.TABLE_OWNER,A.TABLE_NAME,A.TABLE_TYPE,A.UNIQUENESS,A.COMPRESSION,A.PREFIX_LENGTH,A.TABLESPACE_NAME,A.INI_TRANS,A.MAX_TRANS,A.INITIAL_EXTENT,A.NEXT_EXTENT,A.MIN_EXTENTS,A.MAX_EXTENTS,A.PCT_INCREASE,A.PCT_THRESHOLD,A.INCLUDE_COLUMN,A.FREELISTS,A.FREELIST_GROUPS,A.PCT_FREE,A.LOGGING,A.BLEVEL,A.LEAF_BLOCKS,A.DISTINCT_KEYS,A.AVG_LEAF_BLOCKS_PER_KEY,A.AVG_DATA_BLOCKS_PER_KEY,A.CLUSTERING_FACTOR,A.STATUS,A.NUM_ROWS,A.SAMPLE_SIZE,A.LAST_ANALYZED,A.DEGREE,A.INSTANCES,A.PARTITIONED,A.TEMPORARY,A.GENERATED,A.SECONDARY,A.BUFFER_POOL,A.FLASH_CACHE,A.CELL_FLASH_CACHE,A.USER_STATS,A.DURATION,A.PCT_DIRECT_ACCESS,A.ITYP_OWNER,A.ITYP_NAME,A.PARAMETERS,A.GLOBAL_STATS,A.DOMIDX_STATUS,A.DOMIDX_OPSTATUS,A.FUNCIDX_STATUS,A.JOIN_INDEX,A.IOT_REDUNDANT_PKEY_ELIM,A.DROPPED,A.VISIBILITY,A.DOMIDX_MANAGEMENT,A.SEGMENT_CREATED
FROM user_indexes A inner join user_ind_columns B on A.INDEX_NAME=B.INDEX_NAME WHERE A.TABLE_NAME='{tableName}'";
            }

            return queryIndexSqlStr;
        }

        private DBIndexInfoCollection ConvertTableIndexs(string tableName, DataColumnCollection cols, DataRow[] rowArr)
        {
            IEnumerable<IGrouping<string, DataRow>> indexGroups = rowArr.GroupBy(t => { return DBAccessEx.ConvertObject<string>(t["INDEX_NAME"]); });
            var indexinfoList = new List<DBIndexInfo>();
            string indexName, fieldName;
            StringBuilder sbDetail = new StringBuilder();
            DataRow row;
            string[] fieldArr;

            foreach (var indexGroup in indexGroups)
            {
                indexName = indexGroup.Key;

                row = indexGroup.First();
                fieldArr = indexGroup.Select(t => { return DBAccessEx.ConvertObject<string>(t["COLUMN_NAME"]); }).ToArray();
                fieldName = string.Join(",", fieldArr);

                sbDetail.Clear();
                for (int i = 3; i < cols.Count; i++)
                {
                    sbDetail.AppendLine($"[{cols[i].ColumnName}:{DBAccessEx.ConvertObject<string>(row[cols[i]])}]");
                }

                indexinfoList.Add(new DBIndexInfo(tableName, indexName, fieldName, sbDetail.ToString()));
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
            /***********************************************************************************
             * SQL:SELECT * FROM v$version
             * Oracle Database 11g Enterprise Edition Release 11.2.0.1.0 - 64bit Production
             * PL/SQL Release 11.2.0.1.0 - Production
             * CORE	11.2.0.1.0	Production
             * TNS for 64-bit Windows: Version 11.2.0.1.0 - Production
             * NLSRTL Version 11.2.0.1.0 - Production
             * ------------------------------------------------------------------------------------
             * SQL:SELECT * FROM product_component_version
             * NLSRTL                                   11.2.0.1.0  Production 
             * Oracle Database 11g Enterprise Edition   11.2.0.1.0  64bit Production
             * PL/SQL                                   11.2.0.1.0  Production
             * TNS for 64-bit Windows:                  11.2.0.1.0  Production
             ***********************************************************************************/

            const string sqlStr = @"SELECT * FROM product_component_version";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);

            string dataBaseVersion = dt.Rows[0][1].ToString();//11.2.0.1.0
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
            const string sqlStr = @"select current_date from dual";
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
                const string sqlStr = @"select user from dual";
                object obj = base.PrimitiveExecuteScalar(con, sqlStr);
                userName = obj.ToString();
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
                const string queryDatabaseNameSqlStr = @"select instance_name from  V$instance";
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
                int totalConnectCount = this.PrimitiveGetTotalConnectCount(dbConnection);
                int concurrentConnectCount, activeConnectCount;
                this.PrimitiveGetTotalConnectCountAndConcurrentConnectCount(dbConnection, out concurrentConnectCount, out activeConnectCount);

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

                return new DatabasePropertyInfo(memorySize, diskSize, maxConnectCount, totalConnectCount,
                    concurrentConnectCount, activeConnectCount, allUserNameList, startTime, createtTime, cpuRate);
            }
        }

        /// <summary>
        /// 获取内存占用大小，单位/字节
        /// </summary>
        /// <returns>内存占用大小</returns>
        private long PrimitiveGetMemorySize(DbConnection dbConnection)
        {
            const string sqlStr = @"select sum(value) from v$sga";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<long>(obj);
        }

        /// <summary>
        /// 获取磁盘空间占用大小，单位/字节
        /// </summary>
        /// <returns>磁盘空间占用大小</returns>
        private long PrimitiveGetDiskSize(DbConnection dbConnection)
        {
            object obj;
            string userName = this.PrimitiveGetLoginUserName(dbConnection);
            string sqlStr = $@"select SUM(bytes) from dba_segments where owner='{userName}'";
            obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<long>(obj);
        }

        /// <summary>
        /// 获取最大连接数
        /// </summary>
        /// <returns>最大连接数</returns>
        private int PrimitiveGetMaxConnectCount(DbConnection dbConnection)
        {
            const string sqlStr = @"select value from v$parameter where name ='processes'";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<int>(obj);
        }

        /// <summary>
        /// 获取总连接数
        /// </summary>
        /// <returns>总连接数</returns>
        private int PrimitiveGetTotalConnectCount(DbConnection dbConnection)
        {
            const string sqlStr = @"select count(*) from v$process";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<int>(obj);
        }

        /// <summary>
        /// 获取连接数和并发连接数
        /// </summary>
        /// <returns>连接数</returns>
        private void PrimitiveGetTotalConnectCountAndConcurrentConnectCount(DbConnection dbConnection, out int concurrentConnectCount, out int activeConnectCount)
        {
            const string sqlStr = @"SELECT STATUS FROM v$session";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, sqlStr);
            concurrentConnectCount = dt.Rows.Count;
            activeConnectCount = dt.Select("STATUS='ACTIVE'").Length;
        }

        private List<string> GetAllUserNameList(DbConnection dbConnection)
        {
            const string sqlStr = @"select USERNAME from all_users";
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
            string dataBaseName = this.PrimitiveGetDatabaseName(dbConnection);
            dataBaseName = dataBaseName.ToLower();//此处居然要用小写,Oracle果然诡异
            string queryStartTimeSqlStr = $@"select STARTUP_TIME from sys.v_$instance WHERE INSTANCE_NAME='{dataBaseName}'";
            object obj = base.PrimitiveExecuteScalar(dbConnection, queryStartTimeSqlStr);
            DateTime startTime = DBAccessEx.ConvertObject<DateTime>(obj);
            return startTime;
        }

        /// <summary>
        /// 获取数据库创建时间
        /// </summary>
        /// <returns>数据库创建时间</returns>
        private DateTime PrimitiveGetCreatetTime(DbConnection dbConnection)
        {
            const string sqlStr = @"SELECT MIN(CREATED) FROM user_objects";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            DateTime createtTime = DBAccessEx.ConvertObject<DateTime>(obj);
            return createtTime;
        }
        #endregion
    }
}
