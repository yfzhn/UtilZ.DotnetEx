using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBOracle.Core
{
    internal class OracleDBInteraction : DBInteractionAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OracleDBInteraction()
            : base()
        {

        }

        private const string PARASIGN = ":";
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
            //OracleConnectionStringBuilder
            if (config.Port == 0)
            {
                config.Port = 1521;
            }

            //注：不同版本的Oracle.ManagedDataAccess.dll，连接字符串不一定兼容,以下只启用支持的版本
            //return string.Format(@"User Id={0};Password={1};Data Source={2}:{3}/{4}", config.Account, config.Password, config.Host, config.Port, config.DatabaseName);
            //return string.Format(@"User Id={0};Password={1};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={2})(PORT={3})))(CONNECT_DATA=(SERVICE_NAME={4})))",
            //config.Account, config.Password, config.Host, config.Port, config.DatabaseName);            
            return $@"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={config.Host})(PORT={config.Port}))(CONNECT_DATA=(SID={config.DatabaseName})));User Id={config.Account};Password={config.Password}";
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        /// <returns>DbProviderFactory</returns>
        public override DbProviderFactory GetProviderFactory()
        {
            return OracleClientFactory.Instance;
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
            //dataBaseVersion.Version:11.2.0.1.0
            //eg1效率最高,但是不好拼接,所以采用低一点的eg2
            //eg1:SELECT* FROM(SELECT ROWNUM AS RN, t.* FROM TASK_TARGET_POS_RESULT t WHERE TAR_CAP_TIME> 1554604680 AND TAR_CAP_TIME<1555209480 AND ROWNUM<110) WHERE RN>= 100
            //eg2:SELECT* FROM(SELECT UDTWQS_T.*,ROWNUM AS WQS_RNCol FROM(SELECT* FROM TASK_TARGET_POS_RESULT) UDTWQS_T WHERE ROWNUM < 110) WHERE WQS_RNCol>= 100
            pagingAssistFieldName = base.GetPagingAssistColName(sqlStr);
            var startIndex = (pageIndex - 1) * pageSize;
            var endIndex = startIndex + pageSize;
            string dstSqlStr = null;
            if (string.IsNullOrWhiteSpace(orderStr))
            {
                dstSqlStr = $"SELECT* FROM(SELECT UDTWQS_T.*,ROWNUM AS {pagingAssistFieldName} FROM({sqlStr}) UDTWQS_T WHERE ROWNUM < {endIndex}) WHERE {pagingAssistFieldName}>= {startIndex}";
            }
            else
            {
                dstSqlStr = $"SELECT* FROM(SELECT UDTWQS_T.*,ROWNUM AS {pagingAssistFieldName} FROM({sqlStr}) UDTWQS_T WHERE ROWNUM < {endIndex} ORDER BY {orderStr}) WHERE {pagingAssistFieldName}>= {startIndex}";
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
            throw new NotImplementedException("Oracle数据库必须自定义注册实体类型,内置转换未找到相应的方法");
        }

        private readonly OracleSqlFieldValueFormator _sqlFieldValueFormator = new OracleSqlFieldValueFormator();
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
