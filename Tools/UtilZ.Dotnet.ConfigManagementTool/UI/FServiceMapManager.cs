using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using UtilZ.Dotnet.ConfigManagementTool.BLL;
using UtilZ.Dotnet.ConfigManagementTool.Model;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.ConfigManagementTool.UI
{
    public partial class FServiceMapManager : Form
    {
        public FServiceMapManager()
        {
            InitializeComponent();
        }

        private readonly ConfigBLL _configLogic;
        private readonly BindingListEx<ConfigParaServiceMap> _bindItems = new BindingListEx<ConfigParaServiceMap>();
        private List<ConfigParaServiceMap> _srcItems;
        public FServiceMapManager(ConfigBLL configLogic) : this()
        {
            this._configLogic = configLogic;
        }

        private void FServiceMapManager_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                this._srcItems = this._configLogic.GetAllConfigParaServiceMap();
                this._bindItems.AddRange(this._srcItems);
                this.pgServiceMap.ShowData(this._bindItems, "FServiceMapManager.ConfigParaServiceMap", null, null, new ConfigParaServiceMap().GetAllowEditColumns());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }
        }

        private void tsmiAdd_Click(object sender, EventArgs e)
        {
            this._bindItems.Add(new ConfigParaServiceMap());
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pgServiceMap.SelectedRows.Length == 0)
                {
                    return;
                }

                var delItems = (from selectedRow in this.pgServiceMap.SelectedRows select (ConfigParaServiceMap)((DataGridViewRow)selectedRow).DataBoundItem).ToList();
                this._bindItems.RemoveArrange(delItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<ConfigParaServiceMap> addItems = (from tmpItem in this._bindItems where !this._srcItems.Contains(tmpItem) select tmpItem).ToList();
                List<ConfigParaServiceMap> delItems = (from tmpItem in this._srcItems where !this._bindItems.Contains(tmpItem) select tmpItem).ToList();
                List<ConfigParaServiceMap> updateItems = (from tmpItem in this._bindItems where !addItems.Contains(tmpItem) && !delItems.Contains(tmpItem) select tmpItem).ToList();
                this._configLogic.SaveConfigParaServiceMap(addItems, delItems, updateItems);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }
        }
    }
}
