using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Connection
{
    /// <summary>
    /// 数据库连接信息接口
    /// </summary>
    public interface IDbConnectionInfo : IDisposable
    {
        /// <summary>
        /// 数据库编号ID
        /// </summary>
        int DBID { get; }

        /// <summary>
        /// 数据库访问类型
        /// </summary>
        DBVisitType VisitType { get; }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        DbConnection DbConnection { get; }
    }
}
