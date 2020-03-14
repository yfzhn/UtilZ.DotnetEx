using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 图表控件
    /// </summary>
    public partial class Chart : UserControl
    {
        private const double _SCROLL_BAR_DEFAULT_WIDTH = 10d;

        #region 依赖属性
        /// <summary>
        /// 图表集合依赖属性
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(nameof(Series), typeof(ChartCollection<ISeries>), typeof(Chart),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        ///  图表区域最小高度值依赖属性
        /// </summary>
        public static readonly DependencyProperty ChartMinHeightProperty =
           DependencyProperty.Register(nameof(ChartMinHeight), typeof(double), typeof(Chart),
               new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 图表区域最小宽度值依赖属性
        /// </summary>
        public static readonly DependencyProperty ChartMinWidthProperty =
           DependencyProperty.Register(nameof(ChartMinWidth), typeof(double), typeof(Chart),
               new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 坐标集合依赖属性
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
           DependencyProperty.Register(nameof(Axes), typeof(ChartCollection<AxisAbs>), typeof(Chart),
               new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// Legend依赖属性
        /// </summary>
        public static readonly DependencyProperty LegendProperty =
           DependencyProperty.Register(nameof(Legend), typeof(IChartLegend), typeof(Chart),
               new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 滚动条宽度依赖属性
        /// </summary>
        public static readonly DependencyProperty ScrollBarWidthProperty =
           DependencyProperty.Register(nameof(ScrollBarWidth), typeof(double), typeof(Chart),
               new FrameworkPropertyMetadata(_SCROLL_BAR_DEFAULT_WIDTH, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 手机提交依赖属性
        /// </summary>
        public static readonly DependencyProperty ManaulComitProperty =
           DependencyProperty.Register(nameof(ManaulComit), typeof(bool), typeof(Chart),
               new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPropertyChangedCallback)));




        /// <summary>
        /// 图表集合
        /// </summary>
        public ChartCollection<ISeries> Series
        {
            get { return (ChartCollection<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// 图表区域最小高度值,为double.NaN或小于0此值无效,默认为double.NaN
        /// </summary>
        public double ChartMinHeight
        {
            get { return (double)GetValue(ChartMinHeightProperty); }
            set { SetValue(ChartMinHeightProperty, value); }
        }

        /// <summary>
        /// 图表区域最小宽度值,为double.NaN或小于0此值无效,默认为double.NaN
        /// </summary>
        public double ChartMinWidth
        {
            get { return (double)GetValue(ChartMinWidthProperty); }
            set { SetValue(ChartMinWidthProperty, value); }
        }

        /// <summary>
        /// 坐标集合
        /// </summary>
        public ChartCollection<AxisAbs> Axes
        {
            get { return (ChartCollection<AxisAbs>)GetValue(AxesProperty); }
            set { SetValue(AxesProperty, value); }
        }

        /// <summary>
        /// Legend
        /// </summary>
        public IChartLegend Legend
        {
            get { return (IChartLegend)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }

        /// <summary>
        /// 滚动条宽度,默认10像素
        /// </summary>
        public double ScrollBarWidth
        {
            get { return (double)GetValue(ScrollBarWidthProperty); }
            set { SetValue(ScrollBarWidthProperty, value); }
        }

        /// <summary>
        /// 手机提交.true;手动更新;false:自动更新
        /// </summary>
        public bool ManaulComit
        {
            get { return (bool)GetValue(ManaulComitProperty); }
            set { SetValue(ManaulComitProperty, value); }
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (Chart)d;

            if (e.Property == ManaulComitProperty && (bool)e.NewValue)
            {
                //手动提交为true,忽略
                return;
            }

            if (e.Property == SeriesProperty)
            {
                selfControl.SeriesChanged((ChartCollection<ISeries>)e.OldValue, (ChartCollection<ISeries>)e.NewValue);
            }
            else if (e.Property == AxesProperty)
            {
                selfControl.AxesChanged((ChartCollection<AxisAbs>)e.OldValue, (ChartCollection<AxisAbs>)e.NewValue);
            }
            else if (e.Property == ScrollBarWidthProperty)
            {
                selfControl.UpdateScrollBarWidth((double)e.NewValue);
                if (selfControl.GetAxisFreezeInfo().AxisFreezeType == AxisFreezeType.None)
                {
                    return;
                }
            }

            selfControl.UpdateAll();
        }


        #endregion

        private readonly Grid _chartGrid;
        private readonly Canvas _chartCanvas;
        private readonly ScrollViewer _scrollViewer;
        private readonly Grid _chartContentGrid;
        private double _scrollBarWidth;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Chart()
            : base()
        {
            this.MinHeight = 100d;
            this.MinWidth = 100d;

            this._chartGrid = new Grid() { Name = "chartGrid", Background = Brushes.Transparent };
            this._chartCanvas = new Canvas() { Name = "chartCanvas", Background = Brushes.Transparent };
            this._chartContentGrid = new Grid() { Name = "chartContentGrid", Background = Brushes.Transparent };
            this._scrollViewer = new ScrollViewer();
            this.UpdateScrollBarWidth(this.ScrollBarWidth);
            this.Loaded += ChartControl_Loaded;
        }

        private void ChartControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateAll();
        }

        /// <summary>
        /// 重写OnRenderSizeChanged
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.UpdateAll();
        }


        private bool IgnoreUpdateChart()
        {
            if (WPFHelper.InvokeRequired(this))
            {
                return (bool)this.Dispatcher.Invoke(new Func<bool>(this.IgnoreUpdateChart));
            }

            if (this.ManaulComit || !this.IsLoaded)
            {
                return true;
            }

            return false;
        }




        private void UpdateScrollBarWidth(double scrollBarWidth)
        {
            if (!ChartHelper.DoubleHasValue(scrollBarWidth) || scrollBarWidth < ChartConstant.ZERO_D)
            {
                scrollBarWidth = _SCROLL_BAR_DEFAULT_WIDTH;
            }

            this._scrollViewer.Resources[SystemParameters.VerticalScrollBarWidthKey] = scrollBarWidth;
            this._scrollViewer.Resources[SystemParameters.HorizontalScrollBarHeightKey] = scrollBarWidth;
            this._scrollBarWidth = scrollBarWidth;
        }



        private void AxesChanged(ChartCollection<AxisAbs> oldAxisCollection, ChartCollection<AxisAbs> newAxisCollection)
        {
            if (oldAxisCollection != null)
            {
                oldAxisCollection.ChartCollectionChanged -= AxisCollection_ChartCollectionChanged;
                foreach (var axis in oldAxisCollection)
                {
                    axis.PropertyChanged -= Axis_PropertyChanged;
                }
            }

            if (newAxisCollection != null)
            {
                newAxisCollection.ChartCollectionChanged += AxisCollection_ChartCollectionChanged;
                foreach (var axis in newAxisCollection)
                {
                    axis.PropertyChanged += Axis_PropertyChanged;
                }
            }
        }

        private void Axis_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var axis = (AxisAbs)sender;
            if (string.Equals(e.PropertyName, nameof(AxisAbs.Title)))
            {
                axis.UpdateTitle();
            }
            else if (string.Equals(e.PropertyName, nameof(AxisAbs.TitleStyle)))
            {
                axis.UpdateTitleStyle();
            }
            else
            {
                //全部重绘                
                //string.Equals(e.PropertyName, nameof(AxisAbs.LabelStyle)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.DrawAxisLine)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.LabelSize)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.AxisDockOrientation)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.AxisType)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.Orientation)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.XAxisHeight)) ||
                //string.Equals(e.PropertyName, nameof(AxisAbs.YAxisWidth))
                this.UpdateAll();
            }
        }

        private void AxisCollection_ChartCollectionChanged(object sender, ChartCollectionChangedEventArgs<AxisAbs> e)
        {
            bool update = false;
            switch (e.Action)
            {
                case ChartCollectionChangedAction.Add:
                    if (e.NewItems != null && e.NewItems.Count > 0)
                    {
                        foreach (var axis in e.NewItems)
                        {
                            if (axis == null)
                            {
                                continue;
                            }

                            axis.PropertyChanged += Axis_PropertyChanged;
                            update = true;
                        }
                    }
                    break;
                case ChartCollectionChangedAction.Move:
                    break;
                case ChartCollectionChangedAction.Remove:
                    if (e.OldItems != null && e.OldItems.Count > 0)
                    {
                        foreach (var axis in e.OldItems)
                        {
                            if (axis == null)
                            {
                                continue;
                            }

                            axis.PropertyChanged -= Axis_PropertyChanged;
                            update = true;
                        }
                    }
                    break;
                case ChartCollectionChangedAction.Replace:
                    if (e.NewItems != null && e.NewItems.Count > 0)
                    {
                        foreach (var axis in e.NewItems)
                        {
                            if (axis == null)
                            {
                                continue;
                            }

                            axis.PropertyChanged += Axis_PropertyChanged;
                            update = true;
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var axis in e.OldItems)
                        {
                            if (axis == null)
                            {
                                continue;
                            }

                            axis.PropertyChanged -= Axis_PropertyChanged;
                            update = true;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException(e.Action.ToString());
            }

            if (update)
            {
                this.UpdateAll();
            }
        }








        private void SeriesChanged(ChartCollection<ISeries> oldChartSeries, ChartCollection<ISeries> newChartSeries)
        {
            if (oldChartSeries != null)
            {
                oldChartSeries.ChartCollectionChanged -= Series_ChartCollectionChanged;
                foreach (var series in oldChartSeries)
                {
                    if (series == null)
                    {
                        continue;
                    }

                    series.PropertyChanged -= Series_PropertyChanged;
                    series.ValuesCollectionChanged -= Series_ValuesCollectionChanged;
                }
            }

            if (newChartSeries != null)
            {
                newChartSeries.ChartCollectionChanged += Series_ChartCollectionChanged;
                foreach (var series in newChartSeries)
                {
                    if (series == null)
                    {
                        continue;
                    }

                    series.PropertyChanged += Series_PropertyChanged;
                    series.ValuesCollectionChanged += Series_ValuesCollectionChanged;
                }
            }
        }

        private void Series_ValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateSeries((ISeries)sender);
        }

        private void Series_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateSeries((ISeries)sender);
        }

        private void UpdateSeries(ISeries series)
        {
            if (this.IgnoreUpdateChart())
            {
                return;
            }

            series.Update();
        }

        private void Series_ChartCollectionChanged(object sender, ChartCollectionChangedEventArgs<ISeries> e)
        {
            bool update = false;
            bool isFullDraw = false;
            switch (e.Action)
            {
                case ChartCollectionChangedAction.Add:
                    if (e.NewItems != null && e.NewItems.Count > 0)
                    {
                        foreach (var series in e.NewItems)
                        {
                            if (series == null)
                            {
                                continue;
                            }

                            series.PropertyChanged += Series_PropertyChanged;
                            series.ValuesCollectionChanged += Series_ValuesCollectionChanged;
                            update = true;
                        }
                    }
                    break;
                case ChartCollectionChangedAction.Move:
                    break;
                case ChartCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var series in e.OldItems)
                        {
                            if (series == null)
                            {
                                continue;
                            }

                            series.PropertyChanged -= Series_PropertyChanged;
                            series.ValuesCollectionChanged -= Series_ValuesCollectionChanged;
                            isFullDraw = series.Clear();
                        }
                    }
                    break;
                case ChartCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        foreach (var series in e.NewItems)
                        {
                            if (series == null)
                            {
                                continue;
                            }

                            series.PropertyChanged += Series_PropertyChanged;
                            series.ValuesCollectionChanged += Series_ValuesCollectionChanged;
                            update = true;
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var series in e.OldItems)
                        {
                            if (series == null)
                            {
                                continue;
                            }

                            series.PropertyChanged -= Series_PropertyChanged;
                            series.ValuesCollectionChanged -= Series_ValuesCollectionChanged;
                            isFullDraw = series.Clear();
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException(e.Action.ToString());
            }

            if (isFullDraw)
            {
                this.UpdateAll();
            }
            else if (update)
            {
                this.DrawSeries(this._chartCanvas, e.NewItems);
            }
        }







        private void UpdateAll()
        {
            if (this.IgnoreUpdateChart())
            {
                return;
            }

            AxisFreezeInfo axisFreezeInfo = this.GetAxisFreezeInfo();
            ChartCollection<AxisAbs> axes = this.Axes;
            ChartCollection<ISeries> series = this.Series;
            IChartLegend legend = this.Legend;
            Grid chartGrid = this._chartGrid;
            Canvas chartCanvas = this._chartCanvas;
            Grid chartContentGrid = this._chartContentGrid;
            ScrollViewer scrollViewer = this._scrollViewer;

            this.Content = null;
            scrollViewer.Content = null;
            this._scrollViewer.Content = null;
            chartGrid.Children.Clear();
            chartCanvas.Children.Clear();
            chartCanvas.MouseWheel -= ChartCanvas_MouseWheel;
            chartCanvas.MouseLeftButtonDown -= ChartCanvas_MouseLeftButtonDown;
            chartCanvas.MouseMove -= ChartCanvas_MouseMove;
            chartGrid.RowDefinitions.Clear();
            chartGrid.ColumnDefinitions.Clear();
            chartContentGrid.Children.Clear();
            chartContentGrid.RowDefinitions.Clear();
            chartContentGrid.ColumnDefinitions.Clear();

            switch (axisFreezeInfo.AxisFreezeType)
            {
                case AxisFreezeType.None:
                    this.UpdateNoneFreeze(axisFreezeInfo, axes, series, legend, chartCanvas, chartGrid, scrollViewer);
                    break;
                case AxisFreezeType.X:
                    this.UpdateFreezeX(axisFreezeInfo, axes, series, legend, chartCanvas, chartGrid, scrollViewer, chartContentGrid);
                    break;
                case AxisFreezeType.Y:
                    this.UpdateFreezeY(axisFreezeInfo, axes, series, legend, chartCanvas, chartGrid, scrollViewer, chartContentGrid);
                    break;
                case AxisFreezeType.All:
                    this.UpdateFreezeAll(axisFreezeInfo, axes, series, legend, chartCanvas, chartGrid, scrollViewer, chartContentGrid);
                    break;
                default:
                    throw new NotImplementedException(axisFreezeInfo.AxisFreezeType.ToString());
            }
        }




        private AxisFreezeInfo GetAxisFreezeInfo()
        {
            double width, height;
            AxisFreezeType axisFreezeType;
            if (double.IsNaN(this.ChartMinHeight))
            {
                height = this.ActualHeight;
                if (double.IsNaN(this.ChartMinWidth))
                {
                    axisFreezeType = AxisFreezeType.None;
                    width = this.ActualWidth;
                }
                else
                {
                    axisFreezeType = AxisFreezeType.Y;
                    width = this.ChartMinWidth;
                }
            }
            else
            {
                height = this.ChartMinHeight;
                if (double.IsNaN(this.ChartMinWidth))
                {
                    axisFreezeType = AxisFreezeType.X;
                    width = this.ActualWidth;
                }
                else
                {
                    axisFreezeType = AxisFreezeType.All;
                    width = this.ChartMinWidth;
                }
            }

            return new AxisFreezeInfo(width, height, axisFreezeType);
        }
    }
}
