using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBOracle.Core
{
    internal class OracleSqlFieldValueFormator : SqlFieldValueFormatorAbs
    {
        public OracleSqlFieldValueFormator()
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
            //select * from STU t where Bir > TO_DATE('2019-04-23 15:48:17','yyyy-mm-dd hh24:mi:ss') and Bir <TO_DATE('2019-04-29 15:48:17','yyyy-mm-dd hh24:mi:ss')
            string dtStr = ((DateTime)value).ToString(DBConstant.DATA_TIME_FORMAT);
            return $"TO_DATE('{dtStr}','yyyy-mm-dd hh24:mi:ss')";
        }
    }
}
