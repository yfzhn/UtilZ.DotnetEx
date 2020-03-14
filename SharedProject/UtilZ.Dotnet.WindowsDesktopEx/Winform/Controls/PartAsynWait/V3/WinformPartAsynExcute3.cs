﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.AsynWait;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V3
{
    /// <summary>
    /// Winfrom异步执行类
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    internal class WinformPartAsynExcute3<T, TContainer, TResult> : WinformPartAsynExcuteBase<T, TContainer, TResult> where TContainer : class
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static WinformPartAsynExcute3()
        {
            _shadeType = typeof(UCMetroShadeControl3);
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
        private Control _containerControl;

        /// <summary>
        /// 遮罩层控件
        /// </summary>
        private Control _shadeControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WinformPartAsynExcute3()
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

            //设置遮罩层控件
            asynWaitPara.AsynWait.ShadeBackground = PartAsynExcuteFactoryWinform.ConvertShadeBackground(asynWaitPara.AsynWaitBackground);

            //添加遮罩层控件
            this._shadeControl = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.OpacityPanel();
            this._shadeControl.Dock = DockStyle.Fill;
            container.Controls.Add(this._shadeControl);
            container.Controls.SetChildIndex(this._shadeControl, 0);

            var asynWaitControl = (Control)asynWaitPara.AsynWait;
            asynWaitControl.Anchor = AnchorStyles.None;
            asynWaitControl.Location = new System.Drawing.Point((container.Width - asynWaitControl.Width) / 2, (container.Height - asynWaitControl.Height) / 2);
            container.Controls.Add(asynWaitControl);
            container.Controls.SetChildIndex(asynWaitControl, 0);

            //禁用容器控件内的子控件的Tab焦点选中功能
            WinformPartAsynExcuteHelper.DisableTab(container, this._asynModifyControls);

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
                    return;
                }

                WinformPartAsynExcuteHelper.EnableTab(this._asynModifyControls);
                containerControl.Controls.Remove(this._shadeControl);
                containerControl.Controls.Remove((Control)this._asynWaitPara.AsynWait);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
