using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 数据页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable, ComVisible(true)]
    public class Array64Page<T>
    {
        /// <summary>
        /// 当前页存储数据起始位置
        /// </summary>
        private readonly long _begin;

        /// <summary>
        /// 当前页存储数据结束位置
        /// </summary>
        private readonly long _end;

        /// <summary>
        /// 数据页列大小
        /// </summary>
        private readonly int _colSize;

        /// <summary>
        /// 数据页行大小
        /// </summary>
        private readonly int _rowSize;

        /// <summary>
        /// 数据二维数据
        /// </summary>
        private readonly T[][] _data;

        /// <summary>
        /// 获取或设置指定索引处的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>指定索引处的值</returns>
        public T this[long index]
        {
            get
            {
                int rowIndex, colIndex;
                this.GetRowColIndex(index, out rowIndex, out colIndex);
                return this._data[rowIndex][colIndex];
            }
            set
            {
                int rowIndex, colIndex;
                this.GetRowColIndex(index, out rowIndex, out colIndex);
                this._data[rowIndex][colIndex] = value;
            }
        }

        private void GetRowColIndex(long index, out int rowIndex, out int colIndex)
        {
            long currentPostion = index - this._begin;
            if (currentPostion >= this._colSize)
            {
                rowIndex = (int)(currentPostion / this._colSize);
                colIndex = (int)(currentPostion % this._colSize);
            }
            else
            {
                rowIndex = 0;
                colIndex = (int)currentPostion;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="begin">当前页存储数据起始位置</param>
        /// <param name="end">当前页存储数据结束位置</param>
        /// <param name="colSize">数据页列大小</param>
        /// <param name="rowSize">数据页行大小</param>
        public Array64Page(long begin, long end, int colSize, int rowSize)
        {
            this._begin = begin;
            this._end = end;
            this._colSize = colSize;
            this._rowSize = rowSize;

            long length = end - begin;
            long rowCount = length / colSize;
            int mod = (int)(length % colSize);
            if (mod > 0)
            {
                rowCount += 1;
            }

            this._data = new T[rowCount][];
            long lastIndex = rowCount - 1;

            int rowlength = colSize;
            for (int i = 0; i < rowCount; i++)
            {
                if (i == lastIndex && mod > 0)
                {
                    rowlength = mod;
                }

                try
                {
                    this._data[i] = new T[rowlength];
                }
                catch (OutOfMemoryException ex)
                {
                    throw new Exception("当前可用物理内存不足", ex);
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="page">Array64Page</param>
        public Array64Page(Array64Page<T> page)
        {
            this._begin = page._begin;
            this._end = page._end;
            this._colSize = page._colSize;
            this._rowSize = page._rowSize;
            int rowCount = page._data.GetLength(0);
            this._data = new T[rowCount][];
            for (int i = 0; i < rowCount; i++)
            {
                this._data[i] = new T[page._data[i].Length];
            }
        }

        /// <summary>
        /// 从指定偏移位置获取指定长度的数组,如果从偏移位置起目标长度超出数组范围,则以实际数组长度为准
        /// </summary>
        /// <param name="destBuffer">目标数组对象</param>
        /// <param name="destOffset">目标数组对象中数据写入偏移量</param>
        /// <param name="beginIndex">当前数据页中起始位置索引</param>
        /// <param name="length">要写的数据长度</param>
        /// <returns>实际写入数据长度</returns>
        internal int Get(T[] destBuffer, int destOffset, long beginIndex, int length)
        {
            int needReadLength = destBuffer.Length - destOffset;
            long currentNeedOffset = beginIndex - this._begin;
            int pageIndex = (int)(currentNeedOffset / this._colSize);
            int mod = (int)(currentNeedOffset % this._colSize);
            int rowCount = this._data.GetLength(0);
            T[] rowBuffer;
            int readLength = 0;
            for (int i = pageIndex; i < rowCount; i++)
            {
                rowBuffer = this._data[i];
                if (i == pageIndex && mod > 0)
                {
                    int availableLenth = rowBuffer.Length - mod;
                    if (needReadLength <= availableLenth)
                    {
                        Array.Copy(rowBuffer, mod, destBuffer, destOffset, needReadLength);
                        readLength += needReadLength;
                        needReadLength -= needReadLength;
                        destOffset += needReadLength;
                        break;
                    }
                    else
                    {
                        Array.Copy(rowBuffer, mod, destBuffer, destOffset, availableLenth);
                        readLength += availableLenth;
                        destOffset += availableLenth;
                        needReadLength -= availableLenth;
                    }
                }
                else
                {
                    if (needReadLength <= rowBuffer.Length)
                    {
                        Array.Copy(rowBuffer, 0, destBuffer, destOffset, needReadLength);
                        readLength += needReadLength;
                        needReadLength -= needReadLength;
                        destOffset += needReadLength;
                        break;
                    }
                    else
                    {
                        Array.Copy(rowBuffer, 0, destBuffer, destOffset, rowBuffer.Length);
                        needReadLength -= rowBuffer.Length;
                        readLength += rowBuffer.Length;
                        destOffset += rowBuffer.Length;
                    }
                }
            }

            return readLength;
        }

        /// <summary>
        /// 设置数组对象数据,返回实际设置数据长度
        /// </summary>
        /// <param name="buffer">要设置的源数据数组</param>
        /// <param name="offset">源数据数组中中的偏移量</param>
        /// <param name="beginIndex">当前数组对象中的起始位置索引</param>
        /// <param name="length">源数据数组中要写入的数据长度,如果超出当前数组范围,则以当前数组实际长度为准</param>
        /// <returns></returns>
        internal int Set(T[] buffer, int offset, long beginIndex, long length)
        {
            long currentNeedOffset = beginIndex - this._begin;
            long currentTotalOffset = 0;
            int rowCount = this._data.GetLength(0);
            int rowLength;
            for (int i = 0; i < rowCount; i++)
            {
                rowLength = this._data[i].Length;
                if (currentTotalOffset + rowLength >= currentNeedOffset)//如果当前页总共偏移量已经大于等于当前页需要偏移的量,则找到起始行
                {
                    int needWriteLength = buffer.Length - offset;//还需要写的数据长度
                    int currentRowWriteBeginPosition = (int)(currentNeedOffset - currentTotalOffset);//起始行写数据起始位置
                    var row = this._data[i];//起始行数据
                    int currentRowWriteLength = rowLength - currentRowWriteBeginPosition;//当前行可写长度
                    if (needWriteLength <= currentRowWriteLength)//如果需要写的数据长度小于等于起始行可写数据长度,则只写起始行就可以了
                    {
                        Array.Copy(buffer, offset, row, currentRowWriteBeginPosition, needWriteLength);
                        return needWriteLength;
                    }
                    else
                    {
                        int writeLength = 0;//总代写的数据长度
                        Array.Copy(buffer, offset, row, currentRowWriteBeginPosition, currentRowWriteLength);
                        writeLength += currentRowWriteLength;
                        offset += currentRowWriteLength;

                        for (int j = i + 1; j < rowCount; j++)
                        {
                            row = this._data[j];
                            rowLength = row.Length;
                            needWriteLength = buffer.Length - offset;//还需要写的数据长度
                            if (needWriteLength <= rowLength)
                            {
                                Array.Copy(buffer, offset, row, 0, needWriteLength);
                                writeLength += needWriteLength;
                                return writeLength;
                            }
                            else
                            {
                                Array.Copy(buffer, offset, row, 0, rowLength);
                                writeLength += rowLength;
                                offset += rowLength;
                            }
                        }

                        return writeLength;
                    }
                }
                else
                {
                    currentTotalOffset += rowLength;
                }
            }

            return 0;
        }

        /// <summary>
        /// 创建一个数组页
        /// </summary>
        /// <returns>新数据组页</returns>
        public Array64Page<T> ToPage()
        {
            return new Array64Page<T>(this);
        }
    }
}
