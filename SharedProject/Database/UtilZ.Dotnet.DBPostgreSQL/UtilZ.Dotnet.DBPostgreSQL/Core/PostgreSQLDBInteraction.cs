using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBPostgreSQL.Core
{
    internal class PostgreSQLDBInteraction : DBInteractionAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PostgreSQLDBInteraction()
            : base()
        {

        }

        private const string PARASIGN = "@";
        /// <summary>
        /// 数据库参数字符
        /// </summary>
        public override string ParaSign
        {
            get { return PARASIGN; }
        }

        /// <summary>
        /// 创建数据库拼接连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <returns>数据库连接字符串</returns>
        protected override string CreateDBSpliceConStr(DatabaseConfig config, DBVisitType visitType)
        {
            //NpgsqlConnectionStringBuilder
            if (config.Port == 0)
            {
                config.Port = 5432;
            }

            //return $@"Port={config.Port};Host={config.Host};Username={config.Account};Password={config.Password};Database={config.DatabaseName}";
            return $@"Server={config.Host};Port={config.Port};User ID={config.Account};Password={config.Password};Database={config.DatabaseName}";
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        public override DbProviderFactory GetProviderFactory()
        {
            return NpgsqlFactory.Instance;
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
        protected override string ConvertSqlToPagingQuerySql(string sqlStr, string orderStr, long pageIndex, long pageSize, DataBaseVersionInfo dataBaseVersion, out string pagingAssistFieldName)
        {
            //eg:SELECT * from person WHERE ID < 100 ORDER by ID DESC limit 10 offset 0
            pagingAssistFieldName = null;
            var startIndex = (pageIndex - 1) * pageSize;
            string dstSqlStr = null;
            if (string.IsNullOrWhiteSpace(orderStr))
            {
                dstSqlStr = $@"{sqlStr} limit {pageSize} offset {startIndex}";
            }
            else
            {
                dstSqlStr = $@"{sqlStr} ORDER BY {orderStr} limit {pageSize} offset {startIndex}";
            }

            return dstSqlStr;
        }

        /// <summary>
        ///  EF设置
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        protected override void PrimitiveEFSetting(DbModelBuilder modelBuilder)
        {
            base.RemoveConvention(modelBuilder);
            modelBuilder.HasDefaultSchema(string.Empty);//去掉EF的默认格式select * from dbo.stu中的dbo
        }

        /// <summary>
        /// 注册实体类型
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        /// <param name="entityTypeArr">EF实体类型数组</param>
        protected override void PrimitiveEFRegisteEntityType(DbModelBuilder modelBuilder, Type[] entityTypeArr)
        {
            //此处没想到更好的解决方案,目前只得在自定义注册实体类型或实体类定义处通过Attribute特性设置字段及表名
            throw new NotImplementedException("PostgreSQL数据库必须自定义注册实体类型,内置转换未找到相应的方法");
            //Type databaseGeneratedAttributeType = typeof(DatabaseGeneratedAttribute);
            //Type keyAttributeType = typeof(KeyAttribute);

            ////pstsql:默认表名字段名全小写
            //foreach (var entityType in entityTypeArr)
            //{
            //    try
            //    {
            //        //修改泛型方法类型
            //        MethodInfo makeGenericMethod = _dbModelBuilder_Entity_MethodInfo.MakeGenericMethod(entityType);
            //        object obj = makeGenericMethod.Invoke(modelBuilder, new object[] { });//obj is EntityTypeConfiguration<T>

            //        //调用ToTable方法,指定表名
            //        Type etcType = obj.GetType();
            //        Type[] paraTypeArr = new Type[] { typeof(string), typeof(string) };
            //        MethodInfo etc_ToTable_MethodInfo = etcType.GetMethod(nameof(EntityTypeConfiguration<string>.ToTable), paraTypeArr);
            //        string newTblName = ((TableAttribute)entityType.GetCustomAttribute(typeof(TableAttribute), true)).Name.ToLower();
            //        obj = etc_ToTable_MethodInfo.Invoke(obj, new object[] { newTblName, string.Empty });//obj is EntityTypeConfiguration<T>


            //        //调用Property.HasColumnName方法,指定列名
            //        PropertyInfo[] propertyInfoArr = entityType.GetProperties();
            //        foreach (var propertyInfo in propertyInfoArr)
            //        {
            //            PropertyInfo idPropertyInfo = entityType.GetProperty(propertyInfo.Name);
            //            var exParameter = Expression.Parameter(entityType, "p");
            //            var meEx = Expression.Lambda(Expression.Property(exParameter, propertyInfo.Name), exParameter);


            //            Type[] paraTypeArr2 = new Type[] { meEx.GetType() };
            //            MethodInfo etc_Property_MethodInfo = etcType.GetMethod(nameof(EntityTypeConfiguration<string>.Property), paraTypeArr2);

            //            MethodInfo[] etc_Property_MethodInfoArr = etcType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => { return m.IsGenericMethod && string.Equals(m.Name, nameof(EntityTypeConfiguration<string>.Property)); }).ToArray();
            //            //etc_Property_MethodInfoArr[0].GetParameters()[0].Member.`
            //            etc_Property_MethodInfo = etc_Property_MethodInfoArr[0];
            //            obj = etc_Property_MethodInfo.Invoke(obj, new object[] { meEx });//obj is PrimitivePropertyConfiguration

            //            //var idPropertyInfoAccess = Expression.MakeMemberAccess(exParameter, idPropertyInfo);
            //            //var meEx = Expression.Lambda(idPropertyInfoAccess, exParameter);

            //            //Expression<Func<TStructuralType, T>> propertyExpression


            //            //var att = propertyInfo.GetCustomAttribute(databaseGeneratedAttributeType);
            //            //if (att != null)
            //            //{
            //            //    //主键
            //            //    switch (((DatabaseGeneratedAttribute)att).DatabaseGeneratedOption)
            //            //    {
            //            //        case DatabaseGeneratedOption.None://数据库不生成值
            //            //            //modelBuilder.Entity<Stu>().Property(p => p.ID).HasColumnName(nameof(StuOracle.ID).ToLower()).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            //            //            break;
            //            //        case DatabaseGeneratedOption.Computed://插入或更新时数据库会生成一个值
            //            //        case DatabaseGeneratedOption.Identity://插入一行时数据库会生成一个值                                    
            //            //        default:
            //            //            //throw new NotSupportedException($"未知的DatabaseGeneratedOption值[{((DatabaseGeneratedAttribute)att).DatabaseGeneratedOption.ToString()}]");
            //            //            break;
            //            //    }
            //            //}
            //            //else
            //            //{
            //            //    att = propertyInfo.GetCustomAttribute(keyAttributeType);
            //            //    if (att != null)
            //            //    {
            //            //        //主键
            //            //    }
            //            //    else
            //            //    {
            //            //        if (propertyInfo.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
            //            //        {
            //            //            //主键
            //            //        }
            //            //        else
            //            //        {

            //            //        }
            //            //    }
            //            //}
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
        }

        //private void Test(Type entityType, Type etcType)
        //{
        //    ////调用Property.HasColumnName方法,指定列名
        //    //PropertyInfo idPropertyInfo = entityType.GetProperty(nameof(Stu.ID));
        //    //var exParameter = Expression.Parameter(entityType, "p");
        //    //var idPropertyInfoAccess = Expression.MakeMemberAccess(exParameter, idPropertyInfo);


        //    ////var meEx = Expression.Lambda(idPropertyInfoAccess, exParameter);
        //    //var meEx = Expression.Lambda(idPropertyInfoAccess, exParameter);
        //    //Type[] paraTypeArr2 = new Type[] { meEx.GetType() };
        //    //MethodInfo etc_Property_MethodInfo = etcType.GetMethod(nameof(EntityTypeConfiguration<string>.Property), paraTypeArr2);

        //    //MethodInfo[] etc_Property_MethodInfoArr = etcType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => { return m.IsGenericMethod && string.Equals(m.Name, nameof(EntityTypeConfiguration<string>.Property)); }).ToArray();
        //    //etc_Property_MethodInfoArr[0].GetParameters()[0].Member.`
        //    //MethodInfo etc_Property_MethodInfo




        //    //obj = etc_Property_MethodInfo.Invoke(obj, new object[] { meEx });//obj is PrimitivePropertyConfiguration


        //    //MethodInfo ppc_HasColumnName_MethodInfo = typeof(PrimitivePropertyConfiguration).GetMethod(nameof(PrimitivePropertyConfiguration.HasColumnName));
        //    //ppc_HasColumnName_MethodInfo.Invoke(obj, new object[] { nameof(Stu.ID).ToLower() });

        //    //
        //    //PrimitivePropertyConfiguration bpc = modelBuilder.Entity<Stu>().ToTable(nameof(Stu).ToLower(), string.Empty).Property(p => p.ID);



        //    //EntityTypeConfiguration<Stu> stuConfig = modelBuilder.Entity<Stu>();
        //    //Expression<Func<Stu, string>> xx = p => p.Name;
        //    //stuConfig.ToTable("").Property(xx).HasColumnName("");

        //    //Expression ex = Expression.Field()
        //}


        private readonly PostgreSQLSqlFieldValueFormator _sqlFieldValueFormator = new PostgreSQLSqlFieldValueFormator();
        /// <summary>
        /// 获取sql字段值格式化对象
        /// </summary>
        /// <returns>sql字段值格式化对象</returns>
        protected override ISqlFieldValueFormator GetSqlFieldValueFormator()
        {
            return this._sqlFieldValueFormator;
        }
    }
}
