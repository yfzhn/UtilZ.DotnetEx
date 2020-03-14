using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 应用程序辅助类,控制台程序正常结束需要手动调用ApplicationHelper.OnRaiseApplicationExitNotify方法
    /// </summary>
    public class ApplicationEx
    {
        private static readonly List<ApplicationExitNotify> _list = new List<ApplicationExitNotify>();
        private readonly static object _listLock = new object();


        /// <summary>
        /// 触发应用程序结束通知
        /// </summary>
        public static void RaiseApplicationExitNotify()
        {
            lock (_listLock)
            {
                foreach (var applicationExitNotify in _list)
                {
                    applicationExitNotify.OnRaiseApplicationExitNotify();
                }
            }
        }

        /// <summary>
        /// 添加应用程序退出通知
        /// </summary>
        /// <param name="applicationExitNotify"></param>
        public static void Add(ApplicationExitNotify applicationExitNotify)
        {
            if (applicationExitNotify == null)
            {
                return;
            }

            lock (_listLock)
            {
                if (_list.Contains(applicationExitNotify))
                {
                    return;
                }

                _list.Add(applicationExitNotify);
            }
        }

        /// <summary>
        /// 移除应用程序退出通知
        /// </summary>
        /// <param name="applicationExitNotify"></param>
        public static void Remove(ApplicationExitNotify applicationExitNotify)
        {
            if (applicationExitNotify == null)
            {
                return;
            }

            lock (_listLock)
            {
                _list.Remove(applicationExitNotify);
            }
        }

        /// <summary>
        /// 清空应用程序退出通知
        /// </summary>
        public static void Clear()
        {
            lock (_listLock)
            {
                _list.Clear();
            }
        }
    }


    /// <summary>
    /// 应用程序退出通知
    /// </summary>
    public class ApplicationExitNotify
    {
        /// <summary>
        /// 结束通知回调
        /// </summary>
        private readonly Action _exitNotifyCallback;

        /// <summary>
        /// 触发应用程序结束通知
        /// </summary>
        public void OnRaiseApplicationExitNotify()
        {
            this._exitNotifyCallback();
        }

        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="exitNotifyCallback">结束通知回调</param>
        public ApplicationExitNotify(Action exitNotifyCallback)
        {
            if (exitNotifyCallback == null)
            {
                throw new ArgumentNullException(nameof(exitNotifyCallback));
            }

            this._exitNotifyCallback = exitNotifyCallback;
        }
    }
}
