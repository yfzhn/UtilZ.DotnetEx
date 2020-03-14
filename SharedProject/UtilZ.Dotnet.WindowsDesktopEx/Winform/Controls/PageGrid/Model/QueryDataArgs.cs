using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 查询数据参数
    /// </summary>
    public class QueryDataArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageIndex">当前要查询页数</param>
        /// <param name="pageSize">查询页大小</param>
        public QueryDataArgs(long pageIndex, long pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 获取当前要查询页数
        /// </summary>
        public long PageIndex { get; private set; }

        /// <summary>
        /// 查询页大小
        /// </summary>
        public long PageSize { get; private set; }
    }
}
