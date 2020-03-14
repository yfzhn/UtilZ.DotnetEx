using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    public partial class UCPageGridControl
    {
        /// <summary>
        /// 查询数据事件
        /// </summary>
        [Description("查询数据事件")]
        [Category("分页数据显示")]
        public event EventHandler<QueryDataArgs> QueryData;

        /// <summary>
        /// 分页大小改变事件
        /// </summary>
        [Description("分页大小改变事件")]
        [Category("分页数据显示")]
        public event EventHandler<PageSizeChangedArgs> PageSizeChanged;

        /// <summary>
        /// 数据行单击事件
        /// </summary>
        [Description("数据行单击事件")]
        [Category("分页数据显示")]
        public event EventHandler<DataRowClickArgs> DataRowClick;

        /// <summary>
        /// 数据行双击事件
        /// </summary>
        [Description("数据行双击事件")]
        [Category("分页数据显示")]
        public event EventHandler<DataRowClickArgs> DataRowDoubleClick;

        /// <summary>
        /// 选中行改变事件
        /// </summary>
        [Description("选中行改变事件")]
        [Category("分页数据显示")]
        public event EventHandler<DataRowSelectionChangedArgs> DataRowSelectionChanged;
    }
}
