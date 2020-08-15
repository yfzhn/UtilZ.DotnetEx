using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 图元信息
    /// </summary>
    public abstract class PlotElementInfoAbs : IDisposable
    {
        /// <summary>
        /// 背景色
        /// </summary>
        public Brush BackgroudColor { get; private set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public Brush ForeColor { get; private set; }

        /// <summary>
        /// 内容画笔
        /// </summary>
        internal Pen Pen { get; private set; }


        /// <summary>
        /// 绘制顺序,从上往下,依次递增
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// 元素块区域
        /// </summary>
        internal RectangleF Area { get; set; }

        internal AreaType AreaType { get; set; }


        /// <summary>
        /// 构造函数-绘制内容
        /// </summary>
        /// <param name="backgroudColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="lineWidth">绘制线条宽度</param>
        /// <param name="order">绘制顺序,从上往下,依次递增</param>
        public PlotElementInfoAbs(Brush backgroudColor, SolidBrush foreColor, float lineWidth, int order)
        {
            this.BackgroudColor = backgroudColor;
            this.ForeColor = foreColor;
            Color penColor = Color.FromArgb(foreColor.Color.A, foreColor.Color.R, foreColor.Color.G, foreColor.Color.B);
            this.Pen = new Pen(penColor, lineWidth);
            this.Order = order;
        }




        private bool _disposed = false;
        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;

            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.PrimitiveDisposable(this.BackgroudColor);
            this.PrimitiveDisposable(this.ForeColor);
            this.PrimitiveDisposable(this.Pen);
        }



        protected void PrimitiveDisposable(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
