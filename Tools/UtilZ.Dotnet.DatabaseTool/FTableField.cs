using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DatabaseTool
{
    public partial class FTableField : Form, ICollectionOwner
    {
        private readonly DBTableInfoEx _tableInfoEx;
        private readonly List<DBFieldInfo> _tableFieldList = new List<DBFieldInfo>();
        private readonly BindingCollection<DBFieldInfo> _fieldInfoBindingList;
        private readonly BindingCollection<DBIndexInfo> _indexInfoBindingList;

        public FTableField()
        {
            InitializeComponent();
        }

        internal FTableField(DBTableInfoEx tableInfoEx)
            : this()
        {
            this.Text = tableInfoEx.Name;
            this._tableInfoEx = tableInfoEx;
            this._tableFieldList.AddRange(tableInfoEx.DbFieldInfos);
            this._fieldInfoBindingList = new BindingCollection<DBFieldInfo>(this);
            this._indexInfoBindingList = new BindingCollection<DBIndexInfo>(this);
            this._indexInfoBindingList.AddRange(tableInfoEx.Indexs);

            this.txtTableFieldFilter_TextChanged(this, null);
            dgvTableFields.GridControl.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvIndex.GridControl.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        private void FTableField_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                dgvTableFields.ShowData(this._fieldInfoBindingList.DataSource, "FTableField.dgvTableFields");
                dgvIndex.ShowData(this._indexInfoBindingList.DataSource, "FTableField.dgvIndex");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void txtTableFieldFilter_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string filter = txtTableFieldFilter.Text;
                this._fieldInfoBindingList.Clear();
                if (string.IsNullOrWhiteSpace(filter))
                {
                    this._fieldInfoBindingList.AddRange(this._tableFieldList);
                }
                else
                {
                    //字段忽略大小写相等的
                    var list = this._tableFieldList.Where(t => { return string.Equals(t.FieldName, filter, StringComparison.OrdinalIgnoreCase); }).ToList();

                    //包含区分大小写
                    var tmpList = this._tableFieldList.Where(t => { return DBTableInfoEx.StringContains(t.FieldName, filter, false); });
                    var intersectItems = list.Intersect(tmpList);
                    list.RemoveAll(t => intersectItems.Contains(t));
                    list.AddRange(tmpList);

                    //忽略大小写
                    tmpList = this._tableFieldList.Where(t => { return DBTableInfoEx.StringContains(t.FieldName, filter, true); });
                    intersectItems = list.Intersect(tmpList);
                    list.RemoveAll(t => intersectItems.Contains(t));
                    list.AddRange(tmpList);

                    //备注忽略大小写包含
                    tmpList = this._tableFieldList.Where(t => { return DBTableInfoEx.StringContains(t.Comments, filter, true); });
                    intersectItems = list.Intersect(tmpList);
                    list.RemoveAll(t => intersectItems.Contains(t));
                    list.AddRange(tmpList);

                    this._fieldInfoBindingList.AddRange(list);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void dgvIndex_DataRowSelectionChanged(object sender, WindowEx.Winform.Controls.PageGrid.DataRowSelectionChangedArgs e)
        {
            try
            {
                var selectedRows = dgvIndex.SelectedRows;
                if (selectedRows == null || selectedRows.Length != 1)
                {
                    return;
                }

                var indexInfo = (DBIndexInfo)((DataGridViewRow)selectedRows[0]).DataBoundItem;
                rtxtIndexDetail.Text = indexInfo.Detail;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
