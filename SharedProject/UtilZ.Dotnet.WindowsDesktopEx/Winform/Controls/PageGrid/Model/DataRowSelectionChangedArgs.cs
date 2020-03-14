using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 选中行改变事件参数
    /// </summary>
    public class DataRowSelectionChangedArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rowHandle">当前选中行索引</param>
        /// <param name="prevRowHandle">上次选中行索引</param>
        /// <param name="row">当前行数据</param>
        /// <param name="prevRow">上次行数据</param>
        /// <param name="selectedRows">选中行数据集合</param>
        public DataRowSelectionChangedArgs(int rowHandle, int prevRowHandle, object row, object prevRow, List<object> selectedRows)
        {
            this.RowHandle = rowHandle;
            this.PrevRowHandle = prevRowHandle;
            this.Row = row;
            this.PrevRow = prevRow;
            this.SelectedRows = selectedRows;
        }

        /// <summary>
        /// 当前选中行索引
        /// </summary>
        public int RowHandle { get; private set; }

        /// <summary>
        /// 上次选中行索引
        /// </summary>
        public int PrevRowHandle { get; private set; }

        /// <summary>
        /// 当前行数据
        /// </summary>
        public object Row { get; private set; }

        /// <summary>
        /// 上次行数据
        /// </summary>
        public object PrevRow { get; private set; }

        /// <summary>
        /// 选中行数据集合
        /// </summary>
        public List<object> SelectedRows { get; private set; }
    }
}
