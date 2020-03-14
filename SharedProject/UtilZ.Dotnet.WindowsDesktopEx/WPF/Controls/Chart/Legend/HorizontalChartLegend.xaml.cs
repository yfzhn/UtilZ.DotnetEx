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
    /// HorizontalChartLegend.xaml 的交互逻辑
    /// </summary>
    public partial class HorizontalChartLegend : UserControl, IChartLegend
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HorizontalChartLegend()
        {
            InitializeComponent();

            this.UpdateItemsControlStytle();
        }

        /// <summary>
        /// 获取或设置Legend控件停靠方向
        /// </summary>
        public ChartDockOrientation DockOrientation { get; set; } = ChartDockOrientation.Bottom;

        /// <summary>
        /// 获取或设置水平方向高度
        /// </summary>
        public double Size
        {
            get
            {
                return this.Height;
            }
            set
            {
                this.Height = value;
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
