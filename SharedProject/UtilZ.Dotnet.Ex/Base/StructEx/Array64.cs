using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 长度为Int64的数组类
    /// </summary>
    [Serializable, ComVisible(true)]
    public class Array64<T>
    {
        /************************************************************************************
         * page0             page1             page3
         * (p0,r0)+++++++    (p1,r0)+++++++    (p3,r0)+++++++    
         * (p0,r1)+++++++    (p1,r1)+++++++    (p3,r1)+++++++
         * (p0,r2)+++++++    (p1,r2)+++++++    (p3,r2)+++++
         * (p0,r3)+++++++    (p1,r3)+++++++    
         ************************************************************************************/

        /// <summary>
        /// 默认数据页列大小500MB
        /// </summary>
        public const int DefaultColSize = 524288000;

        /// <summary>
        /// 默认数据页行数小500MB
        /// </summary>
        public const int DefaultRowSize = 524288000;

        /// <summary>
        /// 存储页大小
        /// </summary>
        private readonly long _pageSize;

        /// <summary>
        /// 存储页中的列大小
        /// </summary>
        private readonly int _colSize;

        /// <summary>
        /// 存储页中的行大小
        /// </summary>
        private readonly int _rowSize;

        /// <summary>
        /// 数组长度
        /// </summary>
        private readonly long _length;
        /// <summary>
        /// 获取数组长度
        /// </summary>
        public long Length
        {
            get { return this._length; }
        }

        /// <summary>
        /// 页集合
        /// </summary>
        private readonly Array64Page<T>[] _pages;

        /// <summary>
        /// 获取或设置指定索引处的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>指定索引处的值</returns>
        public T this[long index]
        {
            get
            {
                return this.GetPageByPosition(index)[index];
            }
            set
            {
                this.GetPageByPosition(index)[index] = value;
            }
        }

        /// <summary>
        /// 获取指定位置索引所在存储页对象
        /// </summary>
        /// <param name="position">目标位置索引</param>
        /// <returns>指定位置索引所在存储页对象</returns>
        private Array64Page<T> GetPageByPosition(long position)
        {
            if (position < 0 || position > this._length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            int pageIndex = (int)(position / this._pageSize);
            return this._pages[pageIndex];
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="length">数组长度</param>
        public Array64(long length) :
            this(length, DefaultColSize, DefaultRowSize)
        {

        }

        /// <summary>
        /// 构造函数(根据需要指定存储页相关信息可提升性能)
        /// </summary>
        /// <param name="length">数组长度</param>
        /// <param name="colSize">存储页中的列大小</param>
        /// <param name="rowSize">存储页中的行大小</param>
        public Array64(long length, int colSize, int rowSize)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (colSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(colSize));
            }

            if (rowSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowSize));
            }

            long pageSize = (long)colSize * rowSize;
            long pageCount = length / pageSize;
            if (pageCount > int.MaxValue)
            {
                throw new ArgumentException("存储页中的行列值过小");
            }

            this._length = length;
            this._pageSize = pageSize;
            this._colSize = colSize;
            this._rowSize = rowSize;

            int lastPageIndex = (int)pageCount;
            int mod = (int)(length % pageSize);
            if (mod > 0)
            {
                pageCount += 1;
            }

            this._pages = new Array64Page<T>[pageCount];
            long begin = 0, end = 0;

            for (int i = 0; i < lastPageIndex; i++)
            {
                end = (i + 1) * pageSize;
                this._pages[i] = new Array64Page<T>(begin, end, colSize, rowSize);
                begin = end;
            }

            if (mod > 0)
            {
                this._pages[lastPageIndex] = new Array64Page<T>(end, length, colSize, rowSize);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="array">Array64</param>
        public Array64(Array64<T> array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            this._length = array._length;
            this._pageSize = array._pageSize;
            this._colSize = array._colSize;
            this._rowSize = array._rowSize;

            this._pages = new Array64Page<T>[array._pages.Length];
            for (int i = 0; i < array._pages.GetLength(0); i++)
            {
                this._pages.SetValue(((Array64Page<T>)array._pages.GetValue(i)).ToPage(), i);
            }
        }

        /// <summary>
        /// 设置数组对象数据,返回实际设置数据长度
        /// </summary>
        /// <param name="offset">当前数组对象中偏移量</param>
        /// <param name="buffer">要设置的源数据数组</param>
        /// <param name="bufferOffset">源数据数组中中的偏移量</param>
        /// <param name="length">源数据数组中要写入的数据长度,如果超出当前数组范围,则以当前数组实际长度为准</param>
        /// <returns>实际设置数据长度</returns>
        public int Set(long offset, T[] buffer, int bufferOffset, int length)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (offset == this._length ||
                buffer == null || buffer.Length == 0 ||
                length < 1)
            {
                return 0;
            }

            if (length > buffer.Length)
            {
                length = buffer.Length;
            }

            if (offset + length > this._length)
            {
                length = (int)(this._length - offset);
            }

            int pageIndex = (int)(offset / this._pageSize);
            int currentSetLength;
            long setBeginIndex = offset;
            for (int i = pageIndex; i < this._pages.Length; i++)
            {
                currentSetLength = this._pages[i].Set(buffer, bufferOffset, setBeginIndex, length);
                length -= currentSetLength;
                bufferOffset += currentSetLength;
                setBeginIndex += currentSetLength;
                if (length <= 0)
                {
                    break;
                }
            }

            return (int)(setBeginIndex - offset);
        }

        /// <summary>
        /// 从指定偏移位置获取指定长度的数组,如果从偏移位置起目标长度超出数组范围,则以实际数组长度为准
        /// </summary>
        /// <param name="offset">当前对象中数据数据偏移量</param>
        /// <param name="length">要获取的数据长度</param>
        /// <returns>获取到的数据数组</returns>
        public T[] Get(long offset, int length)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length == 0 || offset >= this._length)
            {
                return new T[0];
            }

            if (offset + length > this._length)
            {
                length = (int)(this._length - offset);
            }

            T[] buffer = new T[length];
            int bufferOffset = 0, currentGetLength;
            long getIndex = offset;
            int pageIndex = (int)(offset / this._pageSize);
            for (int i = pageIndex; i < this._pages.Length; i++)
            {
                currentGetLength = this._pages[i].Get(buffer, bufferOffset, getIndex, length);
                length -= currentGetLength;
                offset += currentGetLength;
                bufferOffset += currentGetLength;
                if (length <= 0)
                {
                    break;
                }
            }

            return buffer;
        }

        /// <summary>
        /// 创建一个数组
        /// </summary>
        /// <returns>新数组</returns>
        public Array64<T> ToArray()
        {
            return new Array64<T>(this);
        }
    }
}
