using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 数据库辅助类
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// 静态构造函数,初始化
        /// </summary>
        static DBHelper()
        {
            //初始化数据库字段对应的公共运行时类型枚举集合
            DBHelper.InitFieldClrDbTypes();
        }

        /// <summary>
        /// 数据库字段对应的公共运行时类型枚举集合[key:数据库字段对应的公共运行时类型枚举;value:该类型对应的CLR类型集合]
        /// </summary>
        private static readonly Dictionary<DBFieldType, List<Type>> _dicFieldClrDbTypes = new Dictionary<DBFieldType, List<Type>>();

        /// <summary>
        /// 初始化数据库字段对应的公共运行时类型枚举集合
        /// </summary>
        private static void InitFieldClrDbTypes()
        {
            /**********************************************
             *SQLServer:有如下几种类型不支持
             Microsoft.SqlServer.Types.SqlGeography
             Microsoft.SqlServer.Types.SqlGeometry
             Microsoft.SqlServer.Types.SqlHierarchyId
            ***********************************************/

            //数值类型
            List<Type> numTypes = new List<Type>();
            numTypes.Add(ClrSystemType.BoolType);
            numTypes.Add(ClrSystemType.ByteType);
            numTypes.Add(ClrSystemType.DecimalType);
            numTypes.Add(ClrSystemType.DoubleType);
            numTypes.Add(ClrSystemType.Int16Type);
            numTypes.Add(ClrSystemType.Int32Type);
            numTypes.Add(ClrSystemType.Int64Type);
            numTypes.Add(ClrSystemType.SbyteType);
            numTypes.Add(ClrSystemType.FloatType);
            numTypes.Add(ClrSystemType.UInt16Type);
            numTypes.Add(ClrSystemType.UInt32Type);
            numTypes.Add(ClrSystemType.UInt64Type);
            DBHelper._dicFieldClrDbTypes.Add(DBFieldType.Number, numTypes);

            //二进制类型
            List<Type> binaryTypes = new List<Type>();
            binaryTypes.Add(ClrSystemType.BytesType);
            DBHelper._dicFieldClrDbTypes.Add(DBFieldType.Binary, binaryTypes);

            //日期时间类型
            List<Type> dateTypes = new List<Type>();
            dateTypes.Add(ClrSystemType.DateTimeType);
            dateTypes.Add(ClrSystemType.TimeSpanType);
            dateTypes.Add(ClrSystemType.DateTimeOffsetType);
            DBHelper._dicFieldClrDbTypes.Add(DBFieldType.DateTime, dateTypes);

            //字符串类型
            List<Type> stringTypes = new List<Type>();
            stringTypes.Add(ClrSystemType.StringType);
            stringTypes.Add(ClrSystemType.GuidType);
            DBHelper._dicFieldClrDbTypes.Add(DBFieldType.String, stringTypes);
        }

        /// <summary>
        /// 获取数据库字段对应的公共运行时类型[默认为DbClrFieldType.Other]
        /// </summary>
        /// <param name="fieldDataType">字段数据类型</param>
        /// <returns>数据库字段对应的公共运行时类型</returns>
        public static DBFieldType GetDbClrFieldType(Type fieldDataType)
        {
            foreach (var kv in DBHelper._dicFieldClrDbTypes)
            {
                if (kv.Value.Contains(fieldDataType))
                {
                    return kv.Key;
                }
            }

            return DBFieldType.Other;
        }

        /*
        #region 生成条件SQL
        /// <summary>
        /// 生成Oracle无参数SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="paraSign">参数符号</param>
        /// <param name="conditionGroupCollection">条件组集合</param>
        /// <param name="queryFields">要查询的字段名称集合,为null或空时全查</param>
        /// <returns>Oracle无参数SQL语句</returns>
        public static string GenerateNoParaOracleSql(string tableName, string paraSign, ConditionGroupCollection conditionGroupCollection, List<string> queryFields = null)
        {
            var conditionGenerator = new OracleConditionGenerator(tableName, paraSign, conditionGroupCollection, 0, queryFields);
            return conditionGenerator.GenerateNoParaConditionOracleSql();
        }

        /// <summary>
        /// 生成Oracle无参数SQL语句
        /// </summary>
        /// <param name="conditionGroupCollection">条件组集合</param>
        /// <returns>Oracle无参数SQL语句</returns>
        public static string GenerateNoParaOracleWhere(ConditionGroupCollection conditionGroupCollection)
        {
            var conditionGenerator = new OracleConditionGenerator(null, null, conditionGroupCollection, 0, null);
            return conditionGenerator.GenerateNoParaConditionOracleWhere();
        }

        /// <summary>
        /// 生成Oracle带参数SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="paraSign">参数符号</param>
        /// <param name="conditionGroupCollection">条件组集合</param>
        /// <param name="paraIndex">参数索引</param>
        /// <param name="queryFields">要查询的字段名称集合,为null或空时全查</param>
        /// <returns>Oracle带参数SQL语句对象</returns>
        public static DBSqlInfo GenerateParaSql(string tableName, string paraSign, ConditionGroupCollection conditionGroupCollection, IEnumerable<string> queryFields = null)
        {
            var conditionGenerator = new OracleConditionGenerator(tableName, paraSign, conditionGroupCollection, 1, queryFields);
            return conditionGenerator.GenerateParaSql();
        }

        /// <summary>
        /// 生成Oracle带参数SQL语句
        /// </summary>
        /// <param name="paraSign">参数符号</param>
        /// <param name="conditionGroupCollection">条件组集合</param>
        /// <param name="paraIndex">参数索引</param>
        /// <returns>Oracle带参数SQL语句对象</returns>
        public static DBSqlInfo GenerateParaWhere(string paraSign, ConditionGroupCollection conditionGroupCollection, ref int paraIndex)
        {
            var conditionGenerator = new OracleConditionGenerator(null, paraSign, conditionGroupCollection, paraIndex, null);
            DBSqlInfo dbWhereInfo = conditionGenerator.GenerateParaWhere();
            paraIndex = conditionGenerator.ParaIndex;
            return dbWhereInfo;
        }

        /// <summary>
        /// 生成Oracle带参数SQL语句
        /// </summary>
        /// <param name="paraSign">参数符号</param>
        /// <param name="conditionGroupCollection">条件组集合</param>
        /// <returns>Oracle带参数SQL语句对象</returns>
        public static DBSqlInfo GenerateParaWhere(string paraSign, ConditionGroupCollection conditionGroupCollection)
        {
            var conditionGenerator = new OracleConditionGenerator(null, paraSign, conditionGroupCollection, 1, null);
            DBSqlInfo dbWhereInfo = conditionGenerator.GenerateParaWhere();
            return dbWhereInfo;
        }
        #endregion
    */
    }
}
