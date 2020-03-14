using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 空日志记录器,不作任何输出
    /// </summary>
    internal class EmptyLoger : LogerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EmptyLoger() : base()
        {

        }

        /// <summary>
        /// 静态方法添加日志的方法
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        internal override void ObjectAddLog(LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {

        }

        /// <summary>
        /// 实例添加日志
        /// </summary>
        /// <param name="skipFrames"></param>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        protected override void PrimitiveAddLog(int skipFrames, LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {

        }
    }
}
