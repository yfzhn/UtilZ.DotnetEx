using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute
{
    /// <summary>
    /// 异步执行对象创建工厂基类
    /// </summary>
    public abstract class PartAsynExcuteFactoryAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynExcuteFactoryAbs()
        {

        }

        /// <summary>
        /// 创建异步执行对象
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TContainer">容器控件类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        /// <returns>异步执行对象</returns>
        public abstract IAsynExcute<T, TContainer, TResult> CreateExcute<T, TContainer, TResult>() where TContainer : class;
    }
}
