using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// X日志安全策略类
    /// </summary>
    [Serializable]
    public class XSecurityPolicy : ILogSecurityPolicy
    {
        /// <summary>
        /// 加密key
        /// </summary>
        private const string _encrKey = "12345678";

        /// <summary>
        /// 对称加密初始化向量
        /// </summary>
        private byte[] _rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 加密日志消息
        /// </summary>
        /// <param name="logMsg">日志消息</param>
        /// <returns>加密后的日志消息</returns>
        public string Encryption(string logMsg)
        {
            byte[] byKey = Encoding.UTF8.GetBytes(_encrKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(logMsg);
            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, this._rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// 解密日志消息
        /// </summary>
        /// <param name="logMsg">日志消息</param>
        /// <returns>解密后的日志消息</returns>
        public string Decryption(string logMsg)
        {
            byte[] inputByteArray = new Byte[logMsg.Length];
            byte[] byKey = Encoding.UTF8.GetBytes(_encrKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(logMsg);
            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, this._rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = new UTF8Encoding();
                return encoding.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 解密日志
        /// </summary>
        /// <param name="srcLogFilePath">加密的日志文件路径</param>
        /// <param name="targetLogFilePath">解密后的日志文件路径</param>
        public void Decryption(string srcLogFilePath, string targetLogFilePath)
        {
            string line;
            string logMsg;
            using (StreamReader sr = new StreamReader(srcLogFilePath))
            {
                using (var sw = File.AppendText(targetLogFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        try
                        {
                            logMsg = this.Decryption(line);
                            sw.WriteLine(logMsg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

        }
    }
}
