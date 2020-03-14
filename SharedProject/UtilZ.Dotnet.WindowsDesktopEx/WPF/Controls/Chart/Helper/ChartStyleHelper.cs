using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart样式辅助类
    /// </summary>
    public class ChartStyleHelper
    {
        private static Style _segmentSeriesDefaultStyle = null;
        /// <summary>
        /// 获取SegmentSeries默认样式(Line)
        /// </summary>
        /// <returns>SegmentSeries默认样式</returns>
        public static Style GetSegmentSeriesDefaultStyle()
        {
            if (_segmentSeriesDefaultStyle == null)
            {
                var style = new Style();
                style.TargetType = typeof(Line);
                style.Setters.Add(new Setter(Line.StrokeProperty, ColorBrushHelper.GetColorByIndex(0)));
                style.Setters.Add(new Setter(Line.StrokeThicknessProperty, 2d));

                var trigger = new Trigger();
                trigger.Property = Line.IsMouseOverProperty;
                trigger.Value = true;
                trigger.Setters.Add(new Setter(Line.StrokeThicknessProperty, 3d));
                style.Triggers.Add(trigger);
                _segmentSeriesDefaultStyle = style;
            }

            return _segmentSeriesDefaultStyle;
        }

        /// <summary>
        /// 创建SegmentSeries样式(Line)
        /// </summary>
        /// <param name="stroke">Line.Stroke</param>
        /// <param name="strokeThickness">Line.StrokeThickness</param>
        /// <param name="mouseOverStrokeThickness">鼠标移上去时Line.StrokeThickness的值</param>
        /// <returns>SegmentSeries默认样式</returns>
        public static Style CreateSegmentSeriesStyle(Brush stroke, double strokeThickness = 2d, double mouseOverStrokeThickness = 3d)
        {
            var style = new Style();
            style.TargetType = typeof(Line);
            style.Setters.Add(new Setter(Line.StrokeProperty, stroke));
            style.Setters.Add(new Setter(Line.StrokeThicknessProperty, strokeThickness));

            var trigger = new Trigger();
            trigger.Property = Line.IsMouseOverProperty;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Line.StrokeThicknessProperty, mouseOverStrokeThickness));
            style.Triggers.Add(trigger);
            return style;
        }


        private static Style _defaultAxisLabelLineStyle = null;
        /// <summary>
        /// 获取默认坐标标签线样式(Path)
        /// </summary>
        /// <returns>默认坐标标签线样式</returns>
        public static Style GetDefaultAxisLabelLineStyle()
        {
            if (_defaultAxisLabelLineStyle == null)
            {
                var style = new Style();
                style.TargetType = typeof(System.Windows.Shapes.Path);
                //style.Setters.Add(new Setter(Path.StrokeProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F4F4"))));
                style.Setters.Add(new Setter(Path.StrokeProperty, Brushes.White));
                style.Setters.Add(new Setter(Path.StrokeThicknessProperty, 1.0d));
                _defaultAxisLabelLineStyle = style;
            }

            return _defaultAxisLabelLineStyle;
        }


        private static Style _defaultBackgroundLabelLineStyle = null;
        /// <summary>
        /// 获取默认坐标标签背景线样式(Path)
        /// </summary>
        /// <returns>默认坐标标签背景线样式</returns>
        public static Style GetDefaultBackgroundLabelLineStyle()
        {
            if (_defaultBackgroundLabelLineStyle == null)
            {
                var style = new Style();
                style.TargetType = typeof(System.Windows.Shapes.Path);
                style.Setters.Add(new Setter(Path.StrokeProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F4F4"))));
                //style.Setters.Add(new Setter(Path.StrokeProperty, Brushes.Gray));
                style.Setters.Add(new Setter(Path.StrokeThicknessProperty, 1.0d));
                _defaultBackgroundLabelLineStyle = style;
            }

            return _defaultBackgroundLabelLineStyle;
        }


        private static Dictionary<ChartDockOrientation, Style> _axisLabelDefaultStyleDic = null;
        /// <summary>
        /// 获取默认坐标标签样式(TextBlock)
        /// </summary>
        /// <returns>默认坐标标签样式</returns>
        public static Style GetAxisLabelStyle(ChartDockOrientation axisDockOrientation)
        {
            if (_axisLabelDefaultStyleDic == null)
            {
                _axisLabelDefaultStyleDic = new Dictionary<ChartDockOrientation, Style>();
            }

            Style defaultLabelStyle;
            if (_axisLabelDefaultStyleDic.TryGetValue(axisDockOrientation, out defaultLabelStyle))
            {
                return defaultLabelStyle;
            }

            defaultLabelStyle = new Style();
            defaultLabelStyle.TargetType = typeof(TextBlock);
            //defaultLabelStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Gray));
            defaultLabelStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.White));
            defaultLabelStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 12d));

            const double MARGIN = 5d;
            Thickness margin;
            switch (axisDockOrientation)
            {
                case ChartDockOrientation.Left:
                    margin = new Thickness(0d, 0d, MARGIN, 0d);
                    break;
                case ChartDockOrientation.Right:
                    margin = new Thickness(MARGIN, 0d, 0d, 0d);
                    break;
                case ChartDockOrientation.Top:
                    margin = new Thickness(0d, 0d, 0d, MARGIN);
                    break;
                case ChartDockOrientation.Bottom:
                    margin = new Thickness(0d, MARGIN, 0d, 0d);
                    break;
                default:
                    throw new NotImplementedException();
            }
            defaultLabelStyle.Setters.Add(new Setter(TextBlock.MarginProperty, margin));
            _axisLabelDefaultStyleDic.Add(axisDockOrientation, defaultLabelStyle);

            return defaultLabelStyle;
        }


        private static Dictionary<ChartDockOrientation, Style> _axisTitleStyleDic = null;
        /// <summary>
        /// 创建坐标标题样式(TextBlock)
        /// </summary>
        /// <returns>坐标标题样式</returns>
        public static Style CreateAxisTitleStyle(ChartDockOrientation axisDockOrientation)
        {
            if (_axisTitleStyleDic == null)
            {
                _axisTitleStyleDic = new Dictionary<ChartDockOrientation, Style>();
            }

            Style defaultTitleStyle;
            if (_axisLabelDefaultStyleDic.TryGetValue(axisDockOrientation, out defaultTitleStyle))
            {
                return defaultTitleStyle;
            }

            defaultTitleStyle = new Style();
            defaultTitleStyle.TargetType = typeof(TextBlock);
            defaultTitleStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Gray));
            defaultTitleStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 14d));

            const double MARGIN = 10d;
            Thickness margin;
            switch (axisDockOrientation)
            {
                case ChartDockOrientation.Left:
                    margin = new Thickness(0d - MARGIN, 0d, 0d, 0d);
                    break;
                case ChartDockOrientation.Right:
                    margin = new Thickness(MARGIN, 0d, 0d, 0d);
                    break;
                case ChartDockOrientation.Top:
                    margin = new Thickness(0d, 0d - MARGIN, 0d, 0d);
                    break;
                case ChartDockOrientation.Bottom:
                    margin = new Thickness(0d, MARGIN, 0d, 0d);
                    break;
                default:
                    throw new NotImplementedException();
            }
            defaultTitleStyle.Setters.Add(new Setter(TextBlock.MarginProperty, margin));
            _axisTitleStyleDic.Add(axisDockOrientation, defaultTitleStyle);

            return defaultTitleStyle;
        }


        private static Style _lineSeriesDefaultStyle = null;
        /// <summary>
        /// 获取LineSeries默认样式(Path)
        /// </summary>
        /// <returns>LineSeries默认样式</returns>
        public static Style GetLineSeriesDefaultStyle()
        {
            if (_lineSeriesDefaultStyle == null)
            {
                var style = new Style();
                style.TargetType = typeof(Path);
                style.Setters.Add(new Setter(Path.StrokeProperty, ColorBrushHelper.GetColorByIndex(0)));
                style.Setters.Add(new Setter(Path.StrokeThicknessProperty, 2d));
                //style.Setters.Add(new Setter(Path.FillProperty, Brushes.White));

                var trigger = new Trigger();
                trigger.Property = Path.IsMouseOverProperty;
                trigger.Value = true;
                trigger.Setters.Add(new Setter(Path.StrokeThicknessProperty, 3d));
                style.Triggers.Add(trigger);
                _lineSeriesDefaultStyle = style;
            }

            return _lineSeriesDefaultStyle;
        }

        /// <summary>
        /// 创建LineSeries样式(Path)
        /// </summary>
        /// <param name="stroke">Path.Stroke</param>
        /// <param name="strokeThickness">Path.StrokeThickness</param>
        /// <param name="mouseOverStrokeThickness">鼠标移上去时Path.StrokeThickness的值</param>
        /// <returns>LineSeries样式</returns>
        public static Style CreateLineSeriesStyle(Brush stroke, double strokeThickness = 2d, double mouseOverStrokeThickness = 3d)
        {
            var style = new Style();
            style.TargetType = typeof(Path);
            style.Setters.Add(new Setter(Path.StrokeProperty, stroke));
            style.Setters.Add(new Setter(Path.StrokeThicknessProperty, strokeThickness));
            //style.Setters.Add(new Setter(Path.FillProperty, Brushes.White));

            if (ChartHelper.DoubleHasValue(mouseOverStrokeThickness) && mouseOverStrokeThickness > ChartConstant.ZERO_D)
            {
                var trigger = new Trigger();
                trigger.Property = Path.IsMouseOverProperty;
                trigger.Value = true;
                trigger.Setters.Add(new Setter(Path.StrokeThicknessProperty, mouseOverStrokeThickness));
                style.Triggers.Add(trigger);
            }

            return style;
        }




        /// <summary>
        /// 创建ColumnSeries样式
        /// </summary>
        /// <param name="fill">Rectangle.Fill</param>
        /// <param name="stroke">Rectangle.Stroke</param>
        /// <param name="strokeThickness">Rectangle.StrokeThickness</param>
        /// <param name="mouseOverStrokeThickness">鼠标移上去时Rectangle.StrokeThickness的值</param>
        /// <returns>ColumnSeries样式</returns>
        public static Style CreateColumnSeriesStyle(Brush fill, Brush stroke, double strokeThickness, double mouseOverStrokeThickness = 1d)
        {
            var style = new Style();
            style.TargetType = typeof(Rectangle);
            style.Setters.Add(new Setter(Rectangle.FillProperty, fill));
            style.Setters.Add(new Setter(Rectangle.StrokeProperty, stroke));
            style.Setters.Add(new Setter(Rectangle.StrokeThicknessProperty, strokeThickness));

            if (ChartHelper.DoubleHasValue(mouseOverStrokeThickness) && mouseOverStrokeThickness > ChartConstant.ZERO_D)
            {
                var trigger = new Trigger();
                trigger.Property = Rectangle.IsMouseOverProperty;
                trigger.Value = true;
                trigger.Setters.Add(new Setter(Rectangle.StrokeThicknessProperty, mouseOverStrokeThickness));
                style.Triggers.Add(trigger);
            }

            return style;
        }

        /// <summary>
        /// 创建ColumnSeries样式(Rectangle)
        /// </summary>
        /// <param name="fill">Rectangle.Fill</param>
        /// <param name="mouseOverOpacity">鼠标移上去时Rectangle.Opacity的值</param>
        /// <returns></returns>
        public static Style CreateColumnSeriesStyle(Brush fill, double mouseOverOpacity = 0.8d)
        {
            var style = new Style();
            style.TargetType = typeof(Rectangle);
            style.Setters.Add(new Setter(Rectangle.FillProperty, fill));
            style.Setters.Add(new Setter(Rectangle.StrokeThicknessProperty, ChartConstant.ZERO_D));

            if (ChartHelper.DoubleHasValue(mouseOverOpacity) && mouseOverOpacity > ChartConstant.ZERO_D)
            {
                var trigger = new Trigger();
                trigger.Property = Rectangle.IsMouseOverProperty;
                trigger.Value = true;
                trigger.Setters.Add(new Setter(Rectangle.OpacityProperty, mouseOverOpacity));
                style.Triggers.Add(trigger);
            }

            return style;
        }

        /// <summary>
        /// 创建PieSeries样式(Path)
        /// </summary>
        /// <param name="stroke">Path.Stroke</param>
        /// <param name="fill">Path.Fill</param>
        /// <param name="strokeThickness">Path.StrokeThickness</param>
        /// <param name="mouseOverStrokeThickness">鼠标移上去时Path.StrokeThickness的值</param>
        /// <returns></returns>
        public static Style CreatePieSeriesStyle(Brush stroke, Brush fill, double strokeThickness = 0d, double mouseOverStrokeThickness = 2d)
        {
            Style style = CreateLineSeriesStyle(stroke, strokeThickness, mouseOverStrokeThickness);
            style.Setters.Add(new Setter(Path.FillProperty, fill));
            return style;
        }



        /// <summary>
        /// 创建雷达图坐标样式(Line)
        /// </summary>
        /// <param name="stroke">颜色</param>
        /// <returns>达图坐标样式</returns>
        public static Style CreateRadarSeriesAxisStytle(Brush stroke)
        {
            var style = new Style();
            style.TargetType = typeof(Line);
            style.Setters.Add(new Setter(Line.StrokeProperty, stroke));
            style.Setters.Add(new Setter(Line.StrokeThicknessProperty, 2.0d));
            return style;
        }

        /// <summary>
        /// 创建雷达图坐标样式(Path)
        /// </summary>
        /// <param name="stroke">颜色</param>
        /// <param name="fill">填充</param>
        /// <param name="opacity">透明度</param>
        /// <param name="strokeThickness">Path.StrokeThickness</param>
        /// <param name="mouseOverStrokeThickness">鼠标移上去时Path.StrokeThickness的值</param>
        /// <returns>达图坐标样式</returns>
        public static Style CreateRadarSeriesItemStytle(Brush stroke, Brush fill = null, double opacity = 0.5d, double strokeThickness = 2d, double mouseOverStrokeThickness = 3d)
        {
            Style style = CreateLineSeriesStyle(stroke, strokeThickness, mouseOverStrokeThickness);
            if (fill != null)
            {
                style.Setters.Add(new Setter(Path.FillProperty, fill));
            }

            if (ChartHelper.DoubleHasValue(opacity))
            {
                style.Setters.Add(new Setter(Path.OpacityProperty, opacity));
            }

            return style;
        }
    }
}
