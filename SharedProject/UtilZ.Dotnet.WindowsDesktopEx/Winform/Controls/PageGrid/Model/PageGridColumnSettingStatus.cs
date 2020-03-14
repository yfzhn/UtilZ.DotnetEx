using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 分页表格列设置控件状态
    /// </summary>
    public enum PageGridColumnSettingStatus
    {
        /// <summary>
        /// 浮动
        /// </summary>
        [DisplayNameEx("浮动")]
        Float,

        /// <summary>
        /// 停靠
        /// </summary>
        [DisplayNameEx("停靠")]
        Dock,

        /// <summary>
        /// 隐藏
        /// </summary>
        [DisplayNameEx("隐藏")]
        Hiden,

        /// <summary>
        /// 禁用
        /// </summary>
        [DisplayNameEx("禁用")]
        Disable
    }
}
