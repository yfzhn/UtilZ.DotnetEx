using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V5
{
    /// <summary>
    /// 异步等等遮罩层控件
    /// </summary>
    [Serializable]
    public class ShadeControl5 : System.Windows.Forms.Control, IPartAsynWaitWinform
    {
        #region 透明
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
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
        #endregion

        #region IPartAsynWait接口
        /// <summary>
        /// 是否已经取消
        /// </summary>
        private bool _isCanceled = false;

        /// <summary>
        /// 获取是否已经取消
        /// </summary>
        [Browsable(false)]
        public bool Canceled
        {
            get { return this._isCanceled; }
        }

        /// <summary>
        /// 获取或设置提示标题
        /// </summary>
        [Category("异步等待")]
        [DisplayName("提示标题")]
        [Description("获取或设置提示标题")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Title
        {
            get
            {
                return _caption;
            }
            set
            {
                if (object.Equals(_caption, value))
                {
                    return;
                }

                _caption = value;
                this.Invalidate(this._captionReg);
            }
        }

        /// <summary>
        /// 获取或设置提示内容
        /// </summary>
        [Category("异步等待")]
        [DisplayName("提示内容")]
        [Description("获取或设置提示内容")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Message
        {
            get
            {
                return _hint;
            }
            set
            {
                if (object.Equals(_hint, value))
                {
                    return;
                }

                _hint = value;
                this.Invalidate(this._hintReg);
            }
        }

        /// <summary>
        /// 获取或设置是否显示取消按钮
        /// </summary>
        [Category("异步等待")]
        [DisplayName("是否显示取消按钮")]
        [Description("获取或设置是否显示取消按钮")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool ShowCancel
        {
            get { return _isShowCancel; }
            set
            {
                if (_isShowCancel == value)
                {
                    return;
                }

                _isShowCancel = value;
            }
        }

        /// <summary>
        /// 获取或设置动画背景色
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("获取或设置动画背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object ShadeBackground
        {
            get { return _asynWaitBackground; }
            set
            {
                var color = (Color)value;
                if (_asynWaitBackground == color)
                {
                    return;
                }

                _asynWaitBackground = color;
                this._asynWaitBackgroundBrush = new SolidBrush(color);
                this.Invalidate(this._asynReg);
            }
        }

        /// <summary>
        /// 取消通知事件
        /// </summary>
        public event EventHandler CanceledNotify;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// 取消操作
        /// </summary>
        public void Cancel()
        {
            if (this._isCanceled)
            {
                return;
            }

            lock (this._monitor)
            {
                if (this._isCanceled)
                {
                    return;
                }

                this._isCanceled = true;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ButtonCancel));
            }
            else
            {
                ButtonCancel();
            }

            var handler = this.CanceledNotify;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void ButtonCancel()
        {
            _cancellText = CancellingText;
            this.Invalidate(this._cancelButtonReg);
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void StartAnimation()
        {

        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void StopAnimation()
        {

        }

        /// <summary>
        /// 设置信息(保留接口)
        /// </summary>
        /// <param name="para">参数</param>
        public void SetInfo(object para)
        {

        }

        /// <summary>
        /// 重置异步等待框
        /// </summary>
        public void Reset()
        {
            this._isCanceled = false;
            _cancellText = CancellingText;
        }

        /// <summary>
        /// 获取UI是否处于设计器模式
        /// </summary>
        public bool UIDesignMode
        {
            get { return this.DesignMode; }
        }

        /// <summary>
        /// 控件加载完成事件
        /// </summary>
        public event EventHandler Load;
        #endregion

        /// <summary>
        /// 重写前景色属性
        /// </summary>
        public override System.Drawing.Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                if (base.ForeColor == value)
                {
                    return;
                }

                base.ForeColor = value;
                this._fontBrush = new SolidBrush(value);
                this.Invalidate(this._asynReg);
            }
        }

        /// <summary>
        /// 当前局部刷新区域集合
        /// </summary>
        private readonly List<Rectangle> _partRefreshAreas = new List<Rectangle>();

        private readonly int _asynWidth = 300;
        private readonly int _asynHeight = 150;
        private readonly int _captionHeight = 30;
        private readonly int _hintHeight = 30;
        private readonly int _animalHeight = 60;
        private readonly int _cancelHeight = 30;

        private readonly int _cancelButtonWidth = 80;
        private readonly int _cancelButtonHeight = 25;

        private Rectangle _asynReg;
        private Rectangle _captionReg;
        private Rectangle _hintReg;
        private Rectangle _animalReg;
        private Rectangle _cancelReg;
        private Rectangle _cancelButtonReg;

        private string _caption = string.Empty;
        private string _hint = string.Empty;
        private bool _isShowCancel = true;
        private Color _asynWaitBackground;
        private Brush _asynWaitBackgroundBrush;
        private readonly Font _captionFont = new System.Drawing.Font("微软雅黑", 14F);
        private readonly Font _hintFont = new System.Drawing.Font("微软雅黑", 11F);
        private readonly Font _btnFont = new System.Drawing.Font("微软雅黑", 12F);
        private Brush _fontBrush;

        private const string CancellText = "取消";
        private const string CancellingText = "正在取消";

        private string _cancellText;
        private Brush _btnBrush;
        private Brush _btnMouseOverBrush;
        private bool _btnCancelIsMouseOver = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShadeControl5()
            : base()
        {
            this.Size = new System.Drawing.Size(600, 300);
            this.SetStyle(ControlStyles.Opaque, true);
            this.UpdateStyles();
            this.RealColor = Color.FromArgb(this._opacity, this.BackColor);
            _asynWaitBackground = Color.White;
            this._asynWaitBackgroundBrush = new SolidBrush(_asynWaitBackground);
            this._fontBrush = new SolidBrush(this.ForeColor);

            _cancellText = CancellText;
            this.UpdateArea();

            _btnBrush = new SolidBrush(Color.FromArgb(181, 181, 181));
            _btnMouseOverBrush = new SolidBrush(Color.FromArgb(192, 231, 253));
        }

        private void AddRefreshArea(Rectangle reg)
        {
            if (this._partRefreshAreas.Contains(reg))
            {
                return;
            }

            this._partRefreshAreas.Add(reg);
        }

        private void UpdateArea()
        {
            int width = this.Size.Width;
            int height = this.Size.Height;
            int x = (width - _asynWidth) / 2;
            int y = (height - _asynHeight) / 2;

            this._asynReg = new Rectangle(x, y, _asynWidth, _asynHeight);

            this._captionReg = new Rectangle(x, y, _asynWidth, _captionHeight);
            y += _captionHeight;

            this._hintReg = new Rectangle(x, y, _asynWidth, _hintHeight);
            y += _hintHeight;

            this._animalReg = new Rectangle(x, y, _asynWidth, _animalHeight);
            y += _animalHeight;

            this._cancelReg = new Rectangle(x, y, _asynWidth, _cancelHeight);

            this._cancelButtonReg = new Rectangle(x + (_asynWidth - _cancelButtonWidth) / 2, y + (_cancelHeight - _cancelButtonHeight) / 2, _cancelButtonWidth, _cancelButtonHeight);
        }

        /// <summary>
        /// 重写OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool btnCancelIsMouseOver;
            if (e.X >= _cancelButtonReg.X && e.X <= _cancelButtonReg.X + _cancelButtonReg.Width &&
                e.Y >= _cancelButtonReg.Y && e.Y <= _cancelButtonReg.Y + _cancelButtonReg.Height)
            {
                btnCancelIsMouseOver = true;
            }
            else
            {
                btnCancelIsMouseOver = false;
            }

            if (this._btnCancelIsMouseOver != btnCancelIsMouseOver)
            {
                this._btnCancelIsMouseOver = btnCancelIsMouseOver;
                this.Invalidate(_cancelButtonReg);
            }
        }

        /// <summary>
        /// 重写OnMouseClick
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.X >= _cancelButtonReg.X && e.X <= _cancelButtonReg.X + _cancelButtonReg.Width &&
                e.Y >= _cancelButtonReg.Y && e.Y <= _cancelButtonReg.Y + _cancelButtonReg.Height)
            {
                this.Cancel();
            }
        }

        /// <summary>
        /// 重写OnCreateControl
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            var handler = this.Load;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// 重写OnResize
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.UpdateArea();
        }

        /// <summary>
        /// 重绘控件
        /// </summary>
        /// <param name="e">e</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            var width = this.Size.Width;
            var height = this.Size.Height;
            if (width == 0 || height == 0)
            {
                return;
            }

            var graphics = e.Graphics;
            graphics.FillRectangle(this._backgroundBrush, 0, 0, width, height);

            //绘制异步等等区域
            graphics.FillRectangle(this._asynWaitBackgroundBrush, this._asynReg);

            //截取文本
            SizeF captionTextSize, hintTextSize, cancelTextSize;
            string caption = this.ClipText(graphics, this._caption, this._captionFont, _asynWidth, out captionTextSize);
            string hint = this.ClipText(graphics, this._hint, this._hintFont, _asynWidth, out hintTextSize);
            string cancelText = this.ClipText(graphics, this._cancellText, this._btnFont, _cancelButtonReg.Width, out cancelTextSize);

            graphics.DrawString(caption, this._captionFont, this._fontBrush, this.Calculatelocation(captionTextSize, this._captionReg));
            graphics.DrawString(hint, this._hintFont, this._fontBrush, this.Calculatelocation(hintTextSize, this._hintReg));

            Brush btnBrush;
            if (this._btnCancelIsMouseOver)
            {
                btnBrush = _btnMouseOverBrush;
            }
            else
            {
                btnBrush = _btnBrush;
            }

            graphics.FillRectangle(btnBrush, _cancelButtonReg);
            graphics.DrawString(cancelText, this._btnFont, this._fontBrush, this.Calculatelocation(cancelTextSize, this._cancelButtonReg));
        }

        private PointF Calculatelocation(SizeF realSize, Rectangle reg)
        {
            return new PointF(reg.X + (reg.Width - realSize.Width) / 2, reg.Y + (reg.Height - realSize.Height) / 2);
        }

        private string ClipText(Graphics graphics, string text, Font font, int width, out SizeF captionTextSize)
        {
            captionTextSize = graphics.MeasureString(this._caption, this._captionFont);
            if (captionTextSize.Width <= width)
            {
                return text;
            }

            SizeF lastTmpSize;
            string lastTmpStr = string.Empty;
            string tmpStr;
            string ppp = "...";
            for (int i = 0; i < text.Length; i++)
            {
                tmpStr = text.Substring(0, i + 1);
                tmpStr += ppp;
                lastTmpSize = graphics.MeasureString(tmpStr, font);
                if (lastTmpSize.Width == width)
                {
                    lastTmpStr = tmpStr;
                    captionTextSize = lastTmpSize;
                    break;
                }
                else if (captionTextSize.Width > width)
                {
                    break;
                }
                else
                {
                    lastTmpStr = tmpStr;
                    captionTextSize = lastTmpSize;
                }
            }

            return lastTmpStr;
        }
    }
}
