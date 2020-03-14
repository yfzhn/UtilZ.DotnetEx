using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// UCCabinetControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCCabinetControl : UserControl
    {
        #region 依赖属性
        /// <summary>
        /// 机柜设备组列表依赖属性
        /// </summary>
        public static readonly DependencyProperty CabinetInfoGroupListProperty =
            DependencyProperty.Register(nameof(CabinetInfoGroupList), typeof(List<CabinetInfoGroup>), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜设备行间距依赖属性
        /// </summary>
        public static readonly DependencyProperty RowSeparatorProperty =
            DependencyProperty.Register(nameof(RowSeparator), typeof(double), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(UCCabinetRowControl.SEPARATOR, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜设备列间距依赖属性
        /// </summary>
        public static readonly DependencyProperty ColumnSeparatorProperty =
            DependencyProperty.Register(nameof(ColumnSeparator), typeof(double), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(UCCabinetRowControl.SEPARATOR, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 标题是否显示依赖属性
        /// </summary>
        public static readonly DependencyProperty TitleVisibleProperty =
            DependencyProperty.Register(nameof(TitleVisible), typeof(bool), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(UCCabinetRowControl.TITLE_VISIBLE, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜宽度依赖属性
        /// </summary>
        public static readonly DependencyProperty CabinetWidthProperty =
            DependencyProperty.Register(nameof(CabinetWidth), typeof(double), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(UCCabinetRowControl.CABINET_WIDTH, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 设备名称样式依赖属性
        /// </summary>
        public static readonly DependencyProperty DeviceNameStyleProperty =
            DependencyProperty.Register(nameof(DeviceNameStyle), typeof(Style), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜名称样式依赖属性
        /// </summary>
        public static readonly DependencyProperty CabinetNameStyleProperty =
            DependencyProperty.Register(nameof(CabinetNameStyle), typeof(Style), typeof(UCCabinetControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));



        /// <summary>
        /// 获取或设置机柜设备组列表
        /// </summary>
        public List<CabinetInfoGroup> CabinetInfoGroupList
        {
            get
            {
                return (List<CabinetInfoGroup>)base.GetValue(CabinetInfoGroupListProperty);
            }
            set
            {
                base.SetValue(CabinetInfoGroupListProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置机柜行间距,单位/像素
        /// </summary>
        public double RowSeparator
        {
            get
            {
                return (double)base.GetValue(RowSeparatorProperty);
            }
            set
            {
                if (!double.IsNaN(value) && value < 0d)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                base.SetValue(RowSeparatorProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置机柜列间距,单位/像素
        /// </summary>
        public double ColumnSeparator
        {
            get
            {
                return (double)base.GetValue(ColumnSeparatorProperty);
            }
            set
            {
                if (!double.IsNaN(value) && value < 0d)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                base.SetValue(ColumnSeparatorProperty, value);
            }
        }

        /// <summary>
        /// 标题是否显示[true:显示;false:不显示]
        /// </summary>
        public bool TitleVisible
        {
            get
            {
                return (bool)base.GetValue(TitleVisibleProperty);
            }
            set
            {
                base.SetValue(TitleVisibleProperty, value);
            }
        }

        /// <summary>
        /// 机柜宽度,单位/像素
        /// </summary>
        public double CabinetWidth
        {
            get
            {
                return (double)base.GetValue(CabinetWidthProperty);
            }
            set
            {
                base.SetValue(CabinetWidthProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置设备名称样式
        /// </summary>
        public Style DeviceNameStyle
        {
            get
            {
                return (Style)base.GetValue(DeviceNameStyleProperty);
            }
            set
            {
                base.SetValue(DeviceNameStyleProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置机柜名称样式
        /// </summary>
        public Style CabinetNameStyle
        {
            get
            {
                return (Style)base.GetValue(CabinetNameStyleProperty);
            }
            set
            {
                base.SetValue(CabinetNameStyleProperty, value);
            }
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (UCCabinetControl)d;

            if (e.Property == CabinetInfoGroupListProperty ||
                e.Property == RowSeparatorProperty ||
                e.Property == ColumnSeparatorProperty ||
                e.Property == CabinetWidthProperty)
            {
                selfControl.UpdateCabinet();
            }
            else if (e.Property == TitleVisibleProperty)
            {
                selfControl.SetTitleVisible((bool)e.NewValue);
            }
            else if (e.Property == DeviceNameStyleProperty)
            {
                selfControl.SetDeviceNameStyle((Style)e.NewValue);
            }
            else if (e.Property == CabinetNameStyleProperty)
            {
                selfControl.SetCabinetNameStyle((Style)e.NewValue);
            }
        }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public UCCabinetControl()
        {
            InitializeComponent();
        }

        private void UpdateCabinet()
        {
            List<CabinetInfoGroup> cabinetInfoGroupList = this.CabinetInfoGroupList;
            canvas.Children.Clear();

            if (cabinetInfoGroupList == null || cabinetInfoGroupList.Count == 0)
            {
                return;
            }

            double rowSeparator = this.RowSeparator;
            double rowSeparatorWidth = double.IsNaN(rowSeparator) ? UCCabinetRowControl.SEPARATOR : rowSeparator;
            double top = 0d;
            double maxWidth = 0d;
            int lastItemIndex = cabinetInfoGroupList.Count - 1;

            for (int i = 0; i < cabinetInfoGroupList.Count; i++)
            {
                var cabinetRowControl = new UCCabinetRowControl();
                cabinetRowControl.TitleVisible = this.TitleVisible;
                cabinetRowControl.CabinetWidth = this.CabinetWidth;
                cabinetRowControl.Separator = this.ColumnSeparator;
                cabinetRowControl.DeviceNameStyle = this.DeviceNameStyle;
                cabinetRowControl.CabinetNameStyle = this.CabinetNameStyle;
                cabinetRowControl.CabinetInfoGroup = cabinetInfoGroupList[i];
                canvas.Children.Add(cabinetRowControl);

                Canvas.SetTop(cabinetRowControl, top);
                Canvas.SetLeft(cabinetRowControl, 0d);

                if (i < lastItemIndex)
                {
                    top = top + rowSeparator + cabinetRowControl.Height;
                }
                else
                {
                    top = top + cabinetRowControl.Height;
                }

                if (cabinetRowControl.Width > maxWidth)
                {
                    maxWidth = cabinetRowControl.Width;
                }
            }

            this.Width = maxWidth;
            this.Height = top;
        }

        private void SetTitleVisible(bool visible)
        {
            foreach (UIElement ele in canvas.Children)
            {
                ((UCCabinetRowControl)ele).TitleVisible = visible;
            }
        }

        private void SetDeviceNameStyle(Style style)
        {
            if (canvas.Children.Count == 0)
            {
                return;
            }

            foreach (var ele in canvas.Children)
            {
                if (ele is UCCabinetRowControl)
                {
                    ((UCCabinetRowControl)ele).DeviceNameStyle = style;
                }
            }
        }

        private void SetCabinetNameStyle(Style style)
        {
            if (canvas.Children.Count == 0)
            {
                return;
            }

            foreach (var ele in canvas.Children)
            {
                if (ele is UCCabinetRowControl)
                {
                    ((UCCabinetRowControl)ele).CabinetNameStyle = style;
                }
            }
        }
    }
}
