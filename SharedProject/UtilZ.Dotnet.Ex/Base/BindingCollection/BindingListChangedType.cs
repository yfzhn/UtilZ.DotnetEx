using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// UI绑定集合改变类型
    /// </summary>
    public enum BindingListChangedType
    {
        /// <summary>
        /// 添加项
        /// </summary>
        Add,

        /// <summary>
        /// 添加多项
        /// </summary>
        AddRange,

        /// <summary>
        /// 插入
        /// </summary>
        Insert,

        /// <summary>
        /// 更新项
        /// </summary>
        Update,

        /// <summary>
        /// 删除项
        /// </summary>
        Remove,

        /// <summary>
        /// 移除多项
        /// </summary>
        RemoveRange,

        /// <summary>
        /// 清空项
        /// </summary>
        Clear
    }
}
