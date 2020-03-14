using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// UDP传输类
    /// </summary>
    public class UdpTransferNet : ITransferNet
    {
        private Socket _socket = null;
        private EndPoint _listenEP;
        private Action<ReceiveDatagramInfo> _rev;
        private ThreadEx _receiveDataThread = null;
        private bool _status = false;

        /// <summary>
        /// 状态[true:正常false:停止]
        /// </summary>
        public bool Status
        {
            get { return this._status; }
        }

        private const int _REV_BUFFER_SIZE = 1024 * 10240;
        private readonly byte[] _revBuffer = new byte[_REV_BUFFER_SIZE];
        private int _revBufferParseOffset = 0;
        //private readonly BlockingCollection<RevDataInfo> _revDataPositions = new BlockingCollection<RevDataInfo>(new ConcurrentQueue<RevDataInfo>());
        private readonly BlockingCollection<RevDataInfo> _revDataPositions = new BlockingCollection<RevDataInfo>();
        private readonly List<ThreadEx> _copyRevDataThreads = new List<ThreadEx>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public UdpTransferNet()
        {
            //经过实验得到数据,4个线程比较合适,如果还不够用,则出线问题时会自动增加一个线程
            for (int i = 0; i < 4; i++)
            {
                this.AddCopyRevDataThread(false);
            }
        }

        private void AddCopyRevDataThread(bool isStart)
        {
            var copyRevDataThread = new ThreadEx(this.CopyRevDataThreadMethod, $"拷贝数据线程[{this._copyRevDataThreads.Count}]", true);
            if (isStart)
            {
                Loger.Warn("添加一个用于拷贝接收到数据的线程[接收数据不及时,可能有数据被覆盖]");
                copyRevDataThread.Start();
            }

            this._copyRevDataThreads.Add(copyRevDataThread);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">传输配置</param>
        /// <param name="rev">接收数据回调</param>
        public void Init(NetConfig config, Action<ReceiveDatagramInfo> rev)
        {
            if (rev == null)
            {
                throw new ArgumentNullException(nameof(rev));
            }

            this._listenEP = config.ListenEP;
            this._rev = rev;
            this._status = true;

            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this._socket.ReceiveBufferSize = config.ReceiveBufferSize;
            this._socket.SendBufferSize = config.SendBufferSize;
            this._socket.Bind(this._listenEP);

            //如果没有这两行代码,则windows底层BUG会在接收抛出异常,并将socket重置,然后再也收不到数据
            const int SIO_UDP_CONNRESET = -1744830452;
            this._socket.IOControl(SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);

            //初始化接收线程
            this._receiveDataThread = new ThreadEx(this.ReceiveDtaThreadMethod, "udp接收数据线程", true);
        }

        /// <summary>
        /// 启动接收
        /// </summary>
        public void Start()
        {
            if (this._receiveDataThread == null)
            {
                throw new InvalidOperationException("请先初始化");
            }

            this._copyRevDataThreads.ForEach(item => { item.Start(); });
            this._receiveDataThread.Start();
        }

        /// <summary>
        /// 停止接收
        /// </summary>
        public void Stop()
        {
            if (this._receiveDataThread == null)
            {
                return;
            }

            this._receiveDataThread.Abort();
            this._receiveDataThread = null;
        }

        private void CopyRevDataThreadMethod(CancellationToken token)
        {
            RevDataInfo revDataPosition;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    revDataPosition = this._revDataPositions.Take(token);
                    this._revBufferParseOffset = revDataPosition.Offset;

                    byte[] buffer = new byte[revDataPosition.Length];
                    Array.Copy(this._revBuffer, revDataPosition.Offset, buffer, 0, revDataPosition.Length);
                    this._rev(new ReceiveDatagramInfo(buffer, revDataPosition.RemoteEP));
                }
                catch (ArgumentNullException)
                {
                    //.net bug
                    continue;
                }
                catch (OperationCanceledException)
                { }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Loger.Error(ex, "ProRevRevDataPosition异常");
                }
            }
        }

        private void ReceiveDtaThreadMethod(CancellationToken token)
        {
            int bufLen = TransferConstant.MTU_MAX + 128;//原本+28(IP头20+udp头8)即可,为了靠谱点,大一点无所谓
            byte[] buffer = new byte[bufLen];
            int revBufferOffset = 0;
            int revLen;
            EndPoint remoteEP = new IPEndPoint(1, 1);
            IPEndPoint tmpEndPoint;

            while (this._status)
            {
                try
                {
                    revLen = this._socket.ReceiveFrom(this._revBuffer, revBufferOffset, bufLen, SocketFlags.None, ref remoteEP);
                    if (revLen <= 0)
                    {
                        continue;
                    }

                    if (revBufferOffset >= _revBufferParseOffset)
                    {
                        if ((_REV_BUFFER_SIZE - this._revBufferParseOffset) < bufLen)
                        {
                            this.AddCopyRevDataThread(true);
                        }
                    }
                    else
                    {
                        if ((this._revBufferParseOffset - revBufferOffset) < bufLen)
                        {
                            this.AddCopyRevDataThread(true);
                        }
                    }


                    //if ((_REV_BUFFER_SIZE - this._revBufferParseOffset) < bufLen)
                    //{
                    //    Loger.Warn("接收数据不及时,可能有数据被覆盖,多开一个线程用于拷贝接收到的数据");
                    //    this.AddCopyRevDataThread(true);
                    //}

                    tmpEndPoint = (IPEndPoint)remoteEP;
                    this._revDataPositions.Add(new RevDataInfo(revBufferOffset, revLen, new IPEndPoint(new IPAddress(tmpEndPoint.Address.GetAddressBytes()), tmpEndPoint.Port)));
                    revBufferOffset += revLen;
                    if (_REV_BUFFER_SIZE - revBufferOffset < bufLen)
                    {
                        revBufferOffset = 0;
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException se)
                {
                    //if (se.ErrorCode == 10054)
                    //{
                    //    //windows底层的BUG,UDP本无连接,也不管是否到达,但是windows却诡异的报了这个BUG
                    //    //网上有尝试其它方法,在初始化socket时设置socket内部控制行为,不晓得靠谱否,如果不靠谱则重新初始化
                    //    this.InitSocket();
                    //}
                    //else
                    //{
                    //    Loger.Error(se, "udp接收数据发生异常");
                    //}

                    Loger.Error(se, "udp接收数据发生异常");
                }
                catch (Exception ex)
                {
                    Loger.Error(ex, "udp接收数据发生异常");
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="remoteEP">接收数据EndPoint</param>
        public void Send(byte[] data, IPEndPoint remoteEP)
        {
            try
            {
                int sendLen = 0;
                do
                {
                    sendLen = this._socket.SendTo(data, remoteEP);
                    if (sendLen < data.Length)
                    {
                        data = data.Skip(sendLen).ToArray();
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }
            catch (ObjectDisposedException)
            {
                //资源释放
            }
            catch (Exception ex)
            {
                if (this._status)
                {
                    throw new SendDataException($"发送数据到{remoteEP.ToString()}异常", ex);
                }
                else
                {
                    //资源释放
                }
            }
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            try
            {
                this._status = false;
                if (this._receiveDataThread != null)
                {
                    this._receiveDataThread.Abort();
                    this._receiveDataThread.Dispose();
                }

                this._socket.Dispose();
                this._revDataPositions.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "Dispose异常");
            }
        }
    }
}
