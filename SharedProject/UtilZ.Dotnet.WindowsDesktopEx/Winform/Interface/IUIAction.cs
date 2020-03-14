using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Interface
{
    /// <summary>
    /// UI动作行为接口
    /// </summary>
    public interface IUIAction
    {
        /// <summary>
        /// 鼠标行为动作事件
        /// </summary>
        event EventHandler<MouseActionArgs> MouseAction;
    }

    /// <summary>
    /// 鼠标操作命令事参数
    /// </summary>
    public class MouseActionArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">获取鼠标行为类型</param>
        /// <param name="mouseArgs">鼠标操作参数</param>
        public MouseActionArgs(MouseActionType type, System.Windows.Forms.MouseEventArgs mouseArgs)
        {
            this.Type = type;
            this.Args = null;
            this.MouseArgs = mouseArgs;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">获取鼠标行为类型</param>
        /// <param name="args">事件参数</param>
        public MouseActionArgs(MouseActionType type, EventArgs args)
        {
            this.Type = type;
            this.Args = args;
            this.MouseArgs = null;
        }

        /// <summary>
        /// 获取鼠标行为类型
        /// </summary>
        public MouseActionType Type { get; private set; }

        /// <summary>
        /// 事件参数
        /// </summary>
        public EventArgs Args { get; private set; }

        /// <summary>
        /// 鼠标事件参数
        /// </summary>
        public System.Windows.Forms.MouseEventArgs MouseArgs { get; private set; }
    }

    /// <summary>
    /// 鼠标行为类形
    /// </summary>
    public enum MouseActionType
    {
        /// <summary>
        /// 鼠标按下
        /// </summary>
        MouseDown,

        /// <summary>
        /// 鼠标移动
        /// </summary>
        MouseMove,

        /// <summary>
        /// 鼠标弹起
        /// </summary>
        MouseUp,

        /// <summary>
        /// 移动鼠标滚轮
        /// </summary>
        MouseWheel,

        /// <summary>
        /// 鼠标指针进入控件
        /// </summary>
        MouseEnter,

        /// <summary>
        /// 鼠标指针离开控件
        /// </summary>
        MouseLeave,

        /// <summary>
        /// 鼠标指针停放在控件上
        /// </summary>
        MouseHover
    }
}
