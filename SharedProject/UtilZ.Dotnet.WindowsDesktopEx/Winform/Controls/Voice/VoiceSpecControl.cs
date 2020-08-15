using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 语谱图
    /// </summary>
    public class VoiceSpecControl : Control
    {
        /// <summary>
        /// 图形双缓冲缓冲区对象
        /// </summary>
        private BufferedGraphics _grafx = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        public VoiceSpecControl()
        {
            //设置绘制样式
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            this.Init();

            //大小
            this.Size = new System.Drawing.Size(200, 130);
            //最小大小
            this.MinimumSize = this.Size;
        }

        private void Init()
        {

        }






        /// <summary>
        /// 重写OnSizeChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            try
            {
                //重新创建双缓冲
                this.CreateBufferedGraphics();
                //this.UpdateDrawAreaActualSize();
                //this.SizeChangedResample();
                //this.AllDraw();
                base.OnSizeChanged(e);
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
    }
}
