using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.ConfigManagementTool.Model
{
    public class ConfigParaKeyValue2 : ConfigParaKeyValue
    {
        public ConfigParaKeyValue2(ConfigParaKeyValue value, bool isSelected) : base(value)
        {
            this._isSelected = isSelected;
        }

        private bool _isSelected = false;
        [DisplayName("是否应用")]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.OnRaisePropertyChanged("IsSelected");
            }
        }
    }
}
