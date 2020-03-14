using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Series基类
    /// </summary>
    public abstract class SeriesAbs : NotifyPropertyChangedAbs, ISeries
    {
        private AxisAbs _axisX = null;
        /// <summary>
        /// 获取或设置X坐标轴
        /// </summary>
        public virtual AxisAbs AxisX
        {
            get { return _axisX; }
            set
            {
                _axisX = value;
                base.OnRaisePropertyChanged(nameof(AxisX));
            }
        }

        private AxisAbs _axisY = null;
        /// <summary>
        /// 获取或设置Y坐标轴
        /// </summary>
        public virtual AxisAbs AxisY
        {
            get { return _axisY; }
            set
            {
                _axisY = value;
                base.OnRaisePropertyChanged(nameof(AxisY));
            }
        }

        private Func<PointInfo, FrameworkElement> _createPointFunc = null;
        /// <summary>
        /// 获取或设置创建坐标点对应的附加控件回调
        /// </summary>
        public virtual Func<PointInfo, FrameworkElement> CreatePointFunc
        {
            get { return _createPointFunc; }
            set
            {
                _createPointFunc = value;
                base.OnRaisePropertyChanged(nameof(CreatePointFunc));
            }
        }



        /// <summary>
        /// Values集合改变事件
        /// </summary>

        public event NotifyCollectionChangedEventHandler ValuesCollectionChanged;
        private void Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var handler = this.ValuesCollectionChanged;
            handler?.Invoke(sender, e);
        }


        /// <summary>
        /// 值集合
        /// </summary>
        protected ValueCollection _values = null;
        /// <summary>
        /// 获取或设置值集合
        /// </summary>
        public ValueCollection Values
        {
            get { return _values; }
            set
            {
                if (this._values != null)
                {
                    this._values.CollectionChanged -= Values_CollectionChanged;
                }

                _values = value;
                if (this._values != null)
                {
                    this._values.CollectionChanged += Values_CollectionChanged;
                }

                base.OnRaisePropertyChanged(nameof(Values));
            }
        }





        private Style _style = null;
        /// <summary>
        /// 获取或设置Series样式
        /// </summary>
        public virtual Style Style
        {
            get { return _style; }
            set
            {
                if (_style == value)
                {
                    return;
                }

                _style = value;
                this.StyleChanged(_style);
            }
        }

        /// <summary>
        /// Series样式改变通知
        /// </summary>
        /// <param name="style">新样式</param>
        protected abstract void StyleChanged(Style style);

        private bool _enableTooltip = false;
        /// <summary>
        /// 获取或设置是否启用Tooltip[true:启用Tooltip;false:禁用Tooltip]
        /// </summary>
        public bool EnableTooltip
        {
            get { return _enableTooltip; }
            set
            {
                if (_enableTooltip == value)
                {
                    return;
                }

                _enableTooltip = value;
                this.EnableTooltipChanged(_enableTooltip);
            }
        }
        /// <summary>
        /// EnableTooltip改变通知
        /// </summary>
        /// <param name="enableTooltip">新EnableTooltip值</param>
        protected virtual void EnableTooltipChanged(bool enableTooltip)
        {

        }

        private double _tooltipArea = ChartConstant.TOOLTIP_PRE;
        /// <summary>
        /// 获取或设置Tooltip有效区域,鼠标点周围范围内有点则触发Tooltip,小于0使用默认值
        /// </summary>
        public virtual double TooltipArea
        {
            get { return _tooltipArea; }
            set
            {
                if (value < ChartConstant.ZERO_D)
                {
                    value = ChartConstant.TOOLTIP_PRE;
                }
                _tooltipArea = value;
            }
        }

        private string _title = null;
        /// <summary>
        /// 获取或设置Series标题
        /// </summary>
        public virtual string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                base.OnRaisePropertyChanged(nameof(Title));
            }
        }


        private readonly List<SeriesLegendItem> _currentSeriesLegendItemList = new List<SeriesLegendItem>();
        private List<SeriesLegendItem> _chartSeriesLegendItemList = null;
        /// <summary>
        /// Chart画布
        /// </summary>
        protected Canvas _canvas;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SeriesAbs()
        {

        }



        /// <summary>
        /// 将Series绘制到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        public void Draw(Canvas canvas)
        {
            this._canvas = canvas;
            this._currentSeriesLegendItemList.Clear();
            this.PrimitiveDraw(canvas);
        }
        /// <summary>
        /// 将Series添加到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        protected abstract void PrimitiveDraw(Canvas canvas);




        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        public bool Clear()
        {
            if (this.PrimitiveClear(this._canvas))
            {
                return true;
            }

            if (this._chartSeriesLegendItemList != null)
            {
                foreach (var legendItem in this._currentSeriesLegendItemList)
                {
                    this._currentSeriesLegendItemList.Remove(legendItem);
                }
                this._currentSeriesLegendItemList.Clear();
            }
            this._chartSeriesLegendItemList = null;

            return false;
        }
        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        protected abstract bool PrimitiveClear(Canvas canvas);




        /// <summary>
        /// 更新Series
        /// </summary>
        public void Update()
        {
            this._currentSeriesLegendItemList.Clear();
            this.PrimitiveClear(this._canvas);
            this.PrimitiveDraw(this._canvas);
        }





        /// <summary>
        /// 将Series中的Legend项追加到列表中
        /// </summary>
        /// <param name="legendBrushList">Legend列表</param>
        public void AppendLegendItemToList(List<SeriesLegendItem> legendBrushList)
        {
            legendBrushList.AddRange(this._currentSeriesLegendItemList);
        }
        /// <summary>
        /// 添加或替换SeriesLegendItem
        /// </summary>
        /// <param name="legendItem">新SeriesLegendItem</param>
        protected void AddOrReplaceLegendItem(SeriesLegendItem legendItem)
        {
            this.RemoveLegendItem();
            this._currentSeriesLegendItemList.Add(legendItem);
        }
        /// <summary>
        /// 添加SeriesLegendItem
        /// </summary>
        /// <param name="legendItem">SeriesLegendItem</param>
        protected void AddLegendItem(SeriesLegendItem legendItem)
        {
            this._currentSeriesLegendItemList.Add(legendItem);
        }

        /// <summary>
        /// 移除当前Series中的SeriesLegendItem
        /// </summary>
        protected void RemoveLegendItem()
        {
            this._currentSeriesLegendItemList.Clear();
        }






        private Visibility _visibility = Visibility.Visible;
        /// <summary>
        /// 获取或设置SeriesVisibility
        /// </summary>
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility == value)
                {
                    return;
                }

                Visibility oldVisibility = _visibility;
                _visibility = value;
                this.VisibilityChanged(oldVisibility, _visibility);
            }
        }
        /// <summary>
        /// Visibility改变通知
        /// </summary>
        /// <param name="oldVisibility">旧值</param>
        /// <param name="newVisibility">新值</param>
        protected abstract void VisibilityChanged(Visibility oldVisibility, Visibility newVisibility);

        /// <summary>
        /// 获取或设置Tag
        /// </summary>
        public object Tag { get; set; }
    }
}
