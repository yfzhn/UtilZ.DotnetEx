using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 字体类扩展方法类
    /// </summary>
    public static class FontEx
    {
        /// <summary>
        /// 如何获得系统安装字体列表
        /// </summary>
        /// <returns>系统安装字体列表</returns>
        public static List<System.Drawing.FontFamily> GetSystemInstallFonts()
        {
            List<System.Drawing.FontFamily> fontFamilys = new List<System.Drawing.FontFamily>();
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily fontFamily in fonts.Families)
            {
                fontFamilys.Add(fontFamily);
            }

            return fontFamilys;
        }

        /// <summary>
        /// 如何获得系统字体样式列表
        /// </summary>
        /// <returns>系统字体样式列表</returns>
        public static List<System.Drawing.FontStyle> GetSystemFontStyles()
        {
            List<System.Drawing.FontStyle> fontStyles = new List<System.Drawing.FontStyle>();
            Type fontStyleType = typeof(System.Drawing.FontStyle);
            foreach (int item in Enum.GetValues(fontStyleType))
            {
                fontStyles.Add((System.Drawing.FontStyle)item);
            }

            return fontStyles;
        }
    }
}
