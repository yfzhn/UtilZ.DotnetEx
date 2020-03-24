using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// 逻辑运算符枚举
    /// </summary>
    public enum LogicOperaters
    {
        /// <summary>
        /// 且
        /// </summary>
        [DisplayNameExAttribute("且")]
        AND = 1,

        /// <summary>
        /// 或
        /// </summary>
        [DisplayNameExAttribute("或")]
        OR = 2
    }
}
