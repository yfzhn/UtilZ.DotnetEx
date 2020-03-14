using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.Base;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 当文本长度超出控件长度时,自动显示Tooltip
    /// </summary>
    public class TextTrimmingAutoShowTooltipTextBlock : UserControl
    {
        /// <summary>
        /// TextBlock样式依赖属性
        /// </summary>
        public static readonly DependencyProperty TextBlockStyleProperty =
            DependencyProperty.Register(nameof(TextBlockStyle), typeof(Style), typeof(TextTrimmingAutoShowTooltipTextBlock),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// TextBlock.Text依赖属性
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextTrimmingAutoShowTooltipTextBlock),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 获取或设置TextBlock样式
        /// </summary>
        public Style TextBlockStyle
        {
            get
            {
                return (Style)base.GetValue(TextBlockStyleProperty);
            }
            set
            {
                base.SetValue(TextBlockStyleProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置TextBlock.Text
        /// </summary>
        public string Text
        {
            get
            {
                return (string)base.GetValue(TextProperty);
            }
            set
            {
                base.SetValue(TextProperty, value);
            }
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (TextTrimmingAutoShowTooltipTextBlock)d;
            if (e.Property == TextBlockStyleProperty)
            {
                selfControl._textBlock.Style = (Style)e.NewValue;
            }
            else if (e.Property == TextProperty)
            {
                selfControl._textBlock.Text = (string)e.NewValue;
            }
        }


        private readonly TextBlock _textBlock;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextTrimmingAutoShowTooltipTextBlock()
            : base()
        {
            this.Background = Brushes.Transparent;


            this._textBlock = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.NoWrap
            };

            base.Content = this._textBlock;
            this.SizeChanged += TextTrimmingAutoShowTooltipTextBlock_SizeChanged;
        }


        private void TextTrimmingAutoShowTooltipTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetTooltip();
        }

        private void SetTooltip()
        {
            var size = UITextHelper.MeasureTextSize(this._textBlock);
            if (size.Width > this.ActualWidth)
            {
                this._textBlock.ToolTip = this._textBlock.Text;
            }
            else
            {
                this._textBlock.ToolTip = null;
            }
        }
    }
}
