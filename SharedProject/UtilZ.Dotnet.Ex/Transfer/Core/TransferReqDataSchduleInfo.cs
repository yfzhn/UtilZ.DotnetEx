using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferReqDataSchduleInfo
    {
        private long _beginPostion = 0;
        public long BeginPostion
        {
            get { return _beginPostion; }
        }

        private long _postion = 0;
        public long Postion
        {
            get { return _postion; }
        }

        private readonly long _endPostion;
        public long EndPostion
        {
            get { return _endPostion; }
        }

        private Action _postionUpdateNotify;

        public TransferReqDataSchduleInfo(long postion, long size, Action postionUpdateNotify)
        {
            this._beginPostion = postion;
            this._postion = postion;
            this._endPostion = postion + size;
            this._postionUpdateNotify = postionUpdateNotify;
        }

        public void UpdatePostion(int size)
        {
            this._postion += size;
            this._postionUpdateNotify();
        }

        public bool IsTransferCompleted()
        {
            return this._postion >= this._endPostion;
        }
    }
}
