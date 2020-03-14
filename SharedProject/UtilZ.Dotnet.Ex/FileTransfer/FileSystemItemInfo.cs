using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// 文件信息基类
    /// </summary>
    [Serializable]
    public abstract class FileSystemItemInfo
    {
        /// <summary>
        /// 获取或设置当前文件或目录的创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 获取目录或文件的完整目录
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 对于文件，获取该文件的名称。对于目录，如果存在层次结构，则获取层次结构中最后一个目录的名称。否则，Name 属性获取该目录的名称
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 获取或设置上次访问当前文件或目录的时间
        ///// </summary>
        //public DateTime LastAccessTime { get; set; }

        ///// <summary>
        ///// 获取或设置上次写入当前文件或目录的时间
        ///// </summary>
        //public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileSystemItemInfo()
        {

        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
