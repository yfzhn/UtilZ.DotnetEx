using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 半透明用户控件基类
    /// </summary>
    public partial class UCOpacityControlBase : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UCOpacityControlBase()
        {
            this.RealColor = Color.FromArgb(this._opacity, this.BackColor);
            this.Size = new System.Drawing.Size(100, 80);
            this.SetStyle(ControlStyles.Opaque, true);
            this.UpdateStyles();
        }

        /// <summary>
        /// 透明度
        /// </summary>
        private byte _opacity = 125;

        /// <summary>
        /// 获取或设置控件透明度
        /// </summary>
        [Category("透明控件"), Description("透明度,0-255,值越大，透明度越低,默认半透明")]
        public byte Opacity
        {
            get { return this._opacity; }
            set
            {
                if (this._opacity == value)
                {
                    return;
                }

                this._opacity = value;
                this.RealColor = Color.FromArgb(this._opacity, this.BackColor);
                this.Invalidate();
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.RealColor = Color.FromArgb(this._opacity, this.BackColor);
            }
        }

        /// <summary>
        /// 背景画刷
        /// </summary>
        private Brush _backgroundBrush;

        /// <summary>
        /// 真实背景色
        /// </summary>
        private Color _realColor;

        /// <summary>
        /// 获取或设置真实背景色
        /// </summary>
        protected Color RealColor
        {
            get { return this._realColor; }
            set
            {
                this._realColor = value;
                this._backgroundBrush = new SolidBrush(Color.FromArgb(this._opacity, this.BackColor));
                this.Invalidate();
            }
        }

        /// <summary>
        /// 重写控件生成参数使控件支持透明
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ExStyle |= 0x00000020;
                //parms.Style &= ~0x02000000;
                return parms;
            }
        }

        /// <summary>
        /// 重绘控件
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //e.Graphics.Clear(Color.Transparent);
            var width = this.Size.Width;
            var height = this.Size.Height;
            if (width == 0 || height == 0)
            {
                return;
            }

            e.Graphics.FillRectangle(this._backgroundBrush, 0, 0, width, height);
        }
    }
}
