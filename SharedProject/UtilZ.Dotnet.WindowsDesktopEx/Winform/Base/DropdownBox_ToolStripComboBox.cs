using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// 下拉框控件数据绑定及获取辅助类
    /// </summary>
    public partial class DropdownBoxHelper
    {
        #region System.Windows.Forms.ToolStripComboBox
        #region 枚举
        /// <summary>
        /// 绑定枚举值到ToolStripComboBox
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="defaultSelectedValue">默认选中项值</param>
        /// <param name="ignoreList">忽略项列表</param>
        public static void BindingEnumToToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox, T defaultSelectedValue, IEnumerable<T> ignoreList = null) where T : struct
        {
            Type enumType = typeof(T);
            EnumEx.AssertEnum(enumType);
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            if (ignoreList == null)
            {
                ignoreList = new List<T>();
            }

            try
            {
                toolStripComboBox.Items.Clear();
                List<PropertyFieldInfo> items = EnumEx.GetEnumPropertyFieldInfoList(enumType);
                int selectedIndex = -1;

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    bool isIgnore = (from ignoreItem in ignoreList where object.Equals(item.Value, ignoreItem) select ignoreItem).Count() > 0;
                    if (isIgnore)
                    {
                        continue;
                    }

                    if (object.Equals(item.Value, defaultSelectedValue))
                    {
                        selectedIndex = i;
                    }

                    toolStripComboBox.Items.Add(item);
                }

                toolStripComboBox.SelectedIndex = selectedIndex == -1 ? 0 : selectedIndex;
            }
            catch (Exception ex)
            {
                throw new Exception("绑定值失败", ex);
            }
        }

        /// <summary>
        /// 设置ToolStripComboBox枚举选中项
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="enumValue">选中项值</param>
        public static void SetEnumToToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox, T enumValue) where T : struct
        {
            Type enumType = typeof(T);
            EnumEx.AssertEnum(enumType);
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            if (toolStripComboBox.Items.Count == 0)
            {
                throw new Exception("ToolStripComboBox的中还没有绑定数据项");
            }

            try
            {
                for (int i = 0; i < toolStripComboBox.Items.Count; i++)
                {
                    if (enumValue.Equals(((PropertyFieldInfo)toolStripComboBox.Items[i]).Value))
                    {
                        toolStripComboBox.SelectedIndex = i;
                        return;
                    }
                }

                throw new Exception(string.Format("枚举类型:{0}与绑定到ToolStripComboBox的枚举类型不匹配", enumType.FullName));
            }
            catch (Exception ex)
            {
                throw new Exception("设定值失败", ex);
            }
        }

        /// <summary>
        /// 获取ToolStripComboBox枚举选中项
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <returns>选中项枚举值</returns>
        public static T GetEnumFromToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox) where T : struct
        {
            EnumEx.AssertEnum<T>();
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            try
            {
                PropertyFieldInfo selectedItem = (PropertyFieldInfo)(toolStripComboBox.Items[toolStripComboBox.SelectedIndex]);
                return (T)selectedItem.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("获取值失败", ex);
            }
        }
        #endregion

        #region 泛型集合
        /// <summary>
        /// 绑定集合到ToolStripComboBox
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="bindItemss">要绑定的集合</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>  
        private static void BindingIEnumerableGenericToToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox, List<PropertyFieldInfo> bindItemss, T selectedItem = null) where T : class
        {
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            try
            {
                toolStripComboBox.Items.Clear();
                if (bindItemss.Count == 0)
                {
                    return;
                }

                int selectedIndex = 0;
                PropertyFieldInfo item = null;
                for (int i = 0; i < bindItemss.Count; i++)
                {
                    item = bindItemss[i];
                    if (item.Value == selectedItem || object.Equals(item.Value, selectedItem))
                    {
                        selectedIndex = i;
                    }

                    toolStripComboBox.Items.Add(item);
                }

                toolStripComboBox.SelectedIndex = selectedIndex;
            }
            catch (Exception ex)
            {
                throw new Exception("绑定值失败", ex);
            }
        }

        /// <summary>
        /// 绑定集合到ToolStripComboBox
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="items">要绑定的集合</param>
        /// <param name="displayMember">显示的成员,属性名或字段名,当为null时调用成员的ToString方法的值作为显示值[默认值为null]</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>  
        public static void BindingIEnumerableGenericToToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox, IEnumerable<T> items, string displayMember = null, T selectedItem = null) where T : class
        {
            List<PropertyFieldInfo> bindItems = PropertyFieldInfo.GenericToDropdownBindingItems<T>(items, displayMember);
            BindingIEnumerableGenericToToolStripComboBox<T>(toolStripComboBox, bindItems);
        }

        /// <summary>
        /// 绑定泛型集合到ComboBox
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="displayFun">显示的委托,当为null时调用成员的ToString方法的值作为显示值[默认值为null]</param>
        /// <param name="items">要绑定的集合</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>        
        public static void BindingIEnumerableGenericToToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox, Func<T, string> displayFun, IEnumerable<T> items, T selectedItem = null) where T : class
        {
            List<PropertyFieldInfo> bindItems = PropertyFieldInfo.GenericToDropdownBindingItems<T>(items, displayFun);
            BindingIEnumerableGenericToToolStripComboBox<T>(toolStripComboBox, bindItems);
        }

        /// <summary>
        /// 设置ToolStripComboBox泛型选中项
        /// </summary>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="selectedItem">选中项值</param>
        public static void SetGenericToToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox, T selectedItem)
        {
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            if (selectedItem == null)
            {
                throw new ArgumentNullException(nameof(selectedItem), "选中项不能为null");
            }

            if (toolStripComboBox.Items.Count == 0)
            {
                throw new Exception("ToolStripComboBox的中还没有绑定数据项");
            }

            try
            {
                object value = null;
                for (int i = 0; i < toolStripComboBox.Items.Count; i++)
                {
                    value = ((PropertyFieldInfo)toolStripComboBox.Items[i]).Value;
                    if (object.Equals(value, selectedItem))
                    {
                        toolStripComboBox.SelectedIndex = i;
                        return;
                    }
                }

                throw new Exception(string.Format("ToolStripComboBox集合项中不包含类型:{0}的项:{1}", selectedItem.GetType().Name, selectedItem.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("设定值失败", ex);
            }
        }

        /// <summary>
        /// 获取ToolStripComboBox泛型选中项值
        /// </summary>
        /// <typeparam name="T">绑定时的集合类型</typeparam>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <returns>选中项值</returns>
        public static T GetGenericFromToolStripComboBox<T>(System.Windows.Forms.ToolStripComboBox toolStripComboBox)
        {
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            if (toolStripComboBox.SelectedIndex == -1)
            {
                throw new Exception("ToolStripComboBox的选中项索引为-1,没有选中项");
            }

            try
            {
                return (T)((PropertyFieldInfo)toolStripComboBox.Items[toolStripComboBox.SelectedIndex]).Value;
            }
            catch (Exception ex)
            {
                throw new Exception("获取值失败", ex);
            }
        }
        #endregion

        #region 字符串集合
        /// <summary>
        /// 绑定字符串集合到ToolStripComboBox
        /// </summary>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="items">集合项</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>
        /// <param name="ignoreCase">是否区分大小写[true:区分大小写,false:不区分,默认值为false]</param>
        public static void BindingIEnumerableStringToolStripComboBox(System.Windows.Forms.ToolStripComboBox toolStripComboBox, IEnumerable<string> items, string selectedItem = null, bool ignoreCase = false)
        {
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            try
            {
                toolStripComboBox.Items.Clear();
                if (items == null || items.Count() == 0)
                {
                    return;
                }

                if (selectedItem != null && !ignoreCase)
                {
                    selectedItem = selectedItem.ToUpper();
                }

                int selectedIndex = -1;
                string item = null;
                for (int i = 0; i < items.Count(); i++)
                {
                    item = items.ElementAt(i);
                    toolStripComboBox.Items.Add(new PropertyFieldInfo(item.ToString(), item, string.Empty, item));

                    if (selectedIndex != -1)
                    {
                        continue;
                    }

                    if (!ignoreCase && item != null)
                    {
                        item = item.ToUpper();
                    }

                    if (item == selectedItem || object.Equals(selectedItem, item.ToUpper()))
                    {
                        selectedIndex = i;
                    }
                }

                //toolStripComboBox.DisplayMember = DropdownBindingItem.DisplayNameFieldName;
                toolStripComboBox.SelectedIndex = selectedIndex == -1 ? 0 : selectedIndex;
            }
            catch (Exception ex)
            {
                throw new Exception("绑定值失败", ex);
            }
        }

        /// <summary>
        /// 设置ToolStripComboBox字符串选中项
        /// </summary>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <param name="selectedItem">选中项值</param>
        /// <param name="ignoreCase">是否区分大小写[true:区分大小写,false:不区分,默认值为false]</param>
        public static void SetStringToToolStripComboBox(System.Windows.Forms.ToolStripComboBox toolStripComboBox, string selectedItem, bool ignoreCase = false)
        {
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            if (toolStripComboBox.Items.Count == 0)
            {
                return;
            }

            try
            {
                if (!ignoreCase && selectedItem != null)
                {
                    selectedItem = selectedItem.ToUpper();
                }

                object value = null;
                string item;
                for (int i = 0; i < toolStripComboBox.Items.Count; i++)
                {
                    value = ((PropertyFieldInfo)toolStripComboBox.Items[i]).Value;
                    if (value == null)
                    {
                        if (selectedItem == null)
                        {
                            toolStripComboBox.SelectedIndex = i;
                            return;
                        }

                        continue;
                    }

                    item = value.ToString();
                    if (!ignoreCase && item != null)
                    {
                        item = item.ToUpper();
                    }

                    if (object.Equals(selectedItem, item))
                    {
                        toolStripComboBox.SelectedIndex = i;
                        return;
                    }
                }

                throw new Exception(string.Format("ToolStripComboBox集合项中不包含类型:{0}的项:{1}", selectedItem.GetType().Name, selectedItem.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("设定值失败", ex);
            }
        }

        /// <summary>
        /// 获取ToolStripComboBox字符串选中项值
        /// </summary>
        /// <param name="toolStripComboBox">ToolStripComboBox</param>
        /// <returns>选中项值</returns>
        public static string GetStringFromToolStripComboBox(System.Windows.Forms.ToolStripComboBox toolStripComboBox)
        {
            if (toolStripComboBox == null)
            {
                throw new ArgumentNullException(nameof(toolStripComboBox), "目标控件不能为null");
            }

            if (toolStripComboBox.SelectedIndex == -1)
            {
                throw new Exception("ToolStripComboBox的选中项索引为-1,没有选中项");
            }

            try
            {
                object value = ((PropertyFieldInfo)toolStripComboBox.Items[toolStripComboBox.SelectedIndex]).Value;
                return value == null ? null : value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("获取值失败", ex);
            }
        }
        #endregion
        #endregion
    }
}
