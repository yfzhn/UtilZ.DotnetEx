using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferReqDataSchduleInfoManager : TimeoutBase
    {
        private readonly SendDataNotifyMessage _notifyMessage;

        //[key:请求数据线程ID]
        private readonly Dictionary<int, TransferReqDataSchduleInfo> _reqDataSchduleInfoDic = new Dictionary<int, TransferReqDataSchduleInfo>();

        public SendDataNotifyMessage NotifyMessage
        {
            get { return _notifyMessage; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="reqDataThreads"></param>
        /// <param name="millisecondsTimeout">超时时长,单位毫秒</param>
        public TransferReqDataSchduleInfoManager(SendDataNotifyMessage message, ThreadEx[] reqDataThreads, int millisecondsTimeout)
            : base(millisecondsTimeout)
        {
            this._notifyMessage = message;

            var length = message.Size;
            var segLength = length / reqDataThreads.Length;
            int lastSegIndex = reqDataThreads.Length - 1;
            long postion = 0;

            for (var i = 0; i < reqDataThreads.Length; i++)
            {
                if (i == lastSegIndex)
                {
                    segLength = length - postion;
                }

                this._reqDataSchduleInfoDic.Add(reqDataThreads[i].ManagedThreadId, new TransferReqDataSchduleInfo(postion, segLength, this.UpdateLastAccessTimestamp));
                postion += segLength;
            }
        }

        private readonly object _isSendTransferCompletedMessageLock = new object();
        private bool _isSendTransferCompletedMessage = false;
        private bool _isTransferCompleted = false;
        private readonly object _isTransferCompletedLock = false;

        internal bool IsTransferCompleted()
        {
            if (this._isTransferCompleted)
            {
                return true;
            }

            lock (this._isTransferCompletedLock)
            {
                if (this._isTransferCompleted)
                {
                    return true;
                }

                foreach (var reqDataSchduleInfo in this._reqDataSchduleInfoDic.Values)
                {
                    if (!reqDataSchduleInfo.IsTransferCompleted())
                    {
                        return false;
                    }
                }

                this._isTransferCompleted = true;
                return true;
            }
        }

        internal bool GetSendTransferCompletedMessageLock()
        {
            //是否发送传输完成通知
            if (this._isSendTransferCompletedMessage)
            {
                return false;
            }

            lock (this._isSendTransferCompletedMessageLock)
            {
                if (this._isSendTransferCompletedMessage)
                {
                    return false;
                }

                this._isSendTransferCompletedMessage = true;
            }

            return true;
        }

        public TransferReqDataSchduleInfo GetReqDataSchduleInfo(int threadId)
        {
            return this._reqDataSchduleInfoDic[threadId];
        }

        public TransferReqDataSchduleInfo GetIncompletedReqDataSchduleInfo()
        {
            return this._reqDataSchduleInfoDic.Values.Where(t => { return !t.IsTransferCompleted(); }).FirstOrDefault();
        }
    }
}
