using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum DBFieldType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        [DisplayNameExAttribute("字符串", @"多个值之间通过逗号间隔,如果字符串中含有逗号,则通过\转义")]
        String,

        /// <summary>
        /// 数值
        /// </summary>
        [DisplayNameExAttribute("数值", "多个值之间通过逗号间隔")]
        Number,

        /// <summary>
        /// 日期时间
        /// </summary>
        [DisplayNameExAttribute("日期时间", "多个值之间通过逗号间隔")]
        DateTime,

        /// <summary>
        /// 二进制
        /// </summary>
        [DisplayNameExAttribute("二进制")]
        Binary,

        /// <summary>
        /// 其它,包括各大数据库后续扩展的类型,或是数据库中自定义类型
        /// </summary>
        [DisplayNameExAttribute("其它")]
        Other
    }
}
