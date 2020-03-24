using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 常量定义类
    /// </summary>
    public class DBConstant
    {
        /// <summary>
        /// SQL语句执行默认超时时间,单位/秒
        /// </summary>
        public const int CommandTimeout = -1;

        /// <summary>
        /// sql语句最大长度默认值
        /// </summary>
        public const long SqlMaxLength = -1;

        /// <summary>
        /// 数据库写连接数默认值
        /// </summary>
        public const int WriteConCount = 1;

        /// <summary>
        /// 数据库读连接数默认值
        /// </summary>
        public const int ReadConCount = 1;

        /// <summary>
        /// 获取连接超时时长默认值
        /// </summary>
        public const int GetConTimeout = 10000;

        /// <summary>
        /// 默认,需要拼接
        /// </summary>
        public const int DBCONINFO_TYPE_DEFAULT = 0;

        /// <summary>
        /// 直接使用字符串
        /// </summary>
        public const int DBCONINFO_TYPE_STRING = 1;

        /// <summary>
        /// 空格符
        /// </summary>
        public const string BLACK_SPACE = " ";

        /// <summary>
        /// 日期时间格式
        /// </summary>
        public const string DATA_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 浮点数转换默认精度
        /// </summary>
        public const int DIGITS = -1;
    }
}
