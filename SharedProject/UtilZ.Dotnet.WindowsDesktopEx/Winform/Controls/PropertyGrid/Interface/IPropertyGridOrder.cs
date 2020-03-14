using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface
{
    /// <summary>
    /// 表格属性排序接口
    /// </summary>
    public interface IPropertyGridOrder
    {
        /// <summary>
        /// 排序类型
        /// </summary>
        PropertyGridOrderType OrderType { get; }

        /// <summary>
        /// 获取自定义属性排序数组集合[当OrderType值为Custom时必须实现此接口]
        /// </summary>
        /// <param name="propertyNames">需要排序的属性名称列表</param>
        /// <returns>自定义属性排序数组集合</returns>
        string[] GetCustomSortPropertyName(List<string> propertyNames);
    }

    /// <summary>
    /// 表格排序类型
    /// </summary>
    public enum PropertyGridOrderType
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending,

        /// <summary>
        /// 降序
        /// </summary>
        Descending,

        /// <summary>
        /// 自定义
        /// </summary>
        Custom
    }
}
