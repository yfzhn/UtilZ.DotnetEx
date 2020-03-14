using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;

namespace DotnetWinFormApp
{
    public partial class FTestPRopertyGrid : Form
    {
        public FTestPRopertyGrid()
        {
            InitializeComponent();
        }

        private readonly DemoModel _demoModel = new DemoModel();
        //private readonly PersonDemo _personDemo = new PersonDemo();

        //private readonly PersonDemo[] _personDemos = new PersonDemo[3] { new PersonDemo(), new PersonDemo(), new PersonDemo() };

        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            //_personDemo.CollectionEditCompleted += _personDemo_CollectionEditCompleted;
            //PropertyGridHelper.SetSelectedObject(propertyGrid1, _personDemo);

            //PropertyGridHelper.SetSelectedObject(propertyGrid1, _demoModel);



            //PropertyGridHelper.SetSelectedObject(propertyGrid1, _personDemos);

            //propertyGrid1.SelectedObjectsChanged += new EventHandler(propertyGrid1_SelectedObjectsChanged);
            //propertyGrid1.SelectedObject = new PersonDemo();
            //propertyGrid1.SelectedObject = _demoModel;
        }

        private void _personDemo_CollectionEditCompleted(object sender, EventArgs e)
        {
            PropertyGridHelper.SetSelectedObject(propertyGrid1, null);
            //PropertyGridHelper.SetSelectedObject(propertyGrid1, _personDemo);
        }

        void propertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
        {
            propertyGrid1.Tag = propertyGrid1.PropertySort;
            propertyGrid1.PropertySort = PropertySort.CategorizedAlphabetical;
            propertyGrid1.Paint += new PaintEventHandler(propertyGrid1_Paint);
        }

        void propertyGrid1_Paint(object sender, PaintEventArgs e)
        {
            PropertyGrid propertyGrid = (PropertyGrid)sender;
            if (propertyGrid.SelectedObject == null)
            {
                return;
            }

            if (propertyGrid.SelectedObject.GetType().GetInterface(typeof(IPropertyGridCategoryOrder).FullName) == null)
            {
                return;
            }

            IPropertyGridCategoryOrder propertyGridCategoryOrder = (IPropertyGridCategoryOrder)propertyGrid.SelectedObject;
            List<string> propertyGridCategoryNames = propertyGridCategoryOrder.PropertyGridCategoryNames;
            switch (propertyGridCategoryOrder.OrderType)
            {
                case PropertyGridOrderType.Ascending:
                    propertyGridCategoryNames = (from tmpItem in propertyGridCategoryNames orderby tmpItem ascending select tmpItem).ToList();
                    break;
                case PropertyGridOrderType.Descending:
                    propertyGridCategoryNames = (from tmpItem in propertyGridCategoryNames orderby tmpItem descending select tmpItem).ToList();
                    break;
                case PropertyGridOrderType.Custom:
                    break;
            }

            GridItemCollection currentPropEntries = propertyGrid.GetType().GetField("currentPropEntries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(propertyGrid1) as GridItemCollection;
            propertyGrid.CollapseAllGridItems();
            var newarray = currentPropEntries.Cast<GridItem>().OrderBy((t) => propertyGridCategoryNames.IndexOf(t.Label)).ToArray();
            currentPropEntries.GetType().GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentPropEntries, newarray);
            propertyGrid.ExpandAllGridItems();
            propertyGrid.PropertySort = (PropertySort)propertyGrid.Tag;
            propertyGrid.Paint -= new PaintEventHandler(propertyGrid1_Paint);
        }

        void propertyGrid1_Paint_bk(object sender, PaintEventArgs e)
        {
            var categorysinfo = propertyGrid1.SelectedObject.GetType().GetField("categorys", BindingFlags.NonPublic | BindingFlags.Instance);
            if (categorysinfo != null)
            {
                var categorys = categorysinfo.GetValue(propertyGrid1.SelectedObject) as List<String>;
                propertyGrid1.CollapseAllGridItems();

                GridItemCollection currentPropEntries = propertyGrid1.GetType().GetField("currentPropEntries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(propertyGrid1) as GridItemCollection;
                var newarray = currentPropEntries.Cast<GridItem>().OrderBy((t) => categorys.IndexOf(t.Label)).ToArray();

                currentPropEntries.GetType().GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentPropEntries, newarray);
                propertyGrid1.ExpandAllGridItems();
                propertyGrid1.PropertySort = (PropertySort)propertyGrid1.Tag;
            }
            propertyGrid1.Paint -= new PaintEventHandler(propertyGrid1_Paint);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //if (propertyGrid1.Tag == _personDemo)
            //{
            //    propertyGrid1.Tag = _demoModel;
            //    PropertyGridHelper.SetSelectedObject(propertyGrid1, _demoModel);
            //}
            //else
            //{
            //    propertyGrid1.Tag = _personDemo;
            //    PropertyGridHelper.SetSelectedObject(propertyGrid1, _personDemo);
            //}
        }

        private void FTestPRopertyGrid_FormClosing(object sender, FormClosingEventArgs e)
        {
            PropertyGridHelper.SetSelectedObject(propertyGrid1, null);
        }
    }
}
