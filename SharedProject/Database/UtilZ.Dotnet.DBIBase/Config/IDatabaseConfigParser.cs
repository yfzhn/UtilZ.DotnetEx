using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UtilZ.Dotnet.DBIBase.Config
{
    /// <summary>
    /// 数据库连接信息解密接口
    /// </summary>
    public interface IDatabaseConfigParser
    {
        /// <summary>
        /// 解析数据库配置项
        /// </summary>
        /// <param name="ele">xml配置节点</param>
        /// <returns>数据库配置对象集合</returns>
        IEnumerable<DatabaseConfig> Parse(XElement ele);
    }
}
