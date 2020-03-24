using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Interface
{
    /// <summary>
    /// 接口查询部分
    /// </summary>
    public partial interface IDBAccess
    {
        /// <summary>
        /// 查询分页信息
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>分页信息</returns>
        DBPageInfo QueryPageInfo(long pageSize, string sqlStr, Dictionary<string, object> parameterNameValueDic = null);

        /// <summary>
        /// 查询分页信息
        /// </summary>
        /// <param name="con">娄所谓中连接对象</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>分页信息</returns>
        DBPageInfo QueryPageInfo(IDbConnection con, long pageSize, string sqlStr, Dictionary<string, object> parameterNameValueDic = null);



        /// <summary>
        /// 转换原始查询SQL语句为分页查询SQL语句
        /// </summary>
        /// <param name="sqlStr">原始查询SQL语句</param>
        /// <param name="orderInfos">排序列名集合[null为或空不排序]</param>
        /// <param name="pageIndex">目标页索引</param>
        /// <param name="pageSize">页大小</param>
        ///  <param name="pagingAssistFieldName">分页字段名称</param>
        /// <param name="dataBaseVersion">数据库版本信息</param>
        /// <returns>分页查询SQL语句</returns>
        string ConvertSqlToPagingQuerySql(string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageIndex,
            long pageSize, out string pagingAssistFieldName, DataBaseVersionInfo dataBaseVersion = null);

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
        DataTable QueryPagingData(string sqlStr, string orderByColName, long pageSize, long pageIndex, bool orderFlag,
            Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null);

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
        DataTable QueryPagingData(IDbConnection con, string sqlStr, string orderByColName, long pageSize, long pageIndex,
            bool orderFlag, Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null);

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
        DataTable QueryPagingData(string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageSize, long pageIndex,
            Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null);

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
        DataTable QueryPagingData(IDbConnection con, string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageSize,
            long pageIndex, Dictionary<string, object> parameterNameValueDic = null, DataBaseVersionInfo dataBaseVersion = null);
    }
}
