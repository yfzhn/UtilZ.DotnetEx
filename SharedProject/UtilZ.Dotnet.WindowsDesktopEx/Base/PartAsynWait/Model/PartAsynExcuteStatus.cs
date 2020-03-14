using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model
{
    /// <summary>
    /// 异步等待执行结果状态
    /// </summary>
    public enum PartAsynExcuteStatus
    {
        /// <summary>
        /// 完成
        /// </summary>
        Completed = 0,

        /// <summary>
        /// 取消
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// 异常
        /// </summary>
        Exception = 3
    }
}
