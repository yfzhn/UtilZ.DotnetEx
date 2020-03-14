using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志安全策略接口
    /// </summary>
    public interface ILogSecurityPolicy
    {
        /// <summary>
        /// 加密日志消息
        /// </summary>
        /// <param name="logMsg">日志消息</param>
        /// <returns>加密后的日志消息</returns>
        string Encryption(string logMsg);

        /// <summary>
        /// 解密日志消息
        /// </summary>
        /// <param name="logMsg">日志消息</param>
        /// <returns>解密后的日志消息</returns>
        string Decryption(string logMsg);

        /// <summary>
        /// 解密日志
        /// </summary>
        /// <param name="srcLogFilePath">加密的日志文件路径</param>
        /// <param name="targetLogFilePath">解密后的日志文件路径</param>
        void Decryption(string srcLogFilePath, string targetLogFilePath);
    }
}
