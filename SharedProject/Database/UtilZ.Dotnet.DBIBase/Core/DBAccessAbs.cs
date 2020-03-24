using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Core
{
    public abstract partial class DBAccessAbs : IDBAccess
    {
        /// <summary>
        /// 数据库编号ID
        /// </summary>
        protected readonly int _dbid;

        /// <summary>
        /// 数据库配置
        /// </summary>
        protected readonly DatabaseConfig _config;

        /// <summary>
        /// 数据库交互对象
        /// </summary>
        protected readonly IDBInteraction _dbInteraction;

        #region 属性
        /// <summary>
        /// 数据库编号ID
        /// </summary>
        public int DBID
        {
            get { return _dbid; }
        }

        /// <summary>
        /// 数据库配置实例
        /// </summary>
        public DatabaseConfig Config
        {
            get { return _config; }
        }

        /// <summary>
        /// 数据库参数字符
        /// </summary>
        public string ParaSign
        {
            get
            {
                return this._dbInteraction.ParaSign;
            }
        }

        /// <summary>
        /// 数据库程序集名称
        /// </summary>
        private readonly string _databaseTypeName;

        /// <summary>
        /// 数据库类型名称
        /// </summary>
        public string DatabaseTypeName
        {
            get { return _databaseTypeName; }
        }

        private readonly long _sqlMaxLength;
        /// <summary>
        /// sql语句最大长度
        /// </summary>
        public long SqlMaxLength
        {
            get { return _sqlMaxLength; }
        }

        private IDatabase _database = null;
        /// <summary>
        /// 数据库对象
        /// </summary>
        public IDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    this._database = this.CreateDatabase();
                }

                return this._database;
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbInteraction">数据库交互对象</param>
        /// <param name="config">数据库配置</param>
        /// <param name="databaseTypeName">数据库类型名称</param>
        /// <param name="sqlMaxLength">sql语句最大长度</param>
        public DBAccessAbs(IDBInteraction dbInteraction, DatabaseConfig config, string databaseTypeName, long sqlMaxLength)
        {
            this._dbid = config.DBID;
            this._config = config;
            this._dbInteraction = dbInteraction;
            this._databaseTypeName = databaseTypeName;
            if (config.SqlMaxLength == DBConstant.SqlMaxLength)
            {
                this._sqlMaxLength = sqlMaxLength;
            }
            else
            {
                this._sqlMaxLength = config.SqlMaxLength;
            }

            DbConnectionPoolManager.AddDbConnectionPool(config, dbInteraction);
        }

        #region ADO.NET执行原子操作方法
        /// <summary>
        /// 创建数据库接连对象
        /// </summary>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库接连对象</returns>
        public IDbConnectionInfo CreateConnection(DBVisitType visitType)
        {
            return new DbConnectionInfo(this._dbid, visitType);
        }

        /// <summary>
        /// 检查数据库连接[连接正常返回true;否则返回false]
        /// </summary>
        /// <returns>连接正常返回true;否则返回false</returns>
        public bool CheckDbConnection()
        {
            try
            {
                using (var con = this._dbInteraction.GetProviderFactory().CreateConnection())
                {
                    //this.PrimitiveGetDataBaseSysTime(con);
                    con.ConnectionString = this._dbInteraction.GenerateDBConStr(this._config, DBVisitType.R);
                    con.Open();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建DbDataAdapter
        /// </summary>
        /// <returns>DbDataAdapter</returns>
        public IDbDataAdapter CreateDbDataAdapter()
        {
            return this.PrimitiveCreateDbDataAdapter();
        }

        /// <summary>
        /// 创建EF上下文接口
        /// </summary>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="customRegisteEntityTypeFunc">自定义注册EF实体类型回调,已自定义注册实体返回true;否则返回false</param>
        /// <returns>IEFDbContext</returns>
        public IEFDbContext CreateEFDbContext(DBVisitType visitType, Func<DatabaseConfig, DbModelBuilder, bool> customRegisteEntityTypeFunc)
        {
            return new EFDbContext(new DbConnectionInfo(this._dbid, visitType), customRegisteEntityTypeFunc);
        }

        /// <summary>
        /// ExecuteScalar执行SQL语句,返回执行结果的第一行第一列；
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        public object ExecuteScalar(string sqlStr, DBVisitType visitType, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var connectionInfo = new DbConnectionInfo(this._dbid, Model.DBVisitType.R))
            {
                return this.PrimitiveExecuteScalar(connectionInfo.DbConnection, sqlStr, parameterNameValueDic);
            }
        }

        /// <summary>
        /// ExecuteScalar执行SQL语句,返回执行结果的第一行第一列；
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="con">数据库连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        public object ExecuteScalar(string sqlStr, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null)
        {
            return this.PrimitiveExecuteScalar(con, sqlStr, parameterNameValueDic);
        }

        /// <summary>
        /// ExecuteNonQuery执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        public int ExecuteNonQuery(string sqlStr, DBVisitType visitType, Dictionary<string, object> parameterNameValueDic = null)
        {
            using (var connectionInfo = new DbConnectionInfo(this._dbid, Model.DBVisitType.R))
            {
                return this.PrimitiveExecuteNonQuery(connectionInfo.DbConnection, sqlStr, parameterNameValueDic);
            }
        }

        /// <summary>
        /// ExecuteNonQuery执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="con">数据库连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        public int ExecuteNonQuery(string sqlStr, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null)
        {
            return this.PrimitiveExecuteNonQuery(con, sqlStr, parameterNameValueDic);
        }
        #endregion

        #region 快速查询
        /// <summary>
        /// 执行SQL语句,返回查询结果
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        public DataTable QueryDataToDataTable(string sqlStr, Dictionary<string, object> parameterNameValueDic = null)
        {
            return this.PrimitiveQueryDataToDataTable(sqlStr, parameterNameValueDic);
        }

        /// <summary>
        /// 执行SQL语句,返回查询结果
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="con">数据库连接对象</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>返回执行结果</returns>
        public DataTable QueryDataToDataTable(string sqlStr, IDbConnection con, Dictionary<string, object> parameterNameValueDic = null)
        {
            return this.PrimitiveQueryDataToDataTable(con, sqlStr, parameterNameValueDic);
        }
        #endregion

        /// <summary>
        /// 创建数据库对象
        /// </summary>
        /// <returns></returns>
        protected abstract IDatabase CreateDatabase();

        #region IDisposable
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isDisposing">释放标识</param>
        protected virtual void Dispose(bool isDisposing)
        {

        }
        #endregion
    }
}
