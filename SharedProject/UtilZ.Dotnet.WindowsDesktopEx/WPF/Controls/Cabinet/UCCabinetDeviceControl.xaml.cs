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
    /// UCCabinetDeviceUnitControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCCabinetDeviceControl : UserControl
    {
        #region 依赖属性
        /// <summary>
        /// 设备名称样式依赖属性
        /// </summary>
        public static readonly DependencyProperty DeviceNameStyleProperty =
            DependencyProperty.Register(nameof(DeviceNameStyle), typeof(Style), typeof(UCCabinetDeviceControl),
                new FrameworkPropertyMetadata(GetDeviceNameDefault(), new PropertyChangedCallback(OnPropertyChangedCallback)));

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

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (UCCabinetDeviceControl)d;
            if (e.Property == DeviceNameStyleProperty)
            {
                selfControl.SetDeviceNameStyle((Style)e.NewValue);
            }
        }

        private static Style _deviceNameDefault = null;
        private static Style GetDeviceNameDefault()
        {
            if (_deviceNameDefault == null)
            {
                Style style = new Style();
                style.TargetType = typeof(TextBlock);
                style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right));
                style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                style.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
                style.Setters.Add(new Setter(TextBlock.LineHeightProperty, 12d));
                style.Setters.Add(new Setter(TextBlock.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight));
                style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 15d));
                _deviceNameDefault = style;
            }

            return _deviceNameDefault;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public UCCabinetDeviceControl()
        {
            InitializeComponent();

            this.SetDeviceNameStyle(this.DeviceNameStyle);
        }



        private void SetDeviceNameStyle(Style stytle)
        {
            tbDeviceName.Style = stytle;
        }
    }
}
