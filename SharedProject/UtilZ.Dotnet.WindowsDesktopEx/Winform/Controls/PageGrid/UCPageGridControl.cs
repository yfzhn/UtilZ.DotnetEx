using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Dynamic;
using System.Collections.ObjectModel;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 分页数据表格
    /// </summary>
    public partial class UCPageGridControl : UserControl
    {
        private readonly DataGridViewEx _dataGridView;

        /// <summary>
        /// 列显示设置窗口
        /// </summary>
        private readonly FPageGridColumnsSetting _fPageGridColumnsSetting;

        #region 构造函数-初始化
        /// <summary>
        /// 构造函数
        /// </summary>
        public UCPageGridControl()
        {
            InitializeComponent();

            this._dataGridView = this.CreateDataGridView();
            this._fPageGridColumnsSetting = this.CreateColSetting();

            this.panelContent.Controls.Add(this._dataGridView);
            this.panelContent.Controls.Add(this._fPageGridColumnsSetting);

            this._dataGridView.SelectionChanged += GridView_SelectionChanged;
            this._dataGridView.MouseClick += GridView_MouseClick;
            this._dataGridView.MouseDoubleClick += GridView_MouseDoubleClick;

            this.SetPageInfo(null);

            var pageControls = new List<Control>();
            foreach (Control control in panelPage.Controls)
            {
                pageControls.Add(control);
            }

            this._pageControls = new ReadOnlyCollection<Control>(pageControls);
            //初始化列设置存放目录
            this._settingDirectory = PageGridControlCommon.GetDefaultSettingDirectory();
            this.numPageIndex.ValueChanged += this.numPageIndex_ValueChanged;
            this.EnableRowNum = true;
            this.EnableColumnHeaderContextMenuStripHiden = true;
        }

        private FPageGridColumnsSetting CreateColSetting()
        {
            var fPageGridColumnsSetting = new FPageGridColumnsSetting(this.panelContent, this._dataGridView,
                this.GetColSettingFilePath, this.ColumnVisibleChangedNotify);
            fPageGridColumnsSetting.Dock = DockStyle.Right;
            fPageGridColumnsSetting.TopLevel = false;
            fPageGridColumnsSetting.Show();
            return fPageGridColumnsSetting;
        }

        #region 列显示顺序以及最后一列AutoSizeMode值设定
        /// <summary>
        /// 列可见性改变通知处理方法
        /// </summary>
        /// <param name="columnName">DataGridViewColumn</param>
        /// <param name="visible"></param>
        private void ColumnVisibleChangedNotify(string columnName, bool visible)
        {
            if (this._dataGridView.Columns.Count < 1)
            {
                return;
            }

            DataGridViewColumn targetCol = null;
            foreach (DataGridViewColumn col in this._dataGridView.Columns)
            {
                if (PageGridControlCommon.CompareColumnName(col.Name, columnName))
                {
                    col.Visible = visible;
                    targetCol = col;
                    break;
                }
            }

            this.SetLastDataGridViewColumnAutoSizeModeFill(targetCol);
            //this.SetLastDataGridViewColumnAutoSizeModeFill2();
        }

        private void SetLastDataGridViewColumnAutoSizeModeFill2()
        {
            //重置所有AutoSizeMode设置为Fill的列
            this.ResetDataGridViewColumnAutoSizeModeNotSet();

            if (!this._isLastColumnAutoSizeModeFill)
            {
                //最后一列AutoSizeMode不需要设置为Fill,返回
                return;
            }

            //设置最后一列AutoSizeMode为Fill
            for (int i = this._dataGridView.Columns.Count - 1; i >= 0; i--)
            {
                if (this._dataGridView.Columns[i].Visible)
                {
                    this._dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    break;
                }
            }
        }

        private void SetLastDataGridViewColumnAutoSizeModeFill(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn == null)
            {
                this.ResetDataGridViewColumnAutoSizeModeNotSet();

                if (this._isLastColumnAutoSizeModeFill)
                {
                    //this._dataGridView.Columns[this._dataGridView.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    //倒数找到第一个显示列,设置AutoSizeMode为Fill,返回
                    for (int i = this._dataGridView.Columns.Count - 1; i >= 0; i--)
                    {
                        if (this._dataGridView.Columns[i].Visible)
                        {
                            this._dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            break;
                        }
                    }
                }

                return;
            }
            else
            {
                if (!this._isLastColumnAutoSizeModeFill)
                {
                    //最后一列AutoSizeMode不需要设置为Fill,重置所有AutoSizeMode设置为Fill的列,返回
                    this.ResetDataGridViewColumnAutoSizeModeNotSet();
                    return;
                }

                if (dataGridViewColumn.Visible)
                {
                    //隐藏->可见
                    for (int i = dataGridViewColumn.DisplayIndex + 1; i < this._dataGridView.Columns.Count; i++)
                    {
                        if (this._dataGridView.Columns[i].Visible)
                        {
                            //不是最后一列,返回
                            return;
                        }
                    }

                    //重置所有AutoSizeMode设置为Fill的列
                    this.ResetDataGridViewColumnAutoSizeModeNotSet();

                    //设置当前列AutoSizeMode为Fill,返回
                    dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    //可见->隐藏
                    if (dataGridViewColumn.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    {
                        //AutoSizeMode不是Fill,说明不是最后一个显示列,返回
                        return;
                    }

                    //倒数找到第一个显示列,设置AutoSizeMode为Fill,返回
                    for (int i = dataGridViewColumn.DisplayIndex - 1; i >= 0; i--)
                    {
                        if (this._dataGridView.Columns[i].Visible)
                        {
                            this._dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            return;
                        }
                    }
                }
            }
        }

        private void ResetDataGridViewColumnAutoSizeModeNotSet()
        {
            foreach (DataGridViewColumn col in this._dataGridView.Columns)
            {
                if (col.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                }
            }
        }

        private void _dataGridView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (!this._isLastColumnAutoSizeModeFill || this._dataGridView.Columns.Count == 0)
            {
                return;
            }

            DataGridViewColumn fillCol = null, maxDisplayIndexCol = this._dataGridView.Columns[0];
            foreach (DataGridViewColumn col in this._dataGridView.Columns)
            {
                if (!col.Visible)
                {
                    continue;
                }

                if (col.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                {
                    fillCol = col;
                }

                if (col.DisplayIndex > maxDisplayIndexCol.DisplayIndex)
                {
                    maxDisplayIndexCol = col;
                }
            }

            if (maxDisplayIndexCol == fillCol)
            {
                //理论上不可能进入此分支
                return;
            }

            if (fillCol != null)
            {
                fillCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            }

            maxDisplayIndexCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        #endregion

        private string GetColSettingFilePath()
        {
            if (string.IsNullOrEmpty(this._dataSourceName) ||
                   string.IsNullOrEmpty(this._settingDirectory) ||
                   this._dataGridView.Columns.Count == 0)
            {
                return null;
            }

            return PageGridControlCommon.GetGridColumnSettingFilePath(this._settingDirectory, this._dataSourceName);
        }

        private DataGridViewEx CreateDataGridView()
        {
            var dgv = new DataGridViewEx();
            dgv.AllowDrop = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = true;
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            dgv.Location = new System.Drawing.Point(0, 0);
            dgv.MouseRightButtonChangeSelectedRow = false;
            dgv.Name = "dataGridView";
            dgv.ReadOnly = true;
            dgv.RowTemplate.Height = 23;
            dgv.Size = new System.Drawing.Size(225, 86);
            dgv.TabIndex = 2;
            dgv.VirtualMode = true;

            dgv.MultiSelect = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            return dgv;
        }
        #endregion


        #region 接口事件调用
        /// <summary>
        /// 表格控件鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            object row = null;
            int rowIndex = -1;
            //DataGridView.HitTestInfo hitTestInfo = this.dataGridView.HitTest(e.X, e.Y);
            //if (hitTestInfo != null)
            //{
            //    rowIndex = hitTestInfo.RowIndex;
            //    row = this.dataGridView.Rows[rowIndex].DataBoundItem;
            //}

            if (this._dataGridView.CurrentRow != null)
            {
                rowIndex = this._dataGridView.CurrentRow.Index;
                row = this._dataGridView.CurrentRow.DataBoundItem;
            }

            var handler = this.DataRowDoubleClick;
            if (handler != null)
            {
                this.DataRowDoubleClick(sender, new DataRowClickArgs(row, rowIndex));
            }
        }

        /// <summary>
        /// 表格控件鼠标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                object row = null;
                int rowIndex = -1;
                if (this._dataGridView.CurrentRow != null)
                {
                    rowIndex = this._dataGridView.CurrentRow.Index;
                    row = this._dataGridView.CurrentRow.DataBoundItem;
                }

                var handler = this.DataRowClick;
                if (handler != null)
                {
                    this.DataRowClick(sender, new DataRowClickArgs(row, rowIndex));
                }
            }
            else
            {
                //if (e.Y < this._dataGridView.ColumnHeadersHeight && e.X > this._dataGridView.RowHeadersWidth)
                //{
                //    return;
                //}

                //if (e.Button != System.Windows.Forms.MouseButtons.Right || e.Clicks != 1)
                //{
                //    return;
                //}

                //if (this._dgvContextMenuStrip != null)
                //{
                //    this._dgvContextMenuStrip.Show(Cursor.Position);
                //}
            }
        }

        /// <summary>
        /// 前次选中的行
        /// </summary>
        private DataGridViewRow _prevSelectedRow = null;
        /// <summary>
        /// 表格选中行改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_SelectionChanged(object sender, EventArgs e)
        {
            int rowHandle = -1;//当前选中行索引
            int prevRowHandle = -1;//上次选中行索引
            object row = null;//当前行数据
            object prevRow = null;//上次行数据

            try
            {
                //前一次选中行信息
                if (this._prevSelectedRow != null)
                {
                    prevRowHandle = this._prevSelectedRow.Index;
                    prevRow = this._prevSelectedRow.DataBoundItem;
                }

                this._prevSelectedRow = this._dataGridView.CurrentRow;
                if (this._dataGridView.CurrentRow != null)
                {
                    row = this._prevSelectedRow.DataBoundItem;
                    rowHandle = this._prevSelectedRow.Index;
                }

                //当前选中行数据集合
                List<object> selectedRowDatas = new List<object>();
                object[] selectedRows = this.SelectedRows;
                foreach (DataGridViewRow selectedRow in selectedRows)
                {
                    selectedRowDatas.Add(selectedRow.DataBoundItem);
                }

                var handler = this.DataRowSelectionChanged;
                if (handler != null)
                {
                    handler(this, new DataRowSelectionChangedArgs(rowHandle, prevRowHandle, row, prevRow, selectedRows.ToList()));
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCPageGridControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
        }

        #region 列标题右键菜单隐藏列
        /// <summary>
        /// 目标列
        /// </summary>
        private DataGridViewColumn _preHidenCol = null;
        /// <summary>
        /// 表格列头右键单击事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right && e.Clicks == 1 &&
                   this.ColumnSettingStatus != PageGridColumnSettingStatus.Disable)
                {
                    this._preHidenCol = this._dataGridView.Columns[e.ColumnIndex];
                    //cms.Show(MousePosition.X, MousePosition.Y);
                    this.cmsColVisibleSetting.Show(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void tsmiHidenCol_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._preHidenCol == null)
                {
                    return;
                }

                this._fPageGridColumnsSetting.HidenColumn(this._preHidenCol.Name);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 分页
        private void PageQuery(long pageIndex)
        {
            this.PrimitiveSetPageInfo(new PageInfo(this._pageInfo.PageSize, pageIndex, this._pageInfo.TotalCount));
            this.OnRaiseQueryData(new QueryDataArgs(pageIndex, this._pageInfo.PageSize));
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            if (this._pageInfo == null)
            {
                return;
            }

            int pageIndex = 1;
            if (pageIndex == this._pageInfo.PageIndex)
            {
                return;
            }

            this.PageQuery(pageIndex);
        }

        private void btnPrePage_Click(object sender, EventArgs e)
        {
            if (this._pageInfo == null)
            {
                return;
            }

            var pageIndex = this._pageInfo.PageIndex - 1;
            if (pageIndex < 1)
            {
                return;
            }

            this.PageQuery(pageIndex);
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (this._pageInfo == null)
            {
                return;
            }

            long pageIndex = this._pageInfo.PageIndex + 1;
            if (pageIndex > this._pageInfo.PageCount)
            {
                return;
            }

            this.PageQuery(pageIndex);
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            if (this._pageInfo == null)
            {
                return;
            }

            long pageIndex = this._pageInfo.PageCount;
            if (this._pageInfo.PageIndex == pageIndex)
            {
                return;
            }

            this.PageQuery(pageIndex);
        }

        private void numPageIndex_ValueChanged(object sender, EventArgs e)
        {
            if (this._pageInfo == null)
            {
                return;
            }

            int pageIndex = (int)numPageIndex.Value;
            if (pageIndex == this._pageInfo.PageIndex)
            {
                return;
            }

            this.PageQuery(pageIndex);
        }

        private void numPageSize_ValueChanged(object sender, EventArgs e)
        {
            var handler = this.PageSizeChanged;
            if (handler != null)
            {
                handler(this, new PageSizeChangedArgs((int)numPageSize.Value));
            }
        }
        #endregion
    }
}
