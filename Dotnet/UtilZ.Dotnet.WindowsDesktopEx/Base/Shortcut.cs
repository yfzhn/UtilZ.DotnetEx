//using IWshRuntimeLibrary;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 创建快捷方式及关联程序和分享关联程序
    /// </summary>
    public partial class Shortcut
    {
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="fileInfo">应用程序文件</param>
        /// <param name="filePath">快捷方式指向的目标(某个exe的可执行文件)</param>
        public static void CreateDesktopShortcut(FileInfo fileInfo, string filePath)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            //建立对象
            WshShell shell = new WshShell();

            string shotcutName = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("."));
            //生成快捷方式文件，指定路径及文件名
            //string shortcutPath = string.Format(@"{0}\{1}.lnk", SpecialDirectories.Desktop, shotcutName);
            string shortcutPath = string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), shotcutName);
            //判断快捷方式是否存在,如果存在就删除
            FileInfo shortcutInfo = new FileInfo(shortcutPath);
            if (shortcutInfo.Exists)
            {
                shortcutInfo.Delete();
            }

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            //起始目录
            //shortcut.WorkingDirectory = @"C:\Program Files\Huawei\ATCS4";
            //快捷方式指向的目标
            shortcut.TargetPath = filePath;
            //窗口类型
            shortcut.WindowStyle = 1;
            //描述
            shortcut.Description = shotcutName;
            //图标
            shortcut.IconLocation = string.Format(@"{0},0", filePath);
            //保存，注意一定要保存，否则无效
            shortcut.Save();
        }
    }
}
