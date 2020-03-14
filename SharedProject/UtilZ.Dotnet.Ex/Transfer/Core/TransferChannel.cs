using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Base.MemoryCache;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 传输通道类
    /// </summary>
    public class TransferChannel : IDisposable
    {
        private readonly TransferConfig _config;
        private readonly ITransferNet _net = null;
        //private readonly BlockingCollection<ReceiveDatagramInfo> _waitParseDatas=new BlockingCollection<ReceiveDatagramInfo>(new ConcurrentQueue<ReceiveDatagramInfo>());
        private readonly BlockingCollection<ReceiveDatagramInfo> _waitParseDatas = new BlockingCollection<ReceiveDatagramInfo>();
        private readonly List<ThreadEx> _parseDataThreads = new List<ThreadEx>();
        private readonly object _parseDataThreadsLock = new object();
        private readonly CancellationTokenSource _parseDataThreadsCts = new CancellationTokenSource();
        /// <summary>
        /// key:RID;value:TransferSender
        /// </summary>
        private readonly ConcurrentDictionary<int, TransferSender> _senderDic = new ConcurrentDictionary<int, TransferSender>();

        //private readonly Hashtable _htHostReceiver = Hashtable.Synchronized(new Hashtable());
        private readonly TransferReceiver _receiver;

        /// <summary>
        /// 获取传输配置
        /// </summary>
        public TransferConfig Config
        {
            get { return this._config; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">网络传输配置</param>
        /// <param name="rev">接收函数回调</param>
        public TransferChannel(TransferConfig config, Action<ReceiveDataItem> rev)
        {
            if (config == null)
            {
                config = new TransferConfig();
            }
            else
            {
                config.Validate();
            }

            if (rev == null)
            {
                throw new ArgumentNullException(nameof(rev));
            }

            this._config = config;

            for (int i = 0; i < config.TransferThreadCount; i++)
            {
                this.AddParseDispoatchThread();
            }

            if (config.NetConfig.Protocal == TransferProtocal.Udp)
            {
                this._net = new UdpTransferNet();
            }
            else
            {
                this._net = new TcpTransferNet();
            }

            this._net.Init(config.NetConfig, this.NetRev);
            this._receiver = new TransferReceiver(this._config, this._net, rev);
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            this._net.Start();
        }

        private void NetRev(ReceiveDatagramInfo receiveDatagramInfo)
        {
            this._waitParseDatas.Add(receiveDatagramInfo);
        }

        #region 解析数据
        private int _index = 1;
        private void AddParseDispoatchThread()
        {
            try
            {
                var token = this._parseDataThreadsCts.Token;
                lock (this._parseDataThreadsLock)
                {
                    if (this._parseDataThreads.Count >= this._config.ParseDataMaxThreadCount)
                    {
                        return;
                    }

                    var parseDataThread = new ThreadEx(this.ProReceiveDataThreadMethod, $"解析数据线程方法[{this._index++}]", true);
                    parseDataThread.Start(token);
                    this._parseDataThreads.Add(parseDataThread);
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        private void ProReceiveDataThreadMethod(CancellationToken token)
        {
            Stopwatch watch = new Stopwatch();
            ReceiveDatagramInfo receiveDatagramInfo;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    try
                    {
                        receiveDatagramInfo = this._waitParseDatas.Take(token);
                    }
                    catch (ArgumentNullException)
                    {
                        Loger.Error(".net平台库ArgumentNullException异常,忽略");
                        continue;
                    }
                    catch (OperationCanceledException)
                    {
                        continue;
                    }

                    watch.Restart();
                    this.ProReceiveData(receiveDatagramInfo);
                    watch.Stop();
                    if (watch.ElapsedMilliseconds > this._config.ProReceiveDataAddThreadTimeout)
                    {
                        this.AddParseDispoatchThread();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Error(ex, $"{Thread.CurrentThread.Name}发生异常");
                }
            }
        }

        /// <summary>
        /// 解析收到的数据队列线程方法
        /// </summary>
        /// <param name="receiveDatagramInfo">要解析的数据</param>
        private void ProReceiveData(ReceiveDatagramInfo receiveDatagramInfo)
        {
            try
            {
                var data = receiveDatagramInfo.Data;
                if (data == null)
                {
                    return;
                }

                if (data.Length < TransferConstant.COMMON_HEADER_SIZE)
                {
                    Loger.Warn($"收到的数据长度[{data.Length}]小于公共头[{TransferConstant.COMMON_HEADER_SIZE}],忽略...");
                    return;
                }

                var srcEndPoint = receiveDatagramInfo.SrcEndPoint;
                using (var ms = new MemoryStream(data))
                {
                    var br = new BinaryReader(ms);

                    #region 数据验证
                    Int32 sync = br.ReadInt32();// 同步字
                    if (sync != TransferConstant.SYNC)
                    {
                        throw new Exception($"收到数据的同步头:{sync}与期望的同步头:{TransferConstant.SYNC}不一致,忽略");
                    }

                    Int32 packageLen = br.ReadInt32();// 本次传输数据总长度
                    if (packageLen != data.Length)
                    {
                        throw new Exception($"收到数据的长度:{data.Length}与期望的数据长度:{packageLen}不一致,忽略");
                    }

                    var p = br.BaseStream.Position;
                    Int32 validCode = br.ReadInt32();//校验码,以后再验证

                    //重置校验码为填充值
                    byte[] validCodeBuf = BitConverter.GetBytes(TransferConstant.VALID_CODE_FILL);
                    br.BaseStream.Seek(p, SeekOrigin.Begin);
                    br.BaseStream.Write(validCodeBuf, 0, validCodeBuf.Length);

                    //校验数据传输正确性
                    bool parseResult = ProtocolParser.ValidData(validCode, data);
                    if (!parseResult)
                    {
                        Loger.Warn("数据正确性校验不通过");
                        return;
                    }
                    #endregion

                    var header = new CommonHeader(br);
                    switch (header.Cmd)
                    {
                        case TransferCommands.SendNotify:
                            this._receiver.ProSendNotify(new SendDataNotifyMessage(header, br, srcEndPoint));
                            break;
                        case TransferCommands.ResourceResponse:
                            this._receiver.ProResourceResponse(new ResourceResponseMessage(header, br, srcEndPoint), data);
                            break;
                        case TransferCommands.TransferCompletedAck:
                            this._receiver.ProTransferCompletedAck(new TransferCompletedAckMessage(header, br, srcEndPoint));
                            break;
                        case TransferCommands.ResourceRequest:
                            this.ProResourceRequest(new ResourceRequestMessage(header, br, srcEndPoint));
                            break;
                        case TransferCommands.TransferCompleted:
                            this.ProTransferCompleted(new TransferCompletedMessage(header, br, srcEndPoint));
                            break;
                        default:
                            throw new Exception($"未知命令{header.Cmd}");
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "解析收到的数据异常");
            }
        }

        private void ProTransferCompleted(TransferCompletedMessage message)
        {
            var rid = message.Header.Rid;
            string transferCompletedMessageCacheKey = CacheKeyGenerator.GenerateTransferCompletedMessageCacheKey(message);
            const int revTransferCompletedMessageCacheExpire = 60000;
            TransferSender sender;
            if (this._senderDic.TryGetValue(rid, out sender))
            {
                sender.ProResourceCompleted();
                MemoryCacheEx.Set(transferCompletedMessageCacheKey, rid, revTransferCompletedMessageCacheExpire);
                this.SendTransferCompletedAckMessage(message);
            }
            else
            {
                if (MemoryCacheEx.Get(transferCompletedMessageCacheKey) == null)
                {
                    //Loger.Warn($"未知的资源ID:{rid}");
                }
                else
                {
                    MemoryCacheEx.Set(transferCompletedMessageCacheKey, rid, revTransferCompletedMessageCacheExpire);
                    this.SendTransferCompletedAckMessage(message);
                }
            }
        }

        private void SendTransferCompletedAckMessage(TransferCompletedMessage message)
        {
            try
            {
                var transferCompletedAckMessage = new TransferCompletedAckMessage(message);
                byte[] buffer = transferCompletedAckMessage.GenerateBuffer();
                this._net.Send(buffer, message.SrcEndPoint);
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "发送TransferCompletedAckMessage异常");
            }
        }

        private void ProResourceRequest(ResourceRequestMessage message)
        {
            TransferSender sender;
            if (this._senderDic.TryGetValue(message.Header.Rid, out sender))
            {
                sender.ProResourceRequest(message);
            }
            else
            {
                //Loger.Warn($"未知的资源ID:{message.Header.Rid}");
            }
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="policy">发送策略</param>
        public void SendData(byte[] data, TransferPolicy policy)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                return;
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            using (var resoucesInfo = new ResoucesInfo(data, policy, 0, data.Length))
            {
                this.PrimitiveSend(resoucesInfo);
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
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                return;
            }

            if (postion < 0)
            {
                throw new ArgumentOutOfRangeException($"发送数据在总数据中的起始位置值:{postion}无效", nameof(postion));
            }

            if (length < 1)
            {
                throw new ArgumentOutOfRangeException($"要发送的数据长度值:{length}无效", nameof(length));
            }

            if (postion + length > data.Length)
            {
                throw new ArgumentOutOfRangeException($"要发送的数据长度值:{length}过大,超出数据[{postion}-{data.Length}]范围");
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            using (var resoucesInfo = new ResoucesInfo(data, policy, postion, length))
            {
                this.PrimitiveSend(resoucesInfo);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="filePath">要发送的文件路径</param>
        /// <param name="policy">发送数据在流中的起始位置</param>
        public void SendFile(string filePath, TransferPolicy policy)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("要发送的文件不存在", filePath);
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            using (var resoucesInfo = new ResoucesInfo(filePath, policy, 0, new FileInfo(filePath).Length))
            {
                this.PrimitiveSend(resoucesInfo);
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
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("要发送的文件不存在", filePath);
            }

            if (postion < 0)
            {
                throw new ArgumentOutOfRangeException($"发送数据在文件中的起始位置值:{postion}无效", nameof(postion));
            }

            if (length < 1)
            {
                throw new ArgumentOutOfRangeException($"要发送的数据长度值:{length}无效", nameof(length));
            }

            var totalLen = new FileInfo(filePath).Length;
            if (postion + length > totalLen)
            {
                throw new ArgumentOutOfRangeException($"要发送的数据长度值:{length}过大,超出文件[{postion}-{totalLen}]范围");
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            using (var resoucesInfo = new ResoucesInfo(filePath, policy, postion, length))
            {
                this.PrimitiveSend(resoucesInfo);
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
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("流不可读", nameof(stream));
            }

            if (postion < 0)
            {
                throw new ArgumentOutOfRangeException($"发送数据在流中的起始位置值:{postion}无效", nameof(postion));
            }

            if (length < 1)
            {
                throw new ArgumentOutOfRangeException($"要发送的数据长度值:{length}无效", nameof(length));
            }

            if (postion + length > stream.Length)
            {
                throw new ArgumentOutOfRangeException($"要发送的数据长度值:{length}过大,超出流[{postion}-{stream.Length}]范围");
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            using (var resoucesInfo = new ResoucesInfo(stream, policy, postion, length))
            {
                this.PrimitiveSend(resoucesInfo);
            }
        }

        private void PrimitiveSend(ResoucesInfo resoucesInfo)
        {
            //Loger.Info($"发送资源RID[{resoucesInfo.Rid}]数据开始");
            using (var sender = new TransferSender(this._net, resoucesInfo))
            {
                try
                {
                    if (!this._senderDic.TryAdd(resoucesInfo.Rid, sender))
                    {
                        throw new ApplicationException("this._senderDic.TryAdd失败");
                    }

                    sender.Send();
                }
                finally
                {
                    TransferSender sender2;
                    this._senderDic.TryRemove(resoucesInfo.Rid, out sender2);
                    //Loger.Info($"发送资源RID[{resoucesInfo.Rid}]数据完成");
                }
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
                this._net.Stop();
                this._parseDataThreadsCts.Cancel();
                this._parseDataThreadsCts.Dispose();

                this._net.Dispose();
                this._receiver.Dispose();
                foreach (var parseDataThread in this._parseDataThreads)
                {
                    parseDataThread.Stop();
                    parseDataThread.Dispose();
                }

                this._waitParseDatas.Dispose();
                foreach (var hostSender in _senderDic.Values)
                {
                    hostSender.Dispose();
                }

                _senderDic.Clear();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "释放资源异常");
            }
        }
    }
}
