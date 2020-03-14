using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface
{
    /// <summary>
    /// 异常执行取消接口
    /// </summary>
    public interface IAsynExcuteCancell
    {
        /// <summary>
        /// 获取包含有关控件的数据的对象
        /// </summary>
        object Tag { get; }

        /// <summary>
        /// 取消执行
        /// </summary>
        void Cancell();
    }
}
