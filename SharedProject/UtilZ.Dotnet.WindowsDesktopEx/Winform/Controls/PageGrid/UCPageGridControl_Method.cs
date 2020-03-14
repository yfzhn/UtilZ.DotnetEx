using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Base.MemoryCache;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    public partial class UCPageGridControl
    {
        private TypeCode _lastModelTypeCode = TypeCode.Empty;

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataSourceName">数据源名称</param>
        /// <param name="hidenColumns">隐藏列集合</param>
        /// <param name="colHeadInfos">列标题映射字典[key:列名;value:列标题;默认为null]</param>
        /// <param name="allowEditColumns">允许编辑的列集合[当为null或空时,全部列都可编辑;默认为null]</param>
        public void ShowData(object dataSource, string dataSourceName = null, IEnumerable<string> hidenColumns = null, Dictionary<string, string> colHeadInfos = null, IEnumerable<string> allowEditColumns = null)
        {
            this._lastModelTypeCode = TypeCode.Empty;
            if (this._dataGridView.DataSource == dataSource)
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

            this.DataBinding(dataSourceName, dataSource, hidenColumns, colHeadInfos, allowEditColumns);
        }

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <typeparam name="T">数据模型类型</typeparam>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataSourceName">数据源名称</param>
        /// <param name="hidenColumns">隐藏列集合</param>
        /// <param name="colHeadInfos">列标题映射字典[key:列名;value:列标题;默认为null]</param>
        /// <param name="allowEditColumns">允许编辑的列集合[当为null或空时,全部列都可编辑;默认为null]</param>
        public void ShowData<T>(IEnumerable<T> dataSource, string dataSourceName = null, IEnumerable<string> hidenColumns = null, Dictionary<string, string> colHeadInfos = null, IEnumerable<string> allowEditColumns = null)
        {
            if (this._dataGridView.DataSource == dataSource)
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

            this._dataGridView.ColumnDisplayIndexChanged -= _dataGridView_ColumnDisplayIndexChanged;
            try
            {
                this.DataBinding(dataSourceName, dataSource, hidenColumns, colHeadInfos, allowEditColumns);

                Type type = typeof(T);
                if (this._lastModelTypeCode != Type.GetTypeCode(type))
                {
                    Dictionary<string, DisplayNameExAttribute> proOrderDic = this.GetPropertyDisplayOrderInfo(type);
                    if (proOrderDic.Count > 0)
                    {
                        foreach (DataGridViewColumn col in this._dataGridView.Columns)
                        {
                            if (col.Visible && proOrderDic.ContainsKey(col.Name) && proOrderDic[col.Name].OrderIndex < this._dataGridView.Columns.Count)
                            {
                                col.DisplayIndex = proOrderDic[col.Name].OrderIndex;
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                this._dataGridView.ColumnDisplayIndexChanged += _dataGridView_ColumnDisplayIndexChanged;
            }
        }

        private Dictionary<string, DisplayNameExAttribute> GetPropertyDisplayOrderInfo(Type type)
        {
            var proOrderDic = MemoryCacheEx.Get(type.FullName) as Dictionary<string, DisplayNameExAttribute>;
            if (proOrderDic == null)
            {
                proOrderDic = new Dictionary<string, DisplayNameExAttribute>();
                var proInfos = type.GetProperties();
                Type displayOrderType = typeof(DisplayNameExAttribute);
                var noneOrderProInfos = new List<Tuple<DisplayNameExAttribute, PropertyInfo>>();

                foreach (var proInfo in proInfos)
                {
                    object[] objs = proInfo.GetCustomAttributes(displayOrderType, true);
                    if (objs != null && objs.Length > 0)
                    {
                        noneOrderProInfos.Add(new Tuple<DisplayNameExAttribute, PropertyInfo>((DisplayNameExAttribute)objs[0], proInfo));
                    }
                    else
                    {
                        //noneOrderProInfos.Add(new Tuple<DisplayNameExAttribute, PropertyInfo>(new DisplayOrderAttribute(int.MaxValue), proInfo));
                    }
                }

                var orderProInfos = noneOrderProInfos.OrderBy((p) => { return p.Item1.OrderIndex; });
                foreach (var item in orderProInfos)
                {
                    proOrderDic.Add(item.Item2.Name, item.Item1);
                }

                MemoryCacheEx.Set(type.FullName, proOrderDic, 10 * 60 * 1000);//10分钟过期
            }

            return proOrderDic;
        }

        /// <summary>
        /// DataGridView绑定数据
        /// </summary>
        /// <param name="dataSourceName">数据源名称</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="hidenColumns">隐藏列集合</param>
        /// <param name="colHeadInfos">列标题映射字典[key:列名;value:列标题;默认为null]</param>
        /// <param name="allowEditColumns">允许编辑的列集合[当为null或空时,全部列都可编辑;默认为null]</param>
        private void DataBinding(string dataSourceName, object dataSource, IEnumerable<string> hidenColumns, Dictionary<string, string> colHeadInfos, IEnumerable<string> allowEditColumns)
        {
            if (this._dataGridView.DataSource == dataSource)
            {
                return;
            }

            this._dataSourceName = dataSourceName;//必须要先设置数据源名称,因为列设置加载设置文件时要用此字段组路径            

            if (this._dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                this._dataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                var srcSelectionMode = this._dataGridView.SelectionMode;
                this._dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                this._dataGridView.DataSource = dataSource;
                foreach (DataGridViewColumn col in this._dataGridView.Columns)
                {
                    if (col.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }

                this._dataGridView.SelectionMode = srcSelectionMode;
            }
            else
            {
                this._dataGridView.DataSource = dataSource;
            }

            if (dataSource == null)
            {
                return;
            }

            string caption = null;
            string fieldName = null;
            bool isReadOnly;
            var dt = this._dataGridView.DataSource as System.Data.DataTable;
            this._dataGridView.ReadOnly = allowEditColumns.Count() == 0;
            foreach (DataGridViewColumn gridColumn in this._dataGridView.Columns)
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
        /// 触发查询数据事件
        /// </summary>
        /// <param name="e">查询参数</param>
        public void OnRaiseQueryData(QueryDataArgs e)
        {
            var handler = this.QueryData;
            if (handler != null)
            {
                this.QueryData(this, e);
            }
        }

        /// <summary>
        /// 设置分页
        /// </summary>
        /// <param name="pageInfo">页信息</param>
        public void SetPageInfo(PageInfo pageInfo)
        {
            if (pageInfo == null || pageInfo.PageCount < 1)
            {
                labelPageCount.Text = "0页|共0条记录";
            }
            else
            {
                labelPageCount.Text = string.Format("{0}页|共{1}条记录", pageInfo.PageCount, pageInfo.TotalCount);
            }

            this.PrimitiveSetPageInfo(pageInfo);

            //外部更改查询页，触发分页查询事件
            if (pageInfo != null && pageInfo.PageCount > 0)
            {
                this.OnRaiseQueryData(new QueryDataArgs(1, pageInfo.PageSize));
            }
        }

        /// <summary>
        /// 设置分页
        /// </summary>
        /// <param name="pageInfo">页信息</param>
        private void PrimitiveSetPageInfo(PageInfo pageInfo)
        {
            this.numPageIndex.ValueChanged -= this.numPageIndex_ValueChanged;

            try
            {
                //记录当前分页信息
                this._pageInfo = pageInfo;

                //设置显示信息
                if (pageInfo == null || pageInfo.PageCount <= 0)
                {
                    //设置页数选择控件值
                    numPageIndex.Minimum = 0;
                    numPageIndex.Maximum = 0;
                    numPageIndex.Value = 0;
                    numPageIndex.Enabled = false;
                    numPageSize.Enabled = false;

                    //禁用跳转按钮
                    btnFirstPage.Enabled = false;
                    btnPrePage.Enabled = false;
                    btnNextPage.Enabled = false;
                    btnLastPage.Enabled = false;
                }
                else
                {
                    if (this.numPageSize.Maximum < pageInfo.PageSize)
                    {
                        this.numPageSize.Maximum = pageInfo.PageSize;
                    }

                    if (this.numPageSize.Value != pageInfo.PageSize)
                    {
                        this.numPageSize.Value = pageInfo.PageSize;
                    }

                    numPageIndex.Minimum = 1;
                    numPageIndex.Maximum = pageInfo.PageCount;
                    numPageIndex.Value = pageInfo.PageIndex;
                    numPageIndex.Enabled = true;
                    numPageSize.Enabled = true;

                    //启用跳转按钮
                    btnFirstPage.Enabled = false;
                    btnPrePage.Enabled = false;
                    btnNextPage.Enabled = false;
                    btnLastPage.Enabled = false;

                    //如果当前页为小于等于1,则禁首页和用前一页
                    if (pageInfo.PageIndex <= 1)
                    {
                        btnFirstPage.Enabled = false;
                        btnPrePage.Enabled = false;
                    }
                    else
                    {
                        btnFirstPage.Enabled = true;
                        btnPrePage.Enabled = true;
                    }

                    //如果当前页大于等于总页数,则禁用最后一页和下一页
                    if (pageInfo.PageIndex >= pageInfo.PageCount)
                    {
                        btnNextPage.Enabled = false;
                        btnLastPage.Enabled = false;
                    }
                    else
                    {
                        btnNextPage.Enabled = true;
                        btnLastPage.Enabled = true;
                    }
                }
            }
            finally
            {
                this.numPageIndex.ValueChanged += this.numPageIndex_ValueChanged;
            }

            panelPage.OnSizeChanged();
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            this._dataGridView.DataSource = null;
            this._dataGridView.Columns.Clear();
        }
    }
}
