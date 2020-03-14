using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 透明Panel
    /// </summary>
    //[ToolboxBitmap(typeof(OpacityPanel))]
    [Serializable]
    public class OpacityPanel : System.Windows.Forms.Panel
    {
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
            get { return _opacity; }
            set
            {
                if (_opacity == value)
                {
                    return;
                }

                _opacity = value;
                this.Invalidate();
            }
        }

        private Color _realBackColor;

        /// <summary>
        /// 真实背景色
        /// </summary>
        public Color RealBackColor
        {
            get { return _realBackColor; }
            private set { _realBackColor = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public OpacityPanel()
            : base()
        {
            this.Size = new Size(200, 100);
            this.SetStyle(System.Windows.Forms.ControlStyles.Opaque, true);
        }

        /// <summary>
        /// 重写控件生成参数使控件支持透明
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        /// <summary>
        /// 重绘控件
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            var width = this.Size.Width;
            var height = this.Size.Height;
            if (width == 0 || height == 0)
            {
                return;
            }

            this._realBackColor = Color.FromArgb(this._opacity, this.BackColor);
            e.Graphics.FillRectangle(new SolidBrush(this._realBackColor), 0, 0, width, height);
        }
    }
}
