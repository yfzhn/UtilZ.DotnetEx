using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal class ChartGridRowColumnDefinition
    {
        public RowColumnDefinitionItem Legend { get; private set; }

        public RowColumnDefinitionItem LeftAxis { get; private set; }

        public RowColumnDefinitionItem RightAxis { get; private set; }

        public RowColumnDefinitionItem TopAxis { get; private set; }

        public RowColumnDefinitionItem BottomAxis { get; private set; }

        public RowColumnDefinitionItem Chart { get; private set; }


        /// <summary>
        /// NoFreeze
        /// </summary>
        /// <param name="hasLegend"></param>
        /// <param name="legend"></param>
        /// <param name="chartGrid"></param>
        /// <param name="axisYWidthInfo"></param>
        /// <param name="axisXHeightInfo"></param>
        public ChartGridRowColumnDefinition(bool hasLegend, IChartLegend legend, Grid chartGrid,
           AxisYWidthInfo axisYWidthInfo, AxisXHeightInfo axisXHeightInfo)
        {
            if (hasLegend)
            {
                this.Legend = new RowColumnDefinitionItem();
            }

            if (axisYWidthInfo.LeftAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.LeftAxis = new RowColumnDefinitionItem();
            }

            if (axisYWidthInfo.RightAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.RightAxis = new RowColumnDefinitionItem();
            }

            if (axisXHeightInfo.TopAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.TopAxis = new RowColumnDefinitionItem();
            }

            if (axisXHeightInfo.BottomAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.BottomAxis = new RowColumnDefinitionItem();
            }

            this.Chart = new RowColumnDefinitionItem();



            if (axisYWidthInfo.LeftAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.LeftAxis.CreateColumn(chartGrid, axisYWidthInfo.LeftAxisTotalWidth, GridUnitType.Pixel);
            }
            this.Chart.CreateColumn(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (axisYWidthInfo.RightAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.RightAxis.CreateColumn(chartGrid, axisYWidthInfo.RightAxisTotalWidth, GridUnitType.Pixel);
            }


            if (axisXHeightInfo.TopAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.TopAxis.CreateRow(chartGrid, axisXHeightInfo.TopAxisTotalHeight, GridUnitType.Pixel);
            }
            this.Chart.CreateRow(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (axisXHeightInfo.BottomAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.BottomAxis.CreateRow(chartGrid, axisXHeightInfo.BottomAxisTotalHeight, GridUnitType.Pixel);
            }

            if (hasLegend)
            {
                this.Legend.CreateLegendRowColumn(chartGrid, legend);
            }


            if (this.LeftAxis != null)
            {
                this.LeftAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            if (this.RightAxis != null)
            {
                this.RightAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            if (this.TopAxis != null)
            {
                this.TopAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            if (this.BottomAxis != null)
            {
                this.BottomAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            this.Chart.MergeRowColumn(chartGrid, this.Chart);
        }


        /// <summary>
        /// FreezeY
        /// </summary>
        /// <param name="hasLegend"></param>
        /// <param name="legend"></param>
        /// <param name="chartGrid"></param>
        /// <param name="axisYWidthInfo"></param>
        public ChartGridRowColumnDefinition(bool hasLegend, IChartLegend legend, Grid chartGrid, AxisYWidthInfo axisYWidthInfo)
        {
            if (hasLegend)
            {
                this.Legend = new RowColumnDefinitionItem();
            }

            if (axisYWidthInfo.LeftAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.LeftAxis = new RowColumnDefinitionItem();
            }

            if (axisYWidthInfo.RightAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.RightAxis = new RowColumnDefinitionItem();
            }

            this.Chart = new RowColumnDefinitionItem();





            if (axisYWidthInfo.LeftAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.LeftAxis.CreateColumn(chartGrid, axisYWidthInfo.LeftAxisTotalWidth, GridUnitType.Pixel);
            }
            this.Chart.CreateColumn(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (axisYWidthInfo.RightAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.RightAxis.CreateColumn(chartGrid, axisYWidthInfo.RightAxisTotalWidth, GridUnitType.Pixel);
            }

            if (hasLegend)
            {
                if (legend.DockOrientation == ChartDockOrientation.Top ||
                    legend.DockOrientation == ChartDockOrientation.Bottom)
                {
                    this.Chart.CreateRow(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
                }

                this.Legend.CreateLegendRowColumn(chartGrid, legend);
            }

            if (this.LeftAxis != null)
            {
                this.LeftAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            if (this.RightAxis != null)
            {
                this.RightAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            this.Chart.MergeRowColumn(chartGrid, this.Chart);
        }


        public ChartGridRowColumnDefinition(Grid chartContentGrid, AxisXHeightInfo axisXHeightInfo)
        {
            if (axisXHeightInfo.TopAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.TopAxis = new RowColumnDefinitionItem();
            }

            if (axisXHeightInfo.BottomAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.BottomAxis = new RowColumnDefinitionItem();
            }

            this.Chart = new RowColumnDefinitionItem();


            if (axisXHeightInfo.TopAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.TopAxis.CreateRow(chartContentGrid, axisXHeightInfo.TopAxisTotalHeight, GridUnitType.Pixel);
            }
            this.Chart.CreateRow(chartContentGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (axisXHeightInfo.BottomAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.BottomAxis.CreateRow(chartContentGrid, axisXHeightInfo.BottomAxisTotalHeight, GridUnitType.Pixel);
            }



            if (this.TopAxis != null)
            {
                this.TopAxis.MergeRowColumn(chartContentGrid, this.Chart);
            }

            if (this.BottomAxis != null)
            {
                this.BottomAxis.MergeRowColumn(chartContentGrid, this.Chart);
            }

            this.Chart.MergeRowColumn(chartContentGrid, this.Chart);
        }


        /// <summary>
        /// FreezeY
        /// </summary>
        /// <param name="hasLegend"></param>
        /// <param name="legend"></param>
        /// <param name="chartGrid"></param>
        public ChartGridRowColumnDefinition(bool hasLegend, IChartLegend legend, Grid chartGrid)
        {
            if (hasLegend)
            {
                this.Legend = new RowColumnDefinitionItem();
            }

            this.Chart = new RowColumnDefinitionItem();


            this.Chart.CreateColumn(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            this.Chart.CreateRow(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (hasLegend)
            {
                this.Legend.CreateLegendRowColumn(chartGrid, legend);
            }

            this.Chart.MergeRowColumn(chartGrid, this.Chart);
        }


        /// <summary>
        /// FreezeX
        /// </summary>
        /// <param name="hasLegend"></param>
        /// <param name="legend"></param>
        /// <param name="chartGrid"></param>
        /// <param name="axisXHeightInfo"></param>
        public ChartGridRowColumnDefinition(bool hasLegend, IChartLegend legend, Grid chartGrid, AxisXHeightInfo axisXHeightInfo)
        {
            if (hasLegend)
            {
                this.Legend = new RowColumnDefinitionItem();
            }

            if (axisXHeightInfo.TopAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.TopAxis = new RowColumnDefinitionItem();
            }

            if (axisXHeightInfo.BottomAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.BottomAxis = new RowColumnDefinitionItem();
            }

            this.Chart = new RowColumnDefinitionItem();







            if (axisXHeightInfo.TopAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.TopAxis.CreateRow(chartGrid, axisXHeightInfo.TopAxisTotalHeight, GridUnitType.Pixel);
            }
            this.Chart.CreateRow(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (axisXHeightInfo.BottomAxisTotalHeight > ChartConstant.ZERO_D)
            {
                this.BottomAxis.CreateRow(chartGrid, axisXHeightInfo.BottomAxisTotalHeight, GridUnitType.Pixel);
            }

            if (hasLegend)
            {
                if (legend.DockOrientation == ChartDockOrientation.Left ||
                    legend.DockOrientation == ChartDockOrientation.Right)
                {
                    this.Chart.CreateColumn(chartGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
                }

                this.Legend.CreateLegendRowColumn(chartGrid, legend);
            }

            if (this.TopAxis != null)
            {
                this.TopAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            if (this.BottomAxis != null)
            {
                this.BottomAxis.MergeRowColumn(chartGrid, this.Chart);
            }

            this.Chart.MergeRowColumn(chartGrid, this.Chart);
        }


        public ChartGridRowColumnDefinition(Grid chartContentGrid, AxisYWidthInfo axisYWidthInfo)
        {
            if (axisYWidthInfo.LeftAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.LeftAxis = new RowColumnDefinitionItem();
            }
            this.Chart = new RowColumnDefinitionItem();
            if (axisYWidthInfo.RightAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.RightAxis = new RowColumnDefinitionItem();
            }



            if (axisYWidthInfo.LeftAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.LeftAxis.CreateColumn(chartContentGrid, axisYWidthInfo.LeftAxisTotalWidth, GridUnitType.Pixel);
            }
            this.Chart.CreateColumn(chartContentGrid, ChartConstant.GRID_START_SIZE, GridUnitType.Star);
            if (axisYWidthInfo.RightAxisTotalWidth > ChartConstant.ZERO_D)
            {
                this.RightAxis.CreateColumn(chartContentGrid, axisYWidthInfo.RightAxisTotalWidth, GridUnitType.Pixel);
            }

            if (this.LeftAxis != null)
            {
                this.LeftAxis.MergeRowColumn(chartContentGrid, this.Chart);
            }

            if (this.RightAxis != null)
            {
                this.RightAxis.MergeRowColumn(chartContentGrid, this.Chart);
            }
            this.Chart.MergeRowColumn(chartContentGrid, this.Chart);
        }
    }
}
