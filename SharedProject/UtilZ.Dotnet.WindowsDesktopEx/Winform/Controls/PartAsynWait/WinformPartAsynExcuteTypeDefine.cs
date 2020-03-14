using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform
{
    /// <summary>
    /// Winfrom异步执行对象创建类型定义类(推荐使用OpacityShadeDisableTab值3)
    /// </summary>
    public class WinformPartAsynExcuteTypeDefine
    {
        /// <summary>
        /// 等等动画+禁用容器内控件方式实现(注:不可在异步执行方法中修改控件的Enable属,否则会导致控件的TabStop状态达不到预期效果)
        /// </summary>
        public static readonly int ShadeDisableControl = 1;

        /// <summary>
        /// 半透明用户控件等待动画+禁用控件Tab(最小华还原左上角有一块白色的出现绘制异常,原因不明,猜测与半透明有关)
        /// </summary>
        internal static readonly int OpacityDisableTab = 2;

        /// <summary>
        /// 半透明遮罩层+等待动画+禁用控件Tab(注:不可在异步执行方法中修改控件的TabStop属性,否则会导致控件的TabStop状态达不到预期效果;容器控件大小改变时会闪烁，因为半透明控件winform不支持双缓冲)
        /// </summary>
        public static readonly int OpacityShadeDisableTab = 3;

        /// <summary>
        /// 容器控件截图作为异步等等控件背景+禁用控件Tab(注:控件状态改变在背景中不能体现出现,同时容器控件大小改变时要重新框图并做半透明处理,比较消耗CPU,容器控件越大性能越低)
        /// </summary>
        internal static readonly int ScreenshotImgBackgrounDisableTab = 4;

        /// <summary>
        /// 自定义半透明控件+禁用控件Tab
        /// </summary>
        internal static readonly int OpacityCustomerDisableTab = 5;
    }
}
