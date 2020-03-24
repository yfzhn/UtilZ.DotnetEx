using System;
using System.ComponentModel;
using UtilZ.Dotnet.DBIBase.DBModel.DBObject;
using UtilZ.Dotnet.Ex.Model;

namespace UtilZ.Dotnet.ConfigManagementTool.Model
{
    /// <summary>
    /// 配置参数组模型类
    /// </summary>
    [Serializable]
    [DBTable("ConfigParaKeyValue")]
    public class ConfigParaKeyValue : BaseModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigParaKeyValue()
        {
            this._ID = -1;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigParaKeyValue(ConfigParaKeyValue value)
        {
            this._ID = value.ID;
            this._key = value.Key;
            this._value = value.Value;
            this._GID = value.GID;
            this._group = value.Group;
            this._des = value.Des;
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

        private string _key;

        /// <summary>
        /// 参数Key
        /// </summary>
        [DBColumn("Key")]
        [DisplayName("Key")]
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                this.OnRaisePropertyChanged("Key");
            }
        }

        private string _value;

        /// <summary>
        /// 组名称
        /// </summary>
        [DBColumn("Value")]
        [DisplayName("Value")]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                this.OnRaisePropertyChanged("Value");
            }
        }

        private int _GID;

        /// <summary>
        /// 配置参数组ID
        /// </summary>
        [DBColumn("GID")]
        [Browsable(false)]
        public int GID
        {
            get { return _GID; }
            set
            {
                _GID = value;
            }
        }

        private ConfigParaGroup _group;

        /// <summary>
        /// 配置参数组
        /// </summary>
        [Browsable(false)]
        public ConfigParaGroup Group
        {
            get { return _group; }
            set
            {
                _group = value;
                _GID = _group == null ? -1 : _group.ID;
                this.OnRaisePropertyChanged("GroupName");
            }
        }

        /// <summary>
        /// 配置参数组
        /// </summary>
        [DisplayName("配置参数组")]
        public string GroupName
        {
            get
            {
                return _group == null ? string.Empty : _group.Name;
            }
        }

        private string _des;

        /// <summary>
        /// 描述
        /// </summary>
        [DBColumn("Des")]
        [DisplayName("描述")]
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
            return string.Format("Key:{0};Value:{1}", _key, _value);
        }
    }
}
