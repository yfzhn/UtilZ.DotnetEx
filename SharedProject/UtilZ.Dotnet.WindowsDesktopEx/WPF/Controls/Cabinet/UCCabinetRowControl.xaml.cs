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
    /// UCCabinetRowControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCCabinetRowControl : UserControl
    {
        internal const int COLUMN_COUNT = 5;
        internal const double SEPARATOR = 10d;
        internal const bool TITLE_VISIBLE = true;
        internal const double CABINET_WIDTH = 500d;

        #region 依赖属性
        /// <summary>
        /// 机柜设备列表依赖属性
        /// </summary>
        public static readonly DependencyProperty CabinetInfoGroupProperty =
            DependencyProperty.Register(nameof(CabinetInfoGroup), typeof(CabinetInfoGroup), typeof(UCCabinetRowControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜设备间距依赖属性
        /// </summary>
        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register(nameof(Separator), typeof(double), typeof(UCCabinetRowControl),
                new FrameworkPropertyMetadata(SEPARATOR, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 标题是否显示依赖属性
        /// </summary>
        public static readonly DependencyProperty TitleVisibleProperty =
            DependencyProperty.Register(nameof(TitleVisible), typeof(bool), typeof(UCCabinetRowControl),
                new FrameworkPropertyMetadata(TITLE_VISIBLE, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜宽度依赖属性
        /// </summary>
        public static readonly DependencyProperty CabinetWidthProperty =
            DependencyProperty.Register(nameof(CabinetWidth), typeof(double), typeof(UCCabinetRowControl),
                new FrameworkPropertyMetadata(CABINET_WIDTH, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 设备名称样式依赖属性
        /// </summary>
        public static readonly DependencyProperty DeviceNameStyleProperty =
            DependencyProperty.Register(nameof(DeviceNameStyle), typeof(Style), typeof(UCCabinetRowControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 机柜名称样式依赖属性
        /// </summary>
        public static readonly DependencyProperty CabinetNameStyleProperty =
            DependencyProperty.Register(nameof(CabinetNameStyle), typeof(Style), typeof(UCCabinetRowControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));


        /// <summary>
        /// 获取或设置机柜设备列表
        /// </summary>
        public CabinetInfoGroup CabinetInfoGroup
        {
            get
            {
                return (CabinetInfoGroup)base.GetValue(CabinetInfoGroupProperty);
            }
            set
            {
                base.SetValue(CabinetInfoGroupProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置机柜间距,单位/像素
        /// </summary>
        public double Separator
        {
            get
            {
                return (double)base.GetValue(SeparatorProperty);
            }
            set
            {
                if (!double.IsNaN(value) && value < 0d)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                base.SetValue(SeparatorProperty, value);
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
            var selfControl = (UCCabinetRowControl)d;
            if (e.Property == CabinetInfoGroupProperty ||
                e.Property == CabinetWidthProperty ||
                e.Property == SeparatorProperty)
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

        private static readonly double _titleWidth;

        static UCCabinetRowControl()
        {
            var control = new UCCabinetRowControl();
            _titleWidth = ((GridLength)control.Resources["titleWidth"]).Value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UCCabinetRowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 计算行控件宽度
        /// </summary>
        /// <param name="cabineCount"></param>
        /// <param name="cabinetWidth"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static double CalCabineRowtControlWidth(int cabineCount, double cabinetWidth, double separator)
        {
            return cabineCount * cabinetWidth + (cabineCount - 1) * separator + _titleWidth;
        }

        /// <summary>
        /// 计算机柜控件宽度
        /// </summary>
        /// <param name="cabineCount"></param>
        /// <param name="totalWidth"></param>
        /// <param name="separator"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        public static double CalCabinetWidth(int cabineCount, double totalWidth, double separator, double maxWidth = 1000d)
        {
            totalWidth = totalWidth - _titleWidth - separator * (cabineCount - 1);
            double cabinetWidth = totalWidth / cabineCount;
            if (cabinetWidth > maxWidth)
            {
                cabinetWidth = maxWidth;
            }

            return cabinetWidth;
        }

        private void UpdateCabinet()
        {
            canvas.Children.Clear();

            CabinetInfoGroup cabinetInfoGroup = this.CabinetInfoGroup;
            if (cabinetInfoGroup == null)
            {
                return;
            }


            var group = cabinetInfoGroup.Group;
            if (group == null)
            {
                return;
            }

            tbTitle.Text = this.ConvertTitleToVertical(cabinetInfoGroup.Name);
            if (group.Count == 0)
            {
                return;
            }

            byte cabinetMaxHeight = group.Max(t => { return t.Height; });
            this.Height = UCSingleCabinetControl.CalCabinetControlHeight(cabinetMaxHeight);
            this.Width = CalCabineRowtControlWidth(group.Count, this.CabinetWidth, this.Separator);

            double left = 0d;
            foreach (CabinetInfo cabinetInfo in group)
            {
                var singleCabinetControl = new UCSingleCabinetControl();
                singleCabinetControl.DeviceNameStyle = this.DeviceNameStyle;
                singleCabinetControl.CabinetNameStyle = this.CabinetNameStyle;
                singleCabinetControl.CabinetInfo = cabinetInfo;
                singleCabinetControl.Width = this.CabinetWidth;
                canvas.Children.Add(singleCabinetControl);

                Canvas.SetBottom(singleCabinetControl, 0d);
                Canvas.SetLeft(singleCabinetControl, left);
                left = left + this.Separator + this.CabinetWidth;
            }
        }



        private string ConvertTitleToVertical(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            var chArr = title.ToArray();
            return string.Join($"\r\n", chArr);
        }

        private void SetTitleVisible(bool visible)
        {
            tbTitle.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetDeviceNameStyle(Style style)
        {
            if (canvas.Children.Count == 0)
            {
                return;
            }

            foreach (var ele in canvas.Children)
            {
                if (ele is UCSingleCabinetControl)
                {
                    ((UCSingleCabinetControl)ele).DeviceNameStyle = style;
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
                if (ele is UCSingleCabinetControl)
                {
                    ((UCSingleCabinetControl)ele).CabinetNameStyle = style;
                }
            }
        }
    }
}
