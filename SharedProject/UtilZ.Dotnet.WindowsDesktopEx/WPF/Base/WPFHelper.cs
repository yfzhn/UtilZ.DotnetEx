using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Base
{
    /// <summary>
    /// WPF辅助类
    /// </summary>
    public static class WPFHelper
    {
        /// <summary>
        /// 判断UI是否处于设计模式[处理设计模式返回true;否则返回false]
        /// </summary>
        /// <param name="ele">要判断的UI元素</param>
        /// <returns>处理设计模式返回true;否则返回false</returns>
        public static bool IsInDesignMode(this UIElement ele)
        {
            if (ele == null)
            {
                return false;
            }

            //非UI对象，要判断是否处于设计器模式
            //bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

            //UI对象，要判断是否处于设计器模式
            //bool isInDesignMode = DesignerProperties.GetIsInDesignMode(ele);

            //这两种方式有时会失效（具体什么情况下会失效不明）
            //return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

            return DesignerProperties.GetIsInDesignMode(ele);
        }

        /// <summary>
        /// 判断当前窗口是否已释放[返回值:true:已释放Invoke;false:未释放]
        /// </summary>
        /// <param name="window">要判判断的窗口</param>
        /// <returns>返回值:true:已释放Invoke;false:未释放</returns>
        public static bool IsDisposed(this Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            return new System.Windows.Interop.WindowInteropHelper(window).Handle == IntPtr.Zero;
        }

        /// <summary>
        /// 判断当前操作是否需要调用Invoke[返回值:true:调用Invoke;false:不需要]
        /// </summary>
        /// <param name="dispatcher">判断的对象</param>
        /// <returns>返回值:true:调用Invoke;false:不需要</returns>
        public static bool InvokeRequired(this System.Windows.Threading.DispatcherObject dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return !dispatcher.Dispatcher.CheckAccess();
        }

        /// <summary>
        /// 查找元素根窗口
        /// </summary>
        /// <param name="element">目标控件</param>
        ///  <param name="targetWindowType">父窗口类型,不为null时查找与该类型匹配的父窗口;为null时找到第一级为结果</param>
        /// <returns>查找结果</returns>
        public static Window FindParentForm(FrameworkElement element, Type targetWindowType = null)
        {
            if (element == null)
            {
                return null;
            }

            Window window = null;
            Type controlType = typeof(FrameworkElement);
            FrameworkElement framework = element.Parent as FrameworkElement;

            while (framework != null)
            {
                if (framework.GetType() == controlType)
                {
                    break;
                }

                if (framework is Window &&
                    (targetWindowType == null || framework.GetType() == targetWindowType))
                {
                    window = (Window)framework;
                    break;
                }

                framework = framework.Parent as FrameworkElement;
            }

            return window;
        }

        /// <summary>
        /// 查找指定控件模板里的子控件,未找到返回null
        /// </summary>
        /// <param name="control">目标控件</param>
        /// <param name="predicate">匹配条件</param>
        /// <returns>模板里的子控件</returns>
        public static Visual FindTemplateControl(DependencyObject control, Func<Visual, bool> predicate)
        {
            int count = VisualTreeHelper.GetChildrenCount(control);
            DependencyObject obj;
            Visual child;
            for (int i = 0; i < count; i++)
            {
                obj = VisualTreeHelper.GetChild(control, i);
                child = obj as Visual;
                if (child != null && predicate(child))
                {
                    return child;
                }

                child = FindTemplateControl(obj, predicate);
                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// 查找指定控件模板里的子控件,未找到返回null
        /// </summary>
        /// <param name="control">目标控件</param>
        /// <param name="name">模板内子控件名称</param>
        /// <returns>模板里的子控件</returns>
        public static FrameworkElement FindTemplateControlByName(DependencyObject control, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(control);
            DependencyObject obj;
            FrameworkElement child;
            for (int i = 0; i < count; i++)
            {
                obj = VisualTreeHelper.GetChild(control, i);
                child = obj as FrameworkElement;
                if (child != null && string.Equals(child.Name, name))
                {
                    return child;
                }

                child = FindTemplateControlByName(obj, name);
                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取指定控件模板里的子控件列表
        /// </summary>
        /// <param name="control">目标控件</param>
        /// <returns>模板里的子控件列表</returns>
        public static List<Visual> GetTemplateControlList(DependencyObject control)
        {
            List<Visual> visualList = new List<Visual>();
            PrimitiveGetTemplateControlList(control, visualList);
            return visualList;
        }
        private static void PrimitiveGetTemplateControlList(DependencyObject control, List<Visual> visualList)
        {
            int count = VisualTreeHelper.GetChildrenCount(control);
            if (count == 0)
            {
                return;
            }

            DependencyObject obj;
            Visual child;
            for (int i = 0; i < count; i++)
            {
                obj = VisualTreeHelper.GetChild(control, i);
                child = obj as Visual;
                if (child != null)
                {
                    visualList.Add(child);
                    PrimitiveGetTemplateControlList(obj, visualList);
                }
                else
                {
                    PrimitiveGetTemplateControlList(obj, visualList);
                }
            }
        }
    }
}
