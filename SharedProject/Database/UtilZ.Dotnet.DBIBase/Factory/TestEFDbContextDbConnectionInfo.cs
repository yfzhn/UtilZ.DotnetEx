using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Connection;

namespace UtilZ.Dotnet.DBIBase.Factory
{
    /// <summary>
    /// 测试EF依赖项_数据库连接信息
    /// </summary>
    internal class TestEFDbContextDbConnectionInfo : IDbConnectionInfo
    {
        /// <summary>
        /// 数据库编号ID
        /// </summary>
        public int DBID { get; private set; }

        /// <summary>
        /// 数据库访问类型
        /// </summary>
        public DBIBase.Model.DBVisitType VisitType { get; private set; }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public System.Data.Common.DbConnection DbConnection { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnection">数据库连接对象</param>
        public TestEFDbContextDbConnectionInfo(System.Data.Common.DbConnection dbConnection)
        {
            this.DbConnection = dbConnection;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.DbConnection.Dispose();
            }
            catch (Exception ex)
            {
                Ex.Log.Loger.Error("释放连接异常", ex);
            }
        }
    }
}
