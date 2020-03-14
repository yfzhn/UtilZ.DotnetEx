using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 泛型事件参数
    /// </summary>
    public class TEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="args">参数</param>
        public TEventArgs(T args)
        {
            this.Args = args;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public T Args { get; private set; }
    }
}
