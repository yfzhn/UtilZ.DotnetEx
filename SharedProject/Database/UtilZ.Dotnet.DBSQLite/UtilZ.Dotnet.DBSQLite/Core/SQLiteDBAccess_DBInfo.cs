using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.Core;
using System.Data;
using UtilZ.Dotnet.DBIBase.Model;
using System.Collections;

namespace UtilZ.Dotnet.DBSQLite.Core
{
    internal partial class SQLiteDBAccess
    {
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
            string sqlStr = @"select tbl_name,'' from sqlite_master where type='table'";
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
            string queryTableNameSqlStr = @"select name from sqlite_master where type = 'table'";
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
            string sqlStr = @"select tbl_name from sqlite_master where type='table'";
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
            string sqlStr = @"select sqlite_version()";
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
            string sqlStr = @"select datetime('now','localtime')";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            return DBAccessEx.ConvertObject<DateTime>(value);
        }
    }
}
