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
    public partial class FParaGroupManager : Form
    {
        public FParaGroupManager()
        {
            InitializeComponent();
        }

        private readonly ConfigBLL _configLogic;
        private readonly BindingListEx<ConfigParaGroup> _bindItems = new BindingListEx<ConfigParaGroup>();
        private List<ConfigParaGroup> _srcItems;
        public FParaGroupManager(ConfigBLL configLogic) : this()
        {
            this._configLogic = configLogic;
        }

        private void FParaGroupManager_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                this._srcItems = this._configLogic.GetAllConfigParaGroup();
                this._bindItems.AddRange(this._srcItems);
                this.pgGroup.ShowData(this._bindItems, "FParaGroupManager.ConfigParaGroup", null, null, new ConfigParaGroup().GetAllowEditColumns());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }
        }

        private void tsmiAdd_Click(object sender, EventArgs e)
        {
            this._bindItems.Add(new ConfigParaGroup());
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pgGroup.SelectedRows.Length == 0)
                {
                    return;
                }

                var delItems = (from selectedRow in this.pgGroup.SelectedRows select (ConfigParaGroup)((DataGridViewRow)selectedRow).DataBoundItem).ToList();
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
                List<ConfigParaGroup> addItems = (from tmpItem in this._bindItems where !this._srcItems.Contains(tmpItem) select tmpItem).ToList();
                List<ConfigParaGroup> delItems = (from tmpItem in this._srcItems where !this._bindItems.Contains(tmpItem) select tmpItem).ToList();
                List<ConfigParaGroup> updateItems = (from tmpItem in this._bindItems where !addItems.Contains(tmpItem) && !delItems.Contains(tmpItem) select tmpItem).ToList();
                this._configLogic.SaveGroup(addItems, delItems, updateItems);
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
