using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UtilZ.Dotnet.WindowsDesktopEx.AsynWait;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Log;
using System.Windows.Forms.Integration;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait.V1
{
    /// <summary>
    /// WPF异步执行类
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    internal class WPFPartAsynExcuteV1<T, TContainer, TResult> : WPFAsynExcuteAbs<T, TContainer, TResult> where TContainer : class
    {
        /// <summary>
        /// 异步等待控件类型
        /// </summary>
        private readonly static Type _asynControlType;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static WPFPartAsynExcuteV1()
        {
            _asynControlType = typeof(UIElement);
            _shadeType = typeof(UCShadeControl1);
        }

        /// <summary>
        /// 默认当遮罩层类型为自定义类型时用于创建遮罩层的类型
        /// </summary>
        private static Type _shadeType = null;

        /// <summary>
        /// 当遮罩层类型为自定义类型时用于创建遮罩层的类型
        /// </summary>
        public static Type ShadeType
        {
            get
            {
                return _shadeType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                //断言对象类型是IAsynWait和UserControl的子类对象类型
                AssertIAsynWait(value, _asynControlType);
                _shadeType = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WPFPartAsynExcuteV1()
            : base()
        {

        }



        /// <summary>
        /// 容器控件
        /// </summary>
        private Panel _containerControl;

        /// <summary>
        /// 执行异步委托
        /// </summary>
        /// <param name="asynWaitPara">异步等待执行参数</param>
        /// <param name="containerControl">容器控件</param>
        public override void Excute(PartAsynWaitPara<T, TResult> asynWaitPara, TContainer containerControl)
        {
            if (asynWaitPara.AsynWait == null)
            {
                PartAsynUIParaProxy.SetAsynWait(asynWaitPara, this.CreateAsynWaitShadeControl(_shadeType, asynWaitPara));
            }

            var container = containerControl as UIElement;
            if (container == null)
            {
                throw new ArgumentException($"容器元素类型{containerControl.GetType().FullName}不是{typeof(UIElement).FullName}子类");
            }

            if (asynWaitPara.Islock)
            {
                return;
            }

            lock (asynWaitPara.SyncRoot)
            {
                if (asynWaitPara.Islock)
                {
                    return;
                }

                PartAsynUIParaProxy.Lock(asynWaitPara);
            }


            this._asynWaitPara = asynWaitPara;

            //设置遮罩层控件            
            asynWaitPara.AsynWait.ShadeBackground = PartAsynExcuteFactoryWPF.ConvertShadeBackground(asynWaitPara.AsynWaitBackground);
            this.AddAsynWaitControl((FrameworkElement)asynWaitPara.AsynWait, container);

            //启动执行线程
            base.StartAsynExcuteThread();
        }




        /// <summary>
        /// 释放异步委托资源
        /// </summary>
        protected override void PrimitiveReleseResource()
        {
            try
            {
                var asynWaitPara = this._asynWaitPara;
                if (asynWaitPara.AsynWait.InvokeRequired)
                {
                    asynWaitPara.AsynWait.Invoke(new Action(this.PrimitiveReleseResource));
                    return;
                }

                this.RemoveAsynWaitControl((FrameworkElement)asynWaitPara.AsynWait, this._containerControl);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }



        #region RemoveAsynWaitControl
        /// <summary>
        /// 移除遮罩层控件
        /// </summary>
        /// <param name="asynWaitControl"></param>
        /// <param name="containerControl"></param>
        private void RemoveAsynWaitControl(FrameworkElement asynWaitControl, Panel containerControl)
        {
            //移除控件
            if (this.HasWinformControl)
            {
                containerControl.Children.Remove((FrameworkElement)asynWaitControl.Tag);
            }
            else
            {
                containerControl.Children.Remove(asynWaitControl);
            }

            if (containerControl is Grid)
            {
                //无需其它处理
            }
            else if (containerControl is StackPanel ||
                containerControl is Canvas ||
                containerControl is WrapPanel)
            {
                //this.RemoveAsynWaitControlFromPanel(asynWaitControl, (Panel)containerControl);
                //containerControl.SizeChanged -= Panel_SizeChanged;
            }
            else
            {
                throw new NotSupportedException($"不支持的容器类型{containerControl.GetType().FullName}");
            }
        }
        #endregion



        #region AddAsynWaitControl
        /// <summary>
        /// 添加遮罩层控件
        /// </summary>
        /// <param name="asynWaitControl"></param>
        /// <param name="containerControl"></param>
        private void AddAsynWaitControl(FrameworkElement asynWaitControl, UIElement containerControl)
        {
            if (containerControl is Window ||
                containerControl is UserControl)
            {
                var contentControl = (ContentControl)containerControl;
                var innerContainerControl = contentControl.Content as UIElement;
                this.AddAsynWaitControl(asynWaitControl, innerContainerControl);
                return;
            }

            if (!(containerControl is Panel))
            {
                throw new ArgumentException($"容器元素类型必须为{typeof(Panel).FullName}的子类");
            }

            this._containerControl = (Panel)containerControl;

            if (containerControl is Grid)
            {
                if (this.HasWinformControl)
                {
                    var windowsFormsHost = new WindowsFormsHost();
                    windowsFormsHost.Background = System.Windows.Media.Brushes.Transparent;
                    var elementHost = new ElementHost();
                    elementHost.Child = asynWaitControl;
                    windowsFormsHost.Child = elementHost;
                    asynWaitControl.Tag = windowsFormsHost;
                    asynWaitControl = windowsFormsHost;
                }


                this.AddAsynWaitControlToGrid((Grid)containerControl, asynWaitControl);
            }
            //else if (containerControl is StackPanel)
            //{
            //    this.AddAsynWaitControlToStackPanel((StackPanel)containerControl, asynWaitControl);
            //}
            //else if (containerControl is Canvas)
            //{


            //}
            //else if (containerControl is WrapPanel)
            //{

            //}
            else
            {
                throw new NotSupportedException($"不支持的容器类型{containerControl.GetType().FullName}");
            }
        }

        //private void AddAsynWaitControlToStackPanel(StackPanel stackPanel, FrameworkElement asynWaitControl)
        //{

        //    this.SetAsynWaitControlSize(asynWaitControl, stackPanel);

        //    stackPanel.Children.Add(asynWaitControl);
        //    this.SetMaxZIndex(stackPanel, asynWaitControl);



        //    ResetMargin(stackPanel, asynWaitControl);

        //    //stackPanel.SizeChanged += Panel_SizeChanged;
        //}

        //private void ResetMargin(StackPanel stackPanel, FrameworkElement asynWaitControl)
        //{
        //    double left = 0d, top = 0d;
        //    if (stackPanel.Children.Count > 0)
        //    {
        //        Point p = stackPanel.TranslatePoint(new Point(), asynWaitControl);
        //        //Point p = asynWaitControl.TranslatePoint(new Point(), stackPanel);

        //        if (stackPanel.Orientation == Orientation.Horizontal)
        //        {
        //            var offset = Math.Abs(p.X) + asynWaitControl.Width - stackPanel.ActualWidth;
        //            left = p.X - asynWaitControl.Width+100;
        //            if (offset > 0)
        //            {
        //                left = left - offset;
        //            }
        //        }
        //        else if (stackPanel.Orientation == Orientation.Horizontal)
        //        {
        //            var offset = Math.Abs(p.Y) + asynWaitControl.Height - stackPanel.ActualHeight;
        //            top = p.Y - asynWaitControl.Height;
        //            if (offset > 0)
        //            {
        //                top = top - offset;
        //            }
        //        }
        //        else
        //        {
        //            throw new ArgumentException();
        //        }
        //    }

        //    asynWaitControl.Margin = new Thickness(left, top, 0d, 0d);
        //}

        //private void Panel_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var asynWaitControl = (FrameworkElement)this._asynWaitPara.AsynWait;
        //    this.SetAsynWaitControlSize(asynWaitControl, (FrameworkElement)sender);

        //    StackPanel stackPanel = (StackPanel)sender;
        //    ResetMargin(stackPanel, asynWaitControl);
        //}

        //private void SetAsynWaitControlSize(FrameworkElement asynWaitControl, FrameworkElement containerControl)
        //{
        //    asynWaitControl.Width = containerControl.ActualWidth;
        //    asynWaitControl.Height = containerControl.ActualHeight;
        //}

        private void AddAsynWaitControlToGrid(Grid grid, FrameworkElement asynWaitControl)
        {
            asynWaitControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            asynWaitControl.VerticalAlignment = VerticalAlignment.Stretch;
            grid.Children.Add(asynWaitControl);

            if (grid.RowDefinitions.Count > 0)
            {
                Grid.SetRow(asynWaitControl, 0);
                Grid.SetRowSpan(asynWaitControl, grid.RowDefinitions.Count);
            }

            if (grid.ColumnDefinitions.Count > 0)
            {
                Grid.SetColumn(asynWaitControl, 0);
                Grid.SetColumnSpan(asynWaitControl, grid.ColumnDefinitions.Count);
            }

            this.SetMaxZIndex(grid, asynWaitControl);
        }

        private void SetMaxZIndex(Panel panel, FrameworkElement asynWaitControl)
        {
            int maxZIndex = 0, tmp;
            foreach (UIElement uiElement in panel.Children)
            {
                tmp = Grid.GetZIndex(uiElement);
                if (tmp > maxZIndex)
                {
                    maxZIndex = tmp;
                }
            }

            maxZIndex = maxZIndex + 1;
            Panel.SetZIndex(asynWaitControl, maxZIndex);
        }
        #endregion
    }
}
