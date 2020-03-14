using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Interface
{
    /// <summary>
    /// 窗口接口
    /// </summary>
    public interface IForm : IControl
    {
        /// <summary>
        /// 将窗口显示为模态对话框
        /// </summary>
        /// <returns>对话返回值</returns>
        DialogResult ShowDialog();

        /// <summary>
        /// 获取或设置运行时窗体的起始位置
        /// </summary>
        FormStartPosition StartPosition { get; set; }               

        /// <summary>
        /// 关闭窗体前发生
        /// </summary>
        event FormClosingEventHandler FormClosing;
    }
}
