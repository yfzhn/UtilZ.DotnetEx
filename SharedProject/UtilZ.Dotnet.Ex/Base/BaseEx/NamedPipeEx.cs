using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 命名管道扩展类
    /// </summary>
    public class NamedPipeEx : IDisposable
    {
        private readonly string _pipeName;
        private int _readDataMillisecondsTimeout;
        private readonly ThreadEx _listenThread;
        private Func<byte[], byte[]> _proFunc;

        /// <summary>
        /// 服务端构造函数
        /// </summary>
        /// <param name="pipeName">管道名称</param>
        /// <param name="proFunc">处理回调</param>
        /// <param name="readDataMillisecondsTimeout">读取数据超时时长,默认5000毫秒,小于等于-1,死等</param>
        public NamedPipeEx(string pipeName, Func<byte[], byte[]> proFunc, int readDataMillisecondsTimeout = 5000)
        {
            if (string.IsNullOrWhiteSpace(pipeName))
            {
                throw new ArgumentNullException(nameof(pipeName));
            }

            if (proFunc == null)
            {
                throw new ArgumentNullException(nameof(proFunc));
            }

            this._pipeName = pipeName;
            this._readDataMillisecondsTimeout = readDataMillisecondsTimeout;
            this._proFunc = proFunc;
            this._listenThread = new ThreadEx(ListenThreadMethod, $"管道{pipeName}监听线程", true);
            this._listenThread.Start();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (this._listenThread.IsRuning)
            {
                this._listenThread.Stop();
                using (NamedPipeClientStream pipeClientStream = new NamedPipeClientStream(this._pipeName))
                {
                    pipeClientStream.Connect();
                }
                this._proFunc = null;
                this._listenThread.Dispose();
            }
        }

        private void ListenThreadMethod(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        using (NamedPipeServerStream pipeServerStream = new NamedPipeServerStream(this._pipeName))
                        {
                            try
                            {
                                pipeServerStream.WaitForConnection();
                                if (token.IsCancellationRequested)
                                {
                                    pipeServerStream.Close();
                                    break;
                                }

                                byte[] data = ReadData(this._pipeName, pipeServerStream, this._readDataMillisecondsTimeout, token);
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }

                                //回调操作-各种处理
                                byte[] result = this._proFunc?.Invoke(data);
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }

                                WriteData(pipeServerStream, result);
                            }
                            catch (TimeoutException)
                            {
                                //请求方未知原因未即时发送数据导致读取数据超时,多半是请求方关闭
                                Loger.Debug("TimeoutException");
                            }
                            catch (IOException)
                            {
                                //请求方已关闭
                                Loger.Debug("IOException");
                            }
                            catch (Exception exi)
                            {
                                Loger.Warn(exi);
                            }
                        }
                    }
#if NET4_0
                    catch (IOException ioex) when (ioex.Message.Contains("-2147024665"))
#else
                    catch (IOException ioex) when (ioex.HResult == -2147024665)
#endif
                    {
                        Loger.Debug(ioex);
                        //所有的管道范例都在使用中,等待一段时间重试
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }





        private static void WriteData(PipeStream pipeStream, byte[] data)
        {
            int length = 0;
            if (data != null)
            {
                length = data.Length;
            }

            byte[] dataSize = BitConverter.GetBytes(length);//数据长度
            pipeStream.Write(dataSize, 0, dataSize.Length);//先发送数据长度
            if (data != null)
            {
                pipeStream.Write(data, 0, data.Length);//发送数据
            }
        }

        private static byte[] ReadData(string pipeName, PipeStream pipeStream, int millisecondsTimeout, CancellationToken? cancellationToken)
        {
            byte[] dataSize = new byte[4];

            //读取响应数据长度
            bool timeout = PrimitiveReadData(dataSize, pipeStream, millisecondsTimeout, cancellationToken);
            if (timeout)
            {
                throw new TimeoutException($"命名管道{pipeName}读取数据超时");
            }

            int length = BitConverter.ToInt32(dataSize, 0);//响应数据长度
            if (length <= 0)
            {
                return null;
            }

            //读取响应数据
            byte[] data = new byte[length];
            timeout = PrimitiveReadData(data, pipeStream, millisecondsTimeout, cancellationToken);
            if (timeout)
            {
                throw new TimeoutException($"命名管道{pipeName}读取数据超时");
            }

            return data;
        }

#if NET4_0
        private static bool PrimitiveReadData(byte[] buffer, PipeStream pipeStream, int millisecondsTimeout, CancellationToken? cancellationToken)
        {
            //读取响应数据长度
            IAsyncResult asyncResult = pipeStream.BeginRead(buffer, 0, buffer.Length, null, null);
            //var readDataLengthTask = pipeStream.ReadAsync(buffer, 0, buffer.Length);
            if (millisecondsTimeout >= Timeout.Infinite)
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(millisecondsTimeout))
                {
                    return true;
                }
            }

            return false;
        }
#else
        private static bool PrimitiveReadData(byte[] buffer, PipeStream pipeStream, int millisecondsTimeout, CancellationToken? cancellationToken)
        {
            //读取响应数据长度
            var readDataLengthTask = pipeStream.ReadAsync(buffer, 0, buffer.Length);
            if (millisecondsTimeout >= Timeout.Infinite)
            {
                if (cancellationToken.HasValue)
                {
                    if (!readDataLengthTask.Wait(millisecondsTimeout, cancellationToken.Value))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!readDataLengthTask.Wait(millisecondsTimeout))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (cancellationToken.HasValue)
                {
                    readDataLengthTask.Wait(cancellationToken.Value);
                }
                else
                {
                    readDataLengthTask.Wait();
                }
            }

            return false;
        }
#endif



        /// <summary>
        /// 请求数据
        /// 异常:IOException,TimeoutException
        /// </summary>
        /// <param name="pipeName">管道名称</param>
        /// <param name="data">请求数据</param>
        /// <param name="millisecondsTimeout">超时时长,小于等于-1,死等,默认为Timeout.Infinite</param>
        /// <param name="cancellationToken">取消通知</param>
        /// <returns>响应结果</returns>
        public static byte[] Request(string pipeName, byte[] data, int millisecondsTimeout = Timeout.Infinite, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(pipeName))
            {
                throw new ArgumentNullException(nameof(pipeName));
            }

            using (NamedPipeClientStream pipeClientStream = new NamedPipeClientStream(pipeName))
            {
                pipeClientStream.Connect();
                WriteData(pipeClientStream, data);
                return ReadData(pipeName, pipeClientStream, millisecondsTimeout, cancellationToken);
            }
        }
    }
}
