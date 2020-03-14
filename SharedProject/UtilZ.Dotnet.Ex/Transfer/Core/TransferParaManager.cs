using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferParaManager
    {
        /// <summary>
        /// [key:ip;value:NetParaAnalyze]
        /// </summary>
        private static readonly Hashtable _htNetParas = Hashtable.Synchronized(new Hashtable());
        private static TransferConfig _config;

        public static void Init(TransferConfig config)
        {
            _config = config;
        }

        private static TransferParaAnalyze GetNetParaAnalyze(IPEndPoint endPoint)
        {
            string key = endPoint.Address.ToString();
            var netPara = _htNetParas[key] as TransferParaAnalyze;
            if (netPara == null)
            {
                lock (_htNetParas.SyncRoot)
                {
                    netPara = _htNetParas[key] as TransferParaAnalyze;
                    if (netPara == null)
                    {
                        netPara = new TransferParaAnalyze(_config);
                        _htNetParas[key] = netPara;
                    }
                }
            }

            return netPara;
        }

        public static int GetMtu(IPEndPoint endPoint)
        {
            return GetNetParaAnalyze(endPoint).GetMtu();
        }

        public static int GetRto(IPEndPoint endPoint)
        {
            return GetNetParaAnalyze(endPoint).GetRto();
        }

        public static int AdjustDownMtu(IPEndPoint endPoint)
        {
            return GetNetParaAnalyze(endPoint).AdjustDownMtu();
        }

        public static int AdjustUpRto(IPEndPoint endPoint)
        {
            return GetNetParaAnalyze(endPoint).AdjustUpRto();
        }

        public static void RecordMtuAndRto(IPEndPoint endPoint, int mtu, int rto, bool isTimeout)
        {
            GetNetParaAnalyze(endPoint).RecordMtuAndRto(mtu, rto, isTimeout);
        }
    }
}
