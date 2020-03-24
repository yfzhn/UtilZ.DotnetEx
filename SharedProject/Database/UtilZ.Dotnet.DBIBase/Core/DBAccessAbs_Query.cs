using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Core
{
    public abstract partial class DBAccessAbs
    {
        /// <summary>
        /// 查询分页信息
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>分页信息</returns>
        public DBPageInfo QueryPageInfo(long pageSize, string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var conInfo = new DbConnectionInfo(this._dbid, DBVisitType.R))
            {
                return this.QueryPageInfo(conInfo.DbConnection, pageSize, sqlStr, parameterNameValueDic);
            }
        }

        /// <summary>
        /// 查询分页信息
        /// </summary>
        /// <param name="con">娄所谓中连接对象</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>分页信息</returns>
        public DBPageInfo QueryPageInfo(IDbConnection con, long pageSize, string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("页大小不能小于1", nameof(pageSize));
            }

            object obj = this.PrimitiveExecuteScalar(con, sqlStr, parameterNameValueDic);
            long totalCount = DBAccessEx.ConvertObject<Int64>(obj);
            long pageCount = totalCount / pageSize;
            if (totalCount % pageSize != 0)
            {
                pageCount++;
            }

            return new DBPageInfo(pageCount, totalCount, pageSize);
        }



        /// <summary>
        /// 转换原始查询SQL语句为分页查询SQL语句
        /// </summary>
        /// <param name="sqlStr">原始查询SQL语句</param>
        /// <param name="orderInfos">排序列名集合[null为或空不排序]</param>
        /// <param name="pageIndex">目标页索引</param>
        /// <param name="pageSize">页大小</param>   
        /// <param name="pagingAssistFieldName">分页字段名称</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>分页查询SQL语句</returns>
        public string ConvertSqlToPagingQuerySql(string sqlStr, IEnumerable<DBOrderInfo> orderInfos,
            long pageIndex, long pageSize, out string pagingAssistFieldName, DataBaseVersionInfo dataBaseVersion = null)
        {
            return this.PrimitiveConvertSqlToPagingQuerySql(sqlStr, orderInfos, pageIndex, pageSize, dataBaseVersion, out pagingAssistFieldName);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="sqlStr">查询SQL语句</param>
        /// <param name="orderByColName">排序列名</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">查询页索引</param>
        /// <param name="orderFlag">排序类型[true:升序;false:降序]</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>数据表</returns>
        public DataTable QueryPagingData(string sqlStr, string orderByColName, long pageSize, long pageIndex, bool orderFlag,
            Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null)
        {
            List<DBOrderInfo> orderInfos = null;
            if (!string.IsNullOrWhiteSpace(orderByColName))
            {
                orderInfos = new List<DBOrderInfo>();
                orderInfos.Add(new DBOrderInfo(orderByColName, orderFlag));
            }

            using (var conInfo = new DbConnectionInfo(this._dbid, DBVisitType.R))
            {
                return this.PrimitiveQueryPagingData(conInfo.DbConnection, sqlStr, orderInfos, pageSize, pageIndex, parameterNameValueDic, dataBaseVersion);
            }
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="con">娄所谓中连接对象</param>
        /// <param name="sqlStr">查询SQL语句</param>
        /// <param name="orderByColName">排序列名</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">查询页索引</param>
        /// <param name="orderFlag">排序类型[true:升序;false:降序]</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>数据表</returns>
        public DataTable QueryPagingData(IDbConnection con, string sqlStr, string orderByColName, long pageSize, long pageIndex,
            bool orderFlag, Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null)
        {
            List<DBOrderInfo> orderInfos = null;
            if (!string.IsNullOrWhiteSpace(orderByColName))
            {
                orderInfos = new List<DBOrderInfo>();
                orderInfos.Add(new DBOrderInfo(orderByColName, orderFlag));
            }

            return this.PrimitiveQueryPagingData(con, sqlStr, orderInfos, pageSize, pageIndex, parameterNameValueDic, dataBaseVersion);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="sqlStr">查询SQL语句</param>
        /// <param name="orderInfos">排序列名集合[null为或空不排序]</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">查询页索引</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>数据表</returns>
        public DataTable QueryPagingData(string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageSize, long pageIndex,
            Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null)
        {
            using (var conInfo = new DbConnectionInfo(this._dbid, DBVisitType.R))
            {
                return this.PrimitiveQueryPagingData(conInfo.DbConnection, sqlStr, orderInfos, pageSize, pageIndex, parameterNameValueDic, dataBaseVersion);
            }
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="con">娄所谓中连接对象</param>
        /// <param name="sqlStr">查询SQL语句</param>
        /// <param name="orderInfos">排序列名集合[null为或空不排序]</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">查询页索引</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>数据表</returns>
        public DataTable QueryPagingData(IDbConnection con, string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageSize,
            long pageIndex, Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null)
        {
            return this.PrimitiveQueryPagingData(con, sqlStr, orderInfos, pageSize, pageIndex, parameterNameValueDic, dataBaseVersion);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="con">娄所谓中连接对象</param>
        /// <param name="sqlStr">查询SQL语句</param>
        /// <param name="orderInfos">排序列名集合[null为或空不排序]</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">查询页索引</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>数据表</returns>
        private DataTable PrimitiveQueryPagingData(IDbConnection con, string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageSize,
            long pageIndex, Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null)
        {
            if (string.IsNullOrWhiteSpace(sqlStr))
            {
                throw new ArgumentNullException("查询语句不能为空或null", "sqlStr");
            }

            sqlStr = sqlStr.Trim();

            if (pageIndex < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("查询页索引值不能小于1,值{0}无效", pageIndex), "pageIndex");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("查询页大小不能小于1,值{0}无效", pageSize), "pageSize");
            }

            string pagingAssistFieldName;
            string pagingQuerySql = this.PrimitiveConvertSqlToPagingQuerySql(sqlStr, orderInfos, pageIndex, pageSize, dataBaseVersion, out pagingAssistFieldName);
            DataTable dt = this.PrimitiveQueryDataToDataTable(con, pagingQuerySql, parameterNameValueDic);
            if (!string.IsNullOrWhiteSpace(pagingAssistFieldName))
            {
                dt.Columns.Remove(pagingAssistFieldName);
            }

            return dt;
        }
    }
}
