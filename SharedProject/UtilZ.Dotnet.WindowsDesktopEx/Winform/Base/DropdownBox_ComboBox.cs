using System;
using System.Collections.Generic;
using System.Linq;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// 下拉框控件数据绑定及获取辅助类
    /// </summary>
    public partial class DropdownBoxHelper
    {
        #region System.Windows.Forms.ComboBox
        #region 枚举
        /// <summary>
        /// 绑定枚举值到ComboBox
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <param name="defaultSelectedValue">默认选中项值</param>
        /// <param name="ignoreList">忽略项列表</param>
        public static void BindingEnumToComboBox<T>(System.Windows.Forms.ComboBox combox, T defaultSelectedValue, IEnumerable<T> ignoreList = null) where T : struct
        {
            Type enumType = typeof(T);
            EnumEx.AssertEnum(enumType);
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            if (ignoreList == null)
            {
                ignoreList = new List<T>();
            }

            try
            {
                combox.Items.Clear();
                List<PropertyFieldInfo> items = EnumEx.GetEnumPropertyFieldInfoList(enumType);
                if (items.Count == 0)
                {
                    return;
                }

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

                    combox.Items.Add(item);
                }

                combox.DisplayMember = nameof(PropertyFieldInfo.DisplayName);
                combox.SelectedIndex = selectedIndex == -1 ? 0 : selectedIndex;
            }
            catch (Exception ex)
            {
                throw new Exception("绑定值失败", ex);
            }
        }

        /// <summary>
        /// 设置ComboBox枚举选中项
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <param name="enumValue">选中项值</param>
        public static void SetEnumToComboBox<T>(System.Windows.Forms.ComboBox combox, T enumValue) where T : struct
        {
            Type enumType = typeof(T);
            EnumEx.AssertEnum(enumType);
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            if (combox.Items.Count == 0)
            {
                return;
            }

            try
            {
                for (int i = 0; i < combox.Items.Count; i++)
                {
                    if (object.Equals(enumValue, ((PropertyFieldInfo)combox.Items[i]).Value))
                    {
                        combox.SelectedIndex = i;
                        return;
                    }
                }

                throw new Exception(string.Format("枚举类型:{0}与绑定到ComboBox的枚举类型不匹配", enumType.FullName));
            }
            catch (Exception ex)
            {
                throw new Exception("设定值失败", ex);
            }
        }

        /// <summary>
        /// 获取ComboBox枚举选中项
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <returns>选中项枚举值</returns>
        public static T GetEnumFromComboBox<T>(System.Windows.Forms.ComboBox combox) where T : struct
        {
            EnumEx.AssertEnum<T>();
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            try
            {
                PropertyFieldInfo selectedItem = (PropertyFieldInfo)(combox.Items[combox.SelectedIndex]);
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
        /// 绑定泛型集合到ComboBox
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <param name="bindItems">要绑定的集合</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>        
        private static void BindingIEnumerableGenericToComboBox<T>(System.Windows.Forms.ComboBox combox, List<PropertyFieldInfo> bindItems, T selectedItem = null) where T : class
        {
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            try
            {
                combox.DataSource = bindItems;
                combox.DisplayMember = nameof(PropertyFieldInfo.DisplayName);
                if (bindItems.Count == 0)
                {
                    return;
                }

                PropertyFieldInfo tmpItem;
                for (int i = 0; i < bindItems.Count; i++)
                {
                    tmpItem = bindItems[i];
                    if (selectedItem == tmpItem.Value || object.Equals(selectedItem, tmpItem.Value))
                    {
                        combox.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("绑定值失败", ex);
            }
        }

        /// <summary>
        /// 绑定泛型集合到ComboBox
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <param name="items">要绑定的集合</param>
        /// <param name="displayMember">显示的成员,属性名或字段名,当为null时调用成员的ToString方法的值作为显示值[默认值为null]</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>        
        public static void BindingIEnumerableGenericToComboBox<T>(System.Windows.Forms.ComboBox combox, IEnumerable<T> items, string displayMember = null, T selectedItem = null) where T : class
        {
            List<PropertyFieldInfo> bindItems = PropertyFieldInfo.GenericToDropdownBindingItems<T>(items, displayMember);
            BindingIEnumerableGenericToComboBox<T>(combox, bindItems, selectedItem);
        }

        /// <summary>
        /// 绑定泛型集合到ComboBox
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <param name="displayFun">显示的委托,当为null时调用成员的ToString方法的值作为显示值[默认值为null]</param>
        /// <param name="items">要绑定的集合</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>        
        public static void BindingIEnumerableGenericToComboBox<T>(System.Windows.Forms.ComboBox combox, Func<T, string> displayFun, IEnumerable<T> items, T selectedItem = null) where T : class
        {
            List<PropertyFieldInfo> bindItems = PropertyFieldInfo.GenericToDropdownBindingItems<T>(items, displayFun);
            BindingIEnumerableGenericToComboBox<T>(combox, bindItems, selectedItem);
        }

        /// <summary>
        /// 设置ComboBox泛型选中项
        /// </summary>
        /// <param name="combox">ComboBox</param>
        /// <param name="selectedItem">选中项值</param>
        public static void SetGenericToComboBox(System.Windows.Forms.ComboBox combox, object selectedItem)
        {
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            if (combox.Items.Count == 0)
            {
                return;
            }

            try
            {
                object value = null;
                for (int i = 0; i < combox.Items.Count; i++)
                {
                    value = ((PropertyFieldInfo)combox.Items[i]).Value;
                    if (selectedItem == value || object.Equals(selectedItem, value))
                    {
                        combox.SelectedIndex = i;
                        return;
                    }
                }

                throw new Exception(string.Format("ComboBox集合项中不包含类型:{0}的项:{1}", selectedItem.GetType().Name, selectedItem.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("设定值失败", ex);
            }
        }

        /// <summary>
        /// 获取ComboBox泛型选中项值
        /// </summary>
        /// <typeparam name="T">绑定时的集合类型</typeparam>
        /// <param name="combox">ComboBox</param>
        /// <returns>选中项值</returns>
        public static T GetGenericFromComboBox<T>(System.Windows.Forms.ComboBox combox)
        {
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            if (combox.SelectedIndex == -1)
            {
                throw new Exception("ComboBox的选中项索引为-1,没有选中项");
            }

            try
            {
                return (T)((PropertyFieldInfo)combox.SelectedItem).Value;
            }
            catch (Exception ex)
            {
                throw new Exception("获取值失败", ex);
            }
        }
        #endregion

        #region 字符串集合
        /// <summary>
        /// 绑定字符串集合到ComboBox
        /// </summary>
        /// <param name="combox">ComboBox</param>
        /// <param name="items">集合项</param>
        /// <param name="selectedItem">默认选中项,不设置默认选中时该值为null[默认值为null]</param>
        /// <param name="ignoreCase">是否区分大小写[true:区分大小写,false:不区分,默认值为false]</param>
        public static void BindingIEnumerableStringToComboBox(System.Windows.Forms.ComboBox combox, IEnumerable<string> items, string selectedItem = null, bool ignoreCase = false)
        {
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            try
            {
                combox.Items.Clear();
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
                    combox.Items.Add(new PropertyFieldInfo(item, item, string.Empty, item));

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

                combox.DisplayMember = nameof(PropertyFieldInfo.DisplayName);
                combox.SelectedIndex = selectedIndex == -1 ? 0 : selectedIndex;
            }
            catch (Exception ex)
            {
                throw new Exception("绑定值失败", ex);
            }
        }

        /// <summary>
        /// 设置ComboBox字符串选中项
        /// </summary>
        /// <param name="combox">ComboBox</param>
        /// <param name="selectedItem">选中项值</param>
        /// <param name="ignoreCase">是否区分大小写[true:区分大小写,false:不区分,默认值为false]</param>
        public static void SetStringToComboBox(System.Windows.Forms.ComboBox combox, string selectedItem, bool ignoreCase = false)
        {
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            if (combox.Items.Count == 0)
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
                for (int i = 0; i < combox.Items.Count; i++)
                {
                    value = ((PropertyFieldInfo)combox.Items[i]).Value;
                    if (value == null)
                    {
                        if (selectedItem == null)
                        {
                            combox.SelectedIndex = i;
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
                        combox.SelectedIndex = i;
                        return;
                    }
                }

                throw new Exception(string.Format("ComboBox集合项中不包含类型:{0}的项:{1}", selectedItem.GetType().Name, selectedItem.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("设定值失败", ex);
            }
        }

        /// <summary>
        /// 获取ComboBox字符串选中项值
        /// </summary>
        /// <param name="combox">ComboBox</param>
        /// <returns>选中项值</returns>
        public static string GetStringFromComboBox(System.Windows.Forms.ComboBox combox)
        {
            if (combox == null)
            {
                throw new ArgumentNullException(nameof(combox), "目标控件不能为null");
            }

            if (combox.SelectedIndex == -1)
            {
                throw new Exception("ComboBox的选中项索引为-1,没有选中项");
            }

            try
            {
                object value = ((PropertyFieldInfo)combox.Items[combox.SelectedIndex]).Value;
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
