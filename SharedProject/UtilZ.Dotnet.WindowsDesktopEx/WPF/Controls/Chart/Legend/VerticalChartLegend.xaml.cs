using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// VerticalChartLegend.xaml 的交互逻辑
    /// </summary>
    public partial class VerticalChartLegend : UserControl, IChartLegend
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public VerticalChartLegend()
        {
            InitializeComponent();

            this.UpdateItemsControlStytle();
        }

        /// <summary>
        /// 获取或设置Legend控件停靠方向
        /// </summary>
        public ChartDockOrientation DockOrientation { get; set; } = ChartDockOrientation.Right;

        /// <summary>
        /// 获取或设置垂直方向宽高度
        /// </summary>
        public double Size
        {
            get
            {
                return this.Width;
            }
            set
            {
                this.Width = value;
            }
        }

        private bool _allowChecked = false;
        /// <summary>
        /// 获取或设置允许选中项
        /// </summary>
        public bool AllowChecked
        {
            get
            {
                return _allowChecked;
            }
            set
            {
                if (_allowChecked == value)
                {
                    return;
                }

                _allowChecked = value;
                this.UpdateItemsControlStytle();
            }
        }

        private void UpdateItemsControlStytle()
        {
            if (_allowChecked)
            {
                itemsControl.Style = this.Resources[ChartLegendResourceDictionaryKeyConstant.ALOOW_EDIT_LEGEND_ITEMCONTROL_STYLE_KEY] as Style;
            }
            else
            {
                itemsControl.Style = this.Resources[ChartLegendResourceDictionaryKeyConstant.NO_EDIT_LEGEND_ITEMCONTROL_STYLE_KEY] as Style;
            }
        }

        /// <summary>
        /// 获取或设置获取Legend控件
        /// </summary>
        public FrameworkElement LegendControl
        {
            get { return this; }
        }

        /// <summary>
        /// 更新Legend控件显示项
        /// </summary>
        /// <param name="legendBrushList">Legend控件显示项列表</param>
        public void UpdateLegend(List<SeriesLegendItem> legendBrushList)
        {
            itemsControl.ItemsSource = legendBrushList;
        }
    }
}
