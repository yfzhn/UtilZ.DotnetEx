using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Log
{
    internal class FileAppenderVariateFileNameBuilder : FileAppenderPathBuilderBase
    {
        private readonly FileAppenderPathItem _lastPathItem;
        public FileAppenderVariateFileNameBuilder(FileAppenderConfig config, string[] paths, char[] pathSplitChars) :
            base(config, paths, pathSplitChars)
        {
            this._lastPathItem = this._pathItems.Last();
        }

        public override string CreateLogFilePath()
        {
            string tmpFilePath = this.PrimitiveCreateLogFilePath2();
            string dir = Path.GetDirectoryName(tmpFilePath);
            string logFilePath;

            if (this._isFirstGetFilePath && this._config.IsAppend)
            {
                this._isFirstGetFilePath = false;
                if (Directory.Exists(dir))
                {
                    this.ClearExpireLogFile(dir);
                    logFilePath = base.GetLastWriteLogFilePath(dir);
                    if (string.IsNullOrWhiteSpace(logFilePath))
                    {
                        logFilePath = tmpFilePath;
                    }
                }
                else
                {
                    logFilePath = tmpFilePath;
                    Directory.CreateDirectory(dir);
                }
            }
            else
            {
                logFilePath = tmpFilePath;
                if (Directory.Exists(dir))
                {
                    this.ClearExpireLogFile(dir);
                }
                else
                {
                    Directory.CreateDirectory(dir);
                }
            }

            return logFilePath;
        }

        /// <summary>
        /// 清理过期的日志文件
        /// </summary>
        private void ClearExpireLogFile(string currentLogDir)
        {
            try
            {
                List<FileInfo> fileInfos = this.GetAllLogFileInfos();
                var hsDelLogFileFullPathDirs = new HashSet<string>();

                //按日志保留天数删除
                base.DeleteLogFileByDays(fileInfos, hsDelLogFileFullPathDirs);

                //按日志文件个数删除日志
                base.DeleteLogFileByFileCount(fileInfos, hsDelLogFileFullPathDirs);

                //排除本次写日志目录
                if (hsDelLogFileFullPathDirs.Contains(currentLogDir))
                {
                    hsDelLogFileFullPathDirs.Remove(currentLogDir);
                }

                //删除空目录
                this.DeleteEmptyDirectory(hsDelLogFileFullPathDirs);
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog("清除过期日志异常", ex);
            }
        }

        private void DeleteEmptyDirectory(HashSet<string> hsDelLogFileFullPathDirs)
        {
            if (hsDelLogFileFullPathDirs.Count == 0)
            {
                return;
            }

            foreach (var delLogFileFullPathDir in hsDelLogFileFullPathDirs)
            {
                try
                {
                    //级联删除空目录
                    var delDirInfo = new DirectoryInfo(delLogFileFullPathDir);
                    while (true)
                    {
                        if (!delDirInfo.Exists)
                        {
                            delDirInfo = delDirInfo.Parent;
                            continue;
                        }

                        if (delDirInfo.GetFileSystemInfos("*.*", SearchOption.AllDirectories).Length == 0)
                        {
                            delDirInfo.Delete();
                        }
                        else
                        {
                            break;
                        }

                        delDirInfo = delDirInfo.Parent;
                        if (string.Equals(this._rootDir, delDirInfo.FullName, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogSysInnerLog.OnRaiseLog(this, ex);
                }
            }
        }

        private List<FileInfo> GetAllLogFileInfos()
        {
            List<FileInfo> srcFileInfos = this.GetAllLogFileInfo();
            List<FileInfo> fileInfos = new List<FileInfo>();
            foreach (var fileInfo in srcFileInfos)
            {
                if (this.ValidatePath(fileInfo.FullName))
                {
                    fileInfos.Add(fileInfo);
                }
            }

            return fileInfos;
        }

        /// <summary>
        /// 检查日志文件路径是否是有效路径[有效返回true;无效返回false]
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool ValidatePath(string path)
        {
            if (!string.IsNullOrWhiteSpace(Path.GetPathRoot(path)))
            {
                path = path.Substring(this._rootDir.Length);
            }

            var paths = path.Split(this._pathSplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length != this._pathItems.Length)
            {
                return false;
            }

            DateTime createTime;
            for (int i = 0; i < paths.Length; i++)
            {
                if (!this._pathItems[i].CheckPath(paths[i], out createTime))
                {
                    return false;
                }
            }

            return true;
        }

        private List<FileInfo> GetAllLogFileInfo()
        {
            List<FileInfo> srcFileInfos = new List<FileInfo>();
            try
            {
                if (this._pathItems.Length == 1)
                {
                    //存放于日志根目录
                    srcFileInfos.AddRange(base.RootDirectoryInfo.GetFiles(base._searchPattern, SearchOption.TopDirectoryOnly));
                }
                else
                {
                    //存放于需要实时创建目录的子目录
                    var dirInfos = base.RootDirectoryInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                    foreach (var dirInfo in dirInfos)
                    {
                        try
                        {
                            srcFileInfos.AddRange(dirInfo.GetFiles(base._searchPattern, SearchOption.AllDirectories));
                        }
                        catch (Exception exi)
                        {
                            LogSysInnerLog.OnRaiseLog(this, exi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }

            return srcFileInfos;
        }

        protected override bool CompareLastLogFilePath(FileInfo lastLogFileInfo)
        {
            var time = DateTime.Now;
            var createTime = lastLogFileInfo.CreationTime;
            if (this._config.MaxFileLength > 0 &&
                lastLogFileInfo.Length < this._config.MaxFileLength &&
                createTime.Year == time.Year &&
                createTime.Month == time.Month &&
                createTime.Day == time.Day)
            {
                //最后一个文件是当天创建且小于目标大小
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查日志文件路径是否是有效路径[有效返回true;无效返回false]
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        protected override bool CheckPath(string filePath, out DateTime createTime)
        {
            return this._lastPathItem.CheckPath(Path.GetFileName(filePath), out createTime);
        }

        private string PrimitiveCreateLogFilePath2()
        {
            string logFileFullPath = base.PrimitiveCreateLogFilePath();
            while (File.Exists(logFileFullPath))
            {
                logFileFullPath = base.PrimitiveCreateLogFilePath();
            }

            return logFileFullPath;
        }
    }
}
