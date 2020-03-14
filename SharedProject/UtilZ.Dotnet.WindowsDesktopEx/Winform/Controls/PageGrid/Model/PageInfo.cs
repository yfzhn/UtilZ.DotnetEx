using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="totalCount">总数据记录数</param>
        public PageInfo(long pageSize, long pageIndex, long totalCount)
        {
            long pageCount = totalCount / pageSize;
            if (totalCount % pageSize > 0)
            {
                pageCount += 1;
            }

            this.PageCount = pageCount;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.TotalCount = totalCount;
        }

        /// <summary>
        /// 分总页数
        /// </summary>
        public long PageCount { get; private set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public long PageSize { get; private set; }

        /// <summary>
        /// 页索引
        /// </summary>
        public long PageIndex { get; private set; }

        /// <summary>
        /// 总数据记录数
        /// </summary>
        public long TotalCount { get; private set; }
    }
}
