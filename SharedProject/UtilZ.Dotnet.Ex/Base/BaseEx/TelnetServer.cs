using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Telnet Server
    /// </summary>
    public class TelnetServer : IDisposable
    {
        private readonly TelnetAuthInfo _authInfo;
        private readonly Func<string, string> _proCallback;
        //private readonly Socket _listenerSocket;
        private readonly TcpListener _tcpListener;
        private readonly ThreadEx _thread;
        private readonly string _serviceName;
        private readonly List<TelnetClient> _clients = new List<TelnetClient>();
        private readonly object _clientsLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="proCallback">接收命令回调</param>
        /// <param name="serviceName">回显名称</param>
        /// <param name="authInfo">登录认证信息</param>
        /// <param name="ip">监听IP</param>
        /// <param name="port">监听端口</param>
        /// <param name="backlog">客户最大连接数</param>
        public TelnetServer(Func<string, string> proCallback, string serviceName, TelnetAuthInfo authInfo = null, IPAddress ip = null, int port = 64000, int backlog = 3)
        {
            if (authInfo == null)
            {
                authInfo = new TelnetAuthInfo();
            }

            if (ip == null)
            {
                ip = IPAddress.Parse("0.0.0.0");
            }

            this._tcpListener = new TcpListener(ip, port);
            this._tcpListener.Start(backlog);

            //this._listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //this._listenerSocket.Bind(new IPEndPoint(ip, port));
            //if (backlog < 1)
            //{
            //    backlog = 1;
            //}

            //this._listenerSocket.Listen(backlog);
            this._serviceName = serviceName;
            this._authInfo = authInfo;
            this._proCallback = proCallback;
            this._thread = new ThreadEx(this.StartListen, "TelnetServer监听线程", true);
        }

        private void StartListen(ThreadExPara para)
        {
            Socket client;
            while (!para.Token.IsCancellationRequested)
            {
                //client = this._listenerSocket.Accept();
                client = this._tcpListener.AcceptSocket();
                lock (this._clientsLock)
                {
                    this._clients.Add(new TelnetClient(this._authInfo, this._serviceName, client, this._proCallback, this.ClientClose));
                }
            }
        }

        private void ClientClose(TelnetClient client)
        {
            client.Dispose();
            lock (this._clientsLock)
            {
                this._clients.Remove(client);
            }
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            this._thread.Start();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {
            this._thread.Stop();
            this._thread.Dispose();
        }
    }

    internal class TelnetClient : IDisposable
    {
        private readonly string _serviceName;
        private readonly TelnetAuthInfo _authInfo;
        private readonly Socket _client;
        private readonly Func<string, string> _proCallback;
        private readonly ThreadEx _receiveThread;
        private readonly AsynQueue<byte[]> _receiveQueue;
        private List<byte> _receiveData = new List<byte>();
        private readonly byte[] _target;
        private readonly Action<TelnetClient> _clientClose;
        private readonly string _pre = "> ";
        private readonly string _newLine = "\r\n";
        private bool _lastPre = false;
        private bool _isLogined;

        public TelnetClient(TelnetAuthInfo authInfo, string serviceName, Socket client, Func<string, string> proCallback, Action<TelnetClient> clientClose)
        {
            this._authInfo = authInfo;
            this._isLogined = !authInfo.IsAuth;
            this._serviceName = serviceName;
            this._client = client;
            this._proCallback = proCallback;
            this._clientClose = clientClose;
            this._target = Encoding.Default.GetBytes(_newLine);
            this._receiveThread = new ThreadEx(this.RecieveMethod, "", true);
            this._receiveQueue = new AsynQueue<byte[]>(this.ProThreadMethod, "", true, true);
            this._receiveThread.Start();
        }

        private void ProThreadMethod(byte[] data)
        {
            this._receiveData.AddRange(data);
            int index;
            string recStr, retStr;
            int count;
            string newLine = "\n";
            while (true)
            {
                try
                {
                    recStr = Encoding.Default.GetString(this._receiveData.ToArray());

                    index = recStr.IndexOf(newLine);
                    if (index < 0)
                    {
                        break;
                    }

                    recStr = recStr.Substring(0, index + newLine.Length);
                    count = Encoding.Default.GetByteCount(recStr);
                    this._receiveData.RemoveRange(0, count);

                    recStr = recStr.Replace("\n", string.Empty);
                    recStr = recStr.Replace("\r", string.Empty);
                    if (string.IsNullOrEmpty(recStr))
                    {
                        // retStr = this.Format(string.Empty);
                    }
                    else
                    {
                        if (this._isLogined)
                        {
                            retStr = this.OnRaiseProCallback(recStr);
                        }
                        else
                        {
                            retStr = this.Login(recStr);
                        }

                        this.SendData(retStr);
                        //retStr = this.Format(retStr);
                    }

                    //this.SendData(retStr);
                }
                catch (SocketException)
                {
                    this.ClientClose();
                    break;
                }
            }
        }

        private string Login(string authInfo)
        {
            string[] strs = authInfo.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 2)
            {
                return "登录信息不合法";
            }

            if (!string.Equals(strs[0], this._authInfo.Username))
            {
                return "用户名不正确";
            }

            if (!string.Equals(strs[1], this._authInfo.Password))
            {
                return "密码不正确";
            }

            this._isLogined = true;
            return "登录成功";
        }

        private string Format(string str)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(str))
            {
                if (this._lastPre)
                {
                    sb.Append(Environment.NewLine);
                }
                else
                {
                    this._lastPre = true;
                }

                sb.Append(_pre);
            }
            else
            {
                this._lastPre = false;
                if (str != null)
                {
                    string line;
                    using (var sr = new System.IO.StringReader(str))
                    {
                        line = sr.ReadLine();
                        while (line != null)
                        {
                            sb.Append(_pre);
                            sb.AppendLine(line);
                            line = sr.ReadLine();
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private string OnRaiseProCallback(string cmd)
        {
            var handler = this._proCallback;
            string resultStr;
            if (handler != null)
            {
                resultStr = handler(cmd);
                if (string.IsNullOrWhiteSpace(resultStr))
                {
                    resultStr = Environment.NewLine;
                }
                else
                {
                    if (!resultStr.EndsWith(Environment.NewLine))
                    {
                        resultStr += Environment.NewLine;
                    }
                }
            }
            else
            {
                resultStr = string.Empty;
            }

            return resultStr;
        }

        private void RecieveMethod(ThreadExPara para)
        {
            byte[] buffer = new byte[4096];
            int recCount;
            this.SendWelcom();
            while (!para.Token.IsCancellationRequested)
            {
                try
                {
                    recCount = this._client.Receive(buffer, buffer.Length, SocketFlags.None);
                    if (recCount == 0)
                    {
                        continue;
                    }

                    if (recCount < buffer.Length)
                    {
                        buffer = buffer.Take(recCount).ToArray();
                    }

                    this._receiveQueue.Enqueue(buffer);
                }
                catch (SocketException)
                {
                    this.ClientClose();
                    break;
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
        }

        private void SendWelcom()
        {
            string welcomStr = this._serviceName;
            if (string.IsNullOrWhiteSpace(welcomStr))
            {
                welcomStr = @"
                          _ooOoo_                               
                         o8888888o                              
                         88"". ""88                              
                        (| ^ _ ^ |)
                         O\  =  / O
                      ____ /`---'\____                           
                     .'  \\|     |//  `.                         
                    /  \\|||  :  |||//  \                        

                   / _ ||||| -:- ||||| -  \                       
                  |   | \\\  -  /// |   |                       
                  | \_ | ''\---/ '' |   |
                  \  .-\__  `-`  ___ / -. /
                ___`. .'  /--.--\  `. . ___                     
              ."""" '<  `.___\_<|>_/___.' > '"""".                  
            | | :  `- \`.;`\ _ /`;.`/ - ` : | |
            \  \ `-.   \_ __\ / __ _ /   .-` /  /
  
        ========`-.____`-.___\_____ / ___.-`____.- '========         
                           `= ---= '                              
      ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         佛祖保佑       永无BUG       永不修改

{0}
请登录:";
            }
            else
            {
                welcomStr = welcomStr + Environment.NewLine + "请登录:";
            }


            //welcomStr = this.Format(welcomStr);
            this.SendData(welcomStr);
        }

        private void SendData(string str)
        {
            if (!str.EndsWith(_newLine))
            {
                str = str + _newLine;
            }

            byte[] data = Encoding.Default.GetBytes(str);
            int count, offset = 0;
            while (offset < data.Length)
            {
                count = this._client.Send(data, offset, data.Length - offset, SocketFlags.None);
                offset += count;
            }
        }

        private void ClientClose()
        {
            this._clientClose(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {
            this._receiveThread.Stop();
            this._receiveThread.Dispose();
            this._receiveQueue.Dispose();
        }
    }

    /// <summary>
    /// telnet认证信息
    /// </summary>
    public class TelnetAuthInfo
    {
        private bool _isAuth;
        /// <summary>
        /// 是否需要登录认证[true:需要登录认证;false:不需要登录认证]
        /// </summary>
        public bool IsAuth { get { return _isAuth; } }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TelnetAuthInfo()
        {
            this._isAuth = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public TelnetAuthInfo(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("username");
            }

            string space = " ";
            if (username.Contains(space))
            {
                throw new ArgumentException("用户名不能包含空格");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password");
            }

            if (password.Contains(space))
            {
                throw new ArgumentException("密码不能包含空格");
            }

            this._isAuth = true;
            this.Username = username;
            this.Password = password;
        }
    }
}
