using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// 目录信息
    /// </summary>
    [Serializable]
    public class DirectoryInfoItem : FileSystemItemInfo
    {
        ///// <summary>
        ///// 获取指定子目录的父目录
        ///// </summary>
        //public DirectoryInfoItem Parent { get; }

        ///// <summary>
        ///// 获取目录的根部分
        ///// </summary>
        //public DirectoryInfoItem Root { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DirectoryInfoItem()
            : base()
        {

        }
    }
}
