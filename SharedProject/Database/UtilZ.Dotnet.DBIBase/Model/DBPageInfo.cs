using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 页信息
    /// </summary>
    [Serializable]
    public class DBPageInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageCount">页数</param>
        /// <param name="totalCount">总数据记录数</param>
        /// <param name="pageSize">页大小</param>
        public DBPageInfo(long pageCount, long totalCount, long pageSize)
        {
            this.PageCount = pageCount;
            this.TotalCount = totalCount;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 反序列化构造函数-不可单独调用
        /// </summary>
        public DBPageInfo()
        {

        }

        /// <summary>
        /// 页数
        /// </summary>
        public long PageCount { get; protected set; }

        /// <summary>
        /// 总数据记录数
        /// </summary>
        public long TotalCount { get; protected set; }

        /// <summary>
        /// 数据页大小
        /// </summary>
        public long PageSize { get; protected set; }
    }
}
