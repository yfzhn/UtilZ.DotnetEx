using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model
{
    /// <summary>
    /// PartAsynUIPara类代理
    /// </summary>
    public class PartAsynUIParaProxy
    {
        /// <summary>
        /// 设置一个异步等待
        /// </summary>
        /// <param name="partAsynUIPara">异步等待UI参数</param>
        /// <param name="asynWait">异步等待</param>
        public static void SetAsynWait(PartAsynWaitParaAbs partAsynUIPara, IPartAsynWait asynWait)
        {
            partAsynUIPara.AsynWait = asynWait;
        }

        /// <summary>
        /// 锁住参数对象[true:锁成功;false:锁失败]
        /// </summary>
        /// <param name="partAsynUIPara">异步等待UI参数</param>
        /// <returns>锁结果</returns>
        public static bool Lock(PartAsynWaitParaAbs partAsynUIPara)
        {
            return partAsynUIPara.Lock();
        }

        /// <summary>
        /// 解锁参数对象[true:解锁成功;false:解锁失败]
        /// </summary>
        /// <param name="partAsynUIPara">异步等待UI参数</param>
        /// <returns>解锁结果</returns>
        public static bool UnLock(PartAsynWaitParaAbs partAsynUIPara)
        {
            return partAsynUIPara.UnLock();
        }
    }
}
