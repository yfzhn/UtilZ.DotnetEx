using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// 更新模式
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// 指定操作系统应创建新文件。如果文件已存在，它将被覆盖这需要 System.Security.Permissions.FileIOPermissionAccess.Write
        /// </summary>
        Create = 1,

        /// <summary>
        /// 若存在文件，则打开该文件并查找到文件尾，或者创建一个新文件。这需要 System.Security.Permissions.FileIOPermissionAccess.Append权限
        /// </summary>
        Append = 6,
    }
}
