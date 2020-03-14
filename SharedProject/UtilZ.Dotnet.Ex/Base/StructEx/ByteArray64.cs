using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// byte数组64类
    /// </summary>
    [Serializable]
    public class ByteArray64 : Array64<byte>
    {
        /// <summary>
        /// 每次读10MB数据
        /// </summary>
        private const int _BufferSize = 1024 * 1024 * 10;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="length">数组长度</param>
        public ByteArray64(long length) :
            base(length)
        {

        }

        /// <summary>
        /// 构造函数(根据需要指定存储页相关信息可提升性能)
        /// </summary>
        /// <param name="length">数组长度</param>
        /// <param name="colSize">存储页中的列大小</param>
        /// <param name="rowSize">存储页中的行大小</param>
        public ByteArray64(long length, int colSize, int rowSize) :
            base(length, colSize, rowSize)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ptr">数据指针</param>
        /// <param name="length">数组长度</param>
        /// <param name="colSize">存储页中的列大小</param>
        /// <param name="rowSize">存储页中的行大小</param>
        public ByteArray64(IntPtr ptr, long length, int colSize = DefaultColSize, int rowSize = DefaultRowSize) :
               base(length, colSize, rowSize)
        {
            this.Set(0, ptr, 0, length);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流对象</param>
        /// <param name="position">数据流中读取数据起始位置</param>
        /// <param name="length">源数据数组中要写入的数据长度,如果超出当前数组范围,则以当前数组实际长度为准</param>
        /// <param name="colSize">存储页中的列大小</param>
        /// <param name="rowSize">存储页中的行大小</param>
        public ByteArray64(Stream stream, long position, long length, int colSize = DefaultColSize, int rowSize = DefaultRowSize) :
            this(stream.Length, colSize, rowSize)
        {
            this.Set(0, stream, position, stream.Length);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="index">数据起始位置</param>
        /// <param name="count">数据个数</param>
        public ByteArray64(byte[] buffer, int index, int count) : this(buffer.Length)
        {
            this.Set(0, buffer, index, count);
        }

        /// <summary>
        /// 设置数组对象数据,返回实际设置数据长度
        /// </summary>
        /// <param name="offset">当前数组对象中偏移量</param>
        /// <param name="stream">数据流对象</param>
        /// <param name="position">数据流中读取数据起始位置</param>
        /// <param name="length">源数据数组中要写入的数据长度,如果超出当前数组范围,则以当前数组实际长度为准</param>
        /// <returns>实际设置数据长度</returns>
        public long Set(long offset, Stream stream, long position, long length)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if (length < 1)
            {
                return 0;
            }

            stream.Seek(position, SeekOrigin.Begin);
            long count = length / _BufferSize;
            byte[] buffer;
            long setOffset = offset;
            int mod = (int)(length % _BufferSize);
            if (count > 0)
            {
                buffer = new byte[_BufferSize];
                for (int i = 0; i < count; i++)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    setOffset += this.Set(setOffset, buffer, 0, buffer.Length);
                }
            }
            else
            {
                buffer = new byte[mod];
            }

            if (mod > 0)
            {
                stream.Read(buffer, 0, mod);
                setOffset += this.Set(setOffset, buffer, 0, mod);
            }

            return setOffset - offset;
        }

        /// <summary>
        /// 设置数组对象数据,返回实际设置数据长度
        /// </summary>
        /// <param name="offset">当前数组对象中偏移量</param>
        /// <param name="ptr">源数据指针</param>
        /// <param name="ptrOffset">源源数据指针中的偏移量</param>
        /// <param name="length">源数据数组中要写入的数据长度,如果超出当前数组范围,则以当前数组实际长度为准</param>
        /// <returns>实际设置数据长度</returns>
        public long Set(long offset, IntPtr ptr, long ptrOffset, long length)
        {
            if (ptr == IntPtr.Zero)
            {
                return 0;
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (ptrOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ptrOffset));
            }

            if (length < 1)
            {
                return 0;
            }

            long count = length / _BufferSize;
            byte[] buffer;
            long setOffset = offset;
            int mod = (int)(length % _BufferSize);
            if (count > 0)
            {
                buffer = new byte[_BufferSize];
                for (int i = 0; i < count; i++)
                {
                    Marshal.Copy(ptr, buffer, 0, buffer.Length);
                    ptr += buffer.Length;
                    setOffset += this.Set(setOffset, buffer, 0, buffer.Length);
                }
            }
            else
            {
                buffer = new byte[mod];
            }

            if (mod > 0)
            {
                Marshal.Copy(ptr, buffer, 0, mod);
                setOffset += this.Set(setOffset, buffer, 0, mod);
            }

            return setOffset - offset;
        }

        /// <summary>
        /// 从指定偏移位置获取指定长度的数组,如果从偏移位置起目标长度超出数组范围,则以实际数组长度为准
        /// </summary>
        /// <param name="offset">当前对象中数据数据偏移量</param>
        /// <param name="length">要获取的数据长度</param>
        /// <param name="ptr">存放数据指针</param>
        /// <returns>获取到的数据长度</returns>
        public long Get(long offset, long length, IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return 0;
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 1)
            {
                return 0;
            }

            long count = length / _BufferSize;
            byte[] buffer;
            long setOffset = offset;
            int mod = (int)(length % _BufferSize);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    buffer = this.Get(setOffset, _BufferSize);
                    Marshal.Copy(buffer, 0, ptr, buffer.Length);
                    ptr += buffer.Length;
                    setOffset += buffer.Length;
                }
            }
            else
            {
                buffer = new byte[mod];
            }

            if (mod > 0)
            {
                buffer = this.Get(setOffset, mod);
                Marshal.Copy(buffer, 0, ptr, mod);
                setOffset += mod;
            }

            return setOffset - offset;
        }
    }
}
