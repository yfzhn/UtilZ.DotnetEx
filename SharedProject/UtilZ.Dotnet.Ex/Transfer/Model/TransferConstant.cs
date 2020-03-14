using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 传输常量定义类
    /// </summary>
    public class TransferConstant
    {
        /// <summary>
        /// MTU最大值
        /// </summary>
        public const int MTU_MAX = 65000;//原本是(65507=65535-20-8),保留一点,靠谱点

        /// <summary>
        /// MTU最小值
        /// </summary>
        public const int MTU_MIN = 540;//原本是(548=576-20-8),保留一点,靠谱点

        /// <summary>
        /// RTO最小值
        /// </summary>
        public const int RTO_MIN = 10;

        /// <summary>
        /// RTO默认值
        /// </summary>
        public const int DEFAULT_RTO = 500;

        /// <summary>
        /// 协议版本号
        /// </summary>
        public const int PROTOCOL_VER = 1;

        /// <summary>
        /// 同步字
        /// </summary>
        public const int SYNC = 1024;

        /// <summary>
        /// 校验码填充值
        /// </summary>
        public const int VALID_CODE_FILL = 0;

        /// <summary>
        /// 公共头大小
        /// </summary>
        public const int COMMON_HEADER_SIZE = 28;

        /// <summary>
        /// 并行传输数据最大线程数
        /// </summary>
        public const byte PARALLEL_THREAD_MAX_COUN = 16;

        /// <summary>
        /// 消息数据最大值
        /// </summary>
        public const int MESSAGE_MAX_SIZE = TransferConstant.MTU_MIN - TransferConstant.COMMON_HEADER_SIZE - SendDataNotifyMessage.HEAD_SIZE;

        #region 日志事件ID
        /// <summary>
        /// 追踪事件ID
        /// </summary>
        public const int TRACE_EVENT_ID = 1;

        /// <summary>
        /// 调试事件ID
        /// </summary>
        public const int DEBUG_EVENT_ID = 2;

        /// <summary>
        /// 信息事件ID
        /// </summary>
        public const int INFO_EVENT_ID = 3;

        /// <summary>
        /// 警告事件ID
        /// </summary>
        public const int WARN_EVENT_ID = 4;

        /// <summary>
        /// 错误事件ID
        /// </summary>
        public const int ERROR_EVENT_ID = 5;

        /// <summary>
        /// 致命错误事件ID
        /// </summary>
        public const int FATAL_EVENT_ID = 6;
        #endregion
    }
}
