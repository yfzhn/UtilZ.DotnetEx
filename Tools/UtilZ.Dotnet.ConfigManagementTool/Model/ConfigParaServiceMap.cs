using System;
using System.ComponentModel;
using UtilZ.Dotnet.DBIBase.DBModel.DBObject;
using UtilZ.Dotnet.Ex.Model;

namespace UtilZ.Dotnet.ConfigManagementTool.Model
{
    /// <summary>
    /// 配置参数作用域
    /// </summary>
    [Serializable]
    [DBTable("ConfigParaServiceMap")]
    public class ConfigParaServiceMap : BaseModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigParaServiceMap()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigParaServiceMap(ConfigParaServiceMap service)
        {
            this._ID = service.ID;
            this._serviceMapID = service.ServiceMapID;
            this._name = service.Name;
            this._des = service.Des;
        }

        private int _ID;

        /// <summary>
        /// 主键ID
        /// </summary>
        [DBColumn("ID", true, DBFieldDataAccessType.R)]
        [Browsable(false)]
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                this.OnRaisePropertyChanged("ID");
            }
        }

        private int _serviceMapID;

        /// <summary>
        /// 外部服务映射
        /// </summary>
        [DBColumn("ServiceMapID")]
        [DisplayName("服务映射ID")]
        public int ServiceMapID
        {
            get { return _serviceMapID; }
            set
            {
                _serviceMapID = value;
                this.OnRaisePropertyChanged("ServiceMapID");
            }
        }

        private string _name;

        /// <summary>
        /// 组名称
        /// </summary>
        [DBColumn("Name")]
        [DisplayName("映射名称")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.OnRaisePropertyChanged("Name");
            }
        }

        private string _des;

        /// <summary>
        /// 描述
        /// </summary>
        [DBColumn("Des")]
        [DisplayName("映射描述")]
        public string Des
        {
            get { return _des; }
            set
            {
                _des = value;
                this.OnRaisePropertyChanged("Des");
            }
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return _name;
        }
    }
}
