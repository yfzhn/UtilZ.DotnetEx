using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// web gif数据类
    /// </summary>
    internal class WebReadState
    {
        /// <summary>
        /// web请求对象
        /// </summary>
        public WebRequest WebRequest { get; set; }

        /// <summary>
        /// 内存数据流
        /// </summary>
        public MemoryStream MemoryStream { get; set; }

        /// <summary>
        /// 读取流
        /// </summary>
        public Stream ReadStream { get; set; }

        /// <summary>
        /// 缓存数据流
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}
