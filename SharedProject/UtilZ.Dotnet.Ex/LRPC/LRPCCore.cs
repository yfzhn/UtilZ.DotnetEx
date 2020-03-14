using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.LRPC
{
    /// <summary>
    /// 本地远程调用核心
    /// </summary>
    public class LRPCCore
    {
        /// <summary>
        /// 远程调用Hashtable集合[key:通道名称(string);value:通道(LRPCChannel)]
        /// </summary>
        private static readonly ConcurrentDictionary<string, LRPCChannel> _channelDic = new ConcurrentDictionary<string, LRPCChannel>();

        #region 创建调用通道
        ///// <summary>
        ///// 创建或替换已存在的本地远程调用通道
        ///// </summary>
        ///// <param name="channelName">通道名称</param>
        ///// <param name="pro">通道回调</param>
        //public static void CreateOrReplaceChannelF(string channelName, Func<object, object> pro)
        //{
        //    if (string.IsNullOrWhiteSpace(channelName))
        //    {
        //        throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
        //    }

        //    if (pro == null)
        //    {
        //        throw new ArgumentNullException(nameof(pro), "回调委托不能为null");
        //    }

        //    _channelDic[channelName] = new LRPCChannel(channelName, pro);
        //}

        ///// <summary>
        ///// 创建或替换已存在的本地远程调用通道
        ///// </summary>
        ///// <param name="channelName">通道名称</param>
        ///// <param name="pro">通道回调</param>
        //public static void CreateOrReplaceChannelA(string channelName, Action<object> pro)
        //{
        //    if (string.IsNullOrWhiteSpace(channelName))
        //    {
        //        throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
        //    }

        //    if (pro == null)
        //    {
        //        throw new ArgumentNullException(nameof(pro), "回调委托不能为null");
        //    }

        //    _channelDic[channelName] = new LRPCChannel(channelName, pro);
        //}

        /// <summary>
        /// 创建本地远程调用通道[返回值:true:创建成功;false:创建失败,该通道已存在]
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="pro">通道回调</param>
        /// <returns>创建结果</returns>
        public static bool TryCreateChannelF(string channelName, Func<object, object> pro)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
            }

            if (pro == null)
            {
                throw new ArgumentNullException(nameof(pro), "回调委托不能为null");
            }

            if (_channelDic.ContainsKey(channelName))
            {
                return false;
            }

            _channelDic[channelName] = new LRPCChannel(channelName, pro);
            return true;
        }

        /// <summary>
        /// 创建本地远程调用通道[返回值:true:创建成功;false:创建失败,该通道已存在]
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="pro">通道回调</param>
        /// <returns>创建结果</returns>
        public static bool TryCreateChannelA(string channelName, Action<object> pro)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
            }

            if (pro == null)
            {
                throw new ArgumentNullException(nameof(pro), "回调委托不能为null");
            }

            if (_channelDic.ContainsKey(channelName))
            {
                return false;
            }

            _channelDic[channelName] = new LRPCChannel(channelName, pro);
            return true;
        }

        /// <summary>
        /// 创建本地远程调用通道
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="pro">通道回调</param>
        public static void CreateChannelF(string channelName, Func<object, object> pro)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
            }

            if (pro == null)
            {
                throw new ArgumentNullException(nameof(pro), "回调委托不能为null");
            }

            if (_channelDic.ContainsKey(channelName))
            {
                throw new ArgumentException(string.Format("已存在名称为:{0}的本地远程调用通道", channelName));
            }

            _channelDic[channelName] = new LRPCChannel(channelName, pro);
        }

        /// <summary>
        /// 创建本地远程调用通道
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="pro">通道回调</param>
        public static void CreateChannelA(string channelName, Action<object> pro)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
            }

            if (pro == null)
            {
                throw new ArgumentNullException(nameof(pro), "回调委托不能为null");
            }

            if (_channelDic.ContainsKey(channelName))
            {
                throw new ArgumentException(string.Format("已存在名称为:{0}的本地远程调用通道", channelName));
            }

            _channelDic[channelName] = new LRPCChannel(channelName, pro);
        }
        #endregion

        /// <summary>
        /// 是否存在本地远程调用通道[存在返回true;不存在返回false]
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <returns>存在返回true;不存在返回false</returns>
        public static bool ExistChannel(string channelName)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
            }

            return _channelDic.ContainsKey(channelName);
        }

        /// <summary>
        /// 获取已创建的本地远程调用通道名称列表
        /// </summary>
        /// <returns>已创建的本地远程调用通道名称列表</returns>
        public static string[] GetChannelNames()
        {
            return _channelDic.Keys.ToArray();
        }

        /// <summary>
        /// 删除本地远程调用通道
        /// </summary>
        /// <param name="channelName">通道名称</param>
        public static void DeleteChannel(string channelName)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                throw new ArgumentNullException(nameof(channelName), "本地远程调用通道名称不能为空或全空格");
            }

            LRPCChannel channel;
            _channelDic.TryRemove(channelName, out channel);
        }

        /// <summary>
        /// 清空所有通道
        /// </summary>
        public static void ClearChannel()
        {
            _channelDic.Clear();
        }

        #region 调用
        /// <summary>
        /// 本地远程调用[如果通道未创建则会抛出]NotFoundLRPCChannelException
        /// </summary>
        /// <param name="channelName">远程通道名称</param>
        /// <param name="obj">远程调用参数</param>
        /// <returns>远程调用输出结果</returns>
        public static object RemoteCallF(string channelName, object obj)
        {
            LRPCChannel channel;
            if (_channelDic.TryGetValue(channelName, out channel))
            {
                return channel.OnRaiseProF(obj);
            }
            else
            {
                throw new NotFoundLRPCChannelException(string.Format("名称为:{0}的远程调用通道未创建", channelName));
            }
        }

        /// <summary>
        /// 本地远程调用[如果通道未创建则会抛出]NotFoundLRPCChannelException
        /// </summary>
        /// <param name="channelName">远程通道名称</param>
        /// <param name="obj">远程调用参数</param>
        public static void RemoteCallA(string channelName, object obj)
        {
            LRPCChannel channel;
            if (_channelDic.TryGetValue(channelName, out channel))
            {
                channel.OnRaiseProA(obj);
            }
            else
            {
                throw new NotFoundLRPCChannelException(string.Format("名称为:{0}的远程调用通道未创建", channelName));
            }
        }

        /// <summary>
        /// 尝试本地远程调用[返回值:true:调用成功;false:调用失败]
        /// </summary>
        /// <param name="channelName">远程通道名称</param>
        /// <param name="obj">远程调用参数</param>
        /// <param name="result">远程调用输出结果</param>
        /// <returns>远程调用结果</returns>
        public static bool TryRemoteCallF(string channelName, object obj, out object result)
        {
            LRPCChannel channel;
            if (_channelDic.TryGetValue(channelName, out channel))
            {
                result = channel.OnRaiseProF(obj);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// 尝试本地远程调用[返回值:true:调用成功;false:调用失败]
        /// </summary>
        /// <param name="channelName">远程通道名称</param>
        /// <param name="obj">远程调用参数</param>
        /// <returns>远程调用结果</returns>
        public static bool TryRemoteCallA(string channelName, object obj)
        {
            LRPCChannel channel;
            if (_channelDic.TryGetValue(channelName, out channel))
            {
                channel.OnRaiseProA(obj);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
