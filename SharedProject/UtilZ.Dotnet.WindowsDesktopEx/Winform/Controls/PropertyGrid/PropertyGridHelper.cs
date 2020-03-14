using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Base;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Base;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid
{
    /// <summary>
    /// 属性表格辅助扩展类
    /// </summary>
    public static class PropertyGridHelper
    {
        private static readonly Hashtable _htPropertyGrid = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 更新通过属性表格修改值的对象中的值到目标对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="valueObj">值对象</param>
        /// <param name="targetObj">目标对象</param>
        public static void UpdateValue<T>(T valueObj, T targetObj) where T : class
        {
            AttributeEx.UpdateValue<T, PropertyGridAttribute>(valueObj, targetObj);
        }

        /// <summary>
        /// 设置PropertyGrid的SelectedObject
        /// </summary>
        /// <param name="propertyGrid">PropertyGrid</param>
        /// <param name="selectedObject">SelectedObject</param>
        public static void SetSelectedObject(this System.Windows.Forms.PropertyGrid propertyGrid, object selectedObject)
        {
            if (propertyGrid == null)
            {
                return;
            }

            propertyGrid.SelectedObjectsChanged += propertyGrid_SelectedObjectsChanged;
            try
            {
                lock (_htPropertyGrid.SyncRoot)
                {
                    if (selectedObject == null)
                    {
                        _htPropertyGrid.Remove(propertyGrid);
                    }
                    else
                    {
                        if (_htPropertyGrid.ContainsKey(propertyGrid))
                        {
                            _htPropertyGrid[propertyGrid] = propertyGrid.PropertySort;
                        }
                        else
                        {
                            _htPropertyGrid.Add(propertyGrid, propertyGrid.PropertySort);
                        }
                    }
                }

                propertyGrid.SelectedObject = selectedObject;
            }
            finally
            {
                propertyGrid.SelectedObjectsChanged -= propertyGrid_SelectedObjectsChanged;
            }
        }

        /// <summary>
        /// PropertyGrid控件SelectedObjectsChanged
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            var propertyGrid = sender as System.Windows.Forms.PropertyGrid;
            propertyGrid.PropertySort = PropertySort.CategorizedAlphabetical;
            propertyGrid.Paint += new PaintEventHandler(propertyGrid_Paint);
        }

        /// <summary>
        /// PropertyGrid控件Paint
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void propertyGrid_Paint(object sender, PaintEventArgs e)
        {
            var propertyGrid = (System.Windows.Forms.PropertyGrid)sender;
            try
            {
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

                GridItemCollection currentPropEntries = propertyGrid.GetType().GetField("currentPropEntries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(propertyGrid) as GridItemCollection;
                propertyGrid.CollapseAllGridItems();
                var newarray = currentPropEntries.Cast<GridItem>().OrderBy((t) => propertyGridCategoryNames.IndexOf(t.Label)).ToArray();
                currentPropEntries.GetType().GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentPropEntries, newarray);
                propertyGrid.ExpandAllGridItems();
                propertyGrid.PropertySort = (PropertySort)_htPropertyGrid[propertyGrid];
            }
            finally
            {
                propertyGrid.Paint -= new PaintEventHandler(propertyGrid_Paint);
            }
        }
    }
}
