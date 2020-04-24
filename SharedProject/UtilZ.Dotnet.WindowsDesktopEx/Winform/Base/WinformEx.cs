using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// Winform辅助类
    /// </summary>
    public static class WinformEx
    {
        /// <summary>
        /// 查找控件指定类型的父窗口
        /// </summary>
        /// <param name="control">目标控件</param>
        /// <param name="targetFormType">父窗口类型,不为null时查找与该类型匹配的父窗口;为null时找到第一级为结果</param>
        /// <returns>查找结果</returns>
        public static Form FindParentForm(this Control control, Type targetFormType = null)
        {
            if (control == null)
            {
                return null;
            }

            Form parentForm = null;
            Type controlType = typeof(Control);
            control = control.Parent;

            while (control != null)
            {
                if (control.GetType() == controlType)
                {
                    break;
                }

                if (control is Form &&
                    (targetFormType == null || control.GetType() == targetFormType))
                {
                    parentForm = (Form)control;
                    break;
                }

                control = control.Parent;
            }

            return parentForm;
        }
    }
}
