using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Base
{
    /// <summary>
    /// Bass异常
    /// </summary>
    public class WavException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WavException()
            : base()
        {

        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        public WavException(string message)
            : base(message)
        {

        }
    }
}
