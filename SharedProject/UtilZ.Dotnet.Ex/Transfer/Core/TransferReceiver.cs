using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Base.MemoryCache;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferReceiver : IDisposable
    {
        private readonly TransferConfig _config;
        private readonly ITransferNet _net;
        private readonly Action<ReceiveDataItem> _rev;

        private readonly AsynQueue<ReceiveDataItem> _revOutputQueue;

        //[key:Priority;value:ResourceTransferManager]
        private readonly SortedList<short, TransferResourceManager> _priorityResourceTransferManagerSortedList = new SortedList<short, TransferResourceManager>();
        private readonly object _priorityResourceTransferManagerSortedListLock = new object();
        private bool _priorityResourceTransferManagerSortedListHeartFlag = false;

        //[key:rid;value:ReceiveDataItem]
        private readonly ConcurrentDictionary<int, ReceiveDataItem> _revItemDic = new ConcurrentDictionary<int, ReceiveDataItem>();
        private readonly object _revItemDicLock = new object();

        private readonly ThreadEx[] _reqDataThreads;
        private readonly AutoResetEvent[] _reqDataThreadsEventHandles;

        private readonly ThreadEx _revTimeoutCheckThread;
        private readonly AutoResetEvent _revTimeoutCheckEventHandle = new AutoResetEvent(false);
        private readonly AutoResetEvent _revNewDataNoyifyEventHandle = new AutoResetEvent(false);

        private void RevitemOutput(ReceiveDataItem item)
        {
            try
            {
                this._rev(item);
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "外部处理接收的到数据项异常");
            }
        }

        private void SendReqDataThreadsEventHandleNotify()
        {
            foreach (var reqDataThreadsEventHandle in this._reqDataThreadsEventHandles)
            {
                reqDataThreadsEventHandle.Set();
            }
        }

        public TransferReceiver(TransferConfig config, ITransferNet net, Action<ReceiveDataItem> rev)
        {
            this._config = config;
            this._net = net;
            this._rev = rev;
            TransferParaManager.Init(config);

            this._revOutputQueue = new AsynQueue<ReceiveDataItem>(this.RevitemOutput, "拉收到数据输出线程", true, true);

            int threadCount = config.TransferThreadCount;
            this._reqDataThreads = new ThreadEx[threadCount];
            this._reqDataThreadsEventHandles = new AutoResetEvent[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                this._reqDataThreadsEventHandles[i] = new AutoResetEvent(false);
                this._reqDataThreads[i] = new ThreadEx(this.ReqDataThreadMethod, $"请求数据线程[{i}]", true);
                this._reqDataThreads[i].Start(this._reqDataThreadsEventHandles[i]);
            }

            this._revTimeoutCheckThread = new ThreadEx(this.RevTimeoutCheckThreadMethod, "接收超时检测线程", true);
            this._revTimeoutCheckThread.Start();
        }

        private void RevTimeoutCheckThreadMethod(CancellationToken token)
        {
            const int defaultTimeoutCheckInveral = 100;
            int timeoutCheckInveral = defaultTimeoutCheckInveral;
            ReceiveDataItem[] revItems;
            SendDataNotifyMessage notifyMessage;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    revItems = this._revItemDic.Values.ToArray();
                    if (revItems.Length == 0)
                    {
                        try
                        {
                            this._revNewDataNoyifyEventHandle.WaitOne(30000);
                        }
                        catch (ObjectDisposedException)
                        { }

                        continue;
                    }

                    timeoutCheckInveral = defaultTimeoutCheckInveral;
                    foreach (var revItem in revItems)
                    {
                        try
                        {
                            notifyMessage = revItem.NotifyMessage;
                            if (revItem.IsTimeout())
                            {
                                //Loger.Warn($"RID:[{notifyMessage.Header.Rid}]传输超时,最后一次收到数据时间[{TimeEx.TimestampToDateTime(revItem.LastAccessTimestamp).AddHours(8).ToString("HH:mm:ss.fffffff")}]从传输列表中移除该项");
                                MemoryCacheEx.Set(CacheKeyGenerator.GenerateRevTimeoutKey(notifyMessage.Header.Rid), notifyMessage.Header.Rid, revItem.MillisecondsTimeout);
                                this.RemoveTransferItem(notifyMessage.Header.Rid, false);
                            }
                            else
                            {
                                if (timeoutCheckInveral > notifyMessage.Timeout)
                                {
                                    timeoutCheckInveral = notifyMessage.Timeout;
                                }
                            }
                        }
                        catch (Exception exi)
                        {
                            Loger.Error(exi);
                        }
                    }

                    notifyMessage = null;
                }
                catch (ObjectDisposedException)
                { }
                catch (Exception ex)
                {
                    Loger.Error(ex, "RevTimeoutCheckThreadMethod异常");
                }

                try
                {
                    this._revTimeoutCheckEventHandle.WaitOne(timeoutCheckInveral);
                }
                catch (ObjectDisposedException)
                { }
            }
        }

        private void ReqDataThreadMethod(CancellationToken token, object obj)
        {
            const int reqDataThreadsEventHandleTimeout = 2000;
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            var reqDataThreadsEventHandle = (AutoResetEvent)obj;
            TransferReqDataSchduleInfoManager reqDataSchduleInfoManager = null;
            TransferReqDataSchduleInfo currentReqDataSchduleInfo;
            bool hasSendTransferCompletedMessageLock;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    //var watch = System.Diagnostics.Stopwatch.StartNew();
                    hasSendTransferCompletedMessageLock = this.GetReqDataSchduleInfo(currentThreadId, out reqDataSchduleInfoManager, out currentReqDataSchduleInfo);
                    //watch.Stop();
                    //if (reqDataSchduleInfoManager != null)
                    //{
                    //    Loger.Warn($"RID[{reqDataSchduleInfoManager.NotifyMessage.Header.Rid}]GetReqDataSchduleInfo耗时{watch.Elapsed.Milliseconds}毫秒");
                    //}

                    if (hasSendTransferCompletedMessageLock)
                    {
                        this.SendTransferCompletedMessage(reqDataSchduleInfoManager.NotifyMessage);
                    }
                    else
                    {
                        if (reqDataSchduleInfoManager == null || currentReqDataSchduleInfo == null)
                        {
                            try
                            {
                                reqDataThreadsEventHandle.WaitOne(reqDataThreadsEventHandleTimeout);
                            }
                            catch (ObjectDisposedException)
                            {
                                break;
                            }
                        }
                        else
                        {
                            this.PrimitiveReqData(reqDataSchduleInfoManager, currentReqDataSchduleInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Loger.Error(ex, "请求数据异常");
                }
            }
        }

        private bool GetReqDataSchduleInfo(int currentThreadId, out TransferReqDataSchduleInfoManager reqDataSchduleInfoManager, out TransferReqDataSchduleInfo currentReqDataSchduleInfo)
        {
            bool sendHeartFlag = false;
            TransferResourceManager[] resourceTransferManagers;
            lock (this._priorityResourceTransferManagerSortedListLock)
            {
                resourceTransferManagers = this._priorityResourceTransferManagerSortedList.Values.ToArray();
                if (!this._priorityResourceTransferManagerSortedListHeartFlag)
                {
                    this._priorityResourceTransferManagerSortedListHeartFlag = true;
                    sendHeartFlag = true;
                }
            }

            if (sendHeartFlag)
            {
                //发送心跳数据包
                this.SendHeartData(resourceTransferManagers);

                //重置标识
                this._priorityResourceTransferManagerSortedListHeartFlag = false;
            }

            //查找本次传输项
            foreach (var resourceTransferManager in resourceTransferManagers)
            {
                reqDataSchduleInfoManager = resourceTransferManager.GetReqDataSchduleInfoManager();
                if (reqDataSchduleInfoManager == null)
                {
                    continue;
                }

                if (reqDataSchduleInfoManager.IsTransferCompleted())
                {
                    if (reqDataSchduleInfoManager.GetSendTransferCompletedMessageLock())
                    {
                        currentReqDataSchduleInfo = null;
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    currentReqDataSchduleInfo = reqDataSchduleInfoManager.GetReqDataSchduleInfo(currentThreadId);
                    if (currentReqDataSchduleInfo.IsTransferCompleted())
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            reqDataSchduleInfoManager = null;
            currentReqDataSchduleInfo = null;
            return false;
        }

        private void SendHeartData(TransferResourceManager[] resourceTransferManagers)
        {
            for (int i = resourceTransferManagers.Length - 1; i > 0; i--)
            {
                var timeoutReqDataSchduleInfoManagers = resourceTransferManagers[i].GetIsTimeoutReqDataSchduleInfoManager();
                if (timeoutReqDataSchduleInfoManagers == null || timeoutReqDataSchduleInfoManagers.Count == 0)
                {
                    continue;
                }

                foreach (var timeoutReqDataSchduleInfoManager in timeoutReqDataSchduleInfoManagers)
                {
                    var reqDataSchduleInfo = timeoutReqDataSchduleInfoManager.GetIncompletedReqDataSchduleInfo();
                    if (reqDataSchduleInfo == null)
                    {
                        continue;
                    }

                    //Loger.Warn($"[RID:{timeoutReqDataSchduleInfoManager.NotifyMessage.Header.Rid}]-发送心跳数据包-[SegInfo:{reqDataSchduleInfo.BeginPostion}-{reqDataSchduleInfo.EndPostion}]");
                    this.PrimitiveReqData(timeoutReqDataSchduleInfoManager, reqDataSchduleInfo);
                }
            }
        }

        private void PrimitiveReqData(TransferReqDataSchduleInfoManager reqDataSchduleInfoManager, TransferReqDataSchduleInfo currentReqDataSchduleInfo)
        {
            var endPoint = reqDataSchduleInfoManager.NotifyMessage.SrcEndPoint;
            int repeatCount = 0;
            int mtu, rto;

            while (repeatCount++ < this._config.MtuRepeatMaxCount)
            {
                mtu = TransferParaManager.GetMtu(endPoint);
                rto = TransferParaManager.GetRto(endPoint);
                var ep = currentReqDataSchduleInfo.EndPostion - currentReqDataSchduleInfo.Postion;
                if (mtu > ep)
                {
                    mtu = (int)ep;
                }

                if (mtu <= 0)
                {
                    break;
                }

                var reqMessage = new ResourceRequestMessage(reqDataSchduleInfoManager.NotifyMessage.Header.Rid, currentReqDataSchduleInfo.Postion, mtu);
                var buffer = reqMessage.GenerateBuffer();
                var eventHandleId = reqMessage.ContextId;
                var eventHandleInfo = AutoEventWaitHandleManager.CreateEventWaitHandle(eventHandleId, reqDataSchduleInfoManager.NotifyMessage.Timeout);
                try
                {
                    //_ht[$"{reqMessage.Header.Rid}_{reqMessage.ContextId}"] = _watch.ElapsedMilliseconds;
                    this._net.Send(buffer, endPoint);
                    if (eventHandleInfo.EventWaitHandle.WaitOne(rto))
                    {
                        currentReqDataSchduleInfo.UpdatePostion(mtu);
                        if (currentReqDataSchduleInfo.IsTransferCompleted())
                        {
                            //Loger.Warn($"线程[{System.Threading.Thread.CurrentThread.Name}]传输[Rid:{reqDataSchduleInfoManager.NotifyMessage.Header.Rid}],[{currentReqDataSchduleInfo.BeginPostion}-{currentReqDataSchduleInfo.EndPostion}]完成");
                        }
                    }
                    else
                    {
                        TransferParaManager.RecordMtuAndRto(endPoint, mtu, rto, true);
                        mtu = TransferParaManager.AdjustDownMtu(endPoint);
                        rto = TransferParaManager.AdjustUpRto(endPoint);
                    }
                }
                finally
                {
                    AutoEventWaitHandleManager.RemoveEventWaitHandle(eventHandleId);
                    eventHandleInfo.EventWaitHandle.Dispose();
                }
            }
        }

        internal void ProSendNotify(SendDataNotifyMessage message)
        {
            try
            {
                bool proResult;
                switch (message.ResourceType)
                {
                    case ResourceTypeConstant.Message:
                        proResult = this.ProMessageMode(message);
                        break;
                    case ResourceTypeConstant.ResourceData:
                    case ResourceTypeConstant.ResourceStream:
                    case ResourceTypeConstant.ResourceFile:
                        proResult = this.ProResourceMode(message);
                        break;
                    default:
                        Loger.Warn($"未知的发送模式{message.ResourceType}");
                        proResult = false;
                        break;
                }

                if (proResult)
                {
                    try
                    {
                        this._revNewDataNoyifyEventHandle.Set();
                    }
                    catch (ObjectDisposedException)
                    { }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "ProSendNotify发生异常");
            }
        }

        private bool ProResourceMode(SendDataNotifyMessage message)
        {
            lock (this._revItemDicLock)
            {
                var rid = message.Header.Rid;
                ReceiveDataItem receiveDataItem;
                if (this._revItemDic.TryGetValue(rid, out receiveDataItem))
                {
                    receiveDataItem.UpdateLastAccessTimestamp();
                    //Loger.Warn($"[RID:{rid}]的接收项已存在,忽略");
                    return false;
                }

                if (MemoryCacheEx.Get(CacheKeyGenerator.GenerateRevTimeoutKey(rid)) != null)
                {
                    //Loger.Warn($"[RID:{rid}]接收已超时,忽略");
                    return false;
                }

                //Loger.Warn($"处理RID[{rid}]发送通知");
                receiveDataItem = new ReceiveDataItem(message, this._config.LocalFileDirectory, this._config.TransferThreadCount);
                if (!this._revItemDic.TryAdd(rid, receiveDataItem))
                {
                    receiveDataItem.Close(true);
                    Loger.Error("this._revItemDic.TryAdd失败,原因未知");
                    return false;
                }

                int millisecondsTimeout;
                var revItemCount = this._revItemDic.Count;
                if (revItemCount < 1)
                {
                    millisecondsTimeout = message.Timeout / this._config.TimeoutHeartMul;
                }
                else
                {
                    millisecondsTimeout = message.Timeout / (revItemCount * this._config.TimeoutHeartMul);
                }

                var reqDataSchduleInfoManager = new TransferReqDataSchduleInfoManager(message, this._reqDataThreads, millisecondsTimeout);
                TransferResourceManager resourceTransferManager;
                var priority = message.Priority;
                if (this._priorityResourceTransferManagerSortedList.ContainsKey(priority))
                {
                    resourceTransferManager = this._priorityResourceTransferManagerSortedList[priority];
                }
                else
                {
                    lock (this._priorityResourceTransferManagerSortedListLock)
                    {
                        if (this._priorityResourceTransferManagerSortedList.ContainsKey(priority))
                        {
                            resourceTransferManager = this._priorityResourceTransferManagerSortedList[priority];
                        }
                        else
                        {
                            resourceTransferManager = new TransferResourceManager();
                            this._priorityResourceTransferManagerSortedList.Add(priority, resourceTransferManager);
                        }
                    }
                }

                if (resourceTransferManager.AddReqDataSchduleInfoManager(reqDataSchduleInfoManager))
                {
                    this.SendReqDataThreadsEventHandleNotify();
                    return true;
                }
                else
                {
                    Loger.Error("AddReqDataSchduleInfoManager失败,原因未知");
                    if (!this._revItemDic.TryRemove(rid, out receiveDataItem))
                    {
                        receiveDataItem.Close(true);
                    }

                    return false;
                }
            }
        }

        private bool ProMessageMode(SendDataNotifyMessage message)
        {
            var receiveDataItem = new ReceiveDataItem(message);
            if (!this._revItemDic.TryAdd(message.Header.Rid, receiveDataItem))
            {
                receiveDataItem.Close(true);
                Loger.Error("this._revItemDic.TryAdd失败");
                return false;
            }

            this.SendTransferCompletedMessage(message);
            return true;
        }

        private void SendTransferCompletedMessage(SendDataNotifyMessage notifyMessage)
        {
            var transferCompletedMessage = new TransferCompletedMessage(notifyMessage);
            var buffer = transferCompletedMessage.GenerateBuffer();
            var id = CacheKeyGenerator.GenerateWaitTransferCompletedAckMessageEventWaitHandleId(notifyMessage.Header.Rid);
            var eventHandleInfo = AutoEventWaitHandleManager.CreateEventWaitHandle(id);
            try
            {
                long beginTs = TimeEx.GetTimestamp();
                long endTs = beginTs;

                while (!notifyMessage.IsTimeout(beginTs, endTs))
                {
                    try
                    {
                        int rto = TransferParaManager.GetRto(notifyMessage.SrcEndPoint);
                        this._net.Send(buffer, notifyMessage.SrcEndPoint);
                        if (eventHandleInfo.EventWaitHandle.WaitOne(rto))
                        {
                            break;
                        }
                        else
                        {
                            TransferParaManager.AdjustUpRto(notifyMessage.SrcEndPoint);
                        }
                    }
                    catch (SendDataException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Loger.Error(ex, "发送传输完成消息异常");
                    }

                    endTs = TimeEx.GetTimestamp();
                }
            }
            finally
            {
                AutoEventWaitHandleManager.RemoveEventWaitHandle(id);
                eventHandleInfo.EventWaitHandle.Dispose();
            }

            //注:如果一直都没有收到确认消息,则超时线程处理
        }

        internal void ProResourceResponse(ResourceResponseMessage message, byte[] revData)
        {
            try
            {
                if (message.Size == ResourceResponseMessage.RESOURCE_NOT_EXIST)
                {
                    //资源不存在,发送超时,将接收数据移除
                    Loger.Warn("请求资源不存在");
                }
                else
                {
                    int mtu = message.Size + ResourceResponseMessage.HEAD_SIZE;
                    int rto = (int)(TimeEx.GetTimestamp() - message.Header.Timestamp);
                    TransferParaManager.RecordMtuAndRto(message.SrcEndPoint, mtu, rto, false);

                    ReceiveDataItem receiveDataItem;
                    if (this._revItemDic.TryGetValue(message.Header.Rid, out receiveDataItem))
                    {
                        //发送收到响应数据通知
                        var eventHandle = AutoEventWaitHandleManager.GetEventWaitHandle(message.ContextId);
                        if (eventHandle != null)
                        {
                            try
                            {
                                receiveDataItem.UpdateLastAccessTimestamp();
                                if (message.Size == ResourceResponseMessage.RESOURCE_NOT_EXIST)
                                {
                                    return;
                                }

                                receiveDataItem.Write(message.Position, revData, ResourceResponseMessage.HEAD_SIZE, message.Size);
                                eventHandle.Set();
                            }
                            catch (ObjectDisposedException)
                            { }
                        }
                    }
                    else
                    {
                        //var tt = TimeEx.GetTimestamp();
                        //Loger.Warn($"未知的资源ID:{message.Header.Rid},[ContextId:{message.ContextId}],[请求响应时长:{tt - message.Header.Timestamp}],[请求数据Position:{message.Position},Size:{message.Size}],[线程:{Thread.CurrentThread.Name}]");
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "ProResourceResponse发生异常");
            }
        }

        internal void ProTransferCompletedAck(TransferCompletedAckMessage message)
        {
            try
            {
                this.SendTransferCompletedAckEventhandleNotify(message.Header.Rid);

                //处理确认
                this.RemoveTransferItem(message.Header.Rid, true);
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "ProTransferCompletedAck发生异常");
            }
        }

        private void RemoveTransferItem(int rid, bool isOuputReceiveDataItem)
        {
            //处理确认
            ReceiveDataItem receiveDataItem;
            if (this._revItemDic.TryRemove(rid, out receiveDataItem))
            {
                receiveDataItem.Close(!isOuputReceiveDataItem);
                var priority = receiveDataItem.NotifyMessage.Priority;
                if (this._priorityResourceTransferManagerSortedList.ContainsKey(priority))
                {
                    this._priorityResourceTransferManagerSortedList[priority].RemoveReqDataSchduleInfoManager(rid);
                }

                if (isOuputReceiveDataItem)
                {
                    this._revOutputQueue.Enqueue(receiveDataItem);
                }
                else
                {

                }
            }
            else
            {
                Loger.Warn("this._revItemDic.TryRemove失败");
            }
        }

        private void SendTransferCompletedAckEventhandleNotify(int rid)
        {
            var id = CacheKeyGenerator.GenerateWaitTransferCompletedAckMessageEventWaitHandleId(rid);
            var eventHandle = AutoEventWaitHandleManager.GetEventWaitHandle(id);
            try
            {
                if (eventHandle != null)
                {
                    eventHandle.Set();
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        public void Dispose()
        {
            try
            {
                this._revTimeoutCheckThread.Stop();
                this._revTimeoutCheckEventHandle.Set();
                this._revNewDataNoyifyEventHandle.Set();

                foreach (var reqDataThread in this._reqDataThreads)
                {
                    reqDataThread.Stop();
                    reqDataThread.Dispose();
                }

                this.SendReqDataThreadsEventHandleNotify();
                foreach (var reqDataThreadsEventHandle in this._reqDataThreadsEventHandles)
                {
                    reqDataThreadsEventHandle.Dispose();
                }

                this._revOutputQueue.Dispose();
                this._revNewDataNoyifyEventHandle.Dispose();
                this._revTimeoutCheckEventHandle.Dispose();
                this._revTimeoutCheckThread.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "Dispose异常");
            }
        }
    }
}
