using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.AsynWait;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V1
{
    /// <summary>
    /// Winfrom异步执行类
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    internal class WinformPartAsynExcuteV1<T, TContainer, TResult>
        : WinformPartAsynExcuteBase<T, TContainer, TResult> where TContainer : class
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static WinformPartAsynExcuteV1()
        {
            _shadeType = typeof(MetroShadeControl);
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
                AssertIAsynWait(value);
                _shadeType = value;
            }
        }

        /// <summary>
        /// 容器控件
        /// </summary>
        private System.Windows.Forms.Control _containerControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WinformPartAsynExcuteV1()
            : base()
        {

        }

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

            var container = containerControl as Control;
            this._asynWaitPara = asynWaitPara;
            this._containerControl = container;

            WinformPartAsynExcuteHelper.DisableControls(container, this._asynModifyControls);
            //设置遮罩层控件
            asynWaitPara.AsynWait.ShadeBackground = PartAsynExcuteFactoryWinform.ConvertShadeBackground(asynWaitPara.AsynWaitBackground);
            this.SetShadeControl(container, (Control)asynWaitPara.AsynWait);

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
                var containerControl = this._containerControl;
                if (containerControl.InvokeRequired)
                {
                    containerControl.Invoke(new Action(this.PrimitiveReleseResource));
                }
                else
                {
                    try
                    {
                        containerControl.Controls.Remove((Control)this._asynWaitPara.AsynWait);
                        this._asynWaitPara.AsynWait.Canceled -= CancellExcute;
                        this._asynWaitPara.AsynWait.StopAnimation();
                        //启用当显示类型为只显示提示信息框大小类型时禁用状态的控件
                        WinformPartAsynExcuteHelper.EnableControls(this._asynModifyControls);
                    }
                    catch (Exception exi)
                    {
                        Loger.Error(exi);
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 设置遮罩层控件
        /// </summary>
        /// <param name="containerControl">挡住当前窗体的form 或 userControl</param>
        /// <param name="shadeControl">遮罩层控件</param>
        private void SetShadeControl(Control containerControl, Control shadeControl)
        {
            shadeControl.Anchor = AnchorStyles.None;
            shadeControl.Dock = DockStyle.None;
            shadeControl.Location = new System.Drawing.Point((containerControl.Width - shadeControl.Width) / 2, (containerControl.Height - shadeControl.Height) / 2);
            containerControl.Controls.Add(shadeControl);

            //设置遮罩层控件在最上层
            containerControl.Controls.SetChildIndex(shadeControl, 0);
        }
    }
}
