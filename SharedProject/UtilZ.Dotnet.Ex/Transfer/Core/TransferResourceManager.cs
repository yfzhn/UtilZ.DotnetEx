using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferResourceManager
    {
        /// <summary>
        /// [key: rid;value:UdpTransferReqDataSchduleInfoManager]
        /// </summary>
        private readonly ConcurrentDictionary<int, TransferReqDataSchduleInfoManager> _reqDataSchduleInfoManagerDic = new ConcurrentDictionary<int, TransferReqDataSchduleInfoManager>();

        public TransferResourceManager()
        {

        }

        public bool AddReqDataSchduleInfoManager(TransferReqDataSchduleInfoManager reqDataSchduleInfoManager)
        {
            return this._reqDataSchduleInfoManagerDic.TryAdd(reqDataSchduleInfoManager.NotifyMessage.Header.Rid, reqDataSchduleInfoManager);
        }

        public TransferReqDataSchduleInfoManager RemoveReqDataSchduleInfoManager(int rid)
        {
            TransferReqDataSchduleInfoManager reqDataSchduleInfoManager;
            this._reqDataSchduleInfoManagerDic.TryRemove(rid, out reqDataSchduleInfoManager);
            return reqDataSchduleInfoManager;
        }

        public TransferReqDataSchduleInfoManager GetReqDataSchduleInfoManager()
        {
            return this._reqDataSchduleInfoManagerDic.Values.FirstOrDefault();
        }

        public List<TransferReqDataSchduleInfoManager> GetIsTimeoutReqDataSchduleInfoManager()
        {
            if (this._reqDataSchduleInfoManagerDic.Count == 0)
            {
                return null;
            }

            var reqDataSchduleInfoManagers = this._reqDataSchduleInfoManagerDic.Values.ToArray();
            var timeoutReqDataSchduleInfoManagers = new List<TransferReqDataSchduleInfoManager>();

            foreach (var reqDataSchduleInfoManager in reqDataSchduleInfoManagers)
            {
                if (reqDataSchduleInfoManager.IsTimeout())
                {
                    timeoutReqDataSchduleInfoManagers.Add(reqDataSchduleInfoManager);
                }
            }

            return timeoutReqDataSchduleInfoManagers;
            //return this._reqDataSchduleInfoManagerDic.Values.ToArray().Where(t => { return t.IsTimeout(); }).ToArray();
        }
    }
}
