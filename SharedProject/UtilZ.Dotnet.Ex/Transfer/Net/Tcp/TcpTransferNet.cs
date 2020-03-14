using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// Tcp传输类
    /// </summary>
    public class TcpTransferNet : ITransferNet
    {
        private NetConfig _config;
        private Socket _listenSocket = null;
        private Thread _listenThread = null;

        private bool _isDisposed = false;
        private readonly object _isDisposedLock = new object();

        private readonly List<TcpTransferChannel> _waitAckTcpTransferChannelList = new List<TcpTransferChannel>();
        private readonly object _waitAckTcpTransferChannelListLock = new object();

        private Action<ReceiveDatagramInfo> _rev;

        private readonly Hashtable _htTcpTransferChannel = Hashtable.Synchronized(new Hashtable());

        private void DisposeSocket(Socket socket)
        {
            try
            {
                if (socket == null)
                {
                    return;
                }

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "DisposeSocket异常");
            }
        }

        private string GetId(IPEndPoint endPoint)
        {
            return $"{endPoint.Address.ToString()}:{endPoint.Port}";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TcpTransferNet()
        {

        }


        private bool _status = false;

        /// <summary>
        /// 状态[true:正常false:停止]
        /// </summary>
        public bool Status
        {
            get { return this._status; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">传输配置</param>
        /// <param name="rev">接收数据回调</param>
        public void Init(NetConfig config, Action<ReceiveDatagramInfo> rev)
        {
            this._status = true;
            this._config = config;
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(config.ListenEP);
                this._listenSocket = listenSocket;
                this._listenThread = new Thread(this.AcceptClient);
                this._listenThread.Name = "Tcp监听线程";
                //this._listenThread.IsBackground = true;
                this._rev = rev;
            }
            catch (Exception)
            {
                this._listenSocket = null;
                this.DisposeSocket(listenSocket);
                throw;
            }
        }

        private void AcceptClient(object obj)
        {
            try
            {
                var listenSocket = (Socket)obj;
                listenSocket.Listen(int.MaxValue);
                while (!this._isDisposed)
                {
                    try
                    {
                        var client = listenSocket.Accept();
                        lock (this._isDisposedLock)
                        {
                            if (this._isDisposed)
                            {
                                this.DisposeSocket(client);
                                break;
                            }

                            lock (this._waitAckTcpTransferChannelListLock)
                            {
                                var tcpTransferChannel = new TcpTransferChannel(client, this.ClientDisconnectNotify, this._rev, this.IdAck);
                                this._waitAckTcpTransferChannelList.Add(tcpTransferChannel);
                                tcpTransferChannel.Start();
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (SocketException se)
                    {
                        if (se.ErrorCode == 10004)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Loger.Error(ex, "listenSocket.Accept异常");
                    }
                }
            }
            catch (ObjectDisposedException)
            { }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void IdAck(TcpTransferChannel tcpTransferChannel)
        {
            lock (this._waitAckTcpTransferChannelListLock)
            {
                this._waitAckTcpTransferChannelList.Remove(tcpTransferChannel);
            }

            lock (_htTcpTransferChannel.SyncRoot)
            {
                var id = this.GetId(tcpTransferChannel.IpEndPoint);
                if (_htTcpTransferChannel.ContainsKey(id))
                {
                    ((TcpTransferChannel)_htTcpTransferChannel[id]).Dispose();
                }

                _htTcpTransferChannel[id] = tcpTransferChannel;
            }
        }

        private void ClientDisconnectNotify(TcpTransferChannel tcpTransferChannel)
        {
            lock (_htTcpTransferChannel.SyncRoot)
            {
                var id = this.GetId(tcpTransferChannel.IpEndPoint);
                if (_htTcpTransferChannel.ContainsKey(id))
                {
                    ((TcpTransferChannel)_htTcpTransferChannel[id]).Dispose();
                    _htTcpTransferChannel.Remove(id);
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
                var id = this.GetId(remoteEP);
                var tcpTransferChannel = this._htTcpTransferChannel[id] as TcpTransferChannel;
                if (tcpTransferChannel == null)
                {
                    lock (this._htTcpTransferChannel.SyncRoot)
                    {
                        tcpTransferChannel = this._htTcpTransferChannel[id] as TcpTransferChannel;
                        if (tcpTransferChannel == null)
                        {
                            tcpTransferChannel = this.ConnetDst(remoteEP, id);
                            this._htTcpTransferChannel[id] = tcpTransferChannel;
                        }
                    }
                }

                tcpTransferChannel.SendData(data);
            }
            catch (Exception ex)
            {
                throw new SendDataException($"发送数据到{remoteEP.ToString()}异常", ex);
            }
        }

        private TcpTransferChannel ConnetDst(IPEndPoint remoteEP, string id)
        {
            TcpTransferChannel tcpTransferChannel;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(remoteEP);
                tcpTransferChannel = new TcpTransferChannel(client, remoteEP, this.ClientDisconnectNotify, this._rev);
                tcpTransferChannel.Start();

                var buffer = new byte[8];
                var listenEP = this._config.ListenEP;
                byte[] ipBytes = listenEP.Address.GetAddressBytes();

                using (var ms = new MemoryStream(buffer))
                {
                    var bw = new BinaryWriter(ms);
                    bw.Write(ipBytes[0]);
                    bw.Write(ipBytes[1]);
                    bw.Write(ipBytes[2]);
                    bw.Write(ipBytes[3]);
                    bw.Write(listenEP.Port);
                    bw.Flush();
                }

                tcpTransferChannel.SendData(buffer);
            }
            catch (Exception)
            {
                client.Close();
                client.Dispose();
                throw;
            }

            return tcpTransferChannel;
        }

        /// <summary>
        /// 启动接收
        /// </summary>
        public void Start()
        {
            this._listenThread.Start(this._listenSocket);
        }

        /// <summary>
        /// 停止接收
        /// </summary>
        public void Stop()
        {
            this._listenSocket.Close();
            this._listenSocket.Dispose();
            //this._listenThread.Abort();
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            try
            {
                lock (this._isDisposedLock)
                {
                    this._isDisposed = true;
                    this._listenSocket.Close();
                    this._listenSocket.Dispose();
                    foreach (TcpTransferChannel tcpTransferChannel in this._htTcpTransferChannel.Values)
                    {
                        tcpTransferChannel.Dispose();
                    }

                    this._htTcpTransferChannel.Clear();
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
