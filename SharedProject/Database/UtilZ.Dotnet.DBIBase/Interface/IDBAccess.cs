using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Interface
{
    public partial interface IDBAccess : IDisposable
    {
        #region 属性
        /// <summary>
        /// 数据库编号ID
        /// </summary>
        int DBID { get; }

        /// <summary>
        /// 数据库配置实例
        /// </summary>
        DatabaseConfig Config { get; }

        /// <summary>
        /// 数据库参数字符
        /// </summary>
        string ParaSign { get; }

        /// <summary>
        /// 数据库类型名称
        /// </summary>
        string DatabaseTypeName { get; }

        /// <summary>
        /// sql语句最大长度
        /// </summary>
        long SqlMaxLength { get; }

        /// <summary>
        /// 数据库对象
        /// </summary>
        IDatabase Database { get; }
        #endregion

        #region ADO.NET执行原子操作方法
        /// <summary>
        /// 创建数据库接连对象
        /// </summary>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库接连对象</returns>
        IDbConnectionInfo CreateConnection(DBVisitType visitType);

        /// <summary>
        /// 检查数据库连接[连接正常返回true;否则返回false]
        /// </summary>
        /// <returns>连接正常返回true;否则返回false</returns>
        bool CheckDbConnection();

        /// <summary>
        /// 创建DbDataAdapter
        /// </summary>
        /// <returns>DbDataAdapter</returns>
        IDbDataAdapter CreateDbDataAdapter();

        /// <summary>
        /// 创建EF上下文接口
        /// </summary>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="customRegisteEntityTypeFunc">自定义注册EF实体类型回调,已自定义注册实体返回true;否则返回false</param>
        /// <returns>IEFDbContext</returns>
        IEFDbContext CreateEFDbContext(DBVisitType visitType, Func<DatabaseConfig, DbModelBuilder, bool> customRegisteEntityTypeFunc);

        /// <summary>
        /// ExecuteScalar执行SQL语句,返回执行结果的第一行第一列；
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        object ExecuteScalar(string sqlStr, DBVisitType visitType, Dictionary<string, object> parameterNameValueDic = null);

        /// <summary>
        /// ExecuteScalar执行SQL语句,返回执行结果的第一行第一列；
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="con">数据库连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        object ExecuteScalar(string sqlStr, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null);

        /// <summary>
        /// ExecuteNonQuery执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        int ExecuteNonQuery(string sqlStr, DBVisitType visitType, Dictionary<string, object> parameterNameValueDic = null);

        /// <summary>
        /// ExecuteNonQuery执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="con">数据库连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        int ExecuteNonQuery(string sqlStr, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null);
        #endregion

        #region 快速查询
        /// <summary>
        /// 执行SQL语句,返回查询结果
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        DataTable QueryDataToDataTable(string sqlStr, Dictionary<string, object> parameterNameValueDic = null);

        /// <summary>
        /// 执行SQL语句,返回查询结果
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="con">数据库连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        DataTable QueryDataToDataTable(string sqlStr, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null);
        #endregion
    }
}
