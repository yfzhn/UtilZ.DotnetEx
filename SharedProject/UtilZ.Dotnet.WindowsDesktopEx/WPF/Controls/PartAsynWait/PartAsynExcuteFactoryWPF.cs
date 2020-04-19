using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait
{
    /// <summary>
    /// WPF异步执行对象创建工厂类
    /// </summary>
    public class PartAsynExcuteFactoryWPF : PartAsynExcuteFactoryAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static PartAsynExcuteFactoryWPF()
        {
            _partAsynExcuteType = WPFPartAsynExcuteTypeDefine.Type1;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynExcuteFactoryWPF()
            : base()
        {

        }

        /// <summary>
        /// WPF异步执行对象创建类型
        /// </summary>
        private static int _partAsynExcuteType;

        /// <summary>
        /// 获取或设置WPF异步执行对象创建类型
        /// </summary>
        public static int PartAsynExcuteType
        {
            get { return _partAsynExcuteType; }
            set { _partAsynExcuteType = value; }
        }

        /// <summary>
        /// 转换遮罩层背景色
        /// </summary>
        /// <param name="shadeBackground">遮罩层背景色对象</param>
        /// <returns>遮罩层背景色</returns>
        public static Brush ConvertShadeBackground(object shadeBackground)
        {
            if (shadeBackground == null)
            {
                return Brushes.White;
            }
            else
            {
                return (Brush)shadeBackground;
            }
        }

        /// <summary>
        /// 创建异步执行对象
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TContainer">容器控件类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        /// <returns>异步执行对象</returns>
        public override IAsynExcute<T, TContainer, TResult> CreateExcute<T, TContainer, TResult>()
        {
            int partAsynExcuteType = _partAsynExcuteType;
            IAsynExcute<T, TContainer, TResult> asynExcute;
            if (partAsynExcuteType == WPFPartAsynExcuteTypeDefine.Type1)
            {
                asynExcute = new V1.WPFPartAsynExcuteV1<T, TContainer, TResult>();
            }
            else
            {
                throw new NotSupportedException(string.Format("不支持的异步执行对象创建类型{0}", partAsynExcuteType));
            }

            return asynExcute;
        }
    }
}
