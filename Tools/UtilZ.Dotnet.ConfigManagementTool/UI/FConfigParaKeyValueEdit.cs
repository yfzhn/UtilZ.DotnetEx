using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using UtilZ.Dotnet.ConfigManagementTool.BLL;
using UtilZ.Dotnet.ConfigManagementTool.Model;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;
using UtilZ.Dotnet.WindowEx.Winform.Base;

namespace UtilZ.Dotnet.ConfigManagementTool.UI
{
    public partial class FConfigParaKeyValueEdit : Form
    {
        public FConfigParaKeyValueEdit()
        {
            InitializeComponent();
        }

        private readonly ConfigBLL _configLogic;
        private readonly ConfigParaKeyValue _configParaKeyValue;
        public FConfigParaKeyValueEdit(ConfigBLL configLogic, ConfigParaKeyValue configParaKeyValue) : this()
        {
            this._configLogic = configLogic;
            this._configParaKeyValue = configParaKeyValue;
        }

        private readonly BindingListEx<ConfigParaServiceMap3> _serviceList = new BindingListEx<ConfigParaServiceMap3>();
        private void FConfigParaKeyValueEdit_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                this._serviceList.AddRange(this._configLogic.GetConfigParaServiceMap(this._configParaKeyValue.ID));
                pgValidDomain.ShowData(this._serviceList, "FConfigParaKeyValueEdit.ConfigParaServiceMap", null, null, new string[] { nameof(ConfigParaServiceMap3.IsSelected) });
                List<ConfigParaGroup> groups = this._configLogic.GetAllConfigParaGroup();
                DropdownBoxHelper.BindingIEnumerableGenericToComboBox<ConfigParaGroup>(comGroup, groups, nameof(ConfigParaGroup.Name), this._configParaKeyValue.Group);
                this.txtKey.Text = this._configParaKeyValue.Key;
                this.rtxtValue.Text = this._configParaKeyValue.Value;
                this.rtxtDes.Text = this._configParaKeyValue.Des;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = DropdownBoxHelper.GetGenericFromComboBox<ConfigParaGroup>(comGroup);
                if (selectedItem == null)
                {
                    MessageBox.Show("参数所属组不能为空");
                    return;
                }

                List<int> validDomainIds = (from tmpItem in this._serviceList where tmpItem.IsSelected select tmpItem.ID).ToList();
                if (validDomainIds.Count == 0)
                {
                    MessageBox.Show("参数作用域范围至少需要有一个服务");
                    return;
                }

                var configParaKeyValue = new ConfigParaKeyValue();
                configParaKeyValue.ID = this._configParaKeyValue.ID;
                configParaKeyValue.Key = this._configParaKeyValue.Key;
                configParaKeyValue.Value = rtxtValue.Text;
                configParaKeyValue.GID = selectedItem.ID;
                configParaKeyValue.Des = rtxtDes.Text;

                //修改
                this._configLogic.ModifyConfigParaKeyValue(configParaKeyValue, validDomainIds);

                //更新数据源
                this._configParaKeyValue.Value = configParaKeyValue.Value;
                this._configParaKeyValue.Des = configParaKeyValue.Des;
                this._configParaKeyValue.Group = selectedItem;
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
