using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferSender : IDisposable
    {
        private readonly ITransferNet _net;
        private readonly ResoucesInfo _resoucesInfo;
        //private readonly BlockingCollection<Stream> _streams = new BlockingCollection<Stream>();
        private readonly BlockingCollection<Stream> _streams = new BlockingCollection<Stream>();
        private readonly List<Stream> _streamList = new List<Stream>();
        private bool _isDisposabled = false;
        private readonly object _isDisposabledLock = new object();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ManualResetEvent _transferCompletedAckEventWaitHandle = new ManualResetEvent(false);

        public TransferSender(ITransferNet net, ResoucesInfo resoucesInfo)
        {
            this._net = net;
            this._resoucesInfo = resoucesInfo;
        }

        internal void Send()
        {
            var sendDataNotifyMessage = new SendDataNotifyMessage(this._resoucesInfo);
            byte[] buffer = sendDataNotifyMessage.GenerateBuffer();
            var beginSendDataTimestamp = this._resoucesInfo.LastAccessTimestamp;
            this._net.Send(buffer, this._resoucesInfo.Policy.RemoteEP);

            int repeatCount = 0;
            while (this._net.Status)
            {
                repeatCount++;
                try
                {
                    if (this._transferCompletedAckEventWaitHandle.WaitOne(this._resoucesInfo.Policy.MillisecondsTimeout))
                    {
                        break;
                    }
                    else
                    {
                        //超时一次请求数据或传输完成通知都未收到过,重新发送数据
                        if (beginSendDataTimestamp == this._resoucesInfo.LastAccessTimestamp)
                        {
                            this._net.Send(buffer, this._resoucesInfo.Policy.RemoteEP);
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    throw new TimeoutException("发送超时");
                }

                if (!this._net.Status)
                {
                    //发送取消,已停止
                    break;
                }

                if (this._resoucesInfo.IsTimeout() && repeatCount >= this._resoucesInfo.Policy.RepeatCount)
                {
                    throw new TimeoutException("发送超时");
                }
            }
        }

        internal void ProResourceRequest(ResourceRequestMessage req)
        {
            try
            {
                this._resoucesInfo.UpdateLastAccessTimestamp();
                if (req.Size <= 0)
                {
                    Loger.Warn($"[RID:{req.Header.Rid}]请求数据长度值:{req.Size}无效,忽略");
                    return;
                }

                //Loger.Trace($"收到请求RID:{message.Rid},ContextId:{message.ContextId},Position:{message.Position}-Size:{message.Size}");
                byte[] data = this.GetRequestData(req);
                var res = new ResourceResponseMessage(req, data);
                byte[] buffer = res.GenerateBuffer();
                this._net.Send(buffer, this._resoucesInfo.Policy.RemoteEP);
                //Loger.Trace($"响应请求RID:{req.Rid},ContextId:{req.ContextId},Position:{req.Position}-Size:{req.Size}");
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "处理资源请求异常");
            }
        }

        private byte[] GetRequestData(ResourceRequestMessage req)
        {
            //Loger.Trace($"收到请求RID:{message.Rid},ContextId:{message.ContextId},Position:{message.Position}-Size:{message.Size}");
            byte[] data = null;
            const int timeout = 10;
            var resoucesType = this._resoucesInfo.ResoucesType;

            while (!this._isDisposabled)
            {
                try
                {
                    switch (resoucesType)
                    {
                        case TransferDataType.Data:
                        case TransferDataType.File:
                            Stream stream = null;
                            data = new byte[req.Size];
                            try
                            {
                                if (!this._streams.TryTake(out stream, timeout, this._cts.Token))
                                {
                                    lock (this._isDisposabledLock)
                                    {
                                        if (this._isDisposabled)
                                        {
                                            return null;
                                        }

                                        if (resoucesType == TransferDataType.Data)
                                        {
                                            stream = new MemoryStream(this._resoucesInfo.Data);
                                        }
                                        else if (resoucesType == TransferDataType.File)
                                        {
                                            stream = new FileStream(this._resoucesInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                        }
                                        else
                                        {
                                            throw new NotImplementedException($"未实现的发送模式[{resoucesType.ToString()}]");
                                        }

                                        this._streamList.Add(stream);
                                    }
                                }

                                if (stream == null)
                                {
                                    continue;
                                }

                                stream.Seek(req.Position, SeekOrigin.Begin);
                                stream.Read(data, 0, data.Length);
                            }
                            catch (ObjectDisposedException)
                            { }
                            finally
                            {
                                if (stream != null)
                                {
                                    try
                                    {
                                        this._streams.Add(stream);
                                    }
                                    catch (ObjectDisposedException)
                                    { }
                                }
                            }
                            break;
                        case TransferDataType.Stream:
                            data = this._resoucesInfo.GetStreamData(req.Position, req.Size);
                            break;
                        default:
                            throw new NotImplementedException($"未实现的资源类型:{this._resoucesInfo.ResoucesType.ToString()}");
                    }

                    break;
                }
                catch (ArgumentNullException)
                {
                    //.net BUG
                    continue;
                }
                catch (OperationCanceledException)
                {
                    data = null;
                    break;
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                    data = null;
                    break;
                }
            }

            return data;
        }

        internal void ProResourceCompleted()
        {
            try
            {
                this._transferCompletedAckEventWaitHandle.Set();
            }
            catch (ObjectDisposedException)
            { }
        }

        public void Dispose()
        {
            try
            {
                lock (this._isDisposabledLock)
                {
                    this._isDisposabled = true;
                    this._cts.Cancel();
                    this._cts.Dispose();
                    this._transferCompletedAckEventWaitHandle.Dispose();
                    foreach (var stream in this._streamList)
                    {
                        stream.Close();
                    }

                    this._streams.Dispose();
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex, $"{nameof(TransferSender)}.Dispose异常");
            }
        }
    }
}
