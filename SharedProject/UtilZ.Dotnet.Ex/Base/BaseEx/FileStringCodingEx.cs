using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 将文件编码成字符串与将字符串解码成文件
    /// </summary>
    public class FileStringCodingEx
    {
        /// <summary>        
        /// 解码编码为字符串的数据，算法: ((firstByte - 65) 左移 4) + lastByte - 65 => sourceByte
        /// </summary>
        /// <param name="strData">字符串数据</param>
        /// <param name="filePath">要保存的文件名</param>
        /// <returns></returns>
        public static void DecodingFile(string strData, string filePath)
        {
            byte[] buffer = FileStringCodingEx.DecodingString(strData);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 解码编码为字符串的数据，算法: ((firstByte - 65) 左移 4) + lastByte - 65 => sourceByte
        /// </summary>
        /// <param name="strData">字符串数据</param>
        /// <returns>源数据</returns>
        public static byte[] DecodingString(string strData)
        {
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(strData);
            if (buffer.Length % 2 != 0)
            {
                throw new Exception("buffer length error");
            }

            byte[] data = new byte[buffer.Length / 2];
            for (int i = 0; i < buffer.Length; i = i + 2)
            {
                data[i / 2] = (byte)(((buffer[i] - 65) << 4) + buffer[i + 1] - 65);
            }
            return data;
        }

        /// <summary>
        /// 编码二进制数据为字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>编码后的字符串</returns>
        public static string EncodingBytes(byte[] data)
        {
            //(A ~ Z)，算法:sourceByte => firstByte = (byte 右移 4) + 65 和 lastByte = (byte&(byte)15) + 65

            // 创建临时缓冲
            byte[] encodedData = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                // 编码sourceByte
                encodedData[i * 2] = (byte)((b >> 4) + 65);
                encodedData[i * 2 + 1] = (byte)((b & (byte)15) + 65);
            }

            // 转换为字符串
            return System.Text.Encoding.ASCII.GetString(encodedData);
        }

        /// <summary>
        /// 编码二进制数据为字符串
        /// </summary>
        /// <param name="filePath">数据文件</param>
        /// <returns>编码后的字符串</returns>
        public static string EncodingFile(string filePath)
        {
            //(A ~ Z)，算法:sourceByte => firstByte = (byte 右移 4) + 65 和 lastByte = (byte&(byte)15) + 65
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                return FileStringCodingEx.EncodingBytes(buffer);
            }
        }

        /// <summary>
        /// 压缩字编码符串
        /// </summary>
        /// <param name="input">待压缩的编码字符串</param>
        /// <returns>压缩后的字符串</returns>
        public static string CompressEncoding(string input)
        {
            byte[] inputBytes = System.Text.Encoding.Default.GetBytes(input);
            byte[] result = CompressEx.CompressBytes(inputBytes);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 解压缩编码字符串
        /// </summary>
        /// <param name="input">待解压缩的编码字符串</param>
        /// <returns>解压缩后的字符串</returns>
        public static string DecompressEncoding(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] depressBytes = CompressEx.DecompressBytes(inputBytes);
            return System.Text.Encoding.Default.GetString(depressBytes);
        }
    }
}
