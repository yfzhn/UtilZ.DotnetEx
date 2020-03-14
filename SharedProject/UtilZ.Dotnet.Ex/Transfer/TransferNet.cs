using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 网络传输类
    /// </summary>
    public class TransferNet : IDisposable
    {
        private readonly HashSet<UdpTransferChannelItem> _transferChannels = new HashSet<UdpTransferChannelItem>();
        private readonly object _transferChannelsLock = new object();
        private readonly AsynQueue<ReceiveDataItem> _revQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TransferNet(TransferConfig config, IEnumerable<ushort> listenPorts, Action<ReceiveDataItem> rev)
        {
            if (listenPorts == null || listenPorts.Count() == 0)
            {
                throw new ArgumentNullException(nameof(listenPorts));
            }

            if (rev == null)
            {
                throw new ArgumentNullException(nameof(rev));
            }

            if (config == null)
            {
                config = new TransferConfig();
            }
            else
            {
                config.Validate();
            }

            this._revQueue = new AsynQueue<ReceiveDataItem>(rev, "TransferNet.接收数据输出线程", true, true);

            try
            {
                foreach (var listenPort in listenPorts)
                {
                    TransferConfig config2 = config.Clone();
                    config2.NetConfig.ListenEP = new IPEndPoint(IPAddress.Any, listenPort);
                    this._transferChannels.Add(new UdpTransferChannelItem(new TransferChannel(config2, this.TransferChannelRev)));
                }
            }
            catch (Exception)
            {
                this.Dispose();
                throw;
            }
        }

        private void TransferChannelRev(ReceiveDataItem item)
        {
            this._revQueue.Enqueue(item);
        }

        private void DisposeTransferChannels()
        {
            lock (this._transferChannelsLock)
            {
                foreach (var transferChannel in this._transferChannels)
                {
                    transferChannel.TransferChannel.Dispose();
                }

                this._transferChannels.Clear();
            }
        }

        #region 发送数据
        private UdpTransferChannelItem GetUdpTransferChannel()
        {
            lock (this._transferChannelsLock)
            {
                var transferChannelItem = this._transferChannels.OrderBy(t => { return t.TransferLength; }).FirstOrDefault();
                if (transferChannelItem == null)
                {
                    throw new ObjectDisposedException(nameof(this._transferChannels));
                }

                return transferChannelItem;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="policy">发送策略</param>
        public void SendData(byte[] data, TransferPolicy policy)
        {
            var transferChannelItem = this.GetUdpTransferChannel();
            transferChannelItem.IncrementTransferLength(data.Length);
            try
            {
                transferChannelItem.TransferChannel.SendData(data, policy);
            }
            finally
            {
                transferChannelItem.DecrementTransferLength(data.Length);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="postion">发送数据在流中的起始位置</param>
        /// <param name="length">要发送数据长度</param>
        /// <param name="policy">发送策略</param>
        public void SendData(byte[] data, int postion, int length, TransferPolicy policy)
        {
            var transferChannelItem = this.GetUdpTransferChannel();
            transferChannelItem.IncrementTransferLength(length);
            try
            {
                transferChannelItem.TransferChannel.SendData(data, postion, length, policy);
            }
            finally
            {
                transferChannelItem.DecrementTransferLength(length);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="filePath">要发送的文件路径</param>
        /// <param name="policy">发送数据在流中的起始位置</param>
        public void SendFile(string filePath, TransferPolicy policy)
        {
            var fileInfo = new FileInfo(filePath);

            var transferChannelItem = this.GetUdpTransferChannel();
            transferChannelItem.IncrementTransferLength(fileInfo.Length);
            try
            {
                transferChannelItem.TransferChannel.SendFile(filePath, policy);
            }
            finally
            {
                transferChannelItem.DecrementTransferLength(fileInfo.Length);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="filePath">要发送的文件路径</param>
        /// <param name="postion">发送数据在流中的起始位置</param>
        /// <param name="length">要发送数据长度</param>
        /// <param name="policy">发送策略</param>
        public void SendFile(string filePath, long postion, long length, TransferPolicy policy)
        {
            var transferChannelItem = this.GetUdpTransferChannel();
            transferChannelItem.IncrementTransferLength(length);
            try
            {
                transferChannelItem.TransferChannel.SendFile(filePath, postion, length, policy);
            }
            finally
            {
                transferChannelItem.DecrementTransferLength(length);
            }
        }

        /// <summary>
        /// 发送数据流
        /// </summary>
        /// <param name="stream">要发送的流</param>
        /// <param name="postion">发送数据在流中的起始位置</param>
        /// <param name="length">要发送数据长度</param>
        /// <param name="policy">发送策略</param>
        public void SendData(Stream stream, long postion, long length, TransferPolicy policy)
        {
            var transferChannelItem = this.GetUdpTransferChannel();
            transferChannelItem.IncrementTransferLength(length);
            try
            {
                transferChannelItem.TransferChannel.SendData(stream, postion, length, policy);
            }
            finally
            {
                transferChannelItem.DecrementTransferLength(length);
            }
        }
        #endregion

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.DisposeTransferChannels();
                this._revQueue.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "Dispose异常");
            }
        }
    }

    internal class UdpTransferChannelItem
    {
        public TransferChannel TransferChannel { get; private set; }

        private readonly object _transferLengthLock = new object();
        private long _transferLength = 0;
        public long TransferLength
        {
            get
            {
                lock (this._transferLengthLock)
                {
                    return _transferLength;
                }
            }
        }

        public UdpTransferChannelItem(TransferChannel transferChannel)
        {
            transferChannel.Start();
            this.TransferChannel = transferChannel;
        }

        internal void IncrementTransferLength(long length)
        {
            lock (this._transferLengthLock)
            {
                this._transferLength += length;
            }
        }

        internal void DecrementTransferLength(long length)
        {
            lock (this._transferLengthLock)
            {
                this._transferLength -= length;
            }
        }
    }
}
