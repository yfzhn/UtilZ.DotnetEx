using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;
using CoreWpfApp.Model;

namespace CoreWpfApp
{
    /// <summary>
    /// TestChart.xaml 的交互逻辑
    /// </summary>
    public partial class TestChart : Window
    {
        public TestChart()
        {
            InitializeComponent();
        }

        private TestChartVM _vm;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._vm = (TestChartVM)this.DataContext;

            var redirectAppenderToUI = (RedirectAppender)Loger.GetAppenderByName(null, "RedirectToUI");
            if (redirectAppenderToUI != null)
            {
                redirectAppenderToUI.RedirectOuput += RedirectLogOutput;
            }
        }

        private void RedirectLogOutput(object sender, RedirectOuputArgs e)
        {
            logControl.AddLog($"{DateTime.Now} {e.Item.Content}", e.Item.Level);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            this._vm.Test();
        }
    }

    internal class TestChartVM : NotifyPropertyChangedAbs
    {
        #region define
        private ChartCollection<ISeries> _series = null;
        public ChartCollection<ISeries> Series
        {
            get { return _series; }
            set
            {
                _series = value;
                base.OnRaisePropertyChanged();
            }
        }

        private double _chartMinHeight = double.NaN;
        public double ChartMinHeight
        {
            get { return _chartMinHeight; }
            set
            {
                _chartMinHeight = value;
                base.OnRaisePropertyChanged();
            }
        }

        private double _chartMinWidth = double.NaN;
        public double ChartMinWidth
        {
            get { return _chartMinWidth; }
            set
            {
                _chartMinWidth = value;
                base.OnRaisePropertyChanged();
            }
        }

        private ChartCollection<AxisAbs> _axes = null;
        public ChartCollection<AxisAbs> Axes
        {
            get { return _axes; }
            set
            {
                _axes = value;
                base.OnRaisePropertyChanged();
            }
        }


        private IChartLegend _legend = null;
        public IChartLegend Legend
        {
            get { return _legend; }
            set
            {
                _legend = value;
                base.OnRaisePropertyChanged();
            }
        }

        private bool _manaulComit = false;
        public bool ManaulComit
        {
            get { return _manaulComit; }
            set
            {
                _manaulComit = value;
                base.OnRaisePropertyChanged();
            }
        }
        #endregion

        private readonly Random _rnd = new Random();
        public TestChartVM()
        {

        }


        public void Test()
        {
            //TestNumAxis1();
            //TestNumAxis2();
            //TestDateTimeAxis();


            TestLineSeries();
            //TestVerStepLineSeries();

            //TestColumnSeriesHorizontal();
            //TestColumnSeriesVertical();
            //TestMuiltColumnSeriesVertical();

            //TestStackColumnHorizontal();
            //TestStackColumnVertical();

            //TestAllColumnVertical();

            //ValidateLineSeriesBezierData();


            //TestPieChart();

            //TestSegmentSeries();

            //TestRadarSeries();
        }


        private void TestRadarSeries()
        {
            this.ManaulComit = true;
            var values = new ValueCollection();
            int count = 5;
            int min = 10, mid = 50, max = 100;

            Style labelStyle = new Style();
            labelStyle.TargetType = typeof(TextBlock);
            labelStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.White));

            List<ChartLabelValue> labelList = new List<ChartLabelValue>();
            int index = 0;
            labelList.Add(new ChartLabelValue() { Label = $"Label_{index++}", MinValue = 0, MaxValue = 100, LabelStyle = labelStyle });
            labelList.Add(new ChartLabelValue() { Label = $"Label_{index++}", MinValue = 0, MaxValue = 100, LabelStyle = labelStyle });
            labelList.Add(new ChartLabelValue() { Label = $"Label_{index++}", MinValue = 0, MaxValue = 100, LabelStyle = labelStyle });
            labelList.Add(new ChartLabelValue() { Label = $"Label_{index++}", MinValue = 0, MaxValue = 100, LabelStyle = labelStyle });
            labelList.Add(new ChartLabelValue() { Label = $"Label_{index++}", MinValue = 0, MaxValue = 100, LabelStyle = labelStyle });
            //for (int i = 0; i < count; i++)
            //{
            //    labelList.Add(new ChartLabelValue()
            //    {
            //        Label = $"Label_{i}",
            //        MinValue = _rnd.Next(min, mid),
            //        MaxValue = _rnd.Next(mid, max),
            //        LabelStyle = labelStyle
            //    });
            //}

            int layerCount = 3;
            for (int j = 0; j < layerCount; j++)
            {
                List<IChartRadarValue> itemList = new List<IChartRadarValue>();
                for (int i = 0; i < count; i++)
                {
                    itemList.Add(new ChartRadarValue() { Label = labelList[i], Value = _rnd.Next(50, 90) });
                }
                //for (int i = 0; i < count; i++)
                //{
                //    itemList.Add(new ChartRadarValue() { Label = labelList[i], Value = _rnd.Next((int)labelList[i].MinValue, (int)labelList[i].MaxValue) });
                //}
                values.Add(new ChartNoAxisValue()
                {
                    Value = itemList,
                    TooltipText = $"TooltipText_{j}",
                    Style = ChartStyleHelper.CreateRadarSeriesItemStytle(ColorBrushHelper.GetNextColor(), ColorBrushHelper.GetNextColor()),
                    Title = $"RadarSeries_{j}"
                });
            }



            var series = new ChartCollection<ISeries>();
            series.Add(new RadarSeries()
            {
                EnableTooltip = true,
                Title = $"RadarSeries",
                Values = values,
            });

            this.Series = series;

            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Right,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            //this.Legend = new HorizontalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Top,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.ManaulComit = false;
        }

        private void TestSegmentSeries()
        {
            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                LabelStep = double.NaN,

            });

            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                //MinValue = 0,
                //MaxValue = 1,
                //LabelStep = 1d
                ZoomDelta = 0.3d
            });
            this.Axes = axes;


            int step = 10;
            int x1 = 0, x2, y, count, max = 100;
            var segmentValues = new ValueCollection();

            while (true)
            {
                x2 = x1 + step;
                if (x2 > max)
                {
                    break;
                }
                x1 = _rnd.Next(x1, x2);

                x2 = x1 + step;
                if (x2 > max)
                {
                    x2 = max;
                }
                x2 = _rnd.Next(x1 + 1, x2);
                count = x2 - x1;
                y = _rnd.Next(x1 + 1, x2);
                segmentValues.Add(new ChartAxisDoubleValue() { XValue1 = new ChartChildValue(x1), XValue2 = new ChartChildValue(x2), YValue1 = new ChartChildValue(y), TooltipText = $"{count }" });
            }


            var series = new ChartCollection<ISeries>();
            series.Add(new SegmentSeries()
            {
                AxisX = this._axes[0],
                AxisY = this._axes[1],
                EnableTooltip = true,
                Title = $"Segment_{fileName}",
                Values = segmentValues,
                Style = ChartStyleHelper.CreateSegmentSeriesStyle(ColorBrushHelper.GetNextColor()),
                Orientation = SeriesOrientation.Horizontal
            });

            this.Series = series;
            this.ManaulComit = false;
        }


        private void TestPieChart()
        {
            this.ManaulComit = true;
            var series = new ChartCollection<ISeries>();
            series.Add(new PieSeries()
            {
                //PushOut = double.NaN,
                Radius = double.NaN,
                EnableTooltip = true
            });

            //series.Add(new PieSeries()
            //{
            //    PushOut = double.NaN,
            //    Radius = 100d,
            //    EnableTooltip = true,
            //    Margin=new Thickness(220,0,0,0)
            //});


            int min = 10, max = 100;
            int count = 5;
            int value;
            var values1 = new ValueCollection();
            var values2 = new ValueCollection();

            for (int i = 0; i < count; i++)
            {
                value = _rnd.Next(min, max);
                //values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i}", null, $"{value}%"));
                if (i % 2 == 0)
                {
                    values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_ZZZZAAAAAABBBBBBB {i}", null, null));
                }
                else
                {
                    values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i}", null, null));
                }

                //value = _rnd.Next(min, max);
                //values2.Add(new ChartNoAxisValue(value, $"Item_{i}", null, $"{value}%"));
            }


            //int i = 0;

            //value = 120;
            //values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i++}", null, $"{value}%"));
            //value = 45;
            //values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i++}", null, $"{value}%"));
            //value = 60;
            //values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i++}", null, $"{value}%"));
            //value = 80;
            //values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i++}", null, $"{value}%"));
            //value = 5;
            //values1.Add(new ChartNoAxisValue(value, $"{value}个", $"Item_{i++}", null, $"{value}%"));

            series[0].Values = values1;
            //series[1].Values = values2;


            this.Series = series;
            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Right,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent,
                AllowChecked = true
            };
            this.ManaulComit = false;
        }







        private void ValidateLineSeriesBezierData()
        {
            int minY = 10, maxY = 100;
            DateTime minTime = DateTime.Parse("2010-01-01 00:00:00");
            DateTime maxTime = DateTime.Parse("2011-01-01 00:00:00");

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                //MinValue = minTime,
                //MaxValue = maxTime,
                LabelStep = null,
                EnableBackgroundLabelLine = true,
                ZoomDelta = 0.3d
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = minY,
                MaxValue = maxY,
                LabelStep = double.NaN,
                EnableBackgroundLabelLine = true,
                //ZoomDelta = 0.3d
            });


            this.Axes = axes;


            var series = new ChartCollection<ISeries>();
            series.Add(new LineSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                LineSeriesType = LineSeriesType.Bezier,
                EnableTooltip = true,
                Title = "Bezier",
                Style = ChartStyleHelper.CreateLineSeriesStyle(ColorBrushHelper.GetNextColor())
            });
            //series.Add(new LineSeries()
            //{
            //    AxisX = axes[0],
            //    AxisY = axes[1],
            //    LineSeriesType = LineSeriesType.PolyLine,
            //    EnableTooltip = true,
            //    Title = "PolyLine",
            //    Style = ChartStyleHelper.CreateLineSeriesStyle(ColorBrushHelper.GetNextColor()),
            //    //CreatePointFunc = this.CreatePointFunc
            //});



            int value = (minY + maxY) / 2;
            DateTime time = minTime;

            var values1 = new ValueCollection();
            var values2 = new ValueCollection();
            IChartValue chartValue;
            double stepTotalMilliseconds = TimeSpan.FromDays(1).TotalMilliseconds;
            int offset = 10;

            while (time < maxTime)
            {
                value = this.NextValue(value, minY, offset);
                chartValue = new ChartAxisValue(time, value, $"{time.ToString()}_{value}");
                values1.Add(chartValue);
                values2.Add(chartValue);
                time = time.AddMilliseconds(stepTotalMilliseconds);
            }
            series[0].Values = values1;
            // series[1].Values = values2;


            this.Series = series;
            this.Legend = new HorizontalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent,
                AllowChecked = true
            };
            this.ManaulComit = false;
        }
        private int NextValue(int value, int minY, int offset)
        {
            int min = value - offset;
            if (min < minY)
            {
                min = minY;
            }
            int max = min + offset * 2;
            value = _rnd.Next(min, max);
            return value;
        }









        private void TestAllColumnVertical()
        {
            int min = 10, max = 100;

            this.ManaulComit = true;

            //this.ChartMinWidth = 900d;
            //this.ChartMinHeight = 400d;

            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new LabelAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                EnableBackgroundLabelLine = true,
                LabelSize = 0d,
                //MinValue = min,
                //MaxValue = max,
                LabelStep = double.NaN
            });

            DateTime minTime = DateTime.Parse("2010-01-01 00:00:00");
            DateTime maxTime = DateTime.Parse("2010-12-31 23:23:59");
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.RightToLeft,
                MinValue = minTime,
                MaxValue = maxTime,
                CustomAxisTextFormatCunc = this.DatetimeAsixCustomAxisTextFormatCunc,
                LabelStep = null,
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Right,
                Orientation = AxisLabelOrientation.BottomToTop,
                EnableBackgroundLabelLine = false,
                MinValue = min,
                MaxValue = max,
                //CustomAxisTextFormatCunc = this.DatetimeAsixCustomAxisTextFormatCunc,
                LabelStep = double.NaN
            });
            this.Axes = axes;


            int columnCount = 5;
            var series = new ChartCollection<ISeries>();

            int stackCount = 3;
            var titleStyleDic = new Dictionary<string, Style>();
            Style style;
            for (int i = 0; i < stackCount; i++)
            {
                style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor());
                titleStyleDic.Add($"StackedColumn{i}", style);
            }
            series.Add(new StackedColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                TitleStyleDic = titleStyleDic
            });


            var time = DateTime.Parse("2010-01-01 00:00:00");
            double value;
            var values1 = new ValueCollection();
            for (int i = 0; i < columnCount; i++)
            {
                var x = new List<IChartChildValue>();
                for (int j = 0; j < stackCount; j++)
                {
                    value = _rnd.Next(min, max);
                    x.Add(new ChartChildValue(value, $"{value}", null));
                }
                values1.Add(new ChartAxisValue(time, x, null));
                time = time.AddMonths(1);
            }
            series[0].Values = values1;




            series.Add(new ColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                Title = "ColumnSeries1",
                Style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor())
            });
            series.Add(new ColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                Title = "ColumnSeries2",
                Style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor())
            });

            time = DateTime.Parse("2010-01-01 00:00:00");
            value = min;
            var values2 = new ValueCollection();
            var values3 = new ValueCollection();
            for (int i = 0; i < columnCount; i++)
            {
                value = _rnd.Next(min, max);
                values2.Add(new ChartAxisValue(time, value, $"{value}"));

                value = _rnd.Next(min, max);
                values3.Add(new ChartAxisValue(time, value, $"{value}"));

                time = time.AddMonths(1);
            }
            series[1].Values = values2;
            series[2].Values = values3;


            this.Series = series;
            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Left,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            //this.Legend = new HorizontalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Top,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.ManaulComit = false;
        }

        private void TestStackColumnVertical()
        {
            int min = 0, max = 100;

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new LabelAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                ZoomDelta = 0.3d
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                EnableBackgroundLabelLine = true,
                LabelSize = 0d,
                //MinValue = min,
                //MaxValue = max,
                LabelStep = double.NaN,
            });
            this.Axes = axes;



            var series = new ChartCollection<ISeries>();

            int stackCount = 3;
            var titleStyleDic = new Dictionary<string, Style>();
            Style style;
            for (int i = 0; i < stackCount; i++)
            {
                style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor());
                titleStyleDic.Add($"StackedColumn{i}", style);
            }
            series.Add(new StackedColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                TitleStyleDic = titleStyleDic
            });


            var time = DateTime.Parse("2010-01-01 00:00:00");
            double value;
            var values = new ValueCollection();
            for (int i = 0; i < 5; i++)
            {
                var x = new List<IChartChildValue>();
                for (int j = 0; j < stackCount; j++)
                {
                    value = _rnd.Next(min, max);
                    x.Add(new ChartChildValue(value, $"{value}", null));
                }
                values.Add(new ChartAxisValue(time, x));
                time = time.AddMonths(1);
            }
            series[0].Values = values;




            this.Series = series;
            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Right,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            //this.Legend = new HorizontalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Bottom,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.ManaulComit = false;
        }

        private void TestStackColumnHorizontal()
        {
            int min = 0, max = 100;

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new LabelAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.TopToBottom,
                ZoomDelta = 0.3d
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.LeftToRight,
                //MinValue = min,
                //MaxValue = max,
                LabelStep = double.NaN,
                ZoomDelta = 0.3d
            });
            this.Axes = axes;



            var series = new ChartCollection<ISeries>();

            int stackCount = 3;
            var titleStyleDic = new Dictionary<string, Style>();
            Style style;
            for (int i = 0; i < stackCount; i++)
            {
                style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor());
                titleStyleDic.Add($"StackedColumn{i}", style);
            }
            series.Add(new StackedColumnSeries()
            {
                AxisX = axes[1],
                AxisY = axes[0],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Horizontal,
                TitleStyleDic = titleStyleDic
            });


            var time = DateTime.Parse("2010-01-01 00:00:00");
            double value;
            var values = new ValueCollection();
            for (int i = 0; i < 5; i++)
            {
                var xValues = new List<IChartChildValue>();
                for (int j = 0; j < stackCount; j++)
                {
                    value = _rnd.Next(min, max);
                    xValues.Add(new ChartChildValue(value, $"{value}", null));
                }
                values.Add(new ChartAxisValue(xValues, time));
                time = time.AddMonths(1);
            }
            series[0].Values = values;




            this.Series = series;
            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Right,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            //this.Legend = new HorizontalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Bottom,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.ManaulComit = false;
        }



        private void TestMuiltColumnSeriesVertical()
        {
            int min = -100, max = 100;

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new LabelAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                AxisSize = 200d,
                //CustomAxisTextFormatCunc = (t) => { return ((DateTime)t).ToString("yyyy-MM-dd \r\n    HH:mm:ss"); },
                //Angle = 330  //1:310;2:220;3:130;4:50
                Angle = double.NaN,
                ZoomDelta = 0.3d
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = min,
                MaxValue = max,
                LabelStep = double.NaN,
                EnableBackgroundLabelLine = true,
                LabelSize = 0d,
                ZoomDelta = 0.3d
            });
            this.Axes = axes;




            var series = new ChartCollection<ISeries>();
            series.Add(new ColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                Title = "ColumnSeries1",
                Style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor())
            });
            series.Add(new ColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                Title = "ColumnSeries2",
                Style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor())
            });

            int count = 5;
            var time = DateTime.Parse("2010-01-01 00:00:00");
            double value = min;
            var values = new ValueCollection();
            var values2 = new ValueCollection();
            for (int i = 0; i < count; i++)
            {
                value = _rnd.Next(min, max);
                values.Add(new ChartAxisValue(time, value, $"{value}"));

                value = _rnd.Next(min, max);
                values2.Add(new ChartAxisValue(time, value, $"{value}"));

                time = time.AddMonths(1);
            }
            series[0].Values = values;
            series[1].Values = values2;




            this.Series = series;

            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Right,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            //this.Legend = new HorizontalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Bottom,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.ManaulComit = false;
        }

        private void TestColumnSeriesVertical()
        {
            int min = -100, max = 100;

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new LabelAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                AxisSize = 100d,
                ZoomDelta = 0.3d
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = min,
                MaxValue = max,
                LabelStep = double.NaN,
                ZoomDelta = 0.3d
            });
            this.Axes = axes;




            var series = new ChartCollection<ISeries>();
            series.Add(new ColumnSeries()
            {
                AxisX = axes[0],
                AxisY = axes[1],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Vertical,
                Title = "ColumnSeries2",
                Style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor())
            }); ;


            var time = DateTime.Parse("2010-01-01 00:00:00");
            double value;
            var values = new ValueCollection();
            for (int i = 0; i < 5; i++)
            {
                value = _rnd.Next(min, max);
                values.Add(new ChartAxisValue(time, value, $"{value}"));
                time = time.AddMonths(1);
            }
            series[0].Values = values;




            this.Series = series;

            //this.Legend = new VerticalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Right,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.Legend = new HorizontalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            this.ManaulComit = false;
        }

        private void TestColumnSeriesHorizontal()
        {
            int min = -100, max = 100;

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new LabelAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.TopToBottom,
                ZoomDelta = 0.3d
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = min,
                MaxValue = max,
                LabelStep = double.NaN,
                ZoomDelta = 0.3d
            });
            this.Axes = axes;




            var series = new ChartCollection<ISeries>();
            series.Add(new ColumnSeries()
            {
                AxisX = axes[1],
                AxisY = axes[0],
                EnableTooltip = true,
                Orientation = SeriesOrientation.Horizontal,
                Title = "ColumnSeries1",
                Style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetNextColor())
            });


            var time = DateTime.Parse("2010-01-01 00:00:00");
            double value;
            var values = new ValueCollection();
            for (int i = 0; i < 5; i++)
            {
                value = _rnd.Next(min, max);
                values.Add(new ChartAxisValue(value, time, $"{value}"));
                time = time.AddMonths(1);
            }
            series[0].Values = values;




            this.Series = series;
            this.Legend = new VerticalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Right,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            //this.Legend = new HorizontalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Bottom,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.ManaulComit = false;
        }


        private const string fileName = "StepLineSeriesValues.txt";
        private void TestVerStepLineSeries()
        {
            int minY = -100, maxY = 100;
            double minX = -1000, maxX = 1000;
            DateTime minTime = DateTime.Parse("2010-01-01 00:00:00");
            DateTime maxTime = DateTime.Parse("2012-01-01 00:00:00");

            this.ManaulComit = true;
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.TopToBottom,
                MinValue = minTime,
                MaxValue = maxTime,
                LabelStep = null
            });

            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = minX,
                MaxValue = maxX,
                LabelStep = double.NaN,
                ZoomDelta = 0.3d
            });

            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = minY,
                MaxValue = maxY,
                LabelStep = double.NaN
            });


            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Right,
                Orientation = AxisLabelOrientation.TopToBottom,
                MinValue = -100000,
                MaxValue = 100000,
                LabelStep = double.NaN
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.RightToLeft,
                MinValue = -100000,
                MaxValue = 100000,
                LabelStep = double.NaN
            });
            this.Axes = axes;


            var series = new ChartCollection<ISeries>();
            series.Add(new LineSeries()
            {
                AxisX = axes[1],
                AxisY = axes[0],
                LineSeriesType = LineSeriesType.Bezier,
                EnableTooltip = true,
                Title = "LineSeries",
                Style = ChartStyleHelper.CreateLineSeriesStyle(Brushes.Gray)
            });
            series.Add(new LineSeries()
            {
                AxisX = axes[2],
                AxisY = axes[0],
                LineSeriesType = LineSeriesType.Bezier,
                EnableTooltip = true,
                Title = "DateTimeLineSeries",
                Style = ChartStyleHelper.CreateLineSeriesStyle(Brushes.Green),
                //CreatePointFunc = this.CreatePointFunc
            });
            series.Add(new StepLineSeries()
            {
                AxisX = axes[2],
                AxisY = axes[0],
                EnableTooltip = true,
                Title = "DateTimeStepLineSeries",
                Style = ChartStyleHelper.CreateLineSeriesStyle(Brushes.Red),
                Orientation = SeriesOrientation.Vertical
            });


            double value;
            DateTime time;
            var values3 = new ValueCollection();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = sr.ReadLine();
                string[] strArr;
                while (line != null)
                {
                    strArr = line.Split('_', 2, StringSplitOptions.RemoveEmptyEntries);
                    time = DateTime.Parse(strArr[0]);
                    value = double.Parse(strArr[1]);
                    //values3.Add(new ChartDateTimeItem2(time, value, $"{time.ToString()}_{value}"));
                    values3.Add(new ChartAxisValue(value, time, $"{time.ToString()}_{value}"));

                    line = sr.ReadLine();
                }
            }
            series[2].Values = values3;



            this.Series = series;
            this.Legend = new HorizontalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            this.ManaulComit = false;
        }

        private void TestLineSeries()
        {
            int minY = -100, maxY = 100;
            double minX = -1000, maxX = 1000;
            DateTime minTime = DateTime.Parse("2010-01-01 00:00:00");
            DateTime maxTime = DateTime.Parse("2012-01-01 00:00:00");

            this.ManaulComit = true;

            this.ChartMinWidth = 900d;
            //this.ChartMinHeight = 400d;


            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = minY,
                MaxValue = maxY,
                LabelStep = double.NaN,
                ZoomDelta = double.NaN,
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = minX,
                MaxValue = maxX,
                LabelStep = double.NaN,
                ZoomDelta = double.NaN,
                Title = "XXXBottom"
            });

            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = minTime,
                MaxValue = maxTime,
                LabelStep = null,
                ZoomDelta = 0.3d,
            });


            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Right,
                Orientation = AxisLabelOrientation.TopToBottom,
                MinValue = -100000,
                MaxValue = 100000,
                LabelStep = double.NaN
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.RightToLeft,
                MinValue = -100000,
                MaxValue = 100000,
                LabelStep = double.NaN
            });
            this.Axes = axes;


            var series = new ChartCollection<ISeries>();
            series.Add(new LineSeries()
            {
                AxisX = axes[1],
                AxisY = axes[0],
                LineSeriesType = LineSeriesType.Bezier,
                EnableTooltip = true,
                Title = "LineSeries",
                Style = ChartStyleHelper.CreateLineSeriesStyle(Brushes.Gray)
            });
            series.Add(new LineSeries()
            {
                AxisX = axes[2],
                AxisY = axes[0],
                LineSeriesType = LineSeriesType.Bezier,
                EnableTooltip = true,
                Title = "DateTimeLineSeries",
                Style = ChartStyleHelper.CreateLineSeriesStyle(Brushes.Green),
                //CreatePointFunc = this.CreatePointFunc
            });
            series.Add(new StepLineSeries()
            {
                AxisX = axes[2],
                AxisY = axes[0],
                EnableTooltip = true,
                Title = "DateTimeStepLineSeries",
                Style = ChartStyleHelper.CreateLineSeriesStyle(Brushes.Red),
                Orientation = SeriesOrientation.Horizontal
            });


            double value;
            double axisXValueStep = 10;
            double axisXValue = minX;
            var values = new ValueCollection();
            while (axisXValue < maxX)
            {
                value = _rnd.Next(minY, maxY);
                values.Add(new ChartAxisValue(axisXValue, value, $"{axisXValue}_{value}"));
                axisXValue += axisXValueStep;
            }
            series[0].Values = values;



            DateTime time = minTime;
            TimeSpan ts = maxTime - time;
            var values2 = new ValueCollection();
            double stepTotalMilliseconds = ts.TotalMilliseconds / ((maxY - minY) / axisXValueStep);
            while (time < maxTime)
            {
                value = _rnd.Next(minY, maxY);
                values2.Add(new ChartAxisValue(time, value, $"{time.ToString()}_{value}"));
                time = time.AddMilliseconds(stepTotalMilliseconds);
            }
            series[1].Values = values2;


            //StringBuilder sb = new StringBuilder();
            var values3 = new ValueCollection();
            minY = minY / 10;
            maxY = maxY / 10;
            time = minTime;
            while (time < maxTime)
            {
                value = _rnd.Next(minY, maxY) * 10;
                values3.Add(new ChartAxisValue(time, value, $"{time.ToString()}_{value}"));
                //sb.AppendLine($"{time.ToString("yyyy-MM-dd HH:mm:ss")}_{value}");
                ts = maxTime - time;
                time = time.AddDays(_rnd.Next(1, (int)ts.TotalDays));
            }
            //File.WriteAllText(fileName, sb.ToString());
            series[2].Values = values3;



            this.Series = series;
            //this.Legend = new VerticalChartLegend()
            //{
            //    DockOrientation = ChartDockOrientation.Right,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Transparent
            //};
            this.Legend = new HorizontalChartLegend()
            {
                DockOrientation = ChartDockOrientation.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent
            };
            this.ManaulComit = false;
        }


        private FrameworkElement CreatePointFunc(PointInfo pointInfo)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Red;
            ellipse.StrokeThickness = 0d;
            ellipse.Width = 5d;
            ellipse.Height = 5d;
            ellipse.Margin = new Thickness(-2.5d, -2.5d, 0d, 0d);
            ellipse.ToolTip = $"X:{pointInfo.Point.X}     Y:{pointInfo.Point.Y}       {pointInfo.Item.ToString()}";
            return ellipse;
        }


        private void TestDateTimeAxis()
        {
            DateTime min = DateTime.Parse("2010-01-01 00:00:00");
            DateTime max = DateTime.Parse("2010-12-31 23:23:59");

            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                //MinValue = min,
                MaxValue = max,
                CustomAxisTextFormatCunc = this.DatetimeAsixCustomAxisTextFormatCunc,
                LabelStep = TimeSpan.FromMinutes(20)
            });
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.RightToLeft,
                MinValue = min,
                MaxValue = max,
                CustomAxisTextFormatCunc = this.DatetimeAsixCustomAxisTextFormatCunc,
                LabelStep = null
            });
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = min,
                //MaxValue = max,
                CustomAxisTextFormatCunc = this.DatetimeAsixCustomAxisTextFormatCunc,
                LabelStep = TimeSpan.FromMinutes(200)
            });
            axes.Add(new DateTimeAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Right,
                Orientation = AxisLabelOrientation.TopToBottom,
                MinValue = min,
                MaxValue = max,
                CustomAxisTextFormatCunc = this.DatetimeAsixCustomAxisTextFormatCunc,
                LabelStep = null
            });
            this.Axes = axes;
        }

        private string DatetimeAsixCustomAxisTextFormatCunc(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void TestNumAxis2()
        {
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = -100,
                //MaxValue = 100,
                LabelStep = 50
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Right,
                Orientation = AxisLabelOrientation.TopToBottom,
                MinValue = -100000,
                MaxValue = 100000,
                LabelStep = double.NaN
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                //MinValue = -1000,
                MaxValue = 1000,
                LabelStep = 200
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.RightToLeft,
                MinValue = -100000,
                MaxValue = 100000,
                LabelStep = double.NaN
            });
            this.Axes = axes;
        }

        private void TestNumAxis1()
        {
            var axes = new ChartCollection<AxisAbs>();
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Left,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = 0,
                MaxValue = 100,
                LabelStep = 20
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.Y,
                DockOrientation = ChartDockOrientation.Right,
                Orientation = AxisLabelOrientation.BottomToTop,
                MinValue = -100,
                MaxValue = 100,
                LabelStep = double.NaN
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Bottom,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = 0,
                MaxValue = 1000,
                LabelStep = 200
            });
            axes.Add(new NumberAxis()
            {
                AxisType = AxisType.X,
                DockOrientation = ChartDockOrientation.Top,
                Orientation = AxisLabelOrientation.LeftToRight,
                MinValue = -1000,
                MaxValue = 1000,
                LabelStep = double.NaN
            });
            this.Axes = axes;
        }
    }

    internal class TimerChartValue
    {
        private ChartCollection<IChartValue> _values1;
        private ChartCollection<IChartValue> _values2;
        private DateTime _time;
        private int _offset;
        private int _value;
        private int _minY;
        private ThreadEx _thread;

        public TimerChartValue()
        {
            this._thread = new ThreadEx(this.ThreadMethod, "ThreadMethod", true);
        }

        private void ThreadMethod(ThreadExPara para)
        {
            const int INTERVAL = 500;
            try
            {
                while (!para.Token.IsCancellationRequested)
                {
                    try
                    {
                        para.WaitOne(INTERVAL);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    this._value = this.NextValue(this._value, this._minY, this._offset);
                    //this._values1.Add()
                    //this._values2
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        internal void Start(ChartCollection<IChartValue> values1, ChartCollection<IChartValue> values2, DateTime time, int value, int minY, int offset)
        {
            this._time = time;
            this._offset = offset;
            this._value = value;
            this._minY = minY;
            this._values1 = values1;
            this._values2 = values2;
            this._thread.Start();
        }

        private readonly Random _rnd = new Random();
        internal int NextValue(int value, int minY, int offset)
        {
            int min = value - offset;
            if (min < minY)
            {
                min = minY;
            }
            int max = min + offset * 2;
            value = _rnd.Next(min, max);
            return value;
        }


        internal void Stop()
        {
            this._thread.Stop();
            this._values1 = null;
            this._values2 = null;
        }
    }
}
