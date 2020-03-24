using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    /// <summary>
    /// 比较运算符特性
    /// </summary>
    internal class CompareOperaterAttribute : Attribute
    {
        /// <summary>
        /// 比较格式化字符串
        /// </summary>
        public string OperaterFormat { get; private set; }

        /// <summary>
        /// 条件值生成器
        /// </summary>
        public ICompareOperaterWhereGenerator ConditionValueGenerator { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operaterFormat">比较格式化字符串</param>
        /// <param name="compareOperaterWhereGeneratorType">比较运算符条件成器类型</param>
        public CompareOperaterAttribute(string operaterFormat, Type compareOperaterWhereGeneratorType)
        {
            this.OperaterFormat = operaterFormat;
            this.ConditionValueGenerator = (ICompareOperaterWhereGenerator)Activator.CreateInstance(compareOperaterWhereGeneratorType);
        }
    }
}
