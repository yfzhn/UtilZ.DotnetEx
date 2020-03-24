using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    /// <summary>
    /// 数据库字段值转换基类
    /// </summary>
    public abstract class DBFiledValueConverterAbs : IDBFiledValueConverter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBFiledValueConverterAbs()
        {

        }

        /// <summary>
        /// 外部值转换为数据库字段支持的
        /// </summary>
        /// <param name="value">外部值</param>
        /// <returns>数据库字段值</returns>
        public abstract object Convert(object value);

        /// <summary>
        /// 数据库字段值转换为外部需要的值
        /// </summary>
        /// <param name="value">数据库字段值</param>
        /// <returns>外部值</returns>
        public abstract object ConvertBack(object value);
    }
}
