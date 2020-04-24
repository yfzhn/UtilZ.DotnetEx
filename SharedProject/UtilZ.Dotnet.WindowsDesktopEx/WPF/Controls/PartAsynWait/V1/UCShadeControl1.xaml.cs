using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.AsynWait;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Base;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait.V1
{
    /// <summary>
    /// UCShadeControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UCShadeControl1 : UserControl, IPartAsynWait
    {
        #region IPartAsynWait接口
        /// <summary>
        /// 是否已经取消
        /// </summary>
        private bool _isCanceled = false;

        /// <summary>
        /// 获取是否已经取消
        /// </summary>
        [Browsable(false)]
        public bool Canceled
        {
            get { return this._isCanceled; }
        }

        /// <summary>
        /// 获取或设置提示标题
        /// </summary>
        [Category("异步等待")]
        [DisplayName("提示标题")]
        [Description("获取或设置提示标题")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Title
        {
            get
            {
                string caption;
                if (this.InvokeRequired)
                {
                    caption = (string)this.Dispatcher.Invoke(new Func<string>(() =>
                    {
                        return labelControlCaption.Text;
                    }));
                }
                else
                {
                    caption = labelControlCaption.Text;
                }

                return caption;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Dispatcher.Invoke(new Action<object>((obj) =>
                    {
                        labelControlCaption.Text = obj == null ? string.Empty : (string)obj;
                    }), value);
                }
                else
                {
                    labelControlCaption.Text = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置提示内容
        /// </summary>
        [Category("异步等待")]
        [DisplayName("提示内容")]
        [Description("获取或设置提示内容")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Message
        {
            get
            {
                string hint;
                if (this.InvokeRequired)
                {
                    hint = (string)this.Dispatcher.Invoke(new Func<string>(() =>
                    {
                        return labelControlTitle.Text;
                    }));
                }
                else
                {
                    hint = labelControlTitle.Text;
                }

                return hint;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Dispatcher.Invoke(new Action<object>((obj) =>
                    {
                        labelControlTitle.Text = obj == null ? string.Empty : (string)obj;
                    }), value);
                }
                else
                {
                    labelControlTitle.Text = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置是否显示取消按钮
        /// </summary>
        [Category("异步等待")]
        [DisplayName("是否显示取消按钮")]
        [Description("获取或设置是否显示取消按钮")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool ShowCancel
        {
            get
            {
                bool isShowCancel = false;
                if (this.InvokeRequired)
                {
                    isShowCancel = (bool)this.Dispatcher.Invoke(new Func<bool>(() =>
                    {
                        return btnCancell.Visibility == Visibility.Visible ? true : false;
                    }));
                }
                else
                {
                    isShowCancel = btnCancell.Visibility == Visibility.Visible ? true : false;
                }

                return isShowCancel;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Dispatcher.Invoke(new Action<bool>((visible) =>
                    {
                        btnCancell.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                    }), value);
                }
                else
                {
                    btnCancell.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 获取或设置动画背景色
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("获取或设置动画背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object ShadeBackground
        {
            get
            {
                Brush asynWaitBackground;
                if (this.InvokeRequired)
                {
                    asynWaitBackground = (Brush)this.Dispatcher.Invoke(new Func<Brush>(() =>
                      {
                          return this.Background;
                      }));
                }
                else
                {
                    asynWaitBackground = this.Background;
                }

                return asynWaitBackground;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Dispatcher.Invoke(new Action<object>((obj) =>
                    {
                        this.Background = (Brush)obj;
                    }), value);
                }
                else
                {
                    this.Background = (Brush)value;
                }
            }
        }

        /// <summary>
        /// 取消通知事件
        /// </summary>
        public event EventHandler CanceledNotify;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// 取消操作
        /// </summary>
        public void Cancel()
        {
            if (this.InvokeRequired)
            {
                this.Dispatcher.Invoke(new Action(this.Cancel));
                return;
            }

            lock (this._monitor)
            {
                if (this._isCanceled)
                {
                    return;
                }

                this._isCanceled = true;
            }

            btnCancell.Content = "正在取消";
            btnCancell.IsEnabled = false;

            var handler = this.CanceledNotify;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void StartAnimation()
        {
            waitingAnimalControl.StartAnimal();
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void StopAnimation()
        {
            waitingAnimalControl.StopAnimal();
        }

        /// <summary>
        /// 设置信息(保留接口)
        /// </summary>
        /// <param name="para">参数</param>
        public void SetInfo(object para)
        {

        }

        /// <summary>
        /// 重置异步等待框
        /// </summary>
        public void Reset()
        {
            if (this.InvokeRequired)
            {
                this.Dispatcher.Invoke(new Action(this.Reset));
            }
            else
            {
                this._isCanceled = false;
                btnCancell.Content = "取消";
                btnCancell.IsEnabled = true;
            }
        }

        /// <summary>
        /// 获取 或设置是否需要调用Invoke
        /// </summary>
        public bool InvokeRequired
        {
            get
            {
                return WPFHelper.InvokeRequired(this);
            }
        }

        /// <summary>
        /// 在拥有此控件的基础窗口句柄的线程上执行指定的委托
        /// </summary>
        /// <param name="method">包含要在控件的线程上下文中调用的方法的委托</param>
        /// <returns>正在被调用的委托的返回值，或者如果委托没有返回值，则为 null</returns>
        public object Invoke(Delegate method)
        {
            return this.Dispatcher.Invoke(method);
        }

        /// <summary>
        /// 在拥有控件的基础窗口句柄的线程上，用指定的自变量列表执行指定委托
        /// </summary>
        /// <param name="method">一个方法委托，它采用的参数的数量和类型与 args 参数中所包含的相同</param>
        /// <param name="args">作为指定方法的自变量传递的对象数组。 如果此方法没有参数，该参数可以是 null</param>
        /// <returns>System.Object，它包含正被调用的委托返回值；如果该委托没有返回值，则为 null</returns>
        public object Invoke(Delegate method, params object[] args)
        {
            return this.Dispatcher.Invoke(method, args);
        }

        /// <summary>
        /// 获取UI是否处于设计器模式
        /// </summary>
        public bool UIDesignMode
        {
            get
            {
                bool isDesignMode = false;
                if (this.InvokeRequired)
                {
                    isDesignMode = (bool)this.Dispatcher.Invoke(new Func<bool>(() =>
                    {
                        return WPFHelper.IsInDesignMode(this);
                    }));
                }
                else
                {
                    isDesignMode = WPFHelper.IsInDesignMode(this);
                }

                return isDesignMode;
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public UCShadeControl1()
        {
            InitializeComponent();
        }

        private void btnCancell_Click(object sender, RoutedEventArgs e)
        {
            this.Cancel();
        }
    }
}
