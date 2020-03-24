using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.ConfigManagementTool.Model
{
    public class ConfigParaServiceMap2
    {
        public ConfigParaServiceMap2(ConfigParaServiceMap map)
        {
            this.Name = map.Name;
            this.ServiceMapID = map.ServiceMapID;
            this.Des = map.Des;
        }

        /// <summary>
        /// 组名称
        /// </summary>
        [DisplayName("映射名称")]
        public string Name { get; set; }

        /// <summary>
        /// 外部服务映射
        /// </summary>
        [DisplayName("服务映射ID")]
        public int ServiceMapID { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DisplayName("映射描述")]
        public string Des { get; set; }
    }
}
