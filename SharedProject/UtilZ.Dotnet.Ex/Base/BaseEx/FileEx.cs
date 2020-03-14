using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 文件类扩展方法类[分区文本文件创]
    /// </summary>
    public static class FileEx
    {
        /// <summary>
        /// 创建需要分隔大小的文件路径[2014-10-27_1.log]
        /// </summary>
        /// <param name="createTime">创建文件日期</param>
        /// <param name="lastTime">上次记录日志文件的日期</param>
        /// <param name="index">当前文件索引,没有记录之前的初始值为-1</param>
        /// <param name="dateFormat">日期格式</param>
        /// <param name="directory">文件存放目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="extension">文件扩展名</param>
        /// <param name="fileSize">文件分隔大小,单位/MB</param>
        /// <returns>文件路径</returns>
        public static string CreateFilePath(DateTime createTime, ref DateTime lastTime, ref int index, string dateFormat, string directory, string fileName, string extension, uint fileSize)
        {
            //参数检查
            FileEx.CreateFilePathParaCheck(index, dateFormat, directory, fileSize);

            if (index == -1)
            {
                index = FileEx.UpdateFileIndex(directory, extension, createTime, dateFormat, fileName);
            }
            else
            {
                if (lastTime.Year != createTime.Year || lastTime.DayOfYear != createTime.DayOfYear)
                {
                    //如果日期发生了变化,则重置为1
                    index = 1;
                    lastTime = createTime;
                }
            }

            string dayStr = createTime.ToString(dateFormat);
            string filePath = System.IO.Path.Combine(directory, string.Format(@"{0}{1}_{2}{3}", dayStr, fileName, index, extension));

            if (File.Exists(filePath))
            {
                while (true)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        if (fileInfo.Length / 1024 / 1024 >= fileSize)//如果文件大小大于了fileSize大小，就将文件区分索引+1，再重新创建一个文件路径
                        {
                            index++;
                            filePath = Path.Combine(directory, string.Format(@"{0}_{1}{2}", dayStr, index, extension));
                            if (!File.Exists(filePath))
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch
                    {
                        index++;
                        filePath = Path.Combine(directory, string.Format(@"{0}_{1}{2}", dayStr, index, extension));
                        if (!File.Exists(filePath))
                        {
                            break;
                        }
                    }
                }
            }

            return filePath;
        }

        /// <summary>
        /// 创建需要分隔大小的文件路径[2014-10-27_1.log]方法参数验证
        /// </summary>
        /// <param name="index">当前文件索引,没有记录之前的初始值为-1</param>
        /// <param name="dateFormat">日期格式</param>
        /// <param name="directory">文件存放目录</param>
        /// <param name="fileSize">文件分隔大小</param>
        /// <returns>文件路径</returns>
        private static void CreateFilePathParaCheck(int index, string dateFormat, string directory, uint fileSize)
        {
            if (index == 0)
            {
                throw new ArgumentException(string.Format("文件索引:{0}不是有效值", index));
            }

            try
            {
                if (string.IsNullOrEmpty(dateFormat))
                {
                    throw new ArgumentException("日期转换格式参数不能为空");
                }
                DateTime.Now.ToString(dateFormat);
            }
            catch
            {
                throw new ArgumentException(string.Format("日期转换格式参数不是有效的格式化参数:{0}", dateFormat));
            }

            try
            {
                if (string.IsNullOrEmpty(directory))
                {
                    throw new ArgumentException("文件存放目录不能为空");
                }

                DirectoryInfo dirInfo = new DirectoryInfo(directory);
            }
            catch
            {
                throw new ArgumentException(string.Format("文件存放目录不是有效的参数:{0}", directory));
            }

            if (fileSize <= 0)
            {
                throw new ArgumentException("文件分隔大小不能小于0");
            }
        }

        /// <summary>
        /// 更新文件索引
        /// </summary>
        /// <param name="directory">文件存放目录</param>
        /// <param name="extension">文件扩展名</param>
        /// <param name="dt">日期时间</param>
        /// <param name="dateFormat">日期格式</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件索引</returns>
        private static int UpdateFileIndex(string directory, string extension, DateTime dt, string dateFormat, string fileName)
        {
            int logIndex = 1;
            int tmpLogIndex = -1;
            string regLogFileName = string.Format(@"^{0}{1}_(?<index>\d+){2}$", dt.ToString(dateFormat), fileName, extension);
            Regex logPathRex = new Regex(regLogFileName);
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                if (!dirInfo.Exists)
                {
                    return logIndex;
                }

                string[] filePaths = Directory.GetFiles(directory, "*" + extension);
                Match logPathMatch = null;

                foreach (string filePath in filePaths)
                {
                    logPathMatch = logPathRex.Match(Path.GetFileName(filePath));
                    if (!logPathMatch.Success)
                    {
                        continue;
                    }

                    tmpLogIndex = int.Parse(logPathMatch.Groups["index"].Value);
                    if (tmpLogIndex > logIndex)
                    {
                        logIndex = tmpLogIndex;
                    }
                }
            }
            catch
            { }
            return logIndex;
        }

        /// <summary>
        /// 获取分区文件的创建日期字符串
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>分区文件的创建日期字符串</returns>
        public static string GetFileCreateDate(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            int index = fileName.IndexOf("_");
            if (index != -1)
            {
                fileName = fileName.Substring(0, fileName.IndexOf("_"));
            }
            return fileName;
        }

        /// <summary>
        /// 尝试删除文件[删除成功返回true;失败返回false]
        /// </summary>
        /// <param name="filePath">目标文件路由</param>
        /// <returns>删除成功返回true;失败返回false</returns>
        public static bool TryDeleFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return false;
                }

                if (!File.Exists(filePath))
                {
                    return false;
                }

                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
