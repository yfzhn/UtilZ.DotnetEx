using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface
{
    /// <summary>
    /// 表格文件选择接口
    /// </summary>
    public interface IPropertyGridFile
    {
        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        /// <param name="propertyName">要获取扩展名的文件字段名称</param>
        string GetFileExtension(string propertyName);

        /// <summary>
        /// 获取初始包含目录的全路径文件名,默认请返回null[当GetFileName有返回值时,GetInitialDirectory不调用]
        /// </summary>
        /// <param name="propertyName">要获取扩展名的文件字段名称</param>
        string GetFileName(string propertyName);

        /// <summary>
        /// 获取初始目录,默认请返回null[当GetFileName有返回值时,GetInitialDirectory不调用]
        /// </summary>
        /// <param name="propertyName">要获取扩展名的文件字段名称</param>
        string GetInitialDirectory(string propertyName);
    }
}
