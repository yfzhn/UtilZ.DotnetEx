using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Core
{
    /// <summary>
    /// 扩展方法类DBAccessEx
    /// </summary>
    public static class DBAccessEx
    {
        /// <summary>
        /// 转换ExecuteScalar结果为指定对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="obj">ExecuteScalar结果</param>
        /// <returns>转换结果</returns>
        public static T ConvertObject<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }

            T result;
            Type conversionType = typeof(T);
            if (obj.GetType() == conversionType)
            {
                result = (T)obj;
            }
            else
            {
                result = (T)Convert.ChangeType(obj, conversionType);
            }

            return result;
        }

        /// <summary>
        /// 设置命令配置参数
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <param name="config">配置</param>
        public static void SetCommandPara(IDbCommand cmd, DatabaseConfig config)
        {
            if (config.CommandTimeout != DBConstant.CommandTimeout)
            {
                cmd.CommandTimeout = config.CommandTimeout;
            }
        }

        /// <summary>
        /// 快速创建命令
        /// </summary>
        /// <param name="dbid">数据库ID</param>
        /// <param name="con">连接对象</param>
        /// <returns>命令</returns>
        public static IDbCommand CreateCommand(int dbid, IDbConnection con)
        {
            var config = DatabaseConfigManager.GetConfig(dbid);
            var cmd = con.CreateCommand();
            SetCommandPara(cmd, config);
            return cmd;
        }

        /// <summary>
        /// 快速创建命令
        /// </summary>
        /// <param name="dbAccess">IDBAccess</param>
        /// <param name="con">连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>命令</returns>
        public static IDbCommand CreateCommand(this IDBAccess dbAccess, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null)
        {
            var cmd = con.CreateCommand();
            if (parameterNameValueDic != null && parameterNameValueDic.Count > 0)
            {
                foreach (var kv in parameterNameValueDic)
                {
                    AddParameter(cmd, kv.Key, kv.Value);
                }
            }

            var config = dbAccess.Config;
            SetCommandPara(cmd, config);
            return cmd;
        }

        /// <summary>
        /// 快速创建命令
        /// </summary>
        /// <param name="dbAccess">IDBAccess</param>
        /// <param name="con">连接对象</param>
        /// <param name="sqlStr">SQL语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <param name="transaction">事务对象</param>
        /// <returns>命令</returns>
        public static IDbCommand CreateCommand(this IDBAccess dbAccess, IDbConnection con, string sqlStr,
            Dictionary<string, object> parameterNameValueDic = null, IDbTransaction transaction = null)
        {
            IDbCommand cmd = CreateCommand(dbAccess, con, parameterNameValueDic);
            cmd.Transaction = transaction;
            cmd.CommandText = sqlStr;
            return cmd;
        }

        /// <summary>
        /// 扩展方法添加命令参数
        /// </summary>
        /// <param name="cmd">目标命令</param>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">参数值</param>
        public static void AddParameter(this IDbCommand cmd, string parameterName, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

        /// <summary>
        /// 创建命令参数
        /// </summary>
        /// <param name="cmd">所属命令对象</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns>命令参数</returns>
        public static IDbDataParameter CreateParameter(this IDbCommand cmd, string parameterName)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            cmd.Parameters.Add(parameter);
            return parameter;
        }
    }
}
