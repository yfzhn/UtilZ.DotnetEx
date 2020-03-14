using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 日志控件接口
    /// </summary>
    public interface ILogControl
    {
        /// <summary>
        /// 获取或设置最多显示项数
        /// </summary>
        int MaxItemCount { get; set; }

        /// <summary>
        /// 是否锁定滚动
        /// </summary>
        bool IsLock { get; set; }

        /// <summary>
        /// 设置日志刷新信息
        /// </summary>
        /// <param name="refreshCount">单次最大刷新日志条数</param>
        /// <param name="cacheCapcity">日志缓存容量,建议等于日志最大项数</param>
        void SetLogRefreshInfo(int refreshCount, int cacheCapcity);

        /// <summary>
        /// 设置样式,不存在添加,存在则用新样式替换旧样式
        /// </summary>
        /// <param name="style">样式</param>
        void SetStyle(LogShowStyle style);

        /// <summary>
        /// 移除样式
        /// </summary>
        /// <param name="style">样式标识</param>
        void RemoveStyle(LogShowStyle style);

        /// <summary>
        /// 清空样式
        /// </summary>
        void ClearStyle();

        /// <summary>
        /// 获取当前所有样式数组
        /// </summary>
        /// <returns>当前所有样式数组</returns>
        LogShowStyle[] GetStyles();

        /// <summary>
        /// 根据样式标识ID获取样式
        /// </summary>
        /// <param name="id">样式标识ID</param>
        /// <returns>获取样式</returns>
        LogShowStyle GetStyleById(int id);

        /// <summary>
        /// 添加显示日志
        /// </summary>
        /// <param name="logText">显示内容</param>
        /// <param name="level">日志级别</param>
        void AddLog(string logText, LogLevel level);

        /// <summary>
        /// 添加显示日志
        /// </summary>
        /// <param name="logText">显示内容</param>
        /// <param name="styleId">样式标识ID</param>
        void AddLog(string logText, int styleId);

        /// <summary>
        /// 清空日志
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 日志控件显示样式类
    /// </summary>
    public class LogShowStyle
    {
        /// <summary>
        /// 样式标识ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 文本前景色Brush
        /// </summary>
        public System.Windows.Media.Brush ForegroundBrush { get; private set; }

        /// <summary>
        /// 文本大小
        /// </summary>
        public double FontSize { get; private set; }

        /// <summary>
        /// 字体
        /// </summary>
        public System.Windows.Media.FontFamily FontFamily { get; private set; }

        /// <summary>
        /// 样式名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 默认字体大小
        /// </summary>
        private const double defaultFontSize = 15d;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">样式标识ID</param>
        /// <param name="foregroundBrush">文本前景色Brush</param>
        /// <param name="fontFamily">文本字体[null使用默认字体]</param>
        /// <param name="fontSize">文本大小</param>
        public LogShowStyle(int id, System.Windows.Media.Brush foregroundBrush, System.Windows.Media.FontFamily fontFamily, double fontSize)
        {
            if (fontSize <= 0)
            {
                throw new ArgumentException("字体大小值无效", nameof(fontSize));
            }

            this.ID = id;
            this.ForegroundBrush = foregroundBrush;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">样式标识ID</param>
        /// <param name="foreground">文本前景色</param>
        /// <param name="fontName">文本字体名称[null使用默认字体]</param>
        /// <param name="fontSize">文本大小</param>
        public LogShowStyle(int id, System.Windows.Media.Color foreground, string fontName = null, double fontSize = defaultFontSize) :
            this(id, new System.Windows.Media.SolidColorBrush(foreground), GetFontFamily(fontName), fontSize)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="foreground">文本前景色</param>
        /// <param name="fontName">文本字体名称[null使用默认字体]</param>
        /// <param name="fontSize">文本大小</param>
        public LogShowStyle(LogLevel level, System.Windows.Media.Color foreground, string fontName = null, double fontSize = defaultFontSize) :
            this((int)level, new System.Windows.Media.SolidColorBrush(foreground), GetFontFamily(fontName), fontSize)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">样式标识ID</param>
        /// <param name="foreground">文本前景色</param>
        /// <param name="fontName">文本字体名称[null使用默认字体]</param>
        /// <param name="fontSize">文本大小</param>
        public LogShowStyle(int id, System.Drawing.Color foreground, string fontName = null, double fontSize = defaultFontSize) :
            this(id, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B)), GetFontFamily(fontName), fontSize)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="foreground">文本前景色</param>
        /// <param name="fontName">文本字体名称[null使用默认字体]</param>
        /// <param name="fontSize">文本大小</param>
        public LogShowStyle(LogLevel level, System.Drawing.Color foreground, string fontName = null, double fontSize = defaultFontSize) :
            this((int)level, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B)), GetFontFamily(fontName), fontSize)
        {

        }

        /// <summary>
        /// 获取字体
        /// </summary>
        /// <param name="fontName">文本字体名称</param>
        /// <returns>字体</returns>
        private static System.Windows.Media.FontFamily GetFontFamily(string fontName)
        {
            System.Windows.Media.FontFamily fontFamily;
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontFamily = null;
                //fontName = "Microsoft YaHei UI";
            }
            else
            {
                var fonts = FontEx.GetSystemInstallFonts();
                bool existYahei = (from tmpItem in fonts where string.Equals(tmpItem.Name, fontName, StringComparison.OrdinalIgnoreCase) select tmpItem).Count() > 0;
                if (existYahei)
                {
                    fontFamily = new System.Windows.Media.FontFamily(fontName);
                }
                else
                {
                    throw new ArgumentException(string.Format("当前系统中不存在名称为:{0}的字体", fontName), nameof(fontName));
                }
            }

            return fontFamily;
        }
    }
}
