using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Interaction
{
    /// <summary>
    /// 数据库交互基础基类
    /// </summary>
    public abstract class DBInteractionAbs : IDBInteraction
    {
        /// <summary>
        /// OnModelCreating时修改泛型方法参数类型的目标方法
        /// </summary>
        protected static readonly MethodInfo _dbModelBuilder_Entity_MethodInfo;

        static DBInteractionAbs()
        {
            _dbModelBuilder_Entity_MethodInfo = typeof(DbModelBuilder).GetMethod(nameof(DbModelBuilder.Entity));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBInteractionAbs()
        {

        }

        /// <summary>
        /// 数据库参数字符
        /// </summary>
        public abstract string ParaSign { get; }

        /// <summary>
        /// 生成数据库连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        public string GenerateDBConStr(DatabaseConfig config, DBVisitType visitType)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            string conStr;
            switch (config.DBConInfoType)
            {
                case DBConstant.DBCONINFO_TYPE_DEFAULT:
                    conStr = this.CreateDBOriginConnectionString(config, visitType);
                    break;
                case DBConstant.DBCONINFO_TYPE_STRING:
                    conStr = this.CreateDBSpliceConStr(config, visitType);
                    break;
                default:
                    throw new ArgumentException($"数据库配置项[{config.ConName}]数据库连接信息类型值[{config.DBConInfoType}]无效;可选值[0:内部拼接数据库连接字符串;1:直接使用字符串]");
            }

            return conStr;
        }

        /// <summary>
        /// 创建数据库拼接连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        protected abstract string CreateDBSpliceConStr(DatabaseConfig config, DBVisitType visitType);

        /// <summary>
        /// 创建原生连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        protected virtual string CreateDBOriginConnectionString(DatabaseConfig config, DBVisitType visitType)
        {
            return config.ConStr;
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        public abstract DbProviderFactory GetProviderFactory();

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
        public string ConvertSqlToPagingQuerySql(string sqlStr, IEnumerable<DBOrderInfo> orderInfos, long pageIndex, long pageSize, DataBaseVersionInfo dataBaseVersion, out string pagingAssistFieldName)
        {
            string orderStr = this.CreateOrderStr(orderInfos);
            return this.ConvertSqlToPagingQuerySql(sqlStr, orderStr, pageIndex, pageSize, dataBaseVersion, out pagingAssistFieldName);
        }

        /// <summary>
        /// 转换原始查询SQL语句为分页查询SQL语句
        /// </summary>
        /// <param name="sqlStr">原始查询SQL语句</param>
        /// <param name="orderStr">排序字符串</param>
        /// <param name="pageIndex">目标页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dataBaseVersion">数据库版本号信息</param>
        /// <param name="pagingAssistFieldName">分页字段名称</param>
        /// <returns>分页查询SQL语句</returns>
        protected abstract string ConvertSqlToPagingQuerySql(string sqlStr, string orderStr, long pageIndex, long pageSize, DataBaseVersionInfo dataBaseVersion, out string pagingAssistFieldName);

        /// <summary>
        /// 创建排序字符串
        /// </summary>
        /// <param name="orderInfos">排序列名集合</param>
        /// <returns>排序字符串</returns>
        private string CreateOrderStr(IEnumerable<DBOrderInfo> orderInfos)
        {
            if (orderInfos == null || orderInfos.Count() == 0)
            {
                return null;
            }

            //排序列
            StringBuilder sbOrder = new StringBuilder();
            foreach (var orderInfo in orderInfos)
            {
                sbOrder.Append(orderInfo.FieldName);
                sbOrder.Append(" ");
                sbOrder.Append(orderInfo.OrderFlag ? "ASC" : "DESC");
                sbOrder.Append(",");
            }

            //移除最后一个,
            sbOrder = sbOrder.Remove(sbOrder.Length - 1, 1);
            return sbOrder.ToString();
        }

        /// <summary>
        /// 获取分页辅助列名
        /// </summary>
        /// <param name="sqlStr">查询SQL语句</param>
        /// <returns>分页辅助列名</returns>
        protected string GetPagingAssistColName(string sqlStr)
        {
            string pagingAssistColName = "PACNRN";
            int index = 1;
            while (sqlStr.Contains(pagingAssistColName))
            {
                pagingAssistColName = "PACNRN" + index.ToString();
            }

            return pagingAssistColName;//分页辅助列列名
        }

        /// <summary>
        /// EF设置
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        public void EFSetting(DbModelBuilder modelBuilder)
        {
            this.PrimitiveEFSetting(modelBuilder);
        }

        /// <summary>
        ///  EF设置
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        protected virtual void PrimitiveEFSetting(DbModelBuilder modelBuilder)
        {
            this.RemoveConvention(modelBuilder);
        }

        /// <summary>
        ///移除部分契约
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        protected virtual void RemoveConvention(DbModelBuilder modelBuilder)
        {
            //todo..之后 再来研究这块,这些契约什么条件下需要移除
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();//移除复数表名契约
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();//移除多元化的类型实体数据模型中的实体集的名称
            //modelBuilder.Conventions.Remove<AssociationInverseDiscoveryConvention>();//寻找导航上互相引用的类型,并将他们配置为逆属性的相同的关系
            //modelBuilder.Conventions.Remove<ComplexTypeDiscoveryConvention>();//寻找导航上互相引用的类型,并将他们配置为逆属性的相同的关系
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();//禁用一对多级联删除
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();//禁用多对多级联删除
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();//不创建EdmMetadata,
        }

        /// <summary>
        /// 注册实体类型
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        /// <param name="entityTypeArr">EF实体类型数组</param>
        public void EFRegisteEntityType(DbModelBuilder modelBuilder, Type[] entityTypeArr)
        {
            if (entityTypeArr == null)
            {
                return;
            }

            this.PrimitiveEFRegisteEntityType(modelBuilder, entityTypeArr);
        }

        /// <summary>
        /// 注册实体类型
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        /// <param name="entityTypeArr">EF实体类型集合</param>
        protected virtual void PrimitiveEFRegisteEntityType(DbModelBuilder modelBuilder, Type[] entityTypeArr)
        {
            MethodInfo makeGenericMethod;
            object obj;
            foreach (var entityType in entityTypeArr)
            {
                makeGenericMethod = _dbModelBuilder_Entity_MethodInfo.MakeGenericMethod(entityType);
                obj = makeGenericMethod.Invoke(modelBuilder, new object[] { });
            }
        }

        /// <summary>
        /// 获取sql字段值格式化对象
        /// </summary>
        public ISqlFieldValueFormator SqlFieldValueFormator
        {
            get { return this.GetSqlFieldValueFormator(); }
        }

        /// <summary>
        /// 获取sql字段值格式化对象
        /// </summary>
        /// <returns>sql字段值格式化对象</returns>
        protected abstract ISqlFieldValueFormator GetSqlFieldValueFormator();
    }
}
