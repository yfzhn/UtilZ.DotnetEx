using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// GUID扩展类
    /// </summary>
    public class GUIDEx
    {
        /// <summary>
        /// 获取GUID对应的HskCode
        /// </summary>
        /// <returns>GUID对应的HskCode</returns>
        public static int GetGUIDHashCode()
        {
            return Guid.NewGuid().ToString().Replace("_", string.Empty).GetHashCode();
        }
    }
}
