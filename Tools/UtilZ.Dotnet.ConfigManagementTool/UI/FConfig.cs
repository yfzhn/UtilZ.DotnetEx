using System;
using System.Windows.Forms;
using UtilZ.Dotnet.ConfigManagementTool.BLL;
using UtilZ.Dotnet.ConfigManagementTool.UI.UCViews;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.ConfigManagementTool.UI
{
    public partial class FConfig : Form
    {
        private readonly ConfigBLL _configLogic = new ConfigBLL();
        private readonly UCParaView _paraView = new UCParaView();
        private readonly UCServiceView _serviceView = new UCServiceView();
        private UCViewBase _currentView = null;

        public FConfig()
        {
            InitializeComponent();
        }

        private void RefreshParaGroup()
        {
            this._currentView.RefreshData();
        }

        private void FConfig_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                this._configLogic.Init();
                this._paraView.ConfigLogic = this._configLogic;
                this._paraView.Dock = DockStyle.Fill;

                this._serviceView.ConfigLogic = this._configLogic;
                this._serviceView.Dock = DockStyle.Fill;

                this._currentView = this._paraView;
                this.panelView.Controls.Add(this._currentView);
                this.RefreshParaGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }
        }

        private void tsbServiceMapManager_Click(object sender, EventArgs e)
        {
            var frm = new FServiceMapManager(this._configLogic);
            frm.ShowDialog();
        }

        private void tsbParaGroupManager_Click(object sender, EventArgs e)
        {
            var frm = new FParaGroupManager(this._configLogic);
            frm.ShowDialog();
            this.RefreshParaGroup();
        }

        private void tsbConfigParaManager_Click(object sender, EventArgs e)
        {
            var frm = new FParaManager(this._configLogic);
            frm.ShowDialog();
            this.RefreshParaGroup();
        }

        private void tsbViewSwitch_Click(object sender, EventArgs e)
        {
            if (this._currentView == this._paraView)
            {
                this._currentView = this._serviceView;
            }
            else
            {
                this._currentView = this._paraView;
            }

            this.panelView.Controls.Clear();
            this.panelView.Controls.Add(this._currentView);
            this.RefreshParaGroup();
        }
    }
}
