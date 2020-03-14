using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.NativeMethod;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 无边框窗体
    /// </summary>
    public class NoneBorderForm : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NoneBorderForm() : base()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        /// <summary>
        /// 空
        /// </summary>
        protected const int NONE = 0;

        /// <summary>
        /// 左边缘
        /// </summary>
        protected const int TLEFT = 10;

        /// <summary>
        /// 右边缘
        /// </summary>
        protected const int RIGHT = 11;

        /// <summary>
        /// 上边缘
        /// </summary>
        protected const int TOP = 12;

        /// <summary>
        /// 下边缘
        /// </summary>
        protected const int BOTTOM = 15;

        /// <summary>
        /// 左上边缘
        /// </summary>
        protected const int LEFTTOP = 13;

        /// <summary>
        /// 右上边缘
        /// </summary>
        protected const int RIGHTTOP = 14;

        /// <summary>
        /// 左下边缘
        /// </summary>
        protected const int LEFTBOTTOM = 0x10;

        /// <summary>
        /// 右下边缘
        /// </summary>
        protected const int RIGHTBOTTOM = 17;

        private bool _isDisableDragMoveForm = false;

        /// <summary>
        /// 是否禁用拖动移动窗口[true:禁用]
        /// </summary>
        public bool IsDisableDragMoveForm
        {
            get { return _isDisableDragMoveForm; }
            set { _isDisableDragMoveForm = value; }
        }

        private bool _isAllowMinimize = true;

        /// <summary>
        /// 是否允许最小化[true:允许]
        /// </summary>
        public bool IsAllowMinimize
        {
            get { return _isAllowMinimize; }
            set { _isAllowMinimize = value; }
        }

        /// <summary>
        /// 窗口修改大小样式
        /// </summary>
        private ResizeStyle _formResizeStyle = ResizeStyle.All;

        /// <summary>
        /// 窗口修改大小样式
        /// </summary>
        public ResizeStyle FormResizeStyle
        {
            get { return _formResizeStyle; }
            set { _formResizeStyle = value; }
        }

        /// <summary>
        /// 重写WndProc函数实现可调整大小
        /// </summary>
        /// <param name="m">消息对象</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0084:
                    //当窗口处于最大化时,屏蔽拖动窗口消息
                    if (this._isDisableDragMoveForm || this.WindowState == FormWindowState.Maximized)
                    {
                        return;
                    }

                    base.WndProc(ref m);

                    ResizeStyle tmpStyle;
                    int result;
                    var vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                    {
                        if (vPoint.Y <= 5)
                        {
                            result = LEFTTOP;
                            tmpStyle = ResizeStyle.LeftTop;
                        }
                        else if (vPoint.Y >= ClientSize.Height - 5)
                        {
                            result = LEFTBOTTOM;
                            tmpStyle = ResizeStyle.LeftBottom;
                        }
                        else
                        {
                            result = TLEFT;
                            tmpStyle = ResizeStyle.Left;
                        }
                    }
                    else if (vPoint.X >= ClientSize.Width - 5)
                    {
                        if (vPoint.Y <= 5)
                        {
                            result = RIGHTTOP;
                            tmpStyle = ResizeStyle.RightTop;
                        }
                        else if (vPoint.Y >= ClientSize.Height - 5)
                        {
                            result = RIGHTBOTTOM;
                            tmpStyle = ResizeStyle.RightBottom;
                        }
                        else
                        {
                            result = RIGHT;
                            tmpStyle = ResizeStyle.Right;
                        }
                    }
                    else if (vPoint.Y <= 5)
                    {
                        result = TOP;
                        tmpStyle = ResizeStyle.Top;
                    }
                    else if (vPoint.Y >= ClientSize.Height - 5)
                    {
                        result = BOTTOM;
                        tmpStyle = ResizeStyle.Bottom;
                    }
                    else
                    {
                        result = NONE;
                        tmpStyle = ResizeStyle.None;
                    }

                    if (result != NONE && tmpStyle == (tmpStyle & this._formResizeStyle))
                    {
                        m.Result = (IntPtr)result;
                    }
                    break;
                case 0x0201:                //鼠标左键按下的消息 
                    m.Msg = 0x00A1;         //更改消息为非客户区按下鼠标 
                    m.LParam = IntPtr.Zero; //默认值 
                    m.WParam = new IntPtr(2);//鼠标放在标题栏内 
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  重写参数,否则点击任务栏图片时不会最小化的
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (this._isAllowMinimize)
                {
                    cp.Style = cp.Style | 0x00020000; // 允许最小化操作
                }

                return cp;
            }
        }

        #region 出入场特效
        /// <summary>
        /// 是否启用窗口加载退出特效[true:启用;false:禁用]
        /// </summary>
        private bool _isUseInOutEffect = true;

        /// <summary>
        /// 获取或设置是否启用窗口加载退出特效[true:启用;false:禁用]
        /// </summary>
        [Description("获取或设置是否启用窗口加载退出特效[true:启用;false:禁用]")]
        public bool IsUseInOutEffect
        {
            get { return _isUseInOutEffect; }
            set { _isUseInOutEffect = value; }
        }

        /// <summary>
        /// 入场特效持续时长,单位/毫秒
        /// </summary>
        private int _inDurationTime = 300;

        /// <summary>
        /// 获取或设置入场特效持续时长,单位/毫秒
        /// </summary>
        [Description("获取或设置入场特效持续时长,单位/毫秒,默认300")]
        public int InDurationTime
        {
            get { return _inDurationTime; }
            set { _inDurationTime = value; }
        }

        /// <summary>
        /// 入场动画类型(WindowAnimateType中的值按位枚举运算
        /// </summary>
        private int _inAnimateType = WindowAnimateType.AW_BLEND;

        /// <summary>
        /// 获取或设置入场动画类型(WindowAnimateType中的值按位枚举运算
        /// </summary>
        [Description("获取或设置入场动画类型(WindowAnimateType中的值按位枚举运算,默认AW_BLEND")]
        public int InAnimateType
        {
            get { return _inAnimateType; }
            set { _inAnimateType = value; }
        }

        /// <summary>
        /// 出场特效持续时长,单位/毫秒
        /// </summary>
        private int _outDurationTime = 300;

        /// <summary>
        /// 获取或设置出场特效持续时长,单位/毫秒
        /// </summary>
        [Description("获取或设置出场特效持续时长,单位/毫秒,默认300")]
        public int OutDurationTime
        {
            get { return _outDurationTime; }
            set { _outDurationTime = value; }
        }

        /// <summary>
        /// 出场动画类型(WindowAnimateType中的值按位枚举运算
        /// </summary>
        private int _outAnimateType = WindowAnimateType.AW_SLIDE | WindowAnimateType.AW_HIDE | WindowAnimateType.AW_BLEND;

        /// <summary>
        /// 获取或设置出场动画类型(WindowAnimateType中的值按位枚举运算
        /// </summary>
        [Description("获取或设置出场动画类型(WindowAnimateType中的值按位枚举运算,默认AW_BLEND")]
        public int OutAnimateType
        {
            get { return _outAnimateType; }
            set { _outAnimateType = value; }
        }

        /// <summary>
        /// 重写OnLoad方法实现窗口淡入
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this._isUseInOutEffect)
            {
                //窗口淡入
                NativeMethods.WindowFadeIn(this.Handle, this._inDurationTime, this._inAnimateType);
            }
        }

        /// <summary>
        /// 重写OnFormClosed方法实现窗口淡出
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (this._isUseInOutEffect)
            {
                //窗口淡出
                NativeMethods.WindowFadeOut(this.Handle, this._outDurationTime, this._outAnimateType);
            }

            base.OnFormClosed(e);
        }
        #endregion

        /// <summary>
        /// 设置窗口状态
        /// </summary>
        /// <param name="windowState">窗体状态</param>
        /// <param name="maximizedFullScreen">最大化是否全屏[true:全屏;false:正常窗口的最大化,任务栏可见时显示任务栏,否则全屏;默认为false]</param>
        public void SetWindowState(FormWindowState windowState, bool maximizedFullScreen = false)
        {
            if (windowState == FormWindowState.Maximized && !maximizedFullScreen)
            {
                this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            }

            this.WindowState = windowState;
        }

        /// <summary>
        /// 切换窗口状态
        /// </summary>
        /// <param name="maximizedFullScreen">最大化是否全屏[true:全屏;false:正常窗口的最大化,任务栏可见时显示任务栏,否则全屏;默认为false]</param>
        public void SwitchWindowState(bool maximizedFullScreen = false)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                if (this.WindowState != FormWindowState.Maximized)
                {
                    this.SetWindowState(FormWindowState.Maximized, maximizedFullScreen);
                }
                else
                {
                    this.SetWindowState(FormWindowState.Normal);
                }
            }
        }
    }

    /// <summary>
    /// 无边框窗口大小重设样式
    /// </summary>
    //[Editor("System.Windows.Forms.Design.AnchorEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ResizeStyle
    {
        /// <summary>
        /// 不允许调整
        /// </summary>
        None = 1,

        /// <summary>
        /// 上边缘
        /// </summary>
        Top = 2,

        /// <summary>
        /// 下边缘
        /// </summary>
        Bottom = 4,

        /// <summary>
        /// 左边缘
        /// </summary>
        Left = 8,

        /// <summary>
        /// 右边缘
        /// </summary>
        Right = 16,

        /// <summary>
        /// 左上边缘
        /// </summary>
        LeftTop = 32,

        /// <summary>
        /// 左下边缘
        /// </summary>
        LeftBottom = 64,

        /// <summary>
        /// 右上边缘
        /// </summary>
        RightTop = 128,

        /// <summary>
        /// 右下边缘
        /// </summary>
        RightBottom = 256,

        /// <summary>
        /// 全部八个方向
        /// </summary>
        All = Left | Top | Right | Bottom | LeftTop | LeftBottom | RightTop | RightBottom
    }
}
