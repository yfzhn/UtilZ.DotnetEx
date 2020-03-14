using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 接收到的数据项
    /// </summary>
    public class ReceiveDataItem : TimeoutBase
    {
        /// <summary>
        /// 地址信息
        /// </summary>
        public IPEndPoint SrcEndPoint
        {
            get { return NotifyMessage.SrcEndPoint; }
        }

        /// <summary>
        /// 数据标识[true:byte[]数据;false:文件]
        /// </summary>
        protected bool _flag;
        /// <summary>
        /// 获取数据标识[true:byte[]数据;false:文件]
        /// </summary>
        public bool Flag
        {
            get { return _flag; }
            protected set { _flag = value; }
        }

        /// <summary>
        /// 数据
        /// </summary>
        protected byte[] _data;
        /// <summary>
        /// 获取数据
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            protected set { _data = value; }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        protected string _filePath;
        /// <summary>
        /// 获取文件路径
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            protected set { _filePath = value; }
        }

        /// <summary>
        /// 数据类型[true:资源模式;false:消息模式]
        /// </summary>
        private bool _isResourceMod;

        [NonSerialized]
        private readonly Stream[] _streamArr = null;

        private bool _isClosed = false;

        [NonSerialized]
        private readonly object _isClosedLock = new object();

        [NonSerialized]
        //protected readonly BlockingCollection<Stream> _streams = new BlockingCollection<Stream>();
        private readonly BlockingCollection<Stream> _streams = new BlockingCollection<Stream>();

        internal SendDataNotifyMessage NotifyMessage { get; private set; }

        /// <summary>
        /// 构造函数-消息模式
        /// </summary>
        /// <param name="message"></param>
        internal ReceiveDataItem(SendDataNotifyMessage message)
            : base(message.Timeout)
        {
            this._isResourceMod = false;

            var data = new byte[message.Data.Length];
            Array.Copy(message.Data, data, data.Length);
            this.Data = data;
            this.NotifyMessage = message;
            this._flag = true;
        }

        /// <summary>
        /// 构造函数-资源模式[数据资源-文件-流]
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tmpDir"></param>
        /// <param name="threadCount"></param>
        internal ReceiveDataItem(SendDataNotifyMessage message, string tmpDir, int threadCount)
            : base(message.Timeout)
        {
            this._isResourceMod = true;
            this._streamArr = new Stream[threadCount];

            if (message.ResourceType == ResourceTypeConstant.ResourceData)
            {
                try
                {
                    this.InitMemoryStream((int)message.Size);
                }
                catch (OutOfMemoryException)
                {
                    //内存不足了,写文件
                    this.InitFileStream(tmpDir, null);
                }
            }
            else if (message.ResourceType == ResourceTypeConstant.ResourceFile)
            {
                this.InitFileStream(tmpDir, message.FileName);
            }
            else if (message.ResourceType == ResourceTypeConstant.ResourceStream)
            {
                this.InitFileStream(tmpDir, null);
            }
            else
            {
                throw new NotImplementedException($"未实现的数据资源类型:{message.ResourceType}");
            }

            this.NotifyMessage = message;
        }

        /// <summary>
        /// 初始化内存流
        /// </summary>
        /// <param name="size"></param>
        protected void InitMemoryStream(int size)
        {
            var data = new byte[size];
            this._data = data;
            this._flag = true;
            for (int i = 0; i < this._streamArr.Length; i++)
            {
                this._streamArr[i] = new MemoryStream(data);
                this._streams.Add(this._streamArr[i]);
            }
        }

        /// <summary>
        /// 初始化文件流
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        protected void InitFileStream(string dir, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"{TimeEx.GetTimestamp()}.dat";
            }

            string filePath = Path.Combine(dir, fileName);
            this._filePath = filePath;
            this._flag = false;
            for (int i = 0; i < this._streamArr.Length; i++)
            {
                this._streamArr[i] = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                this._streams.Add(this._streamArr[i]);
            }
        }

        internal void Write(long position, byte[] revData, int offset, int size)
        {
            try
            {
                if (this._isClosed)
                {
                    return;
                }

                var stream = this._streams.Take();
                if (stream == null)
                {
                    return;
                }

                try
                {
                    if (stream.Position != position)
                    {
                        stream.Seek(position, SeekOrigin.Begin);
                    }

                    stream.Write(revData, offset, size);
                }
                finally
                {
                    this._streams.Add(stream);
                }
            }
            catch (OperationCanceledException)
            { }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                Loger.Error(ex, "写响应数据异常");
            }
        }

        internal void Close(bool isDeleteFile)
        {
            try
            {
                if (this._isClosed)
                {
                    return;
                }

                lock (this._isClosedLock)
                {
                    if (this._isClosed)
                    {
                        return;
                    }

                    this._isClosed = true;
                    if (this._isResourceMod)
                    {
                        foreach (var stream in this._streamArr)
                        {
                            stream.Close();
                        }

                        this._streams.Dispose();
                    }

                    if (isDeleteFile && !this._flag)
                    {
                        try
                        {
                            File.Delete(this._filePath);
                        }
                        catch (Exception)
                        { }
                    }

                    GC.Collect();
                    GC.WaitForFullGCComplete();
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
