using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// IP类型扩展方法类
    /// </summary>
    public static class IPAddressEx
    {
        /// <summary>
        /// 验证IP是否是一个IPV4地址[是合法的ipv4地址返回true,否则返回flase]
        /// </summary>
        /// <param name="ipStr">ipv4字符串</param>
        /// <returns>是合法的ipv4地址返回true,否则返回flase</returns>
        public static bool ValidateIPV4(string ipStr)
        {
            return Regex.IsMatch(ipStr, RegexConstant.IPV4Reg);
        }

        /// <summary>
        /// IPV4地址转换为Int32整数(返回值存在负数)
        /// </summary>
        /// <param name="ipStr">ip字符串</param>
        /// <returns>ip对应的整数</returns>
        public static int IPV4ToInt32(string ipStr)
        {
            if (!IPAddressEx.ValidateIPV4(ipStr))
            {
                throw new Exception(string.Format("服务端IP:{0}不是有效有IPV4值", ipStr));
            }

            string[] items = ipStr.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(items[0]) << 24
                    | int.Parse(items[1]) << 16
                    | int.Parse(items[2]) << 8
                    | int.Parse(items[3]);
        }

        /// <summary>
        /// IPV4地址转换为Int64整数(返回值全为正数)
        /// </summary>
        /// <param name="ipStr">ip字符串</param>
        /// <returns>ip对应的整数</returns>
        public static long IPV4ToInt64(string ipStr)
        {
            if (!IPAddressEx.ValidateIPV4(ipStr))
            {
                throw new Exception(string.Format("服务端IP:{0}不是有效有IPV4值", ipStr));
            }

            string[] items = ipStr.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        /// <summary>
        /// 整数转换为IP地址
        /// </summary>
        /// <param name="ipValue">ip对应的整数值</param>
        /// <returns>IP地址</returns>
        public static string IntToIp(long ipValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipValue >> 24) & 0xFF).Append(".");
            sb.Append((ipValue >> 16) & 0xFF).Append(".");
            sb.Append((ipValue >> 8) & 0xFF).Append(".");
            sb.Append(ipValue & 0xFF);
            return sb.ToString();
        }

        /// <summary>
        /// 验证一个端口号是否可用
        /// </summary>
        /// <param name="port">需要验证的端口号</param>
        /// <returns>如果可用返回true,否则返回false</returns>
        public static bool PortAvailable(int port)
        {
            Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            bool result = false;
            try
            {
                sk.Bind(new IPEndPoint(IPAddress.Any, port));
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                sk.Close();
            }

            return result;
        }

        /// <summary>
        /// 验证一个端口号是否被使用
        /// </summary>
        /// <param name="ipaddr">IP对象</param>
        /// <param name="port">需要验证的端口号</param>
        /// <returns>如果被使用返回true,否则返回false</returns>
        public static bool PortAvailable(this IPAddress ipaddr, int port)
        {
            List<int> tcpPorts = null;
            List<int> udpPorts = null;
            List<int> ipPorts = null;
            var allports = IPAddressEx.GetUsedPorts(out tcpPorts, out udpPorts, out ipPorts);
            return allports.Contains(port);
        }

        /// <summary>
        /// 获取已使用的端口号
        /// </summary>
        /// <param name="tcpPorts">TCP占用的端口号</param>
        /// <param name="udpPorts">UDP占用的端口号</param>
        /// <param name="ipPorts">ip占用的端口号</param>
        /// <returns>当前所有已使用端口号集合</returns>
        public static List<int> GetUsedPorts(out List<int> tcpPorts, out List<int> udpPorts, out List<int> ipPorts)
        {
            var allports = new List<int>();
            tcpPorts = new List<int>();
            udpPorts = new List<int>();
            ipPorts = new List<int>();

            //获取本地计算机的网络连接和通信统计数据的信息
            IPGlobalProperties ipGolobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            //tcp监听协议
            IPEndPoint[] ipsTCP = ipGolobalProperties.GetActiveTcpListeners();
            foreach (var ep in ipsTCP)
            {
                tcpPorts.Add(ep.Port);
                allports.Add(ep.Port);
            }
            tcpPorts = tcpPorts.OrderBy((p) => { return p; }).Distinct().ToList();

            //udp监听协议
            IPEndPoint[] ipsUDP = ipGolobalProperties.GetActiveUdpListeners();
            foreach (var ep in ipsUDP)
            {
                udpPorts.Add(ep.Port);
                allports.Add(ep.Port);
            }
            udpPorts = udpPorts.OrderBy((p) => { return p; }).Distinct().ToList();

            //返回本地计算机上的Internet协议连接信息
            TcpConnectionInformation[] tcpConninfoArray = ipGolobalProperties.GetActiveTcpConnections();
            foreach (var conn in tcpConninfoArray)
            {
                ipPorts.Add(conn.LocalEndPoint.Port);
                allports.Add(conn.LocalEndPoint.Port);
            }
            ipPorts = ipPorts.OrderBy((p) => { return p; }).Distinct().ToList();

            allports = allports.OrderBy((p) => { return p; }).Distinct().ToList();
            return allports;
        }

        /// <summary>
        /// 获取已使用的端口号
        /// </summary>
        /// <returns>当前所有已使用端口号集合</returns>
        public static List<int> GetUsedPorts()
        {
            List<int> tcpPorts, udpPorts, ipPorts;
            return IPAddressEx.GetUsedPorts(out tcpPorts, out udpPorts, out ipPorts);
        }

        /// <summary>
        /// 根据服务器IP获取能连接到该服务器的其中一个客户端面IP地址
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <returns>找到的客户端IP地址</returns>
        public static IPAddress GetPingServerIpClientIp(string serverIp)
        {
            IPAddress serverIpAddr;
            if (!IPAddress.TryParse(serverIp, out serverIpAddr))
            {
                throw new Exception(string.Format("服务端IP:{0}不是有效有IP值", serverIp));
            }

            IPAddress ipAddrRet = null;
            IPAddress[] ipaddrs = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ipAddr in ipaddrs)
            {
                //ipAddr.AddressFamily :对于 IPv4，返回 InterNetwork；对于 IPv6，返回 InterNetworkV6。
                if (serverIpAddr.AddressFamily != ipAddr.AddressFamily)
                {
                    continue;
                }

                if (IPAddressEx.ValidateClientIpPingServerIp(ipAddr.ToString(), serverIp))
                {
                    ipAddrRet = ipAddr;
                    break;
                }
            }

            return ipAddrRet;
        }

        /// <summary>
        /// 验证本机中的一个IP是否能连接到服务端
        /// </summary>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="serverIp">服务端IP</param>
        /// <returns>能ping通:true,否则false</returns>
        public static bool ValidateClientIpPingServerIp(string clientIp, string serverIp)
        {
            IPAddress serverIpAddr, clientIpAddr;
            if (!IPAddress.TryParse(clientIp, out clientIpAddr))
            {
                throw new Exception(string.Format("客户端IP:{0}不是有效有IP值", clientIp));
            }
            if (!IPAddress.TryParse(serverIp, out serverIpAddr))
            {
                throw new Exception(string.Format("服务端IP:{0}不是有效有IP值", serverIp));
            }
            //ipAddr.AddressFamily :对于 IPv4，返回 InterNetwork；对于 IPv6，返回 InterNetworkV6。
            if (serverIpAddr.AddressFamily != clientIpAddr.AddressFamily)
            {
                throw new Exception(string.Format("服务端IP:{0}与客户端IP:{1}不是同一类IP", serverIp, clientIp));
            }

            string app = "PING.EXE";
            int count = 4;
            string args = string.Format("{0} -S {1} -n {2}", serverIp, clientIp, count);
            string pingResultStr = ProcessEx.SynExcuteCmd(app, args);
            pingResultStr = pingResultStr.Replace(" ", string.Empty);

            Match receiveCountMatch = Regex.Match(pingResultStr, @"已接收=(?<rcount>\d+)");
            string receiveCountStr = receiveCountMatch.Groups["rcount"].Value;
            if (string.IsNullOrEmpty(receiveCountStr))
            {
                return false;
            }
            //已接收包数
            int receiveCount = int.Parse(receiveCountStr);

            //时间
            MatchCollection mc = Regex.Matches(pingResultStr, @"<(?<rtime>\d+)ms");

            return mc.Count > 0 && receiveCount > 0;
        }

        /// <summary>
        /// 验证本机能否Ping通指定IP或主机名的主机[成功返回true;否则返回false]
        /// </summary>
        /// <param name="hostNameOrAddress">IP或主机名</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>成功返回true;否则返回false</returns>
        public static bool Ping(string hostNameOrAddress, int timeout = 4000)
        {
            bool pingStatus = false;
            using (Ping p = new Ping())
            {
                byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

                try
                {
                    PingReply reply = p.Send(hostNameOrAddress, timeout, buffer);
                    pingStatus = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }

            return pingStatus;
        }

        /// <summary>
        /// win32 IcmpPing
        /// </summary>
        /// <param name="ip">目标IP</param>
        /// <returns></returns>
        public static bool Ping(IPAddress ip)
        {
            using (var p = new Ping())
            {
                PingReply result = p.Send(ip);
                return result.Status == IPStatus.Success;
            }

            //IntPtr icmpHandle = NativeMethods.IcmpCreateFile();
            //ICMP_OPTIONS icmpOptions = new ICMP_OPTIONS();
            //icmpOptions.Ttl = 255;
            //ICMP_ECHO_REPLY icmpReply = new ICMP_ECHO_REPLY();
            //string sData = "x";
            //int iReplies = NativeMethods.IcmpSendEcho(icmpHandle, BitConverter.ToInt32(ip.GetAddressBytes(), 0), sData, (short)sData.Length, ref icmpOptions, ref icmpReply, Marshal.SizeOf(icmpReply), 30);
            //NativeMethods.IcmpCloseHandle(icmpHandle);
            //if (icmpReply.Status == 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}
