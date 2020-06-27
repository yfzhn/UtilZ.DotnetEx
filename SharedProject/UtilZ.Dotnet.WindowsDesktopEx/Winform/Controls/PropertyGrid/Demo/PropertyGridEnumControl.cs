using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo
{
    /// <summary>
    /// 属性表格枚举编辑控件
    /// </summary>
    public partial class PropertyGridEnumControl : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyGridEnumControl(object value)
        {
            InitializeComponent();

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Type valueType = value.GetType();
            if (!valueType.IsEnum)
            {
                throw new ArgumentException(string.Format("类型:{0}不是枚举类型", valueType.FullName));
            }

            List<PropertyFieldInfo> dbiItems = EnumEx.GetEnumPropertyFieldInfoList(valueType);
            PropertyFieldInfo selectedItem = (from item in dbiItems where value.Equals(item.Value) select item).FirstOrDefault();
            DropdownBoxHelper.BindingIEnumerableGenericToComboBox<PropertyFieldInfo>(comboBoxEnum, dbiItems, "Text", selectedItem);
        }

        /// <summary>
        /// 获取编辑的枚举值
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object EnumValue
        {
            get
            {
                return DropdownBoxHelper.GetGenericFromComboBox<PropertyFieldInfo>(comboBoxEnum).Value;
            }
        }
    }
}
