using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.ConfigManagementTool.BLL;

namespace UtilZ.Dotnet.ConfigManagementTool.UI.UCViews
{
    public partial class UCViewBase : UserControl
    {
        public UCViewBase()
        {
            InitializeComponent();
        }

        protected ConfigBLL _configLogic;

        public ConfigBLL ConfigLogic
        {
            get { return _configLogic; }
            set { _configLogic = value; }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public virtual void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}
