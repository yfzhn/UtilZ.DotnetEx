using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using UtilZ.Dotnet.WindowsDesktopEx.Base;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    /// <summary>
    /// 分页数据显示控件列设置窗口
    /// </summary>
    public partial class FPageGridColumnsSetting : NoneBorderForm, ICollectionOwner
    {
        private readonly Control _parentControl;
        private readonly DataGridView _dgv;
        private readonly Func<string> _getColSettingFilePath;
        private Action<string, bool> _columnVisibleChangedNotify;

        /// <summary>
        /// 当前绑定的全部列集合
        /// </summary>
        private readonly BindingCollection<ColumnSettingInfo> _columnSettingInfoList;
        private PageGridColumnSettingStatus _columnSettingDockStatus = PageGridColumnSettingStatus.Hiden;
        private Size _lastParentControlSize;

        /// <summary>
        /// 获取或设置当前列设置状态
        /// </summary>
        public PageGridColumnSettingStatus ColumnSettingDockStatus
        {
            get { return _columnSettingDockStatus; }
            set
            {
                if (_columnSettingDockStatus == value)
                {
                    return;
                }

                PageGridColumnSettingStatus oldStatus = _columnSettingDockStatus;
                _columnSettingDockStatus = value;
                this.SwitchColumnSettingDockStatus(this._columnSettingDockStatus, oldStatus);
            }
        }

        internal FPageGridColumnsSetting()
        {
            InitializeComponent();
        }

        internal FPageGridColumnsSetting(Control parentControl, DataGridView dgv,
            Func<string> getColSettingFilePath, Action<string, bool> columnVisibleChangedNotify)
            : this()
        {
            this._parentControl = parentControl;
            this._dgv = dgv;
            this._getColSettingFilePath = getColSettingFilePath;
            this._columnVisibleChangedNotify = columnVisibleChangedNotify;

            this._columnSettingInfoList = new BindingCollection<ColumnSettingInfo>(this);
            this._dgv.DataSourceChanged += this.DataSourceChanged;

            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            this.IsAllowMinimize = false;
            this.IsUseInOutEffect = false;
            this._lastParentControlSize = new Size(100, 100);

            this.dgvColumnSetting.DataSource = this._columnSettingInfoList.DataSource;
            this.dgvColumnSetting.Columns[this.dgvColumnSetting.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.Visible = true;
            this.SwitchColumnSettingDockStatus(this._columnSettingDockStatus, PageGridColumnSettingStatus.Dock);

            dgvColumnSetting.LostFocus += DgvColumnSetting_LostFocus;
        }

        private void DataSourceChanged(object sender, EventArgs e)
        {
            //加载列设置
            this.LoadColumnsSetting();

            //初始化列设置
            this._columnSettingInfoList.Clear();
            foreach (DataGridViewColumn col in this._dgv.Columns)
            {
                this._columnSettingInfoList.Add(new ColumnSettingInfo(col, this.OnRaiseColumnVisibleChangedNotify));
            }
        }

        private void OnRaiseColumnVisibleChangedNotify(string columnName, bool visible)
        {
            try
            {
                this._columnVisibleChangedNotify(columnName, visible);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        #region 加载保存列设置
        private void LoadColumnsSetting()
        {
            try
            {
                var columnSettingFilePath = this._getColSettingFilePath();
                if (string.IsNullOrWhiteSpace(columnSettingFilePath) ||
                    !File.Exists(columnSettingFilePath))
                {
                    return;
                }

                var xdoc = XDocument.Load(columnSettingFilePath);
                var rootEle = xdoc.Root;
                int count = int.Parse(XmlEx.GetXElementAttributeValue(rootEle, "Count"));
                if (this._dgv.Columns.Count != count)
                {
                    //项数不同,同数据源名称,但是内容有变,不加载,删除无效的设置文件
                    File.Delete(columnSettingFilePath);
                    return;
                }

                //加载数据
                List<dynamic> items = new List<dynamic>();
                try
                {
                    foreach (var itemEle in rootEle.Elements("Item"))
                    {
                        dynamic item = new ExpandoObject();
                        item.Name = XmlEx.GetXElementAttributeValue(itemEle, "Name");
                        item.Width = int.Parse(XmlEx.GetXElementAttributeValue(itemEle, "Width"));
                        item.DisplayIndex = int.Parse(XmlEx.GetXElementAttributeValue(itemEle, "DisplayIndex"));
                        item.Visible = bool.Parse(XmlEx.GetXElementAttributeValue(itemEle, "Visible"));
                        items.Add(item);

                        if (!this._dgv.Columns.Contains(item.Name))
                        {
                            //不包含该列,同数据源名称,但是内容有变,不加载
                            return;
                        }
                    }
                }
                catch (Exception exi)
                {
                    //数据有错误,不加载,删除无效的设置文件
                    Loger.Error(exi);
                    File.Delete(columnSettingFilePath);
                    return;
                }

                foreach (dynamic item in items)
                {
                    DataGridViewColumn col = this._dgv.Columns[item.Name];
                    col.Width = item.Width;
                    col.DisplayIndex = item.DisplayIndex;
                    col.Visible = item.Visible;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            finally
            {
                this.OnRaiseColumnVisibleChangedNotify(null, true);
            }
        }

        private void SaveCurrentColumnsSetting()
        {
            try
            {
                var columnSettingFilePath = this._getColSettingFilePath();
                if (string.IsNullOrWhiteSpace(columnSettingFilePath))
                {
                    return;
                }

                DirectoryInfoEx.CheckFilePathDirectory(columnSettingFilePath);
                var xdoc = new XDocument();
                var rootEle = new XElement("DGVColumnsLayout");
                rootEle.Add(new XAttribute("Count", this._dgv.Columns.Count));

                foreach (DataGridViewColumn col in this._dgv.Columns)
                {
                    var itemEle = new XElement("Item");
                    itemEle.Add(new XAttribute("Name", col.Name));
                    itemEle.Add(new XAttribute("Width", col.Width));
                    itemEle.Add(new XAttribute("DisplayIndex", col.DisplayIndex));
                    itemEle.Add(new XAttribute("Visible", col.Visible));
                    rootEle.Add(itemEle);
                }

                xdoc.Add(rootEle);

                string dir = Path.GetDirectoryName(columnSettingFilePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                xdoc.Save(columnSettingFilePath);
            }
            catch (IOException ioex)
            {
                MessageBox.Show(ioex.Message);
                Loger.Error(ioex);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
        #endregion

        private void FPageGridColumnsSetting_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

        }

        #region 右键菜单
        private void pictureBoxMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip.Show(Cursor.Position);
        }

        private void tsmiAutoHiden_Click(object sender, EventArgs e)
        {
            this.ColumnSettingDockStatus = PageGridColumnSettingStatus.Hiden;
        }

        private void tsmiFloat_Click(object sender, EventArgs e)
        {
            this.ColumnSettingDockStatus = PageGridColumnSettingStatus.Float;
        }

        private void tsmiDock_Click(object sender, EventArgs e)
        {
            this.ColumnSettingDockStatus = PageGridColumnSettingStatus.Dock;
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            this.SaveCurrentColumnsSetting();
        }
        #endregion

        /// <summary>
        /// 切换列设置状态
        /// </summary>
        /// <param name="status">列设置状态</param>
        /// <param name="oldStatus">旧状态</param>
        private void SwitchColumnSettingDockStatus(PageGridColumnSettingStatus status, PageGridColumnSettingStatus oldStatus)
        {
            this._columnSettingDockStatus = status;
            switch (status)
            {
                case PageGridColumnSettingStatus.Dock:
                    this.StatusDock();
                    break;
                case PageGridColumnSettingStatus.Float:
                    this.StatusFloat();
                    break;
                case PageGridColumnSettingStatus.Hiden:
                    this.StatusHiden();
                    break;
                case PageGridColumnSettingStatus.Disable:
                    this.Visible = false;
                    break;
                default:
                    MessageBox.Show(string.Format("不识别的状态:{0}", status.ToString()));
                    break;
            }
        }

        private readonly Size _labelTitleSizeH = new Size(53, 12);
        private readonly Size _labelTitleSizeV = new Size(15, 50);
        private void StatusHiden()
        {
            this.Visible = true;
            this._lastParentControlSize = this.Size;
            this.labelTitle.Size = this._labelTitleSizeV;
            this.Width = this._parentControl.MinimumSize.Width;
            this.dgvColumnSetting.Visible = false;
            this.IsDisableDragMoveForm = true;
            this.FormResizeStyle = ResizeStyle.None;
            this.HidenColSettingDgv();
        }

        private void StatusFloat()
        {
            this.Visible = true;
            this._parentControl.Controls.Remove(this);
            this.TopLevel = true;
            this.TopMost = true;
            this.ShowInTaskbar = true;
            this.ShowInTaskbar = false;
            this.IsDisableDragMoveForm = false;
            this.Location = Cursor.Position;
            this.FormResizeStyle = ResizeStyle.All;
            this.ShowColSettingDgv();
        }

        private void StatusDock()
        {
            this.Visible = true;
            this.TopLevel = false;
            this.IsDisableDragMoveForm = false;
            this._parentControl.Controls.Add(this);
            this.FormResizeStyle = ResizeStyle.Left;
            this.ShowColSettingDgv();
        }

        private void HidenColSettingDgv()
        {
            this.TopLevel = false;
            this._parentControl.Controls.Add(this);
            pictureBoxMenu.Visible = false;
        }

        private void ShowColSettingDgv()
        {
            this.Size = this._lastParentControlSize;
            this.labelTitle.Size = this._labelTitleSizeH;
            this.dgvColumnSetting.Visible = true;
            pictureBoxMenu.Visible = true;
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            switch (this._columnSettingDockStatus)
            {
                case PageGridColumnSettingStatus.Dock:
                    this.tsmiFloat.Enabled = true;
                    this.tsmiAutoHiden.Enabled = true;
                    this.tsmiDock.Enabled = false;
                    break;
                case PageGridColumnSettingStatus.Float:
                    this.tsmiFloat.Enabled = false;
                    this.tsmiAutoHiden.Enabled = true;
                    this.tsmiDock.Enabled = true;
                    break;
                case PageGridColumnSettingStatus.Hiden:
                    this.tsmiFloat.Enabled = true;
                    this.tsmiAutoHiden.Enabled = false;
                    this.tsmiDock.Enabled = true;
                    break;
                default:
                    MessageBox.Show(string.Format("不识别的状态:{0}", this._columnSettingDockStatus.ToString()));
                    return;
            }
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            PageGridColumnSettingStatus newStatus;
            if (this._columnSettingDockStatus == PageGridColumnSettingStatus.Hiden)
            {
                newStatus = PageGridColumnSettingStatus.Dock;
            }
            else if (this._columnSettingDockStatus == PageGridColumnSettingStatus.Dock)
            {
                newStatus = PageGridColumnSettingStatus.Hiden;
            }
            else
            {
                return;
            }

            this.SwitchColumnSettingDockStatus(newStatus, this._columnSettingDockStatus);
        }

        internal void HidenColumn(string hidenColumnName)
        {
            var hidenColumnSettingInfo = this._columnSettingInfoList.Where(t => { return PageGridControlCommon.CompareColumnName(t.ColumnName, hidenColumnName); }).FirstOrDefault();
            if (hidenColumnSettingInfo != null)
            {
                hidenColumnSettingInfo.Visible = false;
            }
        }

        #region 自动隐藏
        private void labelTitle_MouseEnter(object sender, EventArgs e)
        {
            if (this._columnSettingDockStatus == PageGridColumnSettingStatus.Hiden)
            {
                this.StatusDock();
                dgvColumnSetting.Focus();
            }
        }

        private void DgvColumnSetting_LostFocus(object sender, EventArgs e)
        {
            if (this._columnSettingDockStatus == PageGridColumnSettingStatus.Hiden)
            {
                this.StatusHiden();
            }
        }
        #endregion
    }

    /// <summary>
    /// 列设置类
    /// </summary>
    public class ColumnSettingInfo : NotifyPropertyChangedAbs
    {
        /// <summary>
        /// 列可见性改变通知委托
        /// </summary>
        private Action<string, bool> _columnVisibleChangedNotify;

        /// <summary>
        /// 列名
        /// </summary>
        [Browsable(false)]
        public string ColumnName { get; private set; }

        private bool _visible;
        /// <summary>
        /// 是否显示
        /// </summary>
        [DisplayName("是否显示")]
        public bool Visible
        {
            get { return this._visible; }
            set
            {
                if (this._visible == value)
                {
                    return;
                }

                this._visible = value;
                this.OnRaisePropertyChanged("Visible");
                this._columnVisibleChangedNotify(this.ColumnName, this._visible);
            }
        }

        private string _headerText;
        /// <summary>
        /// 列标题
        /// </summary>
        [DisplayName("列")]
        public string HeaderText
        {
            get { return this._headerText; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ColumnSettingInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="columnVisibleChangedNotify">列可见性改变通知委托</param>
        public ColumnSettingInfo(DataGridViewColumn column, Action<string, bool> columnVisibleChangedNotify)
        {
            this.ColumnName = column.Name;
            this._headerText = column.HeaderText;
            this._visible = column.Visible;
            this._columnVisibleChangedNotify = columnVisibleChangedNotify;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", this._headerText, ColumnName);
        }
    }
}
