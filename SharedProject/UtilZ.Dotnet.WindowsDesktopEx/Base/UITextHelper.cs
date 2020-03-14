using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// UI文本辅助类
    /// </summary>
    public static class UITextHelper
    {
#if CORE
        //pixelsPerDip参数参见:https://github.com/Microsoft/WPF-Samples/tree/master/PerMonitorDPI
        //https://code-examples.net/en/q/266958c

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="textBlock">TextBlock</param>
        /// <param name="flowDirection">测量方向</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(TextBlock textBlock, FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            var pixelsPerDip = VisualTreeHelper.GetDpi(textBlock).PixelsPerDip;
            Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
            var formattedText = new FormattedText(textBlock.Text, CultureInfo.InvariantCulture, flowDirection,
                typeface, textBlock.FontSize, textBlock.Foreground, pixelsPerDip);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="textBox">TextBox</param>
        /// <param name="flowDirection">测量方向</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(TextBox textBox, FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            var pixelsPerDip = VisualTreeHelper.GetDpi(textBox).PixelsPerDip;
            var formattedText = new FormattedText(textBox.Text, CultureInfo.InvariantCulture, flowDirection,
                new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
                textBox.FontSize, textBox.Foreground, pixelsPerDip);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="flowDirection">测量方向</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(Label label, FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            string text;
            if (label.Content == null)
            {
                text = string.Empty;
            }
            else
            {
                text = label.Content.ToString();
            }

            var pixelsPerDip = VisualTreeHelper.GetDpi(label).PixelsPerDip;
            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, flowDirection,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize, label.Foreground, pixelsPerDip);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="visual">目标的界面控件</param>
        /// <param name="text">要测量的文本</param>
        /// <param name="typeface">字符样式组合</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="foreground">字体Brush</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(Visual visual, string text, Typeface typeface, double fontSize, Brush foreground)
        {
            var pixelsPerDip = VisualTreeHelper.GetDpi(visual).PixelsPerDip;
            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, fontSize, foreground, pixelsPerDip);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="visual">目标的界面控件</param>
        /// <param name="text">要测量的文本</param>
        /// <param name="cultureInfo">区域信息</param>
        /// <param name="typeface">字符样式组合</param>
        /// <param name="flowDirection">测量方向</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="foreground">字体Brush</param>
        /// <param name="numberSubstitution">要应用于文本的数字替换行为</param>
        /// <param name="textFormattingMode">要应用于文本的 System.Windows.Media.TextFormattingMode</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(Visual visual, string text, CultureInfo cultureInfo, Typeface typeface, FlowDirection flowDirection,
            double fontSize, Brush foreground, NumberSubstitution numberSubstitution, TextFormattingMode textFormattingMode)
        {
            var pixelsPerDip = VisualTreeHelper.GetDpi(visual).PixelsPerDip;
            var formattedText = new FormattedText(text, cultureInfo, flowDirection,
                typeface, fontSize, foreground, numberSubstitution, textFormattingMode, pixelsPerDip);
            return GetTextSize(formattedText);
        }
#else
        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="textBlock">TextBlock</param>
        /// <param name="flowDirection">测量方向</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(TextBlock textBlock, FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            var formattedText = new FormattedText(textBlock.Text, CultureInfo.InvariantCulture, flowDirection,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize, textBlock.Foreground);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="textBox">TextBox</param>
        /// <param name="flowDirection">测量方向</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(TextBox textBox, FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            var formattedText = new FormattedText(textBox.Text, CultureInfo.InvariantCulture, flowDirection,
                new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
                textBox.FontSize, textBox.Foreground);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="flowDirection">测量方向</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(Label label, FlowDirection flowDirection = FlowDirection.LeftToRight)
        {
            string text;
            if (label.Content == null)
            {
                text = string.Empty;
            }
            else
            {
                text = label.Content.ToString();
            }

            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, flowDirection,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize, label.Foreground);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <param name="typeface">字符样式组合</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="foreground">字体Brush</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(string text, Typeface typeface, double fontSize, Brush foreground)
        {
            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, fontSize, foreground);
            return GetTextSize(formattedText);
        }

        /// <summary>
        /// 测量字符串长度
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <param name="cultureInfo">区域信息</param>
        /// <param name="typeface">字符样式组合</param>
        /// <param name="flowDirection">测量方向</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="foreground">字体Brush</param>
        /// <param name="numberSubstitution">要应用于文本的数字替换行为</param>
        /// <param name="textFormattingMode">要应用于文本的 System.Windows.Media.TextFormattingMode</param>
        /// <returns>符串长度</returns>
        public static Size MeasureTextSize(string text, CultureInfo cultureInfo, Typeface typeface, FlowDirection flowDirection,
            double fontSize, Brush foreground, NumberSubstitution numberSubstitution, TextFormattingMode textFormattingMode)
        {
            var formattedText = new FormattedText(text, cultureInfo, flowDirection,
                typeface, fontSize, foreground, numberSubstitution, textFormattingMode);
            return GetTextSize(formattedText);
        }
#endif

        private static Size GetTextSize(FormattedText formattedText)
        {
            return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        }



        /// <summary>
        /// Fast text size calculation
        /// </summary>
        /// <param name="glyphTypeface">The glyph typeface.</param>
        /// <param name="sizeInEm">The size.</param>
        /// <param name="text">The text.</param>
        /// <returns>The text size.</returns>
        public static Size MeasureTextSize(GlyphTypeface glyphTypeface, double sizeInEm, string text)
        {
            double width = 0;
            double lineWidth = 0;
            int lines = 0;
            foreach (char ch in text)
            {
                switch (ch)
                {
                    case '\n':
                        lines++;
                        if (lineWidth > width)
                        {
                            width = lineWidth;
                        }

                        lineWidth = 0;
                        continue;
                    case '\t':
                        continue;
                }

                ushort glyph = glyphTypeface.CharacterToGlyphMap[ch];
                double advanceWidth = glyphTypeface.AdvanceWidths[glyph];
                lineWidth += advanceWidth;
            }

            lines++;
            if (lineWidth > width)
            {
                width = lineWidth;
            }

            return new Size(Math.Round(width * sizeInEm, 2), Math.Round(lines * glyphTypeface.Height * sizeInEm, 2));
        }
    }
}
