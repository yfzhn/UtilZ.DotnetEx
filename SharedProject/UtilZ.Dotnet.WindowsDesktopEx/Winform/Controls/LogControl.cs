using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// Winform日志控件,对WPF版本进行封装得到
    /// </summary>
    public partial class LogControl : UserControl, ILogControl
    {
        /// <summary>
        /// 获取或设置最多显示项数
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("获取或设置最多显示项数,默认为100项,超过之后则会将旧的项移除")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxItemCount
        {
            get { return logControl.MaxItemCount; }
            set { logControl.MaxItemCount = value; }
        }

        /// <summary>
        /// 是否锁定滚动
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("是否锁定滚动[true:锁定;false:不锁定;默认为false]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLock
        {
            get { return logControl.IsLock; }
            set { logControl.IsLock = value; }
        }

        /// <summary>
        /// 设置日志刷新信息
        /// </summary>
        /// <param name="refreshCount">单次最大刷新日志条数</param>
        /// <param name="cacheCapcity">日志缓存容量,建议等于日志最大项数</param>
        public void SetLogRefreshInfo(int refreshCount, int cacheCapcity)
        {
            logControl.SetLogRefreshInfo(refreshCount, cacheCapcity);
        }

        /// <summary>
        /// 设置样式,不存在添加,存在则用新样式替换旧样式
        /// </summary>
        /// <param name="style">样式</param>
        public void SetStyle(LogShowStyle style)
        {
            logControl.SetStyle(style);
        }

        /// <summary>
        /// 移除样式
        /// </summary>
        /// <param name="style">样式标识</param>
        public void RemoveStyle(LogShowStyle style)
        {
            logControl.RemoveStyle(style);
        }

        /// <summary>
        /// 清空样式
        /// </summary>
        public void ClearStyle()
        {
            logControl.ClearStyle();
        }

        /// <summary>
        /// 获取当前所有样式数组
        /// </summary>
        /// <returns>当前所有样式数组</returns>
        public LogShowStyle[] GetStyles()
        {
            return this.logControl.GetStyles();
        }

        /// <summary>
        /// 根据样式标识ID获取样式
        /// </summary>
        /// <param name="id">样式标识ID</param>
        /// <returns>获取样式</returns>
        public LogShowStyle GetStyleById(int id)
        {
            return logControl.GetStyleById(id);
        }

        /// <summary>
        /// 添加显示日志
        /// </summary>
        /// <param name="logText">显示内容</param>
        /// <param name="level">日志级别</param>
        public void AddLog(string logText, LogLevel level)
        {
            logControl.AddLog(logText, level);
        }

        /// <summary>
        /// 添加显示日志
        /// </summary>
        /// <param name="logText">显示内容</param>
        /// <param name="styleId">样式标识ID</param>
        public void AddLog(string logText, int styleId)
        {
            logControl.AddLog(logText, styleId);
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        public void Clear()
        {
            logControl.Clear();
        }

        /// <summary>
        /// 获取或设置日志控件背景色
        /// </summary>
        public override Color BackColor
        {
            get
            {
                //var brush = (System.Windows.Media.SolidColorBrush)logControl.Background;
                //return Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                var color = System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
                logControl.Background = new System.Windows.Media.SolidColorBrush(color);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogControl()
        {
            InitializeComponent();
        }
    }
}
