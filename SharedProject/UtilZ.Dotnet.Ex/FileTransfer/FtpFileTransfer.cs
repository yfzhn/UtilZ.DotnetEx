using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// ftp文件传输类
    /// </summary>
    public class FtpFileTransfer : IFileTransfer
    {
        /// <summary>
        /// init ftp url
        /// </summary>
        private readonly string _ftpUrl;

        /// <summary>
        /// ftp根地址
        /// </summary>
        private readonly string _ftpRootUrl;

        /// <summary>
        /// 根目录层级数组
        /// </summary>
        private readonly string[] _rootDirs;

        /// <summary>
        /// 用户名
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// 密码
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// 代理
        /// </summary>
        private readonly IWebProxy _proxy;

        /// <summary>
        /// 路径拆分字符数组
        /// </summary>
        private readonly char[] _splitDirChs = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        /// <summary>
        /// FTP目录信息匹配正则表达式
        /// </summary>
        private readonly string _ftpDirRegStr = @"(?<pre>^(?<year>\d{2}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1})\s+<[D,d]{1}[I,i]{1}[R,r]{1}>\s+)";

        /// <summary>
        /// FTP文件信息匹配正则表达式
        /// </summary>
        private readonly string _ftpFileRegStr = @"(?<pre>^(?<year>\d{2}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1})\s+(?<length>\d+)\s+)";

        /// <summary>
        /// 拆分文件或目录列表字符数组
        /// </summary>
        private readonly char[] _splitFileInfoChs = { ' ' };

        /// <summary>
        /// 年前缀
        /// </summary>
        private readonly string _yearPre;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ftpUrl">ftpUrl</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="proxy">代理</param>
        public FtpFileTransfer(string ftpUrl, string userName = null, string password = null, IWebProxy proxy = null)
        {
            if (string.IsNullOrWhiteSpace(ftpUrl))
            {
                throw new ArgumentNullException("ftpUrl");
            }

            ftpUrl = ftpUrl.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            Match match = Regex.Match(ftpUrl, RegexConstant.FtpUrl);
            if (!match.Success)
            {
                throw new ArgumentException(string.Format("无效的Ftp地址:{0}", ftpUrl), "ftpUrl");
            }

            string ip = match.Groups["ip"].Value;
            string port = match.Groups["port"].Value;
            string dir = match.Groups["dir"].Value;
            if (string.IsNullOrWhiteSpace(port))
            {
                port = "21";//ftp默认端口号为21
            }

            this._ftpUrl = ftpUrl;
            this._ftpRootUrl = string.Format("ftp://{0}:{1}", ip, port);
            this._rootDirs = dir.Split(this._splitDirChs, StringSplitOptions.RemoveEmptyEntries);
            this._userName = userName;
            this._password = password;
            this._proxy = proxy;
            this._yearPre = (DateTime.Now.Year / 100).ToString();
        }

        /// <summary>
        /// 创建FtpWebRequest
        /// </summary>
        /// <param name="ftpUrl">ftp url</param>
        /// <param name="method">请求方法</param>
        /// <returns>FtpWebRequest</returns>
        private FtpWebRequest CreateRequest(string ftpUrl, string method)
        {
            //根据服务器信息FtpWebRequest创建类的对象
            ftpUrl = ftpUrl.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Credentials = new NetworkCredential(this._userName, this._password);
            request.KeepAlive = false;
            request.UsePassive = false;
            request.UseBinary = true;
            request.Proxy = this._proxy;
            request.Method = method;
            return request;
        }

        /// <summary>
        /// 获取全目录层级名称列表
        /// </summary>
        /// <param name="relativeDir">相对目录</param>
        /// <returns>全目录层级名称列表</returns>
        private List<string> GetFullDirFolderNames(string relativeDir)
        {
            List<string> dirs = new List<string>(this._rootDirs);
            if (!string.IsNullOrWhiteSpace(relativeDir))
            {
                dirs.AddRange(relativeDir.Split(this._splitDirChs, StringSplitOptions.RemoveEmptyEntries));
            }

            return dirs;
        }

        /// <summary>
        /// 获取全路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>全路径</returns>
        private string GetFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return this._ftpUrl;
            }

            //截取路径开始处的路径分隔符
            while (relativePath.StartsWith(@"/") || relativePath.StartsWith(@"\"))
            {
                relativePath = relativePath.Remove(0, 1);
            }

            //截取路径尾部处的路径分隔符
            while (relativePath.EndsWith(@"/") || relativePath.EndsWith(@"\"))
            {
                relativePath = relativePath.Remove(relativePath.Length - 1, 1);
            }

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return this._ftpUrl;
            }
            else
            {
                return Path.Combine(this._ftpUrl, relativePath);
            }
        }

        #region 上传
        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="stream">读取数据的流</param>
        /// <param name="position">数组中读取数据的起始位置</param>
        /// <param name="length">要上传的数据长度</param>
        /// <param name="mode">上传文件的创建模式</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void Upload(string remoteFilePath, Stream stream, long position = 0, long length = -1, UpdateMode mode = UpdateMode.Create, long transferedLength = 0, Action<long, long> scheduleNotify = null)
        {
            #region 参数验证
            if (string.IsNullOrWhiteSpace(remoteFilePath))
            {
                throw new ArgumentNullException("remoteFilePath");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("流不可读");
            }

            if (position < 0)
            {
                position = 0;
            }

            if (length <= 0)
            {
                length = stream.Length - position;
            }

            if (transferedLength < 0)
            {
                transferedLength = 0;
            }
            else
            {
                if (transferedLength > length)
                {
                    throw new ArgumentOutOfRangeException("transferedLength", string.Format("已上传数据长度{0}无效,超出长度范围{1}", transferedLength, length));
                }

                position = position + transferedLength;
            }
            #endregion

            long sendedLength = transferedLength;
            stream.Seek(position, SeekOrigin.Begin);

            string remoteDir = Path.GetDirectoryName(remoteFilePath);
            if (!string.IsNullOrWhiteSpace(remoteDir))
            {
                this.CreateDirectory(remoteDir);
            }

            string ftpUrl = this.GetFullPath(remoteFilePath);
            FtpWebRequest ftpWebRequest;
            if (mode == UpdateMode.Append && this.ExistFile(remoteFilePath))
            {
                ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.AppendFile);
            }
            else
            {
                ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.UploadFile);
            }

            try
            {
                var handler = scheduleNotify;
                long needUploadLength = length - transferedLength;
                ftpWebRequest.ContentLength = needUploadLength;

                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];
                using (Stream requestStream = ftpWebRequest.GetRequestStream())
                {
                    while (needUploadLength > 0)
                    {
                        if (needUploadLength < bufferLength)
                        {
                            bufferLength = (int)needUploadLength;
                        }

                        int readLength = stream.Read(buffer, 0, bufferLength);
                        requestStream.Write(buffer, 0, readLength);

                        //数据偏移
                        needUploadLength -= readLength;
                        sendedLength += readLength;

                        //进度通知
                        if (handler != null)
                        {
                            handler(length, sendedLength);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TransferFileException(sendedLength, "上传文件失败", ex);
            }
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="localFilePath">本地文件路径</param>
        /// <param name="position">数组中读取数据的起始位置</param>
        /// <param name="length">要上传的数据长度</param>
        /// <param name="mode">上传文件的创建模式</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void Upload(string remoteFilePath, string localFilePath, long position = 0, long length = -1, UpdateMode mode = UpdateMode.Create, long transferedLength = 0, Action<long, long> scheduleNotify = null)
        {
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException(string.Empty, localFilePath);
            }

            using (var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.Upload(remoteFilePath, fs, position, length, mode, transferedLength, scheduleNotify);
            }
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="buffer">要上传的数据</param>
        /// <param name="position">数组中读取数据的起始位置</param>
        /// <param name="length">要上传的数据长度</param>
        /// <param name="mode">上传文件的创建模式</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void Upload(string remoteFilePath, byte[] buffer, int position = 0, int length = -1, UpdateMode mode = UpdateMode.Create, long transferedLength = 0, Action<long, long> scheduleNotify = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            using (var ms = new MemoryStream(buffer))
            {
                this.Upload(remoteFilePath, ms, position, length, mode, transferedLength, scheduleNotify);
            }
        }

        /// <summary>
        /// 上传目录
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="localDir">本地目录</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void UploadDirectory(string remoteDir, string localDir, Action<long, long> scheduleNotify = null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(localDir);
            int localDirLength = directoryInfo.FullName.Length + 1;
            string remoteFilePath;
            FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            int index = 1;
            var handler = scheduleNotify;
            //空目录无视,有文件的目录在上传文件时被创建
            foreach (var fileInfo in fileInfos)
            {
                if (handler != null)
                {
                    handler(fileInfos.Length, index++);
                }

                remoteFilePath = Path.Combine(remoteDir, fileInfo.FullName.Substring(localDirLength, fileInfo.FullName.Length - localDirLength));
                this.Upload(remoteFilePath, fileInfo.FullName, 0, -1, UpdateMode.Create, 0, null);
            }
        }
        #endregion

        #region 下载
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="stream">本地存储数据的流</param>
        /// <param name="position">起始位置</param>
        /// <param name="length">数据长度</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void Download(string remoteFilePath, System.IO.Stream stream, long position = 0, long length = -1, long transferedLength = 0, Action<long, long> scheduleNotify = null)
        {
            #region 参数验证
            if (string.IsNullOrWhiteSpace(remoteFilePath))
            {
                throw new ArgumentNullException("remoteFilePath");
            }

            if (!this.ExistFile(remoteFilePath))
            {
                throw new FileNotFoundException("文件不存在", remoteFilePath);
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException("流不可写");
            }

            string ftpUrl;
            long totallLength;
            ftpUrl = this.GetFullPath(remoteFilePath);
            FtpWebRequest ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.GetFileSize);
            using (FtpWebResponse re = (FtpWebResponse)ftpWebRequest.GetResponse())
            {
                totallLength = re.ContentLength;
            }

            if (position < 0)
            {
                position = 0;
                length = totallLength;
            }
            else
            {
                if (position > totallLength)
                {
                    throw new ArgumentOutOfRangeException("position", string.Format("偏移量值{0}无效,超出长度范围{1}", position, totallLength));
                }

                length = totallLength - position;
            }

            if (transferedLength < 0)
            {
                transferedLength = 0;
            }
            else
            {
                if (transferedLength > length)
                {
                    throw new ArgumentOutOfRangeException("transferedLength", string.Format("已上传数据长度{0}无效,超出长度范围{1}", transferedLength, length));
                }
            }
            #endregion

            ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.DownloadFile);
            long downloadLength = transferedLength;
            try
            {
                var handler = scheduleNotify;
                if (transferedLength > 0 || position > 0)
                {
                    ftpWebRequest.ContentOffset = transferedLength + position;
                }

                using (FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
                {
                    using (Stream ftpResponseStream = ftpWebResponse.GetResponseStream())
                    {
                        int bufferLength = 4096;
                        byte[] buffer = new byte[bufferLength];
                        long needDownlopadLength = length - transferedLength;

                        while (needDownlopadLength > 0)
                        {
                            if (needDownlopadLength < bufferLength)
                            {
                                bufferLength = (int)needDownlopadLength;
                            }

                            int readCount = ftpResponseStream.Read(buffer, 0, bufferLength);
                            stream.Write(buffer, 0, readCount);

                            needDownlopadLength -= readCount;
                            downloadLength += readCount;

                            if (handler != null)
                            {
                                handler(length, downloadLength);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TransferFileException(downloadLength, "下载文件失败", ex);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="localFilePath">本地文件路径</param>
        /// <param name="position">起始位置</param>
        /// <param name="length">数据长度</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void Download(string remoteFilePath, string localFilePath, long position = 0, long length = -1, long transferedLength = 0, Action<long, long> scheduleNotify = null)
        {
            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                throw new ArgumentNullException("localFilePath");
            }

            UtilZ.Dotnet.Ex.Base.DirectoryInfoEx.CheckFilePathDirectory(localFilePath);
            using (var fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                this.Download(remoteFilePath, fs, position, length, transferedLength, scheduleNotify);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="buffer">存放数据的数组</param>
        /// <param name="position">起始位置</param>
        /// <param name="length">数据长度</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        public void Download(string remoteFilePath, byte[] buffer, long position = 0, long length = -1, long transferedLength = 0, Action<long, long> scheduleNotify = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            using (var ms = new MemoryStream(buffer))
            {
                this.Download(remoteFilePath, ms, position, length, transferedLength, scheduleNotify);
            }
        }

        /// <summary>
        /// 下载服务器上指定目录及其子目录内的所有文件,并按原结构存放本地
        /// </summary>
        /// <param name="localDir">本地目录</param>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为下载总文件数,第二参数为当前所下载的文件数</param>
        public void DownloadDirectory(string localDir, string remoteDir, Action<long, long> scheduleNotify = null)
        {
            if (string.IsNullOrWhiteSpace(remoteDir))
            {
                throw new ArgumentNullException("remoteDir");
            }

            if (!this.ExistDirectory(remoteDir))
            {
                throw new DirectoryNotFoundException(remoteDir + "目录不存在");
            }

            string localFilePath, remoteFilePath;
            var fileList = this.GetFiles(remoteDir);
            foreach (var ftpFile in fileList)
            {
                //下载文件
                localFilePath = Path.Combine(localDir, ftpFile.Name);
                remoteFilePath = Path.Combine(remoteDir, ftpFile.Name);
                this.Download(remoteFilePath, localFilePath);
            }

            //下载子目录
            var subDirLis = this.GetDirectories(remoteDir);
            foreach (var subDir in subDirLis)
            {
                this.DownloadDirectory(Path.Combine(localDir, subDir.Name), Path.Combine(remoteDir, subDir.Name));
            }
        }
        #endregion

        /// <summary>
        /// 检查文件是否存;在存在返回true;不存在返回false
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <returns>在存在返回true;不存在返回false</returns>
        public bool ExistFile(string remoteFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(remoteFilePath))
                {
                    throw new ArgumentNullException("remoteFilePath");
                }

                string ftpUrl = this.GetFullPath(remoteFilePath);
                FtpWebRequest ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.GetDateTimestamp);
                using (FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
                {
                    //return ftpWebResponse.ContentLength > 0;
                    return true;
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    return false;
                }

                throw;
            }
        }

        private bool CheckDirectoryExists(string ftpUrl, string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return false;
            }

            Match match;
            string dirNanme;
            FtpWebRequest ftp = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.ListDirectoryDetails);
            using (Stream stream = ftp.GetResponse().GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                string line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    match = Regex.Match(line, _ftpDirRegStr);
                    if (match.Success)
                    {
                        dirNanme = line.Substring(match.Groups["pre"].Value.Length).Trim();
                        if (string.Equals(dirNanme, dir, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    line = sr.ReadLine();
                }
            }

            return false;
        }

        /// <summary>
        /// 检查目录是否存在[存在返回true;不存在返回false]
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <returns>存在返回true;不存在返回false</returns>
        public bool ExistDirectory(string remoteDir)
        {
            var dirs = this.GetFullDirFolderNames(remoteDir);
            string ftpUrl = this._ftpRootUrl;
            string cuurentDir;
            for (int i = 0; i < dirs.Count; i++)
            {
                cuurentDir = dirs[i];
                if (this.CheckDirectoryExists(ftpUrl, cuurentDir))//检查目录不存在则创建
                {
                    ftpUrl = Path.Combine(ftpUrl, cuurentDir);
                }
                else
                {
                    //某一级目录不存在
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        public void DeleteFile(string remoteFilePath)
        {
            if (!this.ExistFile(remoteFilePath))
            {
                return;
            }

            string ftpUrl = this.GetFullPath(remoteFilePath);
            FtpWebRequest ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.DeleteFile);
            using (var response = ftpWebRequest.GetResponse())
            {

            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="recursive">是否递归删除,true:先删除文件夹内的所有内容再删除文件夹;false:如果文件夹内有内容则会失败</param>
        public void DeleteDirectory(string remoteDir, bool recursive)
        {
            if (!this.ExistDirectory(remoteDir))
            {
                return;
            }

            if (recursive)
            {
                this.DeleteSubDirectoryAndFile(remoteDir);
            }
            else
            {
                this.PrimitiveDeleteDirectory(remoteDir);
            }
        }

        private void DeleteSubDirectoryAndFile(string remoteDir)
        {
            var ftpFileInfos = this.GetFiles(remoteDir);
            foreach (var ftpFileInfo in ftpFileInfos)
            {
                //删除当前目录中的文件
                this.DeleteFile(Path.Combine(remoteDir, ftpFileInfo.Name));
            }

            var ftpDirInfos = this.GetDirectories(remoteDir);
            foreach (var ftpDirInfo in ftpDirInfos)
            {
                string remoteDirSub = Path.Combine(remoteDir, ftpDirInfo.Name);

                //删除子目录中的下级子目录及文件
                this.DeleteSubDirectoryAndFile(remoteDirSub);

                //删除子目录
                this.PrimitiveDeleteDirectory(remoteDirSub);
            }

            this.PrimitiveDeleteDirectory(remoteDir);
        }

        private void PrimitiveDeleteDirectory(string remoteDir)
        {
            if (!this.ExistDirectory(remoteDir))
            {
                return;
            }

            //删除当前子目录
            string ftpUrl = this.GetFullPath(remoteDir);
            FtpWebRequest ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.RemoveDirectory);
            using (var response = ftpWebRequest.GetResponse())
            {

            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        public void CreateDirectory(string remoteDir)
        {
            if (string.IsNullOrWhiteSpace(remoteDir))
            {
                throw new ArgumentNullException("目录不能为空", "remoteDir");
            }

            // bool createResult = false;
            var dirs = this.GetFullDirFolderNames(remoteDir);
            string ftpUrl = this._ftpRootUrl;
            foreach (var dir in dirs)
            {
                if (this.CheckDirectoryExists(ftpUrl, dir))
                {
                    ftpUrl = Path.Combine(ftpUrl, dir);
                }
                else
                {
                    //检查目录不存在则创建
                    ftpUrl = Path.Combine(ftpUrl, dir);
                    FtpWebRequest ftp = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.MakeDirectory);
                    using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
                    {
                        //createResult = true;
                    }
                }
            }

            //return createResult;
        }

        /// <summary>
        /// 转换FTP创建时间
        /// </summary>
        /// <param name="year">年月日</param>
        /// <param name="time">时间</param>
        /// <returns>FTP创建时间</returns>
        private DateTime ConvertFTPCreateTime(string year, string time)
        {
            /************************
             * 月-日-年 时-分
             * 10-22-17 10:53AM
             * 02-03-18 03:03PM 
             ************************/
            char[] splitYearChs = { '-' };
            string[] yearStrs = year.Split(splitYearChs, StringSplitOptions.RemoveEmptyEntries);
            var dateTimeStr = string.Format("{0}{1}-{2}-{3} {4}", this._yearPre, yearStrs[2], yearStrs[0], yearStrs[1], time);
            var createTime = DateTime.Parse(dateTimeStr);
            return createTime;
        }


        /// <summary>
        /// 获取文件集合
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="option">获取选项</param>
        /// <returns>文件集合</returns>
        public FileInfoItem[] GetFiles(string remoteDir, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            if (!this.ExistDirectory(remoteDir))
            {
                return new FileInfoItem[0];
            }

            var fileList = new List<FileInfoItem>();
            this.PrimitiveGetFiles(fileList, remoteDir, option);
            return fileList.ToArray();
        }

        private void PrimitiveGetFiles(List<FileInfoItem> fileList, string remoteDir, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            string ftpUrl = this.GetFullPath(remoteDir);
            DateTime createTime;
            long length;
            FtpWebRequest ftp = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.ListDirectoryDetails);
            Match match;
            using (Stream stream = ftp.GetResponse().GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                string line = sr.ReadLine();
                string name;
                while (!string.IsNullOrEmpty(line))
                {
                    match = Regex.Match(line, this._ftpFileRegStr);
                    if (match.Success)
                    {
                        createTime = this.ConvertFTPCreateTime(match.Groups["year"].Value, match.Groups["time"].Value);
                        length = long.Parse(match.Groups["length"].Value);
                        name = line.Substring(match.Groups["pre"].Value.Length).Trim();
                        fileList.Add(new FileInfoItem { CreationTime = createTime, Name = name, FullName = Path.Combine(remoteDir, name), Length = length });
                    }

                    line = sr.ReadLine();
                }
            }

            if (option == SearchOption.AllDirectories)
            {
                var dirInfos = this.GetDirectories(remoteDir, SearchOption.TopDirectoryOnly);
                foreach (var dirInfo in dirInfos)
                {
                    this.PrimitiveGetFiles(fileList, remoteDir + "\\" + dirInfo.Name, option);
                }
            }
        }

        /// <summary>
        /// 获取文件夹集合
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="option">获取选项</param>
        /// <returns>文件夹集合</returns>
        public DirectoryInfoItem[] GetDirectories(string remoteDir, System.IO.SearchOption option = SearchOption.TopDirectoryOnly)
        {
            if (!this.ExistDirectory(remoteDir))
            {
                return new DirectoryInfoItem[0];
            }

            var dirList = new List<DirectoryInfoItem>();
            this.PrimitiveGetDirectories(dirList, remoteDir, option);
            return dirList.ToArray();
        }

        private void PrimitiveGetDirectories(List<DirectoryInfoItem> dirList, string remoteDir, System.IO.SearchOption option = SearchOption.TopDirectoryOnly)
        {
            string ftpUrl;
            if (string.IsNullOrWhiteSpace(remoteDir))
            {
                ftpUrl = this._ftpUrl;
            }
            else
            {
                ftpUrl = this.GetFullPath(remoteDir);
            }

            DateTime createTime;
            string name;
            FtpWebRequest ftp = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.ListDirectoryDetails);

            Match match;
            using (Stream stream = ftp.GetResponse().GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                string line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    match = Regex.Match(line, _ftpDirRegStr);
                    if (match.Success)
                    {
                        createTime = this.ConvertFTPCreateTime(match.Groups["year"].Value, match.Groups["time"].Value);
                        name = line.Substring(match.Groups["pre"].Value.Length).Trim();
                        dirList.Add(new DirectoryInfoItem { CreationTime = createTime, Name = name, FullName = Path.Combine(remoteDir, name) });
                    }

                    line = sr.ReadLine();
                }
            }

            if (option == SearchOption.AllDirectories)
            {
                var dirInfos = this.GetDirectories(remoteDir, SearchOption.TopDirectoryOnly);
                foreach (var dirInfo in dirInfos)
                {
                    this.PrimitiveGetDirectories(dirList, remoteDir + "\\" + dirInfo.Name, option);
                }
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <returns>文件大小</returns>
        public long GetFileLength(string remoteFilePath)
        {
            if (!this.ExistFile(remoteFilePath))
            {
                throw new FileNotFoundException("Ftp上文件不存在", remoteFilePath);
            }

            string ftpUrl = this.GetFullPath(remoteFilePath);
            FtpWebRequest ftpWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.GetFileSize);
            using (FtpWebResponse re = (FtpWebResponse)ftpWebRequest.GetResponse())
            {
                return re.ContentLength;
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="oldRemoteFilePath">旧文件路径</param>
        /// <param name="newFileName">新文件名</param>
        public void Rename(string oldRemoteFilePath, string newFileName)
        {
            if (Path.GetInvalidFileNameChars().Intersect(newFileName).Count() > 0)
            {
                throw new ArgumentException("新文件名无效,不能包含目录或其它非法字符,只能是文件名");
            }

            if (!this.ExistFile(oldRemoteFilePath) && !this.ExistDirectory(oldRemoteFilePath))
            {
                throw new FileNotFoundException("FTP文件服务器上原始文件或目录不存在", oldRemoteFilePath);
            }

            string ftpUrl = this.GetFullPath(oldRemoteFilePath);
            FtpWebRequest request = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.Rename);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = newFileName;
            using (var response = request.GetResponse())
            {
            }
        }

        /// <summary>
        /// 复制指定文件到指定路径
        /// </summary>
        /// <param name="sourceRemoteFilePath">原始文件路径</param>
        /// <param name="destRemoteFilePath">目的地文件路径</param>
        public void Copy(string sourceRemoteFilePath, string destRemoteFilePath)
        {
            //string localFilePath = Guid.NewGuid().ToString();
            //using (var fs = new FileStream(localFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            //{
            //    this.Download(sourceRemoteFilePath, fs, 0, -1, 0, null);
            //    fs.Seek(0, SeekOrigin.Begin);

            //    this.Upload(destRemoteFilePath, fs, 0, -1, UpdateMode.Create, 0, null);
            //}

            //File.Delete(localFilePath);

            string ftpUrl;
            long totallLength;
            ftpUrl = this.GetFullPath(sourceRemoteFilePath);
            FtpWebRequest ftpDownloadWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.GetFileSize);
            using (FtpWebResponse re = (FtpWebResponse)ftpDownloadWebRequest.GetResponse())
            {
                totallLength = re.ContentLength;
            }

            ftpDownloadWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.DownloadFile);

            string remoteDir = Path.GetDirectoryName(destRemoteFilePath);
            if (!string.IsNullOrWhiteSpace(remoteDir))
            {
                this.CreateDirectory(remoteDir);
            }

            ftpUrl = this.GetFullPath(destRemoteFilePath);
            FtpWebRequest ftpUploadWebRequest = this.CreateRequest(ftpUrl, WebRequestMethods.Ftp.UploadFile);

            using (FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpDownloadWebRequest.GetResponse())
            {
                using (Stream ftpDownloadResponseStream = ftpWebResponse.GetResponseStream())
                {
                    using (Stream uploadStream = ftpUploadWebRequest.GetRequestStream())
                    {
                        int bufferLength = 4096;
                        byte[] buffer = new byte[bufferLength];

                        while (totallLength > 0)
                        {
                            if (totallLength < bufferLength)
                            {
                                bufferLength = (int)totallLength;
                            }

                            int readCount = ftpDownloadResponseStream.Read(buffer, 0, bufferLength);
                            uploadStream.Write(buffer, 0, readCount);
                            totallLength -= readCount;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除文件到指定路径
        /// </summary>
        /// <param name="sourceRemoteFilePath">原始文件路径</param>
        /// <param name="destRemoteFilePath">目的地文件路径</param>
        public void Move(string sourceRemoteFilePath, string destRemoteFilePath)
        {
            this.Copy(sourceRemoteFilePath, destRemoteFilePath);
            this.DeleteFile(sourceRemoteFilePath);
        }

        ///// <summary>
        ///// 清理空文件夹
        ///// </summary>
        ///// <param name="remoteDir">远程目录</param>
        //public void ClearEmptyFolder(string remoteDir)
        //{
        //    var dirs = this.GetDirectories(remoteDir, SearchOption.AllDirectories);
        //    foreach (var dir in dirs)
        //    {
        //        if (!this.ExistDirectory(dir.FullName))
        //        {
        //            continue;
        //        }

        //        if (this.GetFiles(dir.FullName, SearchOption.AllDirectories).Length == 0)
        //        {
        //            this.DeleteDirecotry(dir.FullName, true);
        //        }
        //    }
        //}
    }
}
