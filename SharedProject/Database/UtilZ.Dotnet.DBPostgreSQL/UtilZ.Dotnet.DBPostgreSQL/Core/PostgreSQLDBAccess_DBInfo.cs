using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.DBIBase.Core;
using System.Collections;

namespace UtilZ.Dotnet.DBPostgreSQL.Core
{
    /// <summary>
    /// PostgreSQLDBAccess_DBInfo
    /// </summary>
    internal partial class PostgreSQLDBAccess
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
            string sqlStr = $@"select count(0) from pg_tables where schemaname='public' and tablename = '{tableName}'";
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
            string sqlStr = $"SELECT * FROM {tableName} limit 0 offset 0";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            var dicFieldDbClrFieldType = this.GetFieldDbClrFieldType(dt.Columns);//字段的公共语言运行时类型字典集合

            //查询表C#中列信息,从空表中获得
            Dictionary<string, Type> colDBType = new Dictionary<string, Type>();
            foreach (DataColumn col in dt.Columns)
            {
                colDBType.Add(col.ColumnName, col.DataType);
            }

            IDbCommand cmd = this.CreateCommand(con);
            cmd.CommandText = $@"select ordinal_position as order,
column_name as name,
data_type as type,
coalesce(character_maximum_length,numeric_precision,-1) as Length,
numeric_scale as Scale,
case is_nullable when 'NO' then 0 else 1 end as null,
column_default as default,
case  when position('nextval' in column_default)>0 then 1 else 0 end as identity, 
case when b.pk_name is null then 0 else 1 end as pk,
c.DeText as comment
from information_schema.columns 
left join (
    select pg_attr.attname as colname,pg_constraint.conname as pk_name from pg_constraint  
    inner join pg_class on pg_constraint.conrelid = pg_class.oid 
    inner join pg_attribute pg_attr on pg_attr.attrelid = pg_class.oid and  pg_attr.attnum = pg_constraint.conkey[1] 
    inner join pg_type on pg_type.oid = pg_attr.atttypid
    where pg_class.relname = '{tableName}' and pg_constraint.contype='p' 
) b on b.colname = information_schema.columns.column_name
left join (
    select attname,description as DeText from pg_class
    left join pg_attribute pg_attr on pg_attr.attrelid= pg_class.oid
    left join pg_description pg_desc on pg_desc.objoid = pg_attr.attrelid and pg_desc.objsubid=pg_attr.attnum
    where pg_attr.attnum>0 and pg_attr.attrelid=pg_class.oid and pg_class.relname='{tableName}'
)c on c.attname = information_schema.columns.column_name
where table_schema='public' and table_name='{tableName}' order by ordinal_position asc";
            List<DBFieldInfo> colInfos = new List<DBFieldInfo>();
            int pk;

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
                    fieldName = reader["name"].ToString();
                    dbTypeName = reader["type"].ToString();
                    allowNull = Convert.ToInt32(reader["null"]) != 0;
                    //if (Convert.ToInt32(reader["identity"]) == 1)
                    //{
                    //    defaultValue = 0;
                    //}
                    //else
                    //{
                    value = reader["default"];
                    defaultValue = DBNull.Value.Equals(value) ? null : value;
                    //}

                    comments = reader["comment"].ToString();
                    type = colDBType[fieldName];
                    fieldType = dicFieldDbClrFieldType[fieldName];
                    pk = Convert.ToInt32(reader["pk"]);
                    colInfos.Add(new DBFieldInfo(tableName, fieldName, dbTypeName, type, comments, defaultValue, allowNull, fieldType, pk == 1));
                }
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
            string sqlStr = $@"select pg_attribute.attname as colname,
pg_type.typname as typename,
pg_constraint.conname as pk_name,
pg_class.relname as tablename 
from 
pg_constraint  
inner join pg_class on pg_constraint.conrelid = pg_class.oid 
inner join pg_attribute on pg_attribute.attrelid = pg_class.oid and  pg_attribute.attnum = pg_constraint.conkey[1]
inner join pg_type on pg_type.oid = pg_attribute.atttypid
where 
pg_class.relname = '{tableName}' and pg_constraint.contype='p'";
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
            string sqlStr = @"select a.relname as name, b.description as value from pg_class a 
left join (select * from pg_description where objsubid =0 ) b on a.oid = b.objoid
where a.relname in (select tablename from pg_tables where schemaname = 'public')
order by a.relname asc";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);

            string queryIndexSql = this.GetQueryIndexSql(null);
            DataTable dtIndex = base.PrimitiveQueryDataToDataTable(con, queryIndexSql);

            var tableInfoList = new List<DBTableInfo>();
            string tableName;
            DBIndexInfoCollection indexInfoCollection = null;
            string comments;//备注
            object tmpValue = null;//临时变量
            List<DBFieldInfo> colInfos = null;//字段集合
            IEnumerable<DBFieldInfo> priKeyColInfos = null;//主键列字段集合
            DataRow[] indexRowArr;

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

                if (getFieldInfo)
                {
                    indexRowArr = dtIndex.Select($"table_name='{tableName}'");
                    indexInfoCollection = this.DataRowConvertToIndex(tableName, indexRowArr);
                    colInfos = this.PrimitiveGetTableFieldInfo(con, tableName);//获取表所有字段集合
                    priKeyColInfos = from col in colInfos where col.IsPriKey select col;//获取主键列字段集合
                }

                tableInfoList.Add(new DBTableInfo(tableName, comments, new DBFieldInfoCollection(colInfos), new DBFieldInfoCollection(priKeyColInfos), indexInfoCollection));
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
            string sqlStr = $@"select b.description as des 
from pg_class a 
left join (select * from pg_description where objsubid =0 ) b on a.oid = b.objoid
where a.relname ='{tableName}'
order by a.relname asc";
            DataTable dt = base.PrimitiveQueryDataToDataTable(con, sqlStr);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            string comments;//备注
            object tmpValue = null;//临时变量
            DataRow row = dt.Rows[0];
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

            List<DBFieldInfo> colInfos = null;//字段集合
            IEnumerable<DBFieldInfo> priKeyColInfos = null;//主键列字段集合

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

            return new DBTableInfo(tableName, comments, new DBFieldInfoCollection(colInfos), new DBFieldInfoCollection(priKeyColInfos), indexInfoCollection);
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
            return this.DataRowConvertToIndex(tableName, dt.Rows);
        }

        private string GetQueryIndexSql(string tableName)
        {
            string queryIndexSql;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                queryIndexSql = @"select
    t.relname as table_name,
    i.relname as index_name,
    array_to_string(array_agg(a.attname), ', ') as column_names
from
    pg_class t,
    pg_class i,
    pg_index ix,
    pg_attribute a
where
    t.oid = ix.indrelid
    and i.oid = ix.indexrelid
    and a.attrelid = t.oid
    and a.attnum = ANY(ix.indkey)
    and t.relkind = 'r'
group by
    t.relname,
    i.relname
order by
    t.relname,
    i.relname;";
            }
            else
            {
                queryIndexSql = $@"select
    t.relname as table_name,
    i.relname as index_name,
    array_to_string(array_agg(a.attname), ', ') as column_names
from
    pg_class t,
    pg_class i,
    pg_index ix,
    pg_attribute a
where
    t.oid = ix.indrelid
    and i.oid = ix.indexrelid
    and a.attrelid = t.oid
    and a.attnum = ANY(ix.indkey)
    and t.relkind = 'r'
	and t.relname = '{tableName}'
group by
    t.relname,
    i.relname
order by
    t.relname,
    i.relname;";
            }

            return queryIndexSql;
        }

        private DBIndexInfoCollection DataRowConvertToIndex(string tableName, IEnumerable rows)
        {
            /*****************************************************************************************************************************
                * table_name    index_name Key_name    column_names
                * stu           stu_name_bir_idx       name, bir
                * stu           stu_pk                 id
                *****************************************************************************************************************************/
            var indexinfoList = new List<DBIndexInfo>();
            string indexName, fieldName;
            foreach (DataRow row in rows)
            {
                indexName = row[1].ToString();
                fieldName = row[2].ToString();
                indexinfoList.Add(new DBIndexInfo(tableName, indexName, fieldName, string.Empty));
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
            //select VERSION();
            //show server_version_num
            string sqlStr = @"select version()";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            string dataBaseVersion = DBAccessEx.ConvertObject<string>(value);//PostgreSQL 10.7, compiled by Visual C++ build 1800, 64-bit

            int beginIndex = dataBaseVersion.IndexOf(' ');
            int endIndex = dataBaseVersion.IndexOf('.');
            string verStr = dataBaseVersion.Substring(beginIndex, endIndex - beginIndex);
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
            //select CURRENT_TIME;
            //select CURRENT_TIMESTAMP;
            //select now();
            string sqlStr = @"select CURRENT_TIMESTAMP";
            object value = base.PrimitiveExecuteScalar(con, sqlStr);
            return DBAccessEx.ConvertObject<DateTime>(value);
        }
    }
}
