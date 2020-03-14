using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface
{
    /// <summary>
    /// 表格分组排序接口
    /// </summary>
    public interface IPropertyGridCategoryOrder
    {
        /// <summary>
        /// 排序类型
        /// </summary>
        PropertyGridOrderType OrderType { get; }

        /// <summary>
        /// 表格排序组名称列表
        /// </summary>
        List<string> PropertyGridCategoryNames { get; }
    }
}
