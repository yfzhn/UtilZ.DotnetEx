using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// UCPagingControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCPagingControl : UserControl
    {
        #region 常量
        private const long _DEFAULT_SELECTED_PAGE_INDEX = -1L;
        private const long _DEFAULT_TOTAL_COUNT = 0L;
        private const long _PAGE_SIZE_MIN = 10L;
        private const long _PAGE_SIZE_MAX = 10000L;
        private const long _DEFAULT_PAGE_SIZE = 100L;
        private const long _PAGE_INDEX_MAX_SHOW_COUNT = 4L;
        private const long _DEFAULT_BEGIN_PAGE_INDEX = 1L;
        #endregion

        #region 依赖属性
        /// <summary>
        /// 当前选中页依赖属性
        /// </summary>
        public static readonly DependencyProperty SelectedPageIndexProperty =
            DependencyProperty.Register(nameof(SelectedPageIndex), typeof(long), typeof(UCPagingControl),
            new FrameworkPropertyMetadata(_DEFAULT_SELECTED_PAGE_INDEX, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 总记录数依赖属性
        /// </summary>
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register(nameof(TotalCount), typeof(long), typeof(UCPagingControl),
            new FrameworkPropertyMetadata(_DEFAULT_TOTAL_COUNT, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 页大小依赖属性
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(long), typeof(UCPagingControl),
            new FrameworkPropertyMetadata(_PAGE_SIZE_MIN, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 页大小最大值依赖属性
        /// </summary>
        public static readonly DependencyProperty PageSizeMaxProperty =
            DependencyProperty.Register(nameof(PageSizeMax), typeof(long), typeof(UCPagingControl),
            new FrameworkPropertyMetadata(_PAGE_SIZE_MAX, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 页大小最小值依赖属性
        /// </summary>
        public static readonly DependencyProperty PageSizeMinProperty =
            DependencyProperty.Register(nameof(PageSizeMin), typeof(long), typeof(UCPagingControl),
            new FrameworkPropertyMetadata(_PAGE_SIZE_MIN, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 当前选中页,小于0或超出范围无效
        /// </summary>
        internal long SelectedPageIndex
        {
            get
            {
                return (long)base.GetValue(SelectedPageIndexProperty);
            }
            set
            {
                base.SetValue(SelectedPageIndexProperty, value);
            }
        }

        /// <summary>
        /// 总记录页数
        /// </summary>
        public long TotalCount
        {
            get
            {
                return (long)base.GetValue(TotalCountProperty);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "总记录数不能为负数");
                }

                base.SetValue(TotalCountProperty, value);
            }
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public long PageSize
        {
            get
            {
                return (long)base.GetValue(PageSizeProperty);
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "分页大小不能小于1");
                }

                if (value < this.PageSizeMin || value > this.PageSizeMax)
                {
                    throw new ArgumentOutOfRangeException($"页大小值发取钱,只能在[{ this.PageSizeMin}-{this.PageSizeMax}]范围内");
                }

                base.SetValue(PageSizeProperty, value);
            }
        }

        /// <summary>
        /// 页大小最大值
        /// </summary>
        public long PageSizeMax
        {
            get
            {
                return (long)base.GetValue(PageSizeMaxProperty);
            }
            set
            {
                if (value <= PageSizeMin)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "分页大小上限值不能小于等于下限值");
                }

                base.SetValue(PageSizeMaxProperty, value);
            }
        }

        /// <summary>
        /// 页大小最小值
        /// </summary>
        public long PageSizeMin
        {
            get
            {
                return (long)base.GetValue(PageSizeMinProperty);
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "分页大小下限值不能小于1");
                }

                base.SetValue(PageSizeMinProperty, value);
            }
        }
        #endregion

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (UCPagingControl)d;
            if (e.Property == SelectedPageIndexProperty)
            {
                selfControl.SetSelectedPageIndex((long)e.NewValue);
            }
            else if (e.Property == TotalCountProperty)
            {
                selfControl.SetTotalCount((long)e.NewValue);
            }
            else if (e.Property == PageSizeProperty)
            {
                selfControl.SetPageSize((long)e.NewValue);
            }
            else if (e.Property == PageSizeMaxProperty)
            {
                selfControl.SetPageSizeMax((long)e.NewValue);
            }
            else if (e.Property == PageSizeMinProperty)
            {
                selfControl.SetPageSizeMin((long)e.NewValue);
            }
        }


        private readonly ObservableCollection<PageInfo> _pageIndexCollection = new ObservableCollection<PageInfo>();
        private long _totalPageCount = 0;


        private void SetTotalCount(long totalCount)
        {
            this.CalTotalPageCount(totalCount, this.PageSize);
        }

        private void SetPageSize(long pageSize)
        {
            txtPageSize.Text = pageSize.ToString();
            this.CalTotalPageCount(this.TotalCount, pageSize);
        }

        private void SetPageSizeMin(long pageSizeMin)
        {
            if (this.PageSize < pageSizeMin)
            {
                this.PageSize = pageSizeMin;
            }
        }

        private void SetPageSizeMax(long pageSizeMax)
        {
            if (this.PageSize > pageSizeMax)
            {
                this.PageSize = pageSizeMax;
            }
        }

        private void CalTotalPageCount(long totalCount, long pageSize)
        {
            long totalPageCount, goPageIndex;
            if (totalCount <= 0)
            {
                totalPageCount = 0L;
                goPageIndex = 0L;
            }
            else
            {
                totalPageCount = totalCount / pageSize;
                if (totalCount % pageSize != 0)
                {
                    totalPageCount += 1;
                }

                long lastgoPageIndex;
                if (long.TryParse(txtGoPageIndex.Text, out lastgoPageIndex))
                {
                    if (lastgoPageIndex > 0 && lastgoPageIndex < totalPageCount)
                    {
                        goPageIndex = lastgoPageIndex;
                    }
                    else
                    {
                        goPageIndex = _DEFAULT_BEGIN_PAGE_INDEX;
                    }
                }
                else
                {
                    goPageIndex = _DEFAULT_BEGIN_PAGE_INDEX;
                }
            }

            this._totalPageCount = totalPageCount;
            txtPagingInfo.Text = $"共{totalPageCount}页,每页";
            txtGoPageIndex.Text = goPageIndex.ToString();


            //更新页集合
            this.UpdatePageIndexCollection(_DEFAULT_BEGIN_PAGE_INDEX);

            long pageIndex = _DEFAULT_BEGIN_PAGE_INDEX;
            this.OnRaiseQueryData(pageIndex);
        }

        private void UpdatePageIndexCollection(long beginPageIndex)
        {
            this.listBoxPage.SelectionChanged -= listBoxPage_SelectionChanged;
            this._pageIndexCollection.Clear();
            this.listBoxPage.SelectionChanged += listBoxPage_SelectionChanged;
            int count = 0;
            for (var i = beginPageIndex; i <= this._totalPageCount; i++)
            {
                this._pageIndexCollection.Add(new PageInfo(i));
                count++;
                if (count >= _PAGE_INDEX_MAX_SHOW_COUNT)
                {
                    break;
                }
            }
        }

        private void SetSelectedPageIndex(long pageIndex)
        {
            long totalPageCount = this._totalPageCount;
            if (this._pageIndexCollection.Count == 0)
            {
                this.SetBtnEnable(pageIndex, totalPageCount);
                return;
            }

            var selectedItem = this._pageIndexCollection.Where(t => { return t.Value == pageIndex; }).FirstOrDefault();
            if (selectedItem == null)
            {
                long beginPageIndex = this.CalBeginPageIndex(pageIndex);
                this.UpdatePageIndexCollection(beginPageIndex);
                selectedItem = this._pageIndexCollection.Where(t => { return t.Value == pageIndex; }).FirstOrDefault();
            }

            if (listBoxPage.SelectedItem != selectedItem)
            {
                this.listBoxPage.SelectionChanged -= listBoxPage_SelectionChanged;
                listBoxPage.SelectedItem = selectedItem;
                this.listBoxPage.SelectionChanged += listBoxPage_SelectionChanged;
            }

            this.SetBtnEnable(selectedItem.Value, totalPageCount);
        }

        private long CalBeginPageIndex(long pageIndex)
        {
            var totalPageCount = this._totalPageCount;
            long beginPageIndex;
            if (totalPageCount >= _PAGE_INDEX_MAX_SHOW_COUNT + pageIndex)
            {
                //之后还有更多页
                beginPageIndex = pageIndex;
            }
            else
            {
                //之后没有更多页
                if (totalPageCount >= _PAGE_INDEX_MAX_SHOW_COUNT)
                {
                    //多于显示页数
                    beginPageIndex = totalPageCount - _PAGE_INDEX_MAX_SHOW_COUNT + 1;
                }
                else
                {
                    //少于显示页数
                    beginPageIndex = _DEFAULT_BEGIN_PAGE_INDEX;
                }
            }

            return beginPageIndex;
        }

        private void SetBtnEnable(long pageIndex, long totalPageCount)
        {
            if (this._pageIndexCollection.Count == 0)
            {
                btnFirst.IsEnabled = false;
                btnPre.IsEnabled = false;
                btnNext.IsEnabled = false;
                btnLast.IsEnabled = false;

                txtPageSize.IsEnabled = false;
                txtGoPageIndex.IsEnabled = false;
                btnGo.IsEnabled = false;
            }
            else if (totalPageCount <= _PAGE_INDEX_MAX_SHOW_COUNT)
            {
                btnFirst.IsEnabled = false;
                if (pageIndex <= _DEFAULT_BEGIN_PAGE_INDEX)
                {
                    btnPre.IsEnabled = false;
                }
                else
                {
                    btnPre.IsEnabled = true;
                }

                if (pageIndex >= totalPageCount)
                {
                    btnNext.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                }

                btnLast.IsEnabled = false;

                txtPageSize.IsEnabled = true;
                txtGoPageIndex.IsEnabled = false;
                btnGo.IsEnabled = false;
            }
            else
            {
                txtPageSize.IsEnabled = true;
                txtGoPageIndex.IsEnabled = true;
                btnGo.IsEnabled = true;


                var minPageIndex = this._pageIndexCollection.First().Value;
                if (minPageIndex <= _DEFAULT_BEGIN_PAGE_INDEX)
                {
                    btnFirst.IsEnabled = false;
                }
                else
                {
                    btnFirst.IsEnabled = true;
                }

                if (pageIndex <= _DEFAULT_BEGIN_PAGE_INDEX)
                {
                    btnPre.IsEnabled = false;
                }
                else
                {
                    btnPre.IsEnabled = true;
                }


                var maxPageIndex = this._pageIndexCollection.Last().Value;
                if (maxPageIndex >= totalPageCount)
                {
                    btnLast.IsEnabled = false;
                }
                else
                {
                    btnLast.IsEnabled = true;
                }

                if (pageIndex >= totalPageCount)
                {
                    btnNext.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 查询数据事件
        /// </summary>
        public event EventHandler<PagingQueryArgs> QueryData;
        private void OnRaiseQueryData(long pageIndex)
        {
            if (this.TotalCount <= 0)
            {
                pageIndex = _DEFAULT_SELECTED_PAGE_INDEX;
            }

            var handler = this.QueryData;
            if (handler != null)
            {
                var args = new PagingQueryArgs(this.TotalCount, this._totalPageCount, pageIndex, this.PageSize);
                args.SetPagingControl(this);
                handler(this, args);
            }
        }

        internal void CompletedCallback(long pageIndex)
        {
            if (this.SelectedPageIndex == pageIndex)
            {
                this.SetSelectedPageIndex(pageIndex);
            }
            else
            {
                this.SelectedPageIndex = pageIndex;
            }
        }

        internal void CancelCallback(long lastPageIndex)
        {
            if (lastPageIndex < _DEFAULT_BEGIN_PAGE_INDEX || lastPageIndex > this._totalPageCount)
            {
                return;
            }

            if (this.SelectedPageIndex != lastPageIndex)
            {
                this.SelectedPageIndex = lastPageIndex;
            }
            else
            {
                this.SetSelectedPageIndex(lastPageIndex);
            }
        }



        /// <summary>
        /// 构造函数
        /// </summary>
        public UCPagingControl()
        {
            InitializeComponent();

            this.listBoxPage.ItemsSource = this._pageIndexCollection;
            this.listBoxPage.SelectionChanged += listBoxPage_SelectionChanged;
            txtPageSize.Text = _DEFAULT_PAGE_SIZE.ToString();

            this.AddTextChanged(this.txtPageSize);
            this.AddTextChanged(this.txtGoPageIndex);
        }

        private void AddTextChanged(TextBox txt)
        {
            if (this.txtPageSize == txt)
            {
                this.txtPageSize.TextChanged += this.txtPageSize_TextChanged;
            }
            else if (this.txtGoPageIndex == txt)
            {
                this.txtGoPageIndex.TextChanged += this.txtGoPageIndex_TextChanged;
            }
        }

        private void RemoveTextChanged(TextBox txt)
        {
            if (this.txtPageSize == txt)
            {
                this.txtPageSize.TextChanged -= this.txtPageSize_TextChanged;
            }
            else if (this.txtGoPageIndex == txt)
            {
                this.txtGoPageIndex.TextChanged -= this.txtGoPageIndex_TextChanged;
            }
        }


        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            long pageIndex = _DEFAULT_BEGIN_PAGE_INDEX;
            this.OnRaiseQueryData(pageIndex);
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            //右移(页数变小)
            if (this._pageIndexCollection.Count == 0)
            {
                return;
            }

            var lastIndex = this._pageIndexCollection.IndexOf(((PageInfo)listBoxPage.SelectedItem));
            if (lastIndex > 0)
            {
                lastIndex -= 1;
                this.OnRaiseQueryData(this._pageIndexCollection[lastIndex].Value);
            }
            else
            {
                var minPageIndex = this._pageIndexCollection.ElementAt(0).Value;
                if (minPageIndex <= 1)
                {
                    //已经显示到首页,忽略
                    return;
                }

                //修改右移页数索引
                var index = this._pageIndexCollection.Count - 1;

                //移除最后一项
                this._pageIndexCollection.RemoveAt(index);

                //添加最小项在起始位置
                minPageIndex -= 1;
                this._pageIndexCollection.Insert(0, new PageInfo(minPageIndex));
                this.OnRaiseQueryData(minPageIndex);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //左移(页数变大)
            if (this._pageIndexCollection.Count == 0)
            {
                return;
            }

            var lastIndex = this._pageIndexCollection.IndexOf(((PageInfo)listBoxPage.SelectedItem));
            if (lastIndex < this._pageIndexCollection.Count - 1)
            {
                lastIndex += 1;
                this.OnRaiseQueryData(this._pageIndexCollection[lastIndex].Value);
            }
            else
            {
                var maxPageIndex = this._pageIndexCollection.Last().Value;
                if (maxPageIndex >= this._totalPageCount)
                {
                    return;
                }

                if (this._pageIndexCollection.Count > 0)
                {
                    this._pageIndexCollection.RemoveAt(0);
                }

                maxPageIndex += 1;
                this._pageIndexCollection.Add(new PageInfo(maxPageIndex));
                this.OnRaiseQueryData(maxPageIndex);
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            var totalPageCount = this._totalPageCount;
            long pageIndex;
            if (totalPageCount > _PAGE_INDEX_MAX_SHOW_COUNT)
            {
                pageIndex = totalPageCount - _PAGE_INDEX_MAX_SHOW_COUNT + 1;
            }
            else
            {
                pageIndex = 1;
            }

            this.OnRaiseQueryData(totalPageCount);
        }


        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long pageIndexGo;
                if (!long.TryParse(txtGoPageIndex.Text, out pageIndexGo))
                {
                    Loger.Error("转换值失败,应用程序内部逻辑错误");
                    return;
                }

                var item = this._pageIndexCollection.Where(t => { return t.Value == pageIndexGo; }).FirstOrDefault();
                if (item != null)
                {
                    this.OnRaiseQueryData(pageIndexGo);
                    return;
                }

                this.OnRaiseQueryData(pageIndexGo);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }



        private void listBoxPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                long pageIndex;
                if (listBoxPage.SelectedItem == null)
                {
                    pageIndex = _DEFAULT_SELECTED_PAGE_INDEX;
                }
                else
                {
                    pageIndex = ((PageInfo)listBoxPage.SelectedItem).Value;
                }

                this.OnRaiseQueryData(pageIndex);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }




        #region 页大小和跳转页值验证
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        //private void NumberTextChangedValidate(object sender, TextChangedEventArgs e, long defaultValue)
        //{
        //    var txt = (TextBox)sender;
        //    try
        //    {
        //        const string _REG = @"(^[0-9]$)|(^[1-9]\d+$)";
        //        if (Regex.IsMatch(txt.Text, _REG))
        //        {
        //            this._txtLastStrDic[txt] = txt.Text;
        //        }
        //        else
        //        {
        //            if (txt.Text.Length == 0)
        //            {
        //                txt.Text = defaultValue.ToString();
        //                return;
        //            }

        //            foreach (var item in e.Changes)
        //            {
        //                if (item.AddedLength > 0)
        //                {
        //                    //输入,新输入的字符不对,移除
        //                    txt.Text = txt.Text.Remove(item.Offset, item.AddedLength);
        //                    txt.SelectionStart = txt.Text.Length;
        //                }
        //                //else if (item.RemovedLength > 0)
        //                //{
        //                //    //删除,删除最左侧的字符,如果剩下最左为0,则移除至非0或最后只剩下一个0为止
        //                //    if (item.Offset != 0)
        //                //    {
        //                //        //删除的非最左侧
        //                //        return;
        //                //    }

        //                //    const char zereCh = (char)48;
        //                //    if (txt.Text.First() == zereCh)
        //                //    {
        //                //        txt.Text = long.Parse(txt.Text).ToString();
        //                //    }
        //                //}
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Loger.Error(ex);

        //        if (this._txtLastStrDic.ContainsKey(txt))
        //        {
        //            txt.Text = this._txtLastStrDic[txt];
        //        }
        //        else
        //        {
        //            txt.Text = defaultValue.ToString();
        //        }
        //    }
        //}

        private void txtPageIndex_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                this.RecoverLastValue(txt, _DEFAULT_BEGIN_PAGE_INDEX);
            }
        }

        private void txtPageSize_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (TextBox)sender;
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = this.PageSize.ToString();
                }
                else
                {
                    this.ValidateTextChangedValueArea((TextBox)sender, null, this.PageSizeMin, this.PageSizeMax, false);
                    this.PageSize = long.Parse(txtPageSize.Text);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }





        private void txtPageSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ValidateTextChangedValueArea((TextBox)sender, e, this.PageSizeMin, this.PageSizeMax, true);
        }

        private void txtGoPageIndex_TextChanged(object sender, TextChangedEventArgs e)
        {
            long min, max;
            if (this._totalPageCount > 0)
            {
                min = 1;
            }
            else
            {
                min = 0;
            }

            max = this._totalPageCount;
            this.ValidateTextChangedValueArea((TextBox)sender, e, min, max, false);
        }



        private Dictionary<TextBox, string> _txtLastValueDic = new Dictionary<TextBox, string>();
        private void ValidateTextChangedValueArea(TextBox txt, TextChangedEventArgs e, long min, long max, bool ignorMinValidate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt.Text) ||
                    e != null && e.Changes.First().RemovedLength > 0)
                //if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    return;
                }

                if (!Regex.IsMatch(txt.Text, @"^[1-9]\d*$"))
                {
                    this.RecoverLastValue(txt, max);
                    return;
                }

                var value = long.Parse(txt.Text);
                if (ignorMinValidate && value < min)
                {
                    return;
                }

                if (value < min || value > max)
                {
                    this.RecoverLastValue(txt, min);
                }
                else
                {
                    if (txt.Text.Contains(' '))
                    {
                        txt.Text = txt.Text.Trim();
                        //光标设置到最后
                        this.SetCursorToEnd(txt);
                        //光标设置到最后
                        this.SetCursorToEnd(txt);
                    }
                    else
                    {
                        this._txtLastValueDic[txt] = txt.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                this.RecoverLastValue(txt, min);
                //光标设置到最后
                this.SetCursorToEnd(txt);
            }
        }



        private void RecoverLastValue(TextBox txt, long min)
        {
            this.RemoveTextChanged(txt);
            if (this._txtLastValueDic.ContainsKey(txt))
            {
                txt.Text = this._txtLastValueDic[txt];
            }
            else
            {
                txt.Text = min.ToString();
            }

            txt.SelectionStart = txt.Text.Length;
            this.AddTextChanged(txt);
        }

        private void SetCursorToEnd(TextBox txt)
        {
            txt.SelectionStart = txt.Text.Length;
        }
        #endregion

        private void txtPageSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                listBoxPage.Focus();
                txtPageSize.Focus();
            }
        }

        private void txtGoPageIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnGo_Click(sender, null);
            }
        }
    }






    internal class PageInfo
    {
        private long _value;
        public long Value
        {
            get { return _value; }
        }

        public string Text
        {
            get { return _value.ToString(); }
        }

        public PageInfo(long pageIndex)
        {
            this._value = pageIndex;
        }
    }





    /// <summary>
    /// 分页查询参数
    /// </summary>
    public class PagingQueryArgs : EventArgs
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalCount { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPageCount { get; private set; }

        /// <summary>
        /// 查询目标页,小于0表示无效
        /// </summary>
        public long PageIndex { get; private set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public long PageSize { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="totalPageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public PagingQueryArgs(long totalCount, long totalPageCount, long pageIndex, long pageSize)
        {
            this.TotalCount = totalCount;
            this.TotalPageCount = totalPageCount;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"TotalCount;{TotalCount},TotalPageCount:{TotalPageCount};PageIndex:{PageIndex};PageSize:{PageSize}";
        }




        private UCPagingControl _pagingControl = null;
        internal void SetPagingControl(UCPagingControl pagingControl)
        {
            this._pagingControl = pagingControl;
        }

        /// <summary>
        /// 取消查询
        /// </summary>
        /// <param name="lastPageIndex">上次选中页</param>
        public void Cancel(long lastPageIndex)
        {
            if (this._pagingControl != null)
            {
                this._pagingControl.CancelCallback(lastPageIndex);
            }
        }

        /// <summary>
        /// 查询完成
        /// </summary>
        public void Completed()
        {
            if (this._pagingControl != null)
            {
                this._pagingControl.CompletedCallback(this.PageIndex);
            }
        }
    }
}
