using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// 文件传输接口
    /// </summary>
    public interface IFileTransfer
    {
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
        void Upload(string remoteFilePath, Stream stream, long position = 0, long length = -1, UpdateMode mode = UpdateMode.Create, long transferedLength = 0, Action<long, long> scheduleNotify = null);

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
        void Upload(string remoteFilePath, string localFilePath, long position = 0, long length = -1, UpdateMode mode = UpdateMode.Create, long transferedLength = 0, Action<long, long> scheduleNotify = null);

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
        void Upload(string remoteFilePath, byte[] buffer, int position = 0, int length = -1, UpdateMode mode = UpdateMode.Create, long transferedLength = 0, Action<long, long> scheduleNotify = null);

        /// <summary>
        /// 上传目录
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="localDir">本地目录</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为下载总文件数,第二参数为当前所下载的文件数</param>
        void UploadDirectory(string remoteDir, string localDir, Action<long, long> scheduleNotify = null);
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
        void Download(string remoteFilePath, Stream stream, long position = 0, long length = -1, long transferedLength = 0, Action<long, long> scheduleNotify = null);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="localFilePath">本地文件路径</param>
        /// <param name="position">起始位置</param>
        /// <param name="length">数据长度</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        void Download(string remoteFilePath, string localFilePath, long position = 0, long length = -1, long transferedLength = 0, Action<long, long> scheduleNotify = null);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <param name="buffer">存放数据的数组</param>
        /// <param name="position">起始位置</param>
        /// <param name="length">数据长度</param>
        /// <param name="transferedLength">已传输数据长度</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为传输总大小,第二参数为已传输数据大小</param>
        void Download(string remoteFilePath, byte[] buffer, long position = 0, long length = -1, long transferedLength = 0, Action<long, long> scheduleNotify = null);

        /// <summary>
        /// 下载服务器上指定目录及其子目录内的所有文件,并按原结构存放本地
        /// </summary>
        /// <param name="localDir">本地目录</param>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="scheduleNotify">进度回调,第一个参数为下载总文件数,第二参数为当前所下载的文件数</param>
        void DownloadDirectory(string localDir, string remoteDir, Action<long, long> scheduleNotify = null);
        #endregion

        /// <summary>
        /// 检查文件是否存;在存在返回true;不存在返回false
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <returns>在存在返回true;不存在返回false</returns>
        bool ExistFile(string remoteFilePath);

        /// <summary>
        /// 检查目录是否存在[存在返回true;不存在返回false]
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <returns>存在返回true;不存在返回false</returns>
        bool ExistDirectory(string remoteDir);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        void DeleteFile(string remoteFilePath);

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="recursive">是否递归删除,true:先删除文件夹内的所有内容再删除文件夹;false:如果文件夹内有内容则会失败</param>
        void DeleteDirectory(string remoteDir, bool recursive);

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        void CreateDirectory(string remoteDir);

        /// <summary>
        /// 获取文件集合
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="option">获取选项</param>
        /// <returns>文件集合</returns>
        FileInfoItem[] GetFiles(string remoteDir, SearchOption option = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// 获取文件夹集合
        /// </summary>
        /// <param name="remoteDir">远程目录</param>
        /// <param name="option">获取选项</param>
        /// <returns>文件夹集合</returns>
        DirectoryInfoItem[] GetDirectories(string remoteDir, SearchOption option = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="remoteFilePath">远程文件路径</param>
        /// <returns>文件大小</returns>
        long GetFileLength(string remoteFilePath);

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="oldRemoteFilePath">旧文件路径</param>
        /// <param name="newFileName">新文件名</param>
        void Rename(string oldRemoteFilePath, string newFileName);

        /// <summary>
        /// 复制指定文件到指定路径
        /// </summary>
        /// <param name="sourceRemoteFilePath">原始文件路径</param>
        /// <param name="destRemoteFilePath">目的地文件路径</param>
        void Copy(string sourceRemoteFilePath, string destRemoteFilePath);

        /// <summary>
        /// 移除文件到指定路径
        /// </summary>
        /// <param name="sourceRemoteFilePath">原始文件路径</param>
        /// <param name="destRemoteFilePath">目的地文件路径</param>
        void Move(string sourceRemoteFilePath, string destRemoteFilePath);

        ///// <summary>
        ///// 清理空文件夹
        ///// </summary>
        ///// <param name="remoteDir">远程目录</param>
        //void ClearEmptyFolder(string remoteDir);
    }
}
