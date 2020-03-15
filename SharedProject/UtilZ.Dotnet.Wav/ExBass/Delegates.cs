using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.ExBass
{
    /// <summary>
    /// 回调委托
    /// </summary>
    /// <param name="buffer">缓存</param>
    /// <param name="length">长度</param>
    /// <param name="user">句柄</param>
    public delegate void DOWNLOADPROC(IntPtr buffer, int length, IntPtr user);
}
