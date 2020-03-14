using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface
{
    /// <summary>
    /// 表格密码设置接口
    /// </summary>
    public interface IPropertyGridPassword
    {
        /// <summary>
        /// 获取密码显示字符
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>密码显示字符</returns>
        char GetPasswordChar(string propertyName);
    }
}
