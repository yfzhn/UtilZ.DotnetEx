using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 条形图控件
    /// </summary>
    public class BarChartControl : PaintBaseControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BarChartControl()
            : base()
        {

        }

        /// <summary>
        /// 重写
        /// </summary>
        protected override void CustomerPaint()
        {
            try
            {
                if (this._grafx == null)
                {
                    return;
                }

                Graphics graphics = this._grafx.Graphics;
                //重置平移
                graphics.ResetTransform();

                //重置平移
                graphics.ResetTransform();

                //清空所有已绘制的图形
                graphics.Clear(this.BackColor);

                if (graphics.ClipBounds.Width <= 0)
                {
                    return;
                }

                //var linePen = this._gridLinePen;
                //if (linePen.Width - _GRID_SIZE > _PRE)
                //{
                //    return;
                //}

                float width = this.Width, height = this.Height;
                //float minValue, maxValue;
                //this.GetBestValue(out minValue, out maxValue);

                ////绘制表格
                //if (this._drawDirection)
                //{
                //    this.LeftToRightDraw(graphics, width, height, linePen);
                //}
                //else
                //{
                //    this.RightToLeftDraw(graphics, width, height, linePen);
                //}

                this.Refresh();
                this.Update();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "绘图异常");
            }
        }

    }
}
