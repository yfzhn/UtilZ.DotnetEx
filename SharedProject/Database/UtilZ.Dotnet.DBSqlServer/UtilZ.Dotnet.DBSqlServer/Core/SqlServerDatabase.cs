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

namespace UtilZ.Dotnet.DBSqlServer.Core
{
    internal class SqlServerDatabase : DatabaseAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbAccess">数据库访问对象</param>
        public SqlServerDatabase(IDBAccess dbAccess)
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
            //string sqlStr =@"select COUNT(0) from sysobjects where id = object_id('表名') and type = 'u'";
            //string sqlStr = @"select COUNT(0) from sys.tables where name='表名' and type = 'u';";
            string sqlStr = $@"select COUNT(0) from sys.tables where name='{tableName}' and type = 'u'";
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
            string sqlStr = $@"select count(0) from syscolumns where name='{fieldName}' and objectproperty(id,'IsUserTable')=1 and object_name(id)='{tableName}'";
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
            string sqlStr = string.Format("SELECT TOP 0 *  FROM {0}", tableName);
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            var dicFieldDbClrFieldType = base.GetFieldDbClrFieldType(dt.Columns);//字段的公共语言运行时类型字典集合
            Dictionary<string, Type> colDBType = new Dictionary<string, Type>();
            foreach (DataColumn col in dt.Columns)
            {
                colDBType.Add(col.ColumnName, col.DataType);
            }

            using (IDbCommand cmd = DBAccessEx.CreateCommand(base._dbAccess, con))
            {
                cmd.CommandText = string.Format(@"SELECT  C.name as [字段名]
	                                                    ,T.name as [字段类型]
                                                        ,convert(bit,C.IsNullable)  as [可否为空]
                                                        ,convert(bit,case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=c.id and name in (
                                                            SELECT name FROM sysindexes WHERE indid in(
                                                                SELECT indid FROM sysindexkeys WHERE id = c.id AND colid=c.colid))) then 1 else 0 end) 
                                                                    as [是否主键]
                                                        ,convert(bit,COLUMNPROPERTY(c.id,c.name,'IsIdentity')) as [自动增长]
                                                        ,C.Length as [占用字节] 
                                                        ,COLUMNPROPERTY(C.id,C.name,'PRECISION') as [长度]
                                                        ,isnull(COLUMNPROPERTY(c.id,c.name,'Scale'),0) as [小数位数]
                                                        ,ISNULL(CM.text,'') as [默认值]
                                                        ,isnull(ETP.value,'') AS [字段描述]
                                                        --,ROW_NUMBER() OVER (ORDER BY C.name) AS [Row]
                                                FROM syscolumns C
                                                INNER JOIN systypes T ON C.xusertype = T.xusertype 
                                                left JOIN sys.extended_properties ETP   ON  ETP.major_id = c.id AND ETP.minor_id = C.colid AND ETP.name ='MS_Description' 
                                                left join syscomments CM on C.cdefault=CM.id
                                                WHERE C.id = object_id('{0}')", tableName);

                List<DBFieldInfo> colInfos = new List<DBFieldInfo>();
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    string fieldName;
                    string dbTypeName;
                    bool allowNull;
                    object defaultValue;
                    string comments;
                    Type type;
                    DBFieldType fieldType;

                    while (reader.Read())
                    {
                        fieldName = reader.GetString(0);
                        dbTypeName = reader.GetString(1);
                        allowNull = reader.GetBoolean(2);
                        defaultValue = reader.GetValue(8);
                        comments = reader.GetString(9);
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
            //string sqlStr = @"select column_name as primarykey from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where Table_name='Person' and constraint_name like 'PK_%'";
            string sqlStr = string.Format(@"select column_name as primarykey from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where Table_name='{0}'", tableName);
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
            //string sqlStr= @"select name from sysobjects where xtype='u'";
            string sqlStr = @"select c.name,cast(isnull(f.[value], '') as nvarchar(100)) as remark from sys.objects c left join sys.extended_properties f on f.major_id=c.object_id and f.minor_id=0 and f.class=1 where c.type='u'";
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
                tableInfoList.Add(this.PrimitiveGetTableInfoByName(con, tableName, false, null));
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
                indexTupleRowArr[i] = new Tuple<string, DataRow>(DBAccessEx.ConvertObject<string>(dtIndex.Rows[i]["TableName"]), dtIndex.Rows[i]);
            }

            DataRow[] indexArr;
            var tableInfoList = new List<DBTableInfo>();
            string tableName;
            DBIndexInfoCollection indexInfoCollection = null;

            foreach (DataRow row in dt.Rows)
            {
                tableName = row[0].ToString();
                indexArr = indexTupleRowArr.Where(t => { return string.Equals(t.Item1, tableName); }).Select(t => { return t.Item2; }).ToArray();
                indexInfoCollection = this.ConvertTableIndexs(tableName, indexArr);
                tableInfoList.Add(this.PrimitiveGetTableInfoByName(con, tableName, true, indexInfoCollection));
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
            //      string sqlStr = @"select name from sysobjects where xtype='u'";
            string sqlStr = string.Format(@"select c.name,cast(isnull(f.[value], '') as nvarchar(100)) as remark from sys.objects c left join sys.extended_properties f on f.major_id=c.object_id and f.minor_id=0 and f.class=1 where c.type='u' and c.name='{0}'", tableName);
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
                    colInfos = null;
                    priKeyColInfos = null;
                }

                return new DBTableInfo(tableName, comments, new DBFieldInfoCollection(colInfos),
                    new DBFieldInfoCollection(priKeyColInfos), indexInfoCollection);
            }

            throw new ArgumentException($"未查询到表[{tableName}]信息");
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
            return this.ConvertTableIndexs(tableName, rowArr);
        }

        private string GetQueryIndexSql(string tableName)
        {
            /*
           string sqlStr = $@"select 
i.name as IndexName,
o.name as TableName,
ic.key_ordinal as ColumnOrder,
co.[name] as ColumnName
from sys.indexes i 
join sys.objects o on i.object_id=o.object_id
join sys.index_columns ic on ic.object_id=i.object_id and ic.index_id=i.index_id
join sys.columns co on co.object_id=i.object_id and co.column_id=ic.column_id
where
o.name='Stu'
and o.[type]='U'
and ic.is_included_column=0
order by o.[name],i.[name],ic.is_included_column,ic.key_ordinal";*/

            string queryIndexSqlStr;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                queryIndexSqlStr = @"select 
i.name as IndexName,
ic.key_ordinal as ColumnOrder,
co.[name] as ColumnName,
o.name as TableName
from sys.indexes i 
join sys.objects o on i.object_id=o.object_id
join sys.index_columns ic on ic.object_id=i.object_id and ic.index_id=i.index_id
join sys.columns co on co.object_id=i.object_id and co.column_id=ic.column_id
where
o.[type]='U'
and ic.is_included_column=0
order by o.[name],i.[name],ic.is_included_column,ic.key_ordinal";
            }
            else
            {
                queryIndexSqlStr = $@"select 
i.name as IndexName,
ic.key_ordinal as ColumnOrder,
co.[name] as ColumnName
from sys.indexes i 
join sys.objects o on i.object_id=o.object_id
join sys.index_columns ic on ic.object_id=i.object_id and ic.index_id=i.index_id
join sys.columns co on co.object_id=i.object_id and co.column_id=ic.column_id
where
o.name='{tableName}'
and o.[type]='U'
and ic.is_included_column=0
order by o.[name],i.[name],ic.is_included_column,ic.key_ordinal";
            }

            return queryIndexSqlStr;
        }

        private DBIndexInfoCollection ConvertTableIndexs(string tableName, DataRow[] rowArr)
        {
            IEnumerable<IGrouping<string, DataRow>> indexGroups = rowArr.GroupBy(t => { return DBAccessEx.ConvertObject<string>(t["IndexName"]); });
            string indexName, fieldName, detail;
            DataRow row;
            string[] fieldArr;
            var indexinfoList = new List<DBIndexInfo>();

            foreach (var indexGroup in indexGroups)
            {
                row = indexGroup.First();
                indexName = indexGroup.Key;
                detail = $"[ColumnOrder:{DBAccessEx.ConvertObject<string>(row["ColumnOrder"])}]";

                fieldArr = indexGroup.Select(t => { return DBAccessEx.ConvertObject<string>(t["ColumnName"]); }).ToArray();
                fieldName = string.Join(",", fieldArr);

                indexinfoList.Add(new DBIndexInfo(tableName, indexName, fieldName, detail));
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
            const string sqlStr = @"select @@version";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            string dataBaseVersion = DBAccessEx.ConvertObject<string>(value);

            //dataBaseVersion:Microsoft SQL Server 2008 R2 (RTM) - 10.50.1600.1 (X64)   Apr  2 2010 15:48:46   Copyright (c) Microsoft Corporation  Data Center Edition (64-bit) on Windows NT 6.1 <X64> (Build 7600: )
            /*******************************************************************************************************************
             * dataBaseVersion
             * Microsoft SQL Server 2008 R2 (RTM) - 10.50.1600.1 (X64)   
             * Apr  2 2010 15:48:46   
             * Copyright (c) Microsoft Corporation  Data Center Edition (64-bit) on Windows NT 6.1 <X64> (Build 7600: )
             *******************************************************************************************************************/
            const string startIndex = "Microsoft SQL Server ";
            string str = dataBaseVersion.Substring(startIndex.Length);
            string verStr = str.Substring(0, str.IndexOf(' '));
            int version;
            if (!int.TryParse(verStr, out version))
            {
                version = 2008;
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
            const string sqlStr = @"select GETDATE()";
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
                const string sqlStr = @"SELECT SUSER_SNAME()";
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
                const string queryDatabaseNameSqlStr = @"Select Name From Master..SysDataBases Where DbId=(Select Dbid From Master..SysProcesses Where Spid = @@spid)";
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
            const string sqlStr = @"SELECT physical_memory_in_use_kb FROM sys.dm_os_process_memory";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<long>(obj) * 1024;
        }

        /// <summary>
        /// 获取磁盘空间占用大小，单位/字节
        /// </summary>
        /// <returns>磁盘空间占用大小</returns>
        private long PrimitiveGetDiskSize(DbConnection dbConnection)
        {
            long size;
            const string sqlStr = @"Exec sp_spaceused";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, sqlStr);
            object obj = dt.Rows[0][1];
            string mbSizeStr = obj.ToString();
            int index = mbSizeStr.IndexOf(' ');
            if (index > 0)
            {
                string str = mbSizeStr.Substring(0, index);
                double tmp = double.Parse(str);
                tmp = tmp * 1024 * 1024;
                size = (long)tmp;
            }
            else
            {
                size = 0;
            }

            return size;
        }

        /// <summary>
        /// 获取最大连接数
        /// </summary>
        /// <returns>最大连接数</returns>
        private int PrimitiveGetMaxConnectCount(DbConnection dbConnection)
        {
            const string sqlStr = @"select @@max_connections";
            //select value from master.dbo.sysconfigures where [config] = 103--0 表示不限制,最大值是32767
            //select* from master.dbo.sysconfigures(高级配置 - 比如数据库最大使用内存大小等通过修改此表中的值达到目的)
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            return DBAccessEx.ConvertObject<int>(obj);
        }

        /// <summary>
        /// 获取总连接数和并发连接数
        /// </summary>
        /// <returns>连接数</returns>
        private void PrimitiveGetTotalConnectCountAndConcurrentConnectCount(DbConnection dbConnection, out int totalConnectCount, out int concurrentConnectCount)
        {
            string databaseName = this.PrimitiveGetDatabaseName(dbConnection);
            string sqlStr = $@"SELECT status FROM [Master].[dbo].[SYSPROCESSES] WHERE [DBID] IN (SELECT  [DBID] FROM [Master].[dbo].[SYSDATABASES] WHERE NAME='{databaseName}')";
            DataTable dt = base.PrimitiveQueryDataToDataTable(dbConnection, sqlStr);
            totalConnectCount = dt.Rows.Count;
            concurrentConnectCount = totalConnectCount - dt.Select("status='runnable'").Length;
        }


        private List<string> GetAllUserNameList(DbConnection dbConnection)
        {
            const string sqlStr = @"select name from sysusers";
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
            const string sqlStr = @"SELECT sqlserver_start_time FROM sys.dm_os_sys_info";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            DateTime startTime = DBAccessEx.ConvertObject<DateTime>(obj);
            return startTime;
        }

        /// <summary>
        /// 获取数据库创建时间
        /// </summary>
        /// <returns>数据库创建时间</returns>
        private DateTime PrimitiveGetCreatetTime(DbConnection dbConnection)
        {
            string dataBaseName = this.PrimitiveGetDatabaseName(dbConnection);
            string sqlStr = $@"Select create_date From sys.databases where name='{dataBaseName}'";
            object obj = base.PrimitiveExecuteScalar(dbConnection, sqlStr);
            DateTime createtTime = DBAccessEx.ConvertObject<DateTime>(obj);
            return createtTime;
        }
        #endregion
    }
}
