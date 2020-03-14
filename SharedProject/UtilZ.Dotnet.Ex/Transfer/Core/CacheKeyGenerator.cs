using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class CacheKeyGenerator
    {
        public static string GenerateTransferCompletedMessageCacheKey(TransferCompletedMessage message)
        {
            return $"UDP_TransferCompletedMessageCacheKey_{message.Header.Rid}";
        }

        public static string GenerateWaitTransferCompletedAckMessageEventWaitHandleId(int rid)
        {
            return $"UDP_WaitTransferCompletedAckMessageEventWaitHandleKey_{rid}";
        }

        public static string GenerateRevTimeoutKey(int rid)
        {
            return $"UDP_RevTimeoutKey_{rid}";
        }
    }
}
