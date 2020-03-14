using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// DataGridView扩展类
    /// </summary>
    [ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]//定义工具栏中的图标
    public class DataGridViewEx : DataGridView
    {
        /// <summary>
        /// DataGridView绑定数据
        /// </summary>
        /// <param name="dgv">DataGridView</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="hidenColumns">隐藏列集合</param>
        /// <param name="colHeadInfos">列标题映射字典[key:列名;value:列标题;默认为null]</param>
        /// <param name="allowEditColumns">允许编辑的列集合[当为null或空时,全部列都可编辑;默认为null]</param>
        public static void DataBinding(DataGridView dgv, object dataSource, IEnumerable<string> hidenColumns = null, Dictionary<string, string> colHeadInfos = null, IEnumerable<string> allowEditColumns = null)
        {
            if (dgv == null)
            {
                throw new ArgumentNullException("dgv");
            }

            if (dgv.DataSource == dataSource)
            {
                return;
            }

            if (dgv.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                dgv.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                var srcSelectionMode = dgv.SelectionMode;
                dgv.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                dgv.DataSource = dataSource;
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }

                dgv.SelectionMode = srcSelectionMode;
            }
            else
            {
                dgv.DataSource = dataSource;
            }

            if (dataSource == null)
            {
                return;
            }

            if (hidenColumns == null)
            {
                hidenColumns = new List<string>();
            }

            if (colHeadInfos == null)
            {
                colHeadInfos = new Dictionary<string, string>();
            }

            if (allowEditColumns == null)
            {
                allowEditColumns = new List<string>();
            }

            string caption = null;
            string fieldName = null;
            bool isReadOnly;
            var dt = dgv.DataSource as System.Data.DataTable;
            dgv.ReadOnly = allowEditColumns.Count() == 0;
            foreach (DataGridViewColumn gridColumn in dgv.Columns)
            {
                //获取字段名
                fieldName = gridColumn.Name;
                if (hidenColumns.Contains(fieldName))
                {
                    gridColumn.Visible = false;
                    break;
                }

                isReadOnly = !allowEditColumns.Contains(fieldName);
                //设置为可编辑性
                if (isReadOnly != gridColumn.ReadOnly)
                {
                    gridColumn.ReadOnly = isReadOnly;
                }

                //设置显示标题
                if (colHeadInfos.ContainsKey(fieldName))
                {
                    caption = colHeadInfos[fieldName];
                }
                else if (dt != null && dt.Columns.Contains(fieldName))
                {
                    caption = dt.Columns[fieldName].Caption;
                }

                if (!string.IsNullOrEmpty(caption))
                {
                    gridColumn.HeaderText = caption;
                    caption = null;
                }
            }
        }

        /// <summary>
        /// 鼠标右键是否选中行
        /// </summary>
        private bool _mouseRightButtonChangeSelectedRow = false;

        /// <summary>
        /// 鼠标右键是否选中行[true:选中;false:不选中]
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("鼠标右键是否选中行[true:选中;false:不选中]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool MouseRightButtonChangeSelectedRow
        {
            get { return _mouseRightButtonChangeSelectedRow; }
            set
            {
                if (_mouseRightButtonChangeSelectedRow == value)
                {
                    return;
                }

                _mouseRightButtonChangeSelectedRow = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridViewEx() : base()
        {

        }

        /// <summary>
        /// 重写OnCellMouseDown
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseDown(e);

            if (this._mouseRightButtonChangeSelectedRow)
            {
                this.UpdateSelectedRow(e);
            }
        }

        /// <summary>
        /// 更新选中行
        /// </summary>
        /// <param name="e">e</param>
        private void UpdateSelectedRow(DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            //清空之前的选中项
            this.ClearSelection();
            if (!this.Rows[e.RowIndex].Selected)
            {
                switch (this.SelectionMode)
                {
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        this.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                        break;
                    case DataGridViewSelectionMode.FullRowSelect:
                        this.Rows[e.RowIndex].Selected = true;
                        break;
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                    case DataGridViewSelectionMode.FullColumnSelect:
                        this.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                        break;
                }
            }
        }
    }
}
