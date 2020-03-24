using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase.Connection
{
    /// <summary>
    /// 数据库连接信息
    /// </summary>
    internal class DbConnectionInfo : IDbConnectionInfo
    {
        private readonly DbConnectionPool _dbConnectionPool;

        /// <summary>
        /// 数据库编号ID
        /// </summary>
        public int DBID { get; private set; }

        /// <summary>
        /// 数据库访问类型
        /// </summary>
        public DBVisitType VisitType { get; private set; }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public DbConnection DbConnection { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbid">数据库编号ID</param>
        /// <param name="visitType">数据库访问类型</param>
        public DbConnectionInfo(int dbid, DBVisitType visitType)
        {
            this.DBID = dbid;
            this.VisitType = visitType;
            this._dbConnectionPool = DbConnectionPoolManager.GetConnectionPoolByDBID(dbid);
            this.DbConnection = this._dbConnectionPool.GetDbConnection(visitType);
        }

        #region IDispose接口实现
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.Dispose(true);
                GC.SuppressFinalize(this.DbConnection);
            }
            catch (Exception ex)
            {
                Loger.Error("释放连接到连接池异常", ex);
            }
        }

        private bool _isDisposed = false;
        private readonly object _isDisposedLock = new object();
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isDisposing">是否释放资源标识</param>
        protected virtual void Dispose(bool isDisposing)
        {
            lock (this._isDisposedLock)
            {
                if (this._isDisposed)
                {
                    return;
                }

                this._dbConnectionPool.ReleaseDbConnection(this.DbConnection, this.VisitType);
                this._isDisposed = true;
            }
        }
        #endregion
    }
}
