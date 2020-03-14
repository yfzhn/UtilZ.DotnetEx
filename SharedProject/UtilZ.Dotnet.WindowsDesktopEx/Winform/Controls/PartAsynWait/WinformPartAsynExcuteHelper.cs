using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform
{
    /// <summary>
    /// Winform异步执行辅助类
    /// </summary>
    public class WinformPartAsynExcuteHelper
    {
        /// <summary>
        /// 启用当显示类型为只显示提示信息框大小类型时禁用状态的控件
        /// </summary>
        /// <param name="disableControls">当显示类型为只显示提示信息框大小类型时当前处于禁用状态的控件集合</param>
        public static void EnableControls(List<Control> disableControls)
        {
            foreach (Control control in disableControls)
            {
                control.Enabled = true;
            }

            disableControls.Clear();
        }

        /// <summary>
        /// 当显示类型为只显示提示信息框大小类型时禁用状态的控件集合
        /// </summary>
        /// <param name="containerControl">容器控件</param>
        /// <param name="disableControls">当显示类型为只显示提示信息框大小类型时当前处于禁用状态的控件集合</param>
        public static void DisableControls(Control containerControl, List<Control> disableControls)
        {
            disableControls.Clear();
            foreach (Control control in containerControl.Controls)
            {
                if (control.Enabled)
                {
                    control.Enabled = false;
                    disableControls.Add(control);
                }
            }
        }

        /// <summary>
        /// 禁用容器控件内的子控件的Tab焦点选中功能
        /// </summary>
        /// <param name="containerControl">容器控件</param>
        /// <param name="disableTabControls">禁用Tab键可获取焦点的控件集合</param>
        public static void DisableTab(Control containerControl, List<Control> disableTabControls)
        {
            disableTabControls.Clear();
            foreach (Control control in containerControl.Controls)
            {
                if (control.Controls.Count > 0)
                {
                    DisableTab(control, disableTabControls);
                }

                if (control.TabStop)
                {
                    control.TabStop = false;
                    disableTabControls.Add(control);
                }
            }
        }

        /// <summary>
        /// 启用容器控件内的子控件的Tab焦点选中功能
        /// </summary>
        /// <param name="disableTabControls">禁用Tab键可获取焦点的控件集合</param>
        public static void EnableTab(List<Control> disableTabControls)
        {
            foreach (Control control in disableTabControls)
            {
                control.TabStop = true;
            }
        }
    }
}
