using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;

namespace UtilZ.Dotnet.WindowsDesktopEx.AsynWait
{
    /// <summary>
    /// 正在执行的IPartAsynWait辅助类
    /// </summary>
    public class AsynExcuteCancellHelper
    {
        private static List<IAsynExcuteCancell> _partAsynWaitParaList = null;
        private static object _partAsynWaitParaListLock = new object();

        private static bool _enable = false;
        /// <summary>
        /// 获取或设置启用异步执行取消
        /// </summary>
        public static bool Enable
        {
            get { return _enable; }
            set
            {
                lock (_partAsynWaitParaListLock)
                {
                    if (_enable == value)
                    {
                        return;
                    }
                    _enable = value;
                    if (_enable)
                    {
                        if (_partAsynWaitParaList == null)
                        {
                            _partAsynWaitParaList = new List<IAsynExcuteCancell>();
                        }
                    }
                    else
                    {
                        if (_partAsynWaitParaList != null)
                        {
                            _partAsynWaitParaList.Clear();
                            _partAsynWaitParaList = null;
                        }
                    }
                }
            }
        }

        internal static void AddAsynExcuteCancell(IAsynExcuteCancell asynExcuteCancell)
        {
            lock (_partAsynWaitParaListLock)
            {
                if (!_enable)
                {
                    return;
                }

                _partAsynWaitParaList.Add(asynExcuteCancell);
            }
        }

        internal static void RemoveAsynExcuteCancell(IAsynExcuteCancell asynExcuteCancell)
        {
            lock (_partAsynWaitParaListLock)
            {
                if (!_enable)
                {
                    return;
                }

                _partAsynWaitParaList.Remove(asynExcuteCancell);
            }
        }



        /// <summary>
        /// 取消全部执行
        /// </summary>
        public static void CancellAll()
        {
            PrimitiveCancellAll(null);
        }

        /// <summary>
        /// 取消执行符合条件的部分
        /// </summary>
        /// <param name="condition">匹配条件回调,返回true时取消,false忽略</param>
        public static void CancellAll(Func<object, bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            PrimitiveCancellAll(condition);
        }

        /// <summary>
        /// 取消执行符合条件的部分
        /// </summary>
        /// <param name="condition">匹配条件回调,返回true时取消,false忽略</param>
        private static void PrimitiveCancellAll(Func<object, bool> condition)
        {
            IAsynExcuteCancell[] asynExcuteCancellArr = null;
            lock (_partAsynWaitParaListLock)
            {
                if (!_enable || _partAsynWaitParaList.Count == 0)
                {
                    return;
                }

                asynExcuteCancellArr = _partAsynWaitParaList.ToArray();
            }

            foreach (var asynExcuteCancell in asynExcuteCancellArr)
            {
                if (condition == null ||
                    condition != null && condition(asynExcuteCancell.Tag))
                {
                    asynExcuteCancell.Cancell();
                }
            }
        }
    }
}
