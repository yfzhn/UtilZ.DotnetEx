using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Base;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.TypeConverters;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo
{
    /// <summary>
    /// Person
    /// </summary>
    [TypeConverter(typeof(PropertyGridSortConverter))]
    public class Person
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Person()
        {

        }

        /// <summary>
        /// 最后一个名字
        /// </summary>
        [DisplayName("属性顺序1")]
        [PropertyGridOrderAttribute(2)]
        public string LastName { get; set; }

        /// <summary>
        /// 第一个名字
        /// </summary>
        [DisplayName("属性顺序2")]
        [PropertyGridOrderAttribute(1)]
        public string FirstName { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [DisplayName("属性顺序3")]
        [PropertyGridOrderAttribute(0)]
        public int Age { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return this.LastName + ", " + this.FirstName + " (" + this.Age.ToString() + ")";
            //return base.ToString();
        }
    }
}
