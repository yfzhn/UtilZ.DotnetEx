using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.Core
{
    public partial class DatabaseAbs
    {
        #region pp
        /// <summary>
        /// 执行SQL语句,返回查询结果
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        protected DataTable PrimitiveQueryDataToDataTable(string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var connectionInfo = this.CreateConnection())
            {
                return this.PrimitiveQueryDataToDataTable(connectionInfo.DbConnection, sqlStr, parameterNameValueDic);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回查询结果
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        protected DataTable PrimitiveQueryDataToDataTable(IDbConnection con, string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var cmd = DBAccessEx.CreateCommand(this._dbAccess, con, sqlStr, parameterNameValueDic))
            {
                var da = this._dbAccess.CreateDbDataAdapter();
                da.SelectCommand = cmd;
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        dt.Dispose();
                        return dt;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ExecuteScalar执行SQL语句,返回执行结果的第一行第一列；
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        protected object PrimitiveExecuteScalar(string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var conInfo = this.CreateConnection())
            {
                return this.PrimitiveExecuteScalar(conInfo.DbConnection, sqlStr, parameterNameValueDic);
            }
        }

        /// <summary>
        /// ExecuteScalar执行SQL语句,返回执行结果的第一行第一列；
        /// </summary>
        /// <param name="con">数据库连接对象</param>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        protected object PrimitiveExecuteScalar(IDbConnection con, string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var cmd = DBAccessEx.CreateCommand(this._dbAccess, con, sqlStr, parameterNameValueDic))
            {
                return cmd.ExecuteScalar();
            }
        }
        #endregion
    }
}
