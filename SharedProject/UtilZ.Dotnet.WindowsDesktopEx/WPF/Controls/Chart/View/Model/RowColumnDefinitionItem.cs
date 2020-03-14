using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal class RowColumnDefinitionItem
    {
        private RowDefinition _row = null;
        private ColumnDefinition _column = null;

        public int RowIndex { get; set; } = ChartConstant.ROW_COLUMN_DEFAULT_INDEX;

        public int ColumnIndex { get; set; } = ChartConstant.ROW_COLUMN_DEFAULT_INDEX;

        public int RowSpan { get; set; } = ChartConstant.ROW_COLUMN_DEFAULT_INDEX;

        public int ColumnSpan { get; set; } = ChartConstant.ROW_COLUMN_DEFAULT_INDEX;


        public RowColumnDefinitionItem()
        {

        }


        internal void CreateRow(Grid chartGrid, double height, GridUnitType gridUnitType)
        {
            this._row = new RowDefinition() { Height = new GridLength(height, gridUnitType) };
            chartGrid.RowDefinitions.Add(this._row);

        }

        internal void CreateColumn(Grid chartGrid, double width, GridUnitType gridUnitType)
        {
            this._column = new ColumnDefinition() { Width = new GridLength(width, gridUnitType) };
            chartGrid.ColumnDefinitions.Add(this._column);
        }


        internal void CreateLegendRowColumn(Grid chartGrid, IChartLegend legend)
        {
            double legendSize = legend.Size;
            switch (legend.DockOrientation)
            {
                case ChartDockOrientation.Left:
                case ChartDockOrientation.Right:
                    this._column = new ColumnDefinition() { Width = new GridLength(legend.Size, GridUnitType.Pixel) };
                    if (legend.DockOrientation == ChartDockOrientation.Left)
                    {
                        chartGrid.ColumnDefinitions.Insert(0, this._column);
                        this.ColumnIndex = 0;
                    }
                    else
                    {
                        chartGrid.ColumnDefinitions.Add(this._column);
                        this.ColumnIndex = chartGrid.ColumnDefinitions.Count - 1;
                    }
                    this.RowIndex = 0;
                    this.RowSpan = chartGrid.RowDefinitions.Count;
                    break;
                case ChartDockOrientation.Top:
                case ChartDockOrientation.Bottom:
                    this._row = new RowDefinition() { Height = new GridLength(legend.Size, GridUnitType.Pixel) };
                    if (legend.DockOrientation == ChartDockOrientation.Top)
                    {
                        chartGrid.RowDefinitions.Insert(0, this._row);
                        this.RowIndex = 0;
                    }
                    else
                    {
                        chartGrid.RowDefinitions.Add(this._row);
                        this.RowIndex = chartGrid.RowDefinitions.Count - 1;
                    }

                    this.ColumnIndex = 0;
                    this.ColumnSpan = chartGrid.ColumnDefinitions.Count;
                    break;
                default:
                    throw new NotImplementedException(legend.DockOrientation.ToString());
            }
        }


        internal void MergeRowColumn(Grid chartGrid, RowColumnDefinitionItem chartRowColumnDefinitionItem)
        {
            if (this._column == null)
            {
                this._column = chartRowColumnDefinitionItem._column;
            }

            if (this._row == null)
            {
                this._row = chartRowColumnDefinitionItem._row;
            }

            if (this._row != null)
            {
                this.RowIndex = chartGrid.RowDefinitions.IndexOf(this._row);
            }

            if (this._column != null)
            {
                this.ColumnIndex = chartGrid.ColumnDefinitions.IndexOf(this._column);
            }
        }
    }
}
