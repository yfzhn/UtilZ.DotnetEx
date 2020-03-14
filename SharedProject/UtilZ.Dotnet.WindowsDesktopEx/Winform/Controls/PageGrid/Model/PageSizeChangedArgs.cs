using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 分页大小改变事件参数
    /// </summary>
    public class PageSizeChangedArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageSize">查询页大小</param>
        public PageSizeChangedArgs(int pageSize)
        {
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 查询页大小
        /// </summary>
        public int PageSize { get; private set; }
    }
}
