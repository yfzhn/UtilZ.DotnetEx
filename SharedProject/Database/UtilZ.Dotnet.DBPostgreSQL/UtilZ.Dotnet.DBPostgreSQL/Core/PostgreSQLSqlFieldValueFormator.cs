using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBPostgreSQL.Core
{
    internal class PostgreSQLSqlFieldValueFormator : SqlFieldValueFormatorAbs
    {
        public PostgreSQLSqlFieldValueFormator()
            : base()
        {

        }

        /// <summary>
        /// 字段值格式化
        /// </summary>
        /// <param name="dataType">数据库字段对应于.net平台运行时数据类型</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>字段值格式化后的字符串</returns>
        protected override string DateTimeFieldValueFormat(Type dataType, object value)
        {
            return ((DateTime)value).ToString(DBConstant.DATA_TIME_FORMAT);
        }
    }
}
