using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// 窗口控件类扩展方法类
    /// </summary>
    public static class ScrollableControlEx
    {
        /// <summary>
        /// 添加窗体到窗口控件中显示
        /// </summary>
        /// <param name="containerControl">容器控件</param>
        /// <param name="form">窗口</param>
        /// <param name="dock">停靠方式[默认为Fill]</param>
        public static void AddForm(this Control containerControl, Form form, DockStyle dock = DockStyle.Fill)
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.TopLevel = false;
            form.Dock = dock;
            containerControl.Controls.Clear();
            containerControl.Controls.Add(form);
            form.Show();
        }
    }
}
