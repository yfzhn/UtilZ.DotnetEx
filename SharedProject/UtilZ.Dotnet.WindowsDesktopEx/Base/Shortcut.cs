//using IWshRuntimeLibrary;
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
        /// 使本程序与文件类型关联
        /// </summary>
        /// <param name="fileTypeName">文件类型名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <param name="associateAppPath">安装程序路径</param>
        public static void AssociateWithFile(string fileTypeName, string fileExtension, string associateAppPath)
        {
            if (string.IsNullOrWhiteSpace(fileTypeName))
            {
                throw new ArgumentNullException("fileTypeName");
            }

            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                throw new ArgumentNullException("fileExtension");
            }

            if (string.IsNullOrWhiteSpace(associateAppPath))
            {
                throw new ArgumentNullException("associateAppPath");
            }

            // 检查fileExtension参数格式是否正确
            Regex rgx = new Regex(@"^\.?(?<extension>\w+)$");
            Match mh = rgx.Match(fileExtension);
            if (!mh.Success)
            {
                throw new Exception("参数fileExtension格式错误");
            }

            RegistryKey classesRootKey = RegistryKeyEx.OpeBaseKey(RegistryHive.ClassesRoot);
            // 在HKEY_CLASSES_ROOT下创建名为fileTypeName的节点
            RegistryKey key = classesRootKey.CreateSubKey(fileTypeName);//临时的注册表项
            // 保存自定义节点，用于以后检查关联程序路径是否正确
            key.SetValue("Create", associateAppPath);
            // 创建关联图标节点
            RegistryKey keyico = key.CreateSubKey("DefaultIcon");//关联图标的注册表项
            // 设置关联图标路径
            keyico.SetValue(string.Empty, associateAppPath + ",1");
            // 设置关联文件描述
            key.SetValue(string.Empty, fileTypeName);
            key = key.CreateSubKey("Shell");
            key = key.CreateSubKey("Open");
            key = key.CreateSubKey("Command");
            // 关联的程序的启动位置
            key.SetValue(string.Empty, string.Format("\"{0}\"  \"%1\"", associateAppPath));
            // 或旧的文件扩展名关联节点
            string extension = string.Format(".{0}", mh.Groups["extension"].Value);
            // 关联的文件扩展名
            key = classesRootKey.CreateSubKey(extension);
            key.SetValue(string.Empty, fileTypeName);
            classesRootKey.Close();
            key.Close();
            keyico.Close();
        }

        /// <summary>
        /// 分离本程序与文件类型的关联
        /// </summary>
        /// <param name="fileTypeName">文件类型名</param>
        /// <param name="fileExtension">文件扩展名</param>
        public static void DisassocateWithFile(string fileTypeName, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileTypeName))
            {
                throw new ArgumentNullException("fileTypeName");
            }

            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                throw new ArgumentNullException("fileExtension");
            }

            // 检查fileExtension参数格式是否正确
            Regex rgx = new Regex(@"^\.?(?<extension>\w+)$");
            Match mh = rgx.Match(fileExtension);
            if (!mh.Success)
            {
                throw new Exception("参数fileExtension格式错误");
            }

            // 获取文件扩展名(有点号)
            string extension = string.Format(".{0}", mh.Groups["extension"].Value);
            RegistryKey classesRootKey = RegistryKeyEx.OpeBaseKey(RegistryHive.ClassesRoot);
            // 获取文件关联注册表项
            RegistryKey fileAssociationKey = classesRootKey.OpenSubKey(extension);
            // 如果注册表项关联类型为fileTypeName则，删除这项
            if (null != fileAssociationKey &&
                string.Equals(fileAssociationKey.GetValue(string.Empty).ToString(), fileTypeName, StringComparison.OrdinalIgnoreCase))
            {
                Registry.ClassesRoot.DeleteSubKeyTree(extension);
                fileAssociationKey.Close();
            }

            // 获取名为fileTypeName的注册表ClassesRoot的子项fileAssociationKey
            RegistryKey fileTypeDiscriptionKey = classesRootKey.OpenSubKey(fileTypeName);
            if (fileTypeDiscriptionKey != null)
            {
                // 程序启动路径正确则表示已经关联，则删除这个注册表项
                Registry.ClassesRoot.DeleteSubKeyTree(fileTypeName);
                fileTypeDiscriptionKey.Close();
            }

            classesRootKey.Close();
        }


        /*
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
        */
    }
}
