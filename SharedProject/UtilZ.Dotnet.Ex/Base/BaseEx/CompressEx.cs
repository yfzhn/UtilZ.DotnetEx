using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 压缩解压
    /// </summary>
    public class CompressEx
    {
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="bytes">待压缩字节数组</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressBytes(byte[] bytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true);
                zipStream.Write(bytes, 0, bytes.Length);
                //zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                return outStream.ToArray();
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="bytes">待解压字节数组</param>
        /// <returns>解压后的字节数组</returns>
        public static byte[] DecompressBytes(byte[] bytes)
        {
            using (MemoryStream inputStream = new MemoryStream(bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress);
                    zipStream.CopyTo(outStream);
                    //zipStream.Close();
                    return outStream.ToArray();
                }
            }
        }


        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="dir"></param>
        public static void CompressDir(DirectoryInfo dir)
        {
            foreach (FileInfo fileToCompress in dir.GetFiles())
            {
                CompressFile(fileToCompress);
            }
        }

        /// <summary>
        /// 解压缩目录
        /// </summary>
        /// <param name="dir"></param>
        public static void DecompressDir(DirectoryInfo dir)
        {
            foreach (FileInfo fileToCompress in dir.GetFiles())
            {
                DecompressFile(fileToCompress);
            }
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToCompress"></param>
        public static void CompressFile(FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="fileToDecompress"></param>
        public static void DecompressFile(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
        }
    }
}
