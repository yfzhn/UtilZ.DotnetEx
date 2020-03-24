using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// Sql字段值格式化接口
    /// </summary>
    public interface ISqlFieldValueFormator
    {
        /// <summary>
        /// 字段值格式化
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <param name="dataType">数据库字段对应于.net平台运行时数据类型</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>字段值格式化后的字符串</returns>
        string FieldValueFormat(DBFieldType fieldType, Type dataType, object value);
    }
}
