using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 纯绘制控件基类
    /// </summary>
    public abstract class PaintBaseControl : Control
    {
        /// <summary>
        /// 图形双缓冲缓冲区对象
        /// </summary>
        protected BufferedGraphics _grafx = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PaintBaseControl()
        {
            //大小
            this.Size = new System.Drawing.Size(100, 30);

            //最小大小
            this.MinimumSize = this.Size;
            this.BackColor = Color.Black;
            //设置绘制样式
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// 重写OnCreateControl
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            try
            {
                //创建默认双缓冲
                this.CreateBufferedGraphics();

                //初始化
                if (this.DesignMode)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 重写OnSizeChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            try
            {
                base.OnSizeChanged(e);

                //重新创建双缓冲
                this.CreateBufferedGraphics();

                //刷新波形图
                this.CustomerPaint();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 创建双缓冲绘图
        /// </summary>
        private void CreateBufferedGraphics()
        {
            // Sets the maximum size for the primary graphics buffer
            // of the buffered graphics context for the application
            // domain.  Any allocation requests for a buffer larger 
            // than this will create a temporary buffered graphics 
            // context to host the graphics buffer.
            BufferedGraphicsManager.Current.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            BufferedGraphics grafx = null;
            //当宽度或高度为0时,缓冲区创建会报系统异常
            if (this.Width != 0 && this.Height != 0)
            {
                // Allocates a graphics buffer the size of this form
                // using the pixel format of the Graphics created by 
                // the Form.CreateGraphics() method, which returns a 
                // Graphics object that matches the pixel format of the form.
                //BufferedGraphics grafx = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
                grafx = BufferedGraphicsManager.Current.Allocate(Graphics.FromHwnd(this.Handle), new Rectangle(0, 0, this.Width, this.Height));
                grafx.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                grafx.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            }

            if (this._grafx != null)
            {
                this._grafx.Graphics.Dispose();
                this._grafx.Dispose();
            }

            this._grafx = grafx;
        }

        /// <summary>
        /// 重写OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                this._grafx.Render(e.Graphics);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 检查当前是否在主线程
        /// </summary>
        protected void CheckInvokeRequired()
        {
            if (this.InvokeRequired)
            {
                throw new InvalidOperationException("在该控件上执行的操作正从错误的线程调用。使用 Control.Invoke 或 Control.BeginInvoke 封送到正确的线程才能执行此操作。");
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        protected abstract void CustomerPaint();

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this._grafx != null)
            {
                if (this._grafx.Graphics != null)
                {
                    this._grafx.Graphics.Dispose();
                }

                this._grafx.Dispose();
                this._grafx = null;
            }
        }
    }
}
