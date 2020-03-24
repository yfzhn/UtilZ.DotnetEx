using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// Sql字段值格式化基类
    /// </summary>
    public abstract class SqlFieldValueFormatorAbs : ISqlFieldValueFormator
    {
        /// <summary>
        /// 字段值格式化
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <param name="dataType">数据库字段对应于.net平台运行时数据类型</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>字段值格式化后的字符串</returns>
        public string FieldValueFormat(DBFieldType fieldType, Type dataType, object value)
        {
            return this.PrimitiveFieldValueFormat(fieldType, dataType, value);
        }

        /// <summary>
        /// 字段值格式化
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <param name="dataType">数据库字段对应于.net平台运行时数据类型</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>字段值格式化后的字符串</returns>
        protected virtual string PrimitiveFieldValueFormat(DBFieldType fieldType, Type dataType, object value)
        {
            string result;
            switch (fieldType)
            {
                case DBFieldType.DateTime:
                    result = this.DateTimeFieldValueFormat(dataType, value);
                    break;
                case DBFieldType.Number:
                    result = value.ToString();
                    break;
                case DBFieldType.String:
                    result = $"'{value}'";
                    break;
                case DBFieldType.Binary:
                case DBFieldType.Other:
                default:
                    throw new NotSupportedException($"不支持的字段类型[{fieldType.ToString()}]");
            }

            return result;
        }

        /// <summary>
        /// 字段值格式化
        /// </summary>
        /// <param name="dataType">数据库字段对应于.net平台运行时数据类型</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>字段值格式化后的字符串</returns>
        protected abstract string DateTimeFieldValueFormat(Type dataType, object value);
    }
}
