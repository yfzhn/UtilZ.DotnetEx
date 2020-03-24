using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    /// <summary>
    /// 数据库字段值转换接口
    /// </summary>
    public interface IDBFiledValueConverter
    {
        /// <summary>
        /// 外部值转换为数据库字段支持的
        /// </summary>
        /// <param name="value">外部值</param>
        /// <returns>数据库字段值</returns>
        object Convert(object value);

        /// <summary>
        /// 数据库字段值转换为外部需要的值
        /// </summary>
        /// <param name="value">数据库字段值</param>
        /// <returns>外部值</returns>
        object ConvertBack(object value);
    }
}
