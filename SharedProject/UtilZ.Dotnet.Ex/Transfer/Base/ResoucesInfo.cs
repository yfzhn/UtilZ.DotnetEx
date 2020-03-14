using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class ResoucesInfo : TimeoutBase, IDisposable
    {
        /// <summary>
        /// 资源标识
        /// </summary>
        public Int32 Rid { get; private set; }

        public TransferDataType ResoucesType { get; private set; }

        public TransferPolicy Policy { get; private set; }

        public long Postion { get; private set; }

        public long Length { get; private set; }

        public byte[] Data { get; private set; }

        public string FilePath { get; private set; }

        private readonly Stream _stream;
        private readonly object _streamLock = new object();
        private bool _isDisposed = false;

        private ResoucesInfo(TransferDataType resoucesType, TransferPolicy policy, long postion, long length) : base(policy.MillisecondsTimeout)
        {
            this.Rid = GUIDEx.GetGUIDHashCode();
            this.ResoucesType = resoucesType;
            this.Policy = policy;
            this.Postion = postion;
            this.Length = length;
        }

        public ResoucesInfo(byte[] data, TransferPolicy policy, int postion, int length)
            : this(TransferDataType.Data, policy, postion, length)
        {
            this.Data = data;
        }

        public ResoucesInfo(Stream stream, TransferPolicy policy, long postion, long length)
             : this(TransferDataType.Stream, policy, postion, length)
        {
            this._stream = stream;
        }

        public ResoucesInfo(string filePath, TransferPolicy policy, long postion, long length)
             : this(TransferDataType.File, policy, postion, length)
        {
            this.FilePath = filePath;
        }

        public byte[] GetStreamData(long position, int length)
        {
            var buffer = new byte[length];
            lock (this._streamLock)
            {
                if (this._isDisposed)
                {
                    return null;
                }

                this._stream.Seek(position, SeekOrigin.Begin);
                this._stream.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        public void Dispose()
        {
            lock (this._streamLock)
            {
                this._isDisposed = true;
            }
        }
    }
}
