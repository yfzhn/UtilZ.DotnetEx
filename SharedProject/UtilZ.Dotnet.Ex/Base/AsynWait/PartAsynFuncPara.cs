using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 异步执行执行委托参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartAsynFuncPara<T> : CancellationTokenEx
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        public T Para { get; private set; }

        /// <summary>
        /// 异步等待提示UI
        /// </summary>
        public IPartAsynWait AsynWait { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="para">输入参数</param>
        /// <param name="token">取消通知对象</param>
        /// <param name="asynWait">异步等待提示UI</param>
        public PartAsynFuncPara(T para, CancellationToken token, IPartAsynWait asynWait)
            : base(token)
        {
            this.Para = para;
            this.AsynWait = asynWait;
        }
    }
}
