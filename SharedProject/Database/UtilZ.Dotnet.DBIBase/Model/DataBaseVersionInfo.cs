using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 数据库版本信息
    /// </summary>
    public class DataBaseVersionInfo
    {
        /// <summary>
        /// 数据库大版本号
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// 数据库版本信息
        /// </summary>
        public string VersionInfo { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="version"></param>
        /// <param name="versionInfo"></param>
        public DataBaseVersionInfo(int version, string versionInfo)
        {
            this.Version = version;
            this.VersionInfo = versionInfo;
        }

        /// <summary>
        /// 重写GetHashCode
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            if (this.VersionInfo == null)
            {
                return base.GetHashCode();
            }
            else
            {
                return this.VersionInfo.GetHashCode();
            }
        }
    }
}
