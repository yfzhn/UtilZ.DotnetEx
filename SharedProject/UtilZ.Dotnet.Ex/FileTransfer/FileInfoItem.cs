using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [Serializable]
    public class FileInfoItem : FileSystemItemInfo
    {
        ///// <summary>
        ///// 获取表示文件扩展名部分的字符串
        ///// </summary>
        //public string Extension { get; set; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileInfoItem()
            : base()
        {

        }
    }
}
