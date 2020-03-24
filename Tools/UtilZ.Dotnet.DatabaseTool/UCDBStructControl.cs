using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;
using UtilZ.Dotnet.WindowEx.Winform.Base;
using UtilZ.Dotnet.WindowEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.WindowEx.Winform.Controls.PartAsynWait;
using UtilZ.Dotnet.WindowEx.Winform.Controls.PageGrid;
using UtilZ.Dotnet.Ex.AsynWait;
using System.IO;
using UtilZ.Dotnet.Ex.Model;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.DatabaseTool
{
    public partial class UCDBStructControl : UserControl, ICollectionOwner
    {
        private List<DBTableInfoEx> _tableList;
        private readonly List<DBFieldInfo> _tableFieldList = new List<DBFieldInfo>();

        private readonly BindingCollection<DBTableInfoEx> _tableBindingList;
        private readonly BindingCollection<DBFieldInfo> _fieldInfoBindingList;
        private readonly BindingCollection<DBIndexInfo> _indexInfoBindingList;

        public UCDBStructControl()
        {
            InitializeComponent();

            this._tableBindingList = new BindingCollection<DBTableInfoEx>(this);
            this._fieldInfoBindingList = new BindingCollection<DBFieldInfo>(this);
            this._indexInfoBindingList = new BindingCollection<DBIndexInfo>(this);
            dgvTables.GridControl.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvTableFields.GridControl.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvIndex.GridControl.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        private void UCDBStructControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                DBTableInfoEx.ShowFieldChanged = this.ShowFieldChanged;
                List<DatabaseConfig> items = DatabaseConfigManager.GetAllConfigItems();
                DropdownBoxHelper.BindingIEnumerableGenericToComboBox<DatabaseConfig>(comboBoxDB, items, nameof(DatabaseConfig.ConName));
                dgvTables.ShowData(this._tableBindingList.DataSource, "UCDBStructControl.dgvTables", null, null, new string[] { nameof(DBTableInfoEx.ShowField) });
                dgvTableFields.ShowData(this._fieldInfoBindingList.DataSource, "UCDBStructControl.dgvTableFields");
                dgvIndex.ShowData(this._indexInfoBindingList.DataSource, "UCDBStructControl.dgvIndex");
                this.checkMultTable_CheckedChanged(sender, e);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowFieldChanged(DBTableInfoEx tableInfoEx)
        {
            if (tableInfoEx.ShowField)
            {
                this._tableFieldList.AddRange(tableInfoEx.DbFieldInfos);
            }
            else
            {
                this._tableFieldList.RemoveAll(t => tableInfoEx.DbFieldInfos.Contains(t));
            }

            this.txtTableFieldFilter_TextChanged(this, null);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var dbConfig = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                var para = new PartAsynWaitPara<DatabaseConfig, object>();
                para.Caption = "查询数据库结构";
                para.IsShowCancel = false;
                para.Para = dbConfig;
                para.Function = (p) =>
                {
                    p.AsynWait.Hint = $"正在查询数据库[{dbConfig.ConName}]结构,请稍候...";
                    IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(p.Para.DBID);
                    this._tableList = dbAccess.Database.GetTableInfoList(true).OrderBy(t => { return t.Name; }).Select(t => { return new DBTableInfoEx(t); }).ToList();
                    if (p.AsynWait.InvokeRequired)
                    {
                        p.AsynWait.Invoke(new Action(() =>
                        {
                            this._tableBindingList.Clear();
                            this._tableBindingList.AddRange(this._tableList);
                        }));
                    }
                    else
                    {
                        this._tableBindingList.Clear();
                        this._tableBindingList.AddRange(this._tableList);
                    }

                    return null;
                };

                para.Completed = (ret) =>
                {
                    if (ret.Status == PartAsynExcuteStatus.Exception)
                    {
                        Loger.Error(ret.Exception);
                    }
                };

                PartAsynWaitHelper.Wait(para, this);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvTables_DataRowSelectionChanged(object sender, DataRowSelectionChangedArgs e)
        {
            try
            {
                if (checkMultTable.Checked)
                {
                    return;
                }

                this.ShowDbField();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvTables_DataRowDoubleClick(object sender, DataRowClickArgs e)
        {
            try
            {
                var tableInfo = (DBTableInfoEx)e.Row;
                var frm = new FTableField(tableInfo);
                frm.Show();
                //var tableInfo = (DBTableInfoEx)((DataGridViewRow)e.Row).DataBoundItem;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowDbField()
        {
            var selectedRows = dgvTables.SelectedRows;
            if (selectedRows == null || selectedRows.Length != 1)
            {
                return;
            }

            var tableInfo = (DBTableInfoEx)((DataGridViewRow)selectedRows[0]).DataBoundItem;
            this._tableFieldList.Clear();
            this._tableFieldList.AddRange(tableInfo.DbFieldInfos);
            this.CelarIndexBindingList();
            this._indexInfoBindingList.AddRange(tableInfo.Indexs);
            this.txtTableFieldFilter_TextChanged(this, null);
        }

        private void txtTableFilter_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string filter = txtTableFilter.Text;
                this._tableBindingList.Clear();
                if (string.IsNullOrWhiteSpace(filter))
                {
                    this._tableBindingList.AddRange(this._tableList);
                }
                else
                {
                    //字段忽略大小写相等的
                    var list = this._tableList.Where(t => { return string.Equals(t.Name, filter, StringComparison.OrdinalIgnoreCase); }).ToList();

                    //包含区分大小写
                    var tmpList = this._tableList.Where(t => { return DBTableInfoEx.StringContains(t.Name, filter, false); });
                    var intersectItems = list.Intersect(tmpList);
                    list.RemoveAll(t => intersectItems.Contains(t));
                    list.AddRange(tmpList);

                    //忽略大小写
                    tmpList = this._tableList.Where(t => { return DBTableInfoEx.StringContains(t.Name, filter, true); });
                    intersectItems = list.Intersect(tmpList);
                    list.RemoveAll(t => intersectItems.Contains(t));
                    list.AddRange(tmpList);

                    //备注忽略大小写包含
                    tmpList = this._tableList.Where(t => { return DBTableInfoEx.StringContains(t.Comments, filter, true); });
                    intersectItems = list.Intersect(tmpList);
                    list.RemoveAll(t => intersectItems.Contains(t));
                    list.AddRange(tmpList);

                    this._tableBindingList.AddRange(list);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
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


        private void checkMultTable_CheckedChanged(object sender, EventArgs e)
        {
            string showFiledColName = nameof(DBTableInfoEx.ShowField);
            if (checkMultTable.Checked)
            {
                dgvTables.GridControl.ContextMenuStrip = cmsCheck;
                dgvTables.GridControl.Columns[showFiledColName].ReadOnly = false;
                this._tableFieldList.Clear();
                this._fieldInfoBindingList.Clear();
                this.CelarIndexBindingList();
            }
            else
            {
                dgvTables.GridControl.ContextMenuStrip = null;
                this.tsmiClearCheck_Click(sender, e);
                dgvTables.GridControl.Columns[showFiledColName].ReadOnly = true;
                this.ShowDbField();
            }
        }

        private void tsmiAllCheck_Click(object sender, EventArgs e)
        {
            if (this._tableList == null || this._tableList.Count == 0)
            {
                return;
            }

            DBTableInfoEx.ShowFieldChanged = null;
            this._tableList.ForEach(t => { t.ShowField = true; });
            this.RefreshField();
            DBTableInfoEx.ShowFieldChanged = this.ShowFieldChanged;
        }

        private void tsmiUnCheck_Click(object sender, EventArgs e)
        {
            if (this._tableList == null || this._tableList.Count == 0)
            {
                return;
            }

            DBTableInfoEx.ShowFieldChanged = null;
            this._tableList.ForEach(t => { t.ShowField = !t.ShowField; });
            this.RefreshField();
            DBTableInfoEx.ShowFieldChanged = this.ShowFieldChanged;
        }

        private void tsmiClearCheck_Click(object sender, EventArgs e)
        {
            if (this._tableList == null || this._tableList.Count == 0)
            {
                return;
            }

            DBTableInfoEx.ShowFieldChanged = null;
            this._tableList.ForEach(t => { t.ShowField = false; });
            this._tableFieldList.Clear();
            this._fieldInfoBindingList.Clear();
            DBTableInfoEx.ShowFieldChanged = this.ShowFieldChanged;
        }

        private void CelarIndexBindingList()
        {
            this._indexInfoBindingList.Clear();
            rtxtIndexDetail.Text = string.Empty;
        }

        private void RefreshField()
        {
            this._tableFieldList.Clear();
            this._tableList.ForEach(t =>
            {
                if (t.ShowField)
                {
                    this._tableFieldList.AddRange(t.DbFieldInfos);
                    this._indexInfoBindingList.AddRange(t.Indexs);
                }
            });

            this.txtTableFieldFilter_TextChanged(this, null);
        }

        #region 生成数据模型
        private void tsmiGenerateModel_Click(object sender, EventArgs e)
        {
            var tableInfoArr = this._tableList.Where(t => { return t.ShowField; }).ToArray();
            if (tableInfoArr.Length == 0)
            {
                return;
            }

            var frm = new FModelInfo();
            if (frm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var para = new PartAsynWaitPara<Tuple<string, string, string, DBTableInfoEx[]>, object>();
            para.Caption = "生成model结构";
            para.IsShowCancel = false;
            para.Para = new Tuple<string, string, string, DBTableInfoEx[]>(frm.Namespace, frm.BaseClassName, frm.Dir, tableInfoArr);
            para.Function = this.GenerateModel;

            para.Completed = (ret) =>
            {
                if (ret.Status == PartAsynExcuteStatus.Exception)
                {
                    Loger.Error(ret.Exception);
                }
            };

            PartAsynWaitHelper.Wait(para, this);
        }

        private object GenerateModel(PartAsynFuncPara<Tuple<string, string, string, DBTableInfoEx[]>> para)
        {
            if (!Directory.Exists(para.Para.Item3))
            {
                Directory.CreateDirectory(para.Para.Item3);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < para.Para.Item4.Length; i++)
            {
                var tableInfo = para.Para.Item4[i];
                para.AsynWait.Hint = $"正在生成[({tableInfo.Name}){i + 1}/{para.Para.Item4.Length}]表模型...";
                sb.Clear();

                //命名空间
                sb.AppendLine(@"using System;");
                sb.AppendLine();
                sb.Append("namespace ");
                sb.AppendLine(para.Para.Item1);
                sb.AppendLine(@"{");

                //类注释
                sb.AppendLine(@"    /// <summary>");
                sb.Append(@"    /// ");
                sb.AppendLine(tableInfo.Comments);
                sb.AppendLine(@"    /// </summary>");
                sb.Append(@"    [App.Service.Common.Attributes.Table(""");
                sb.Append(tableInfo.Name);
                sb.AppendLine(@""")]");

                //类名
                sb.Append(@"    public class ");
                if (string.IsNullOrWhiteSpace(para.Para.Item2))
                {
                    sb.AppendLine(tableInfo.Name);
                }
                else
                {
                    sb.Append(tableInfo.Name);
                    sb.Append(" : ");
                    sb.AppendLine(para.Para.Item2);
                }

                sb.AppendLine(@"    {");

                foreach (var fieldInfo in tableInfo.DbFieldInfos)
                {
                    //字段注释
                    sb.AppendLine(@"        /// <summary>");
                    sb.Append(@"        /// ");
                    sb.AppendLine(fieldInfo.Comments);
                    sb.AppendLine(@"        /// </summary>");

                    //主键标识
                    if (fieldInfo.IsPriKey)
                    {
                        sb.AppendLine(@"        [Common.Attributes.Key]");
                    }

                    //属性
                    sb.Append(@"        public ");
                    sb.Append(this.GetFiledClrTypeString(fieldInfo));
                    sb.Append(" ");
                    sb.Append(fieldInfo.FieldName);
                    sb.AppendLine(@" { get; set; }");
                    sb.AppendLine();
                }

                //默认构造函数
                sb.AppendLine(@"        /// <summary>");
                sb.AppendLine(@"        /// 默认构造函数");
                sb.AppendLine(@"        /// </summary>");
                sb.Append(@"        public ");
                sb.Append(tableInfo.Name);
                sb.AppendLine("()");
                sb.AppendLine("        {");
                sb.AppendLine();
                sb.AppendLine("        }");

                sb.AppendLine(@"    }");
                sb.AppendLine(@"}");

                var filePath = Path.Combine(para.Para.Item3, tableInfo.Name + ".cs");
                File.WriteAllText(filePath, sb.ToString());
            }

            return null;
        }

        private string GetFiledClrTypeString(DBFieldInfo fieldInfo)
        {
            string typeStr;
            DBFieldType dbFieldType = DBHelper.GetDbClrFieldType(fieldInfo.DataType);
            switch (dbFieldType)
            {
                case DBFieldType.Binary:
                    typeStr = "byte[]";
                    break;
                case DBFieldType.DateTime:
                    typeStr = fieldInfo.DataType.Name;
                    break;
                case DBFieldType.Number:
                    if (fieldInfo.FieldName.EndsWith("ID", StringComparison.OrdinalIgnoreCase) ||
               fieldInfo.FieldName.EndsWith("no", StringComparison.OrdinalIgnoreCase))
                    {
                        typeStr = "long";
                    }
                    else
                    {
                        if (fieldInfo.DataType == ClrSystemType.DecimalType)
                        {
                            typeStr = "double";
                        }
                        else
                        {
                            typeStr = fieldInfo.DataType.Name.ToLower();
                        }
                    }
                    break;
                case DBFieldType.String:
                    typeStr = "string";
                    break;
                case DBFieldType.Other:
                default:
                    typeStr = fieldInfo.DataType.Name.ToLower();
                    break;
            }

            return typeStr;
        }
        #endregion

        private void dgvIndex_DataRowSelectionChanged(object sender, DataRowSelectionChangedArgs e)
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
