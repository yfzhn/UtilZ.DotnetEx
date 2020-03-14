using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    public partial class UCPageGridControl
    {
        private bool _enableColumnHeaderContextMenuStripHiden = false;
        /// <summary>
        /// 获取或设置是否启用列标题右键菜单隐藏列[true:启用;false:禁用]
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("是否启用列标题右键菜单隐藏列[true:启用;false:禁用]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool EnableColumnHeaderContextMenuStripHiden
        {
            get { return _enableColumnHeaderContextMenuStripHiden; }
            set
            {
                if (_enableColumnHeaderContextMenuStripHiden == value)
                {
                    return;
                }

                _enableColumnHeaderContextMenuStripHiden = value;
                if (_enableColumnHeaderContextMenuStripHiden)
                {
                    this._dataGridView.ColumnHeaderMouseClick += this.dataGridView_ColumnHeaderMouseClick;
                }
                else
                {
                    this._dataGridView.ColumnHeaderMouseClick -= this.dataGridView_ColumnHeaderMouseClick;
                }
            }
        }

        private readonly ReadOnlyCollection<Control> _pageControls;
        private bool _alignDirection = true;
        /// <summary>
        /// 获取或设置分页控件对齐方向[true:LeftToRight;false:RightToLeft]
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("分页控件对齐方向")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool AlignDirection
        {
            get { return _alignDirection; }
            set
            {
                if (_alignDirection == value)
                {
                    return;
                }

                _alignDirection = value;

                Control[] pageControls;
                FlowDirection flowDirection;
                if (_alignDirection)
                {
                    pageControls = _pageControls.ToArray();
                    flowDirection = FlowDirection.LeftToRight;

                }
                else
                {
                    pageControls = _pageControls.Reverse().ToArray();
                    flowDirection = FlowDirection.RightToLeft;
                }

                panelPage.Controls.Clear();
                panelPage.FlowDirection = flowDirection;
                panelPage.Controls.AddRange(pageControls);
            }
        }

        /// <summary>
        /// 获取或设置列设置区域宽度
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("列设置区域宽度")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public int ColumnSettingWidth
        {
            get { return _fPageGridColumnsSetting.Width; }
            set { _fPageGridColumnsSetting.Width = value; }
        }

        /// <summary>
        /// 获取或设置是否启用分页栏
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("分页栏是否可见[true:可见;false:隐藏]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool EnablePagingBar
        {
            get { return panelPage.Visible; }
            set { panelPage.Visible = value; }
        }

        ///// <summary>
        ///// 获取或设置列设置是否可见
        ///// </summary>
        //[EditorBrowsable(EditorBrowsableState.Always)]
        //[Description("列设置是否可见[true:可见;false:隐藏]")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //[Category("扩展")]
        //public bool EnableColumnSetting
        //{
        //    get { return this._fPageGridColumnsSetting.Visible; }
        //    set { this._fPageGridColumnsSetting.Visible = value; }
        //}

        ///// <summary>
        ///// 列显示高级设置是否可见
        ///// </summary>
        //[EditorBrowsable(EditorBrowsableState.Always)]
        //[Description("列显示高级设置是否可见")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //[Category("扩展")]
        //public bool AdvanceSettingVisible
        //{
        //    get { return this._fPageGridColumnsSetting.AdvanceSettingVisible; }
        //    set { this._fPageGridColumnsSetting.AdvanceSettingVisible = value; }
        //}

        /// <summary>
        /// 获取或设置列设置控件状态
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("列设置控件状态")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public PageGridColumnSettingStatus ColumnSettingStatus
        {
            get { return _fPageGridColumnsSetting.ColumnSettingDockStatus; }
            set
            {
                if (_fPageGridColumnsSetting.ColumnSettingDockStatus == value)
                {
                    return;
                }

                _fPageGridColumnsSetting.ColumnSettingDockStatus = value;
            }
        }

        /// <summary>
        /// 是否启用显示行号显示
        /// </summary>
        private bool _enableRowNum = false;
        /// <summary>
        /// 获取或设置是否启用显示行号显示
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("行号是否显示[true:显示行号;false:不显示行号]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool EnableRowNum
        {
            get { return _enableRowNum; }
            set
            {
                if (_enableRowNum == value)
                {
                    return;
                }

                _enableRowNum = value;
                if (_enableRowNum)
                {
                    this._dataGridView.RowPostPaint += DataGridView_RowPostPaint;
                }
                else
                {
                    this._dataGridView.RowPostPaint -= DataGridView_RowPostPaint;
                }
            }
        }

        private bool _enableUserAdjustPageSize = true;
        /// <summary>
        /// 获取或设置是否启用用户调整分页大小
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("是否启用用户调整分页大小[true:显示行号;false:不显示行号]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool EnableUserAdjustPageSize
        {
            get { return _enableUserAdjustPageSize; }
            set
            {
                if (_enableUserAdjustPageSize == value)
                {
                    return;
                }

                _enableUserAdjustPageSize = value;
                this.SwitchPageSizeVisible(_enableUserAdjustPageSize);
            }
        }

        /// <summary>
        /// 切换分页大小可见性
        /// </summary>
        /// <param name="visible">分页大小可见性</param>
        private void SwitchPageSizeVisible(bool visible)
        {
            label2.Visible = visible;
            label4.Visible = visible;
            numPageSize.Visible = visible;
        }

        /// <summary>
        /// 获取或设置分页大小最大值
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("分页大小最大值")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public int PageSizeMaximum
        {
            get { return (int)numPageSize.Maximum; }
            set
            {
                if (numPageSize.Maximum == value)
                {
                    return;
                }

                numPageSize.Maximum = value;
            }
        }

        /// <summary>
        /// 获取或设置分页大小最小值
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("分页大小最小值")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public int PageSizeMinimum
        {
            get { return (int)numPageSize.Minimum; }
            set
            {
                if (numPageSize.Minimum == value)
                {
                    return;
                }

                numPageSize.Minimum = value;
            }
        }

        /// <summary>
        /// 最后一列显示模式是否默认Fill[true:Fill;false:系统默认;默认值:false]
        /// </summary>
        private bool _isLastColumnAutoSizeModeFill = true;
        /// <summary>
        /// 获取或设置最后一列显示模式是否默认Fill[true:Fill;false:系统默认;默认值:true]
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("最后一列显示模式是否默认Fill[true:Fill;false:系统默认;默认值:true]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public bool IsLastColumnAutoSizeModeFill
        {
            get { return _isLastColumnAutoSizeModeFill; }
            set
            {
                if (_isLastColumnAutoSizeModeFill == value)
                {
                    return;
                }

                _isLastColumnAutoSizeModeFill = value;
                if (this._dataGridView.Columns.Count > 0)
                {
                    //this.SetLastDataGridViewColumnAutoSizeModeFill2();
                    this.SetLastDataGridViewColumnAutoSizeModeFill(null);
                }
            }
        }

        /// <summary>
        /// 绘制表格行号
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, _dataGridView.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), _dataGridView.RowHeadersDefaultCellStyle.Font, rectangle,
                _dataGridView.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 获取表格控件
        /// </summary>
        [Description("表格控件")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("扩展")]
        public DataGridView GridControl
        {
            get { return _dataGridView; }
        }



        /// <summary>
        /// 用户设置数据存放目录
        /// </summary>
        private string _settingDirectory;
        /// <summary>
        /// 获取或设置用户设置数据存放目录
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("获取或设置用户设置数据存放目录")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("扩展")]
        public string SettingDirectory
        {
            get { return _settingDirectory; }
            set { _settingDirectory = value; }
        }

        /// <summary>
        /// 获取
        /// </summary>
        private string _dataSourceName = null;
        /// <summary>
        /// 获取当前数据源名称
        /// </summary>
        [Browsable(false)]
        public string DataSourceName
        {
            get { return _dataSourceName; }
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        [Browsable(false)]
        public object DataSource
        {
            get { return _dataGridView.DataSource; }
        }

        /// <summary>
        /// 当前分页信息
        /// </summary>
        private PageInfo _pageInfo = null;
        /// <summary>
        /// 获取当前分页信息
        /// </summary>
        [Browsable(false)]
        public PageInfo PageInfo
        {
            get { return _pageInfo; }
        }

        /// <summary>
        /// 获取或设置焦点行索引
        /// </summary>
        [Browsable(false)]
        public int FocusedRowIndex
        {
            get
            {
                if (this._dataGridView.CurrentRow != null)
                {
                    return this._dataGridView.CurrentRow.Index;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (value > this._dataGridView.RowCount - 1 ||
                    value < -1)
                {
                    throw new ArgumentOutOfRangeException("value", "焦点行索引值超出行数范围");
                }

                foreach (DataGridViewRow row in this._dataGridView.SelectedRows)
                {
                    row.Selected = false;
                }

                if (value >= 0)
                {
                    this._dataGridView.Rows[value].Selected = true;
                }
            }
        }

        /// <summary>
        /// 获取选中行集合
        /// </summary>
        [Description("获取选中行集合")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DataGridViewRow[] SelectedRows
        {
            get
            {
                Dictionary<int, DataGridViewRow> dicSelectedRows = new Dictionary<int, DataGridViewRow>();
                foreach (DataGridViewCell cell in this._dataGridView.SelectedCells)
                {
                    if (dicSelectedRows.ContainsKey(cell.RowIndex))
                    {
                        continue;
                    }
                    else
                    {
                        dicSelectedRows.Add(cell.RowIndex, cell.OwningRow);
                    }
                }

                return dicSelectedRows.Values.ToArray();
            }
        }
    }
}
