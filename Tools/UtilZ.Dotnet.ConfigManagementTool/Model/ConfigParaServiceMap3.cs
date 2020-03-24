using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.ConfigManagementTool.Model
{
    public class ConfigParaServiceMap3 : ConfigParaServiceMap
    {
        public ConfigParaServiceMap3(ConfigParaServiceMap service, bool isSelected) : base(service)
        {
            _isSelected = isSelected;
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
