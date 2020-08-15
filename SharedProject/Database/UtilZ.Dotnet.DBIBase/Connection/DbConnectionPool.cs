using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase.Connection
{
    /// <summary>
    /// DbConnection连接池
    /// </summary>
    internal class DbConnectionPool : IDisposable
    {
        /// <summary>
        /// 读连接对象集合池
        /// </summary>
        private readonly BlockingCollection<DbConnection> _readConPool;

        /// <summary>
        /// 写连接对象集合池
        /// </summary>
        private readonly BlockingCollection<DbConnection> _writeConPool;

        /// <summary>
        /// 数据库配置
        /// </summary>
        private readonly DatabaseConfig _config;

        /// <summary>
        /// 数据库交互实例
        /// </summary>
        private readonly IDBInteraction _interaction;

        private readonly string _readConStr;
        private readonly string _writeConStr;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="interaction">数据库交互实例</param>
        public DbConnectionPool(DatabaseConfig config, IDBInteraction interaction)
        {
            this._config = config;
            this._interaction = interaction;

            this._readConPool = new BlockingCollection<DbConnection>(new ConcurrentStack<DbConnection>());
            this._writeConPool = new BlockingCollection<DbConnection>(new ConcurrentStack<DbConnection>());

            this._readConStr = this._interaction.GenerateDBConStr(config, DBVisitType.R);
            for (int i = 0; i < config.ReadConCount; i++)
            {
                this._readConPool.Add(this.CreateConnection(config, this._readConStr));
            }

            this._writeConStr = this._interaction.GenerateDBConStr(config, DBVisitType.W);
            for (int i = 0; i < config.WriteConCount; i++)
            {
                this._writeConPool.Add(this.CreateConnection(config, this._writeConStr));
            }
        }

        private DbConnection CreateConnection(DatabaseConfig config, string conStr)
        {
            DbProviderFactory dbProviderFactory = this._interaction.GetProviderFactory();
            var con = dbProviderFactory.CreateConnection();
            con.ConnectionString = conStr;
            //con.ConnectionTimeout = config.ConnectionTimeout;,居然是只读的
            //con.Open();
            return con;
        }

        /// <summary>
        /// 获取数据库访问连接对象
        /// </summary>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库访问连接对象</returns>
        internal DbConnection GetDbConnection(DBVisitType visitType)
        {
            DbConnection con = null;
            switch (visitType)
            {
                case DBVisitType.R:
                    if (this._config.ReadConCount < DBConstant.ReadConCount)
                    {
                        con = this.CreateConnection(this._config, this._readConStr);
                    }
                    else
                    {
                        if (!this._readConPool.TryTake(out con, this._config.GetConTimeout))
                        {
                            throw new ApplicationException("从连接池获取读连接超时");
                        }
                    }
                    break;
                case DBVisitType.W:
                    if (this._config.WriteConCount < DBConstant.WriteConCount)
                    {
                        con = this.CreateConnection(this._config, this._writeConStr);
                    }
                    else
                    {
                        if (!this._writeConPool.TryTake(out con, this._config.GetConTimeout))
                        {
                            throw new ApplicationException("从连接池获取写连接超时");
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format("不支持的访问类型:{0}", visitType.ToString()));
            }

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
            }
            catch (Exception ex)
            {
                this.ReleaseDbConnection(con, visitType);
                throw new ApplicationException("打开数据库连接异常", ex);
            }

            return con;
        }

        /// <summary>
        /// 释放数据库访问连接对象
        /// </summary>
        /// <param name="con">数据库访问连接对象</param>
        /// <param name="visitType">数据库访问类型</param>
        internal void ReleaseDbConnection(DbConnection con, DBVisitType visitType)
        {
            if (visitType == DBVisitType.R)
            {
                if (this._config.ReadConCount < DBConstant.ReadConCount)
                {
                    con.Close();
                }
                else
                {
                    this._readConPool.Add(con);
                }
            }
            else if (visitType == DBVisitType.W)
            {
                if (this._config.WriteConCount < DBConstant.WriteConCount)
                {
                    con.Close();
                }
                else
                {
                    this._writeConPool.Add(con);
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("不支持的访问类型:{0}", visitType.ToString()));
            }
        }

        #region IDispose接口实现
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isDisposing">是否释放资源标识</param>
        protected virtual void Dispose(bool isDisposing)
        {
            try
            {
                foreach (var readCon in this._readConPool)
                {
                    try
                    {
                        readCon.Close();
                    }
                    catch (Exception exi)
                    {
                        Loger.Error(exi);
                    }
                }

                foreach (var writeCon in this._writeConPool)
                {
                    try
                    {
                        writeCon.Close();
                    }
                    catch (Exception exi)
                    {
                        Loger.Error(exi);
                    }
                }

                this._readConPool.Dispose();
                this._writeConPool.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
        #endregion
    }
}
