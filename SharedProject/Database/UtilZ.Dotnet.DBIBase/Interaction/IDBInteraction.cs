using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Interaction
{
    /// <summary>
    /// 数据库交互基础接口
    /// </summary>
    public interface IDBInteraction
    {
        /// <summary>
        /// 数据库参数字符
        /// </summary>
        string ParaSign { get; }

        /// <summary>
        /// 生成数据库连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        string GenerateDBConStr(DatabaseConfig config, DBVisitType visitType);

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        DbProviderFactory GetProviderFactory();

        /// <summary>
        /// 转换原始查询SQL语句为分页查询SQL语句
        /// </summary>
        /// <param name="sqlStr">原始查询SQL语句</param>
        /// <param name="orderInfos">排序列名集合[null为或空不排序]</param>
        /// <param name="pageIndex">目标页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dataBaseVersion">数据库版本号信息</param>
        /// <param name="pagingAssistFieldName">分页字段名称</param>
        /// <returns>分页查询SQL语句</returns>
        string ConvertSqlToPagingQuerySql(string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageIndex, long pageSize, DataBaseVersionInfo dataBaseVersion, out string pagingAssistFieldName);

        /// <summary>
        /// EF设置
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        void EFSetting(DbModelBuilder modelBuilder);

        /// <summary>
        /// 注册实体类型
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        /// <param name="entityTypeArr">EF实体类型数组</param>
        void EFRegisteEntityType(DbModelBuilder modelBuilder, Type[] entityTypeArr);

        /// <summary>
        /// 获取字段值格式化对象
        /// </summary>
        ISqlFieldValueFormator SqlFieldValueFormator { get; }
    }
}
