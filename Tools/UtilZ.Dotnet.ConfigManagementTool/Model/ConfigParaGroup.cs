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
    [DBTable("ConfigParaGroup")]
    public class ConfigParaGroup : BaseModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigParaGroup()
        {

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

        private string _name;

        /// <summary>
        /// 组名称
        /// </summary>
        [DBColumn("Name")]
        [DisplayName("组名")]
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

        /// <summary>
        /// 重写Equals
        /// </summary>
        /// <param name="obj">)</param>
        /// <returns>Equals</returns>
        public override bool Equals(object obj)
        {
            var exObj = obj as ConfigParaGroup;
            if (exObj == null)
            {
                return false;
            }

            return this.ID == exObj.ID;
        }

        /// <summary>
        /// 重写GetHashCode
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}
