using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// 比较运算符
    /// </summary>
    public enum CompareOperater
    {
        /// <summary>
        /// 等于
        /// </summary>
        [DisplayNameEx("等于")]
        [CompareOperaterAttribute("=", typeof(SingleValueCompareOperaterWhereGenerator))]
        Equal = 1,

        /// <summary>
        /// 不等于
        /// </summary>
        [DisplayNameEx("不等于")]
        [CompareOperaterAttribute("!=", typeof(SingleValueCompareOperaterWhereGenerator))]
        NotEqual = 2,

        /// <summary>
        /// 小于
        /// </summary>
        [DisplayNameEx("小于")]
        [CompareOperaterAttribute("<", typeof(SingleValueCompareOperaterWhereGenerator))]
        LessThan = 3,

        /// <summary>
        /// 小于等于
        /// </summary>
        [DisplayNameEx("小于等于")]
        [CompareOperaterAttribute("<=", typeof(SingleValueCompareOperaterWhereGenerator))]
        LessThanOrEqual = 4,

        /// <summary>
        /// 大于
        /// </summary>
        [DisplayNameEx("大于")]
        [CompareOperaterAttribute(">", typeof(SingleValueCompareOperaterWhereGenerator))]
        GreaterThan = 5,

        /// <summary>
        /// 大于等于
        /// </summary>
        [DisplayNameEx("大于等于")]
        [CompareOperaterAttribute(">=", typeof(SingleValueCompareOperaterWhereGenerator))]
        GreaterThanOrEqual = 6,

        /// <summary>
        /// 存在[eg:=>field in (1,2,3)]
        /// </summary>
        [DisplayNameEx("包含")]
        [CompareOperaterAttribute("IN", typeof(MultValueCompareOperaterWhereGenerator))]
        In = 7,

        /// <summary>
        /// 不存在[eg:=>field not in (1,2,3)]
        /// </summary>
        [DisplayNameEx("不包含")]
        [CompareOperaterAttribute("NOT IN", typeof(MultValueCompareOperaterWhereGenerator))]
        NotIn = 8,

        /// <summary>
        /// 不在某个范围[eg:=><![CDATA[field <=20 or filed>=100]]>]
        /// </summary>
        [DisplayNameEx("不在某个范围")]
        [CompareOperaterAttribute("<={0} or >={1}", typeof(NotInRangeCompareOperaterWhereGenerator))]
        NotInRange = 9,

        /// <summary>
        /// 字符串包含[eg:字符串包含abc=>"%abc%"]
        /// </summary>
        [DisplayNameEx("包含字符串")]
        [CompareOperaterAttribute("like '%{0}%'", typeof(LikeCompareOperaterWhereGenerator))]
        Like = 10,

        /// <summary>
        /// 字符串左侧包含[eg:字符串左侧包含abc=>"%abc"]
        /// </summary>
        [DisplayNameEx("以字符串开关")]
        [CompareOperaterAttribute("like '{0}%'", typeof(LikeCompareOperaterWhereGenerator))]
        LeftLike = 11,

        /// <summary>
        /// 字符串右侧包含[eg:字符串右侧包含abc=>"abc%"]
        /// </summary>
        [DisplayNameEx("以字符串结尾")]
        [CompareOperaterAttribute("like '%{0}'", typeof(LikeCompareOperaterWhereGenerator))]
        RightLike = 12,

        /// <summary>
        /// 字段值为NULL[eg:IS NULL]
        /// </summary>
        [DisplayNameEx("以字符串结尾")]
        [CompareOperaterAttribute("IS NULL", typeof(NullCompareOperaterWhereGenerator))]
        IsNull = 13,

        /// <summary>
        /// 字段值不为NULL[eg:IS NOT NULL]
        /// </summary>
        [DisplayNameEx("以字符串结尾")]
        [CompareOperaterAttribute("IS NOT NULL", typeof(NullCompareOperaterWhereGenerator))]
        IsNotNull = 14
    }
}