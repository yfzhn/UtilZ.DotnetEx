using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TcpTransferChannel : IDisposable
    {
        private const int _TLV_HEAD_LEN = 8;
        private const int _HEAD_PACKGE_TAG = 1000;

        private bool _isRun = true;
        private bool _isAck = false;
        private IPEndPoint _ipEndPoint;
        public IPEndPoint IpEndPoint
        {
            get { return _ipEndPoint; }
        }

        private readonly Socket _client;
        private readonly Thread _socketRevDataThread;
        private readonly Action<ReceiveDatagramInfo> _rev;
        private readonly Action<TcpTransferChannel> _clientDisconnectNotify;
        private readonly Action<TcpTransferChannel> _idAck;
        private readonly BlockingCollection<byte[]> _parseDatas = new BlockingCollection<byte[]>();
        private readonly ThreadEx _parseDataThread;



        private TcpTransferChannel(Socket client,
            Action<TcpTransferChannel> clientDisconnectNotify,
            Action<ReceiveDatagramInfo> rev)
        {
            this._client = client;
            this._clientDisconnectNotify = clientDisconnectNotify;
            this._rev = rev;

            this._parseDataThread = new ThreadEx(this.ParseData, "数据解析线程", true);
            this._socketRevDataThread = new Thread(this.RevDataThreadMethod);
            this._socketRevDataThread.Name = "接收数据线程";
            this._socketRevDataThread.IsBackground = true;
        }

        /// <summary>
        /// 监听连接构造函数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientDisconnectNotify"></param>
        /// <param name="rev"></param>
        /// <param name="idAck"></param>
        public TcpTransferChannel(Socket client,
            Action<TcpTransferChannel> clientDisconnectNotify,
            Action<ReceiveDatagramInfo> rev,
            Action<TcpTransferChannel> idAck)
            : this(client, clientDisconnectNotify, rev)
        {
            this._idAck = idAck;
        }

        /// <summary>
        /// 主动建立连接构造函数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="remoteEP"></param>
        /// <param name="clientDisconnectNotify"></param>
        /// <param name="rev"></param>
        public TcpTransferChannel(Socket client,
            IPEndPoint remoteEP,
            Action<TcpTransferChannel> clientDisconnectNotify,
            Action<ReceiveDatagramInfo> rev)
             : this(client, clientDisconnectNotify, rev)
        {
            this._ipEndPoint = remoteEP;
            this._isAck = true;
        }

        public void Start()
        {
            this._parseDataThread.Start();
            this._socketRevDataThread.Start(this._client);
        }

        private void ParseData(ThreadExPara para)
        {
            const int millisecondsTimeout = 500;
            byte[] parsePackgeBuffer = new byte[TransferConstant.MTU_MAX * 3];

            using (var ms = new MemoryStream(parsePackgeBuffer))
            {
                var br = new BinaryReader(ms);
                int writePostion = 0;
                int parsePostion = 0;
                byte[] buffer;

                while (!para.Token.IsCancellationRequested)
                {
                    try
                    {
                        try
                        {
                            if (!this._parseDatas.TryTake(out buffer, millisecondsTimeout, para.Token))
                            {
                                continue;
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            continue;
                        }
                        catch (OperationCanceledException)
                        {
                            continue;
                        }
                        catch (ObjectDisposedException)
                        {
                            continue;
                        }
                        catch (InvalidOperationException)
                        {
                            continue;
                        }

                        //合并数据
                        this.MergeData(parsePackgeBuffer, ref writePostion, ref parsePostion, buffer);

                        //数据拆包
                        this.SplitPackge(writePostion, ref parsePostion, br, parsePackgeBuffer);
                    }
                    catch (Exception ex)
                    {
                        Loger.Error(ex, "tlv拆包异常");
                    }
                }
            }
        }

        private void MergeData(byte[] parsePackgeBuffer, ref int writePostion, ref int parsePostion, byte[] buffer)
        {
            if (parsePackgeBuffer.Length - writePostion < buffer.Length)
            {
                if (writePostion > parsePostion)
                {
                    //将羊粘包的未解析数据拷贝到缓冲起始位置
                    Array.Copy(parsePackgeBuffer, parsePostion, parsePackgeBuffer, 0, writePostion - parsePostion);
                }

                writePostion = 0;
                parsePostion = 0;
            }

            Array.Copy(buffer, 0, parsePackgeBuffer, writePostion, buffer.Length);
            writePostion += buffer.Length;
        }

        private void SplitPackge(int writePostion, ref int parsePostion, BinaryReader br, byte[] parsePackgeBuffer)
        {
            if (parsePostion < 0)
            {
                parsePostion = this.FindTlvFirstTagPostion(parsePackgeBuffer, 0, writePostion);
                if (parsePostion < 0)
                {
                    return;
                }
            }

            int tag, dataLen;
            while (writePostion - parsePostion > _TLV_HEAD_LEN)
            {
                br.BaseStream.Seek(parsePostion, SeekOrigin.Begin);
                tag = br.ReadInt32();
                if (tag == _HEAD_PACKGE_TAG)
                {
                    //心跳包
                    parsePostion += _TLV_HEAD_LEN;
                    continue;
                }

                if (tag != TransferConstant.SYNC)
                {
                    //重新查找到SLV头
                    parsePostion = this.FindTlvFirstTagPostion(parsePackgeBuffer, parsePostion, writePostion - parsePostion);
                    if (parsePostion < 0)
                    {
                        return;
                    }

                    br.BaseStream.Seek(parsePostion, SeekOrigin.Begin);
                    tag = br.ReadInt32();
                }

                dataLen = br.ReadInt32();
                dataLen = dataLen - _TLV_HEAD_LEN;
                if (writePostion - parsePostion < dataLen)
                {
                    break;
                }

                var data = new byte[dataLen];
                Array.Copy(parsePackgeBuffer, parsePostion + _TLV_HEAD_LEN, data, 0, dataLen);
                parsePostion = (int)br.BaseStream.Position + dataLen;

                if (this._isAck)
                {
                    //输出
                    this._rev(new ReceiveDatagramInfo(data, this._ipEndPoint));
                }
                else
                {
                    this.ParseAckId(data);
                }
            }
        }

        /// <summary>
        /// 找到返回亲自tag位置,失败返回-1
        /// </summary>
        /// <param name="parsePackgeBuffer"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private int FindTlvFirstTagPostion(byte[] parsePackgeBuffer, int startIndex, int length)
        {
            const int FAIL = -1;
            int tag, tag2, dataLen, tagIndex;
            int endIndex = startIndex + length - 4;

            for (int i = 0; i < endIndex; i++)
            {
                tag = BitConverter.ToInt32(parsePackgeBuffer, i);
                if (tag == TransferConstant.SYNC)
                {
                    dataLen = BitConverter.ToInt32(parsePackgeBuffer, i + 4);
                }
                else if (tag == _HEAD_PACKGE_TAG)
                {
                    dataLen = 0;
                }
                else
                {
                    continue;
                }

                tagIndex = i + _TLV_HEAD_LEN + dataLen;
                if (tagIndex <= endIndex)
                {
                    tag2 = BitConverter.ToInt32(parsePackgeBuffer, i + _TLV_HEAD_LEN + dataLen);
                    if (tag == TransferConstant.SYNC || tag == _HEAD_PACKGE_TAG)
                    {
                        return i;
                    }
                }
            }

            return FAIL;
        }

        private void ParseAckId(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var br = new BinaryReader(ms);
                var seg1 = br.ReadByte();
                var seg2 = br.ReadByte();
                var seg3 = br.ReadByte();
                var seg4 = br.ReadByte();
                var port = br.ReadUInt16();

                var ipStr = $"{seg1}.{seg2}.{seg3}.{seg4}";
                this._ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
                this._idAck(this);
            }

            this._isAck = true;
        }


        private void RevDataThreadMethod(object obj)
        {
            try
            {
                Socket client = (Socket)obj;
                byte[] buffer = new byte[TransferConstant.MTU_MAX * 2];
                int len;
                //todo socket断开异常...处理

                while (this._isRun)
                {
                    try
                    {
                        len = client.Receive(buffer);
                        if (len > 0)
                        {
                            var data = new byte[len];
                            Array.Copy(buffer, data, len);
                            this._parseDatas.Add(data);
                        }
                        else
                        {
                            //心跳包
                            //构建tlv
                            byte[] headBuffer = this.BuildTLV(new byte[] { }, _HEAD_PACKGE_TAG);

                            //发送
                            this.PrimitiveSendData(buffer);
                        }
                    }
                    catch (SocketException sex)
                    {
                        if (sex.ErrorCode != 10054)
                        {
                            Loger.Error(sex, "接收数据异常SocketException");
                        }

                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Loger.Error(ex, "接收数据异常");
                    }
                }
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        internal void SendData(byte[] data)
        {
            //构建tlv
            byte[] buffer = this.BuildTLV(data, TransferConstant.SYNC);

            //发送
            this.PrimitiveSendData(buffer);
        }

        private void PrimitiveSendData(byte[] data)
        {
            //发送
            int length = data.Length;
            int sendOffset = 0;
            while (sendOffset < length)
            {
                sendOffset += this._client.Send(data, sendOffset, length - sendOffset, SocketFlags.None);
            }
        }

        private byte[] BuildTLV(byte[] data, int tag)
        {
            //构建tlv
            byte[] buffer = new byte[data.Length + _TLV_HEAD_LEN];
            using (var ms = new MemoryStream(buffer))
            {
                var bw = new BinaryWriter(ms);
                bw.Write(tag);
                bw.Write(buffer.Length);
                bw.Write(data, 0, data.Length);
                bw.Flush();
            }

            return buffer;
        }

        public void Dispose()
        {
            try
            {
                this._client.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
