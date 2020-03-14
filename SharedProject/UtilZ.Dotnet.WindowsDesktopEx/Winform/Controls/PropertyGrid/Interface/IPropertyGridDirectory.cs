using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface
{
    /// <summary>
    /// 表格文件选择接口
    /// </summary>
    public interface IPropertyGridDirectory
    {
        /// <summary>
        /// 获取初始目录
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        string GetInitialSelectedPath(string propertyName);
    }
}
