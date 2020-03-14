using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// SeriesLegendItem
    /// </summary>
    public class SeriesLegendItem : NotifyPropertyChangedAbs
    {
        /// <summary>
        /// Series Brush
        /// </summary>
        public Brush Brush { get; private set; }

        /// <summary>
        /// Series Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Series
        /// </summary>
        public ISeries Series { get; private set; }

        /// <summary>
        /// 片所包含的控件集合
        /// </summary>
        public IEnumerable<FrameworkElement> CliceControls { get; private set; }







        private bool _showSeries = true;
        /// <summary>
        /// 是否显示当前项
        /// </summary>
        public bool ShowSeries
        {
            get { return this._showSeries; }
            set
            {
                if (this._showSeries == value)
                {
                    return;
                }

                this._showSeries = value;

                var visibility = _showSeries ? Visibility.Visible : Visibility.Collapsed;
                if (this.CliceControls != null)
                {
                    foreach (var child in this.CliceControls)
                    {
                        child.Visibility = visibility;
                    }
                }
                else
                {
                    this.Series.Visibility = visibility;
                }

                base.OnRaisePropertyChanged(nameof(ShowSeries));
            }
        }




        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="brush">Series Brush</param>
        /// <param name="title">Series Title</param>
        /// <param name="series">Series</param>
        public SeriesLegendItem(Brush brush, string title, ISeries series)
        {
            this.Brush = brush;
            this.Title = title;
            this.Series = series;

            if (series.Visibility == Visibility.Visible)
            {
                this._showSeries = true;
            }
            else
            {
                this._showSeries = false;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="brush">Series Brush</param>
        /// <param name="title">Series Title</param>
        /// <param name="series">Series</param>
        /// <param name="sliceControls">片所包含的控件集合</param>
        public SeriesLegendItem(Brush brush, string title, ISeries series, IEnumerable<FrameworkElement> sliceControls)
            : this(brush, title, series)
        {
            this.CliceControls = sliceControls;
        }
    }
}
