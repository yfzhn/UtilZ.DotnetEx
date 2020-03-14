using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// gif图片路由异常事件参数
    /// </summary>
    public class GifImageExceptionRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 错误异常
        /// </summary>
        public Exception ErrorException { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="sender">路由事件触发者</param>
        /// <param name="ex">错误异常</param>
        public GifImageExceptionRoutedEventArgs(RoutedEvent routedEvent, object sender, Exception ex)
            : base(routedEvent, sender)
        {
            this.ErrorException = ex;
        }
    }
}
