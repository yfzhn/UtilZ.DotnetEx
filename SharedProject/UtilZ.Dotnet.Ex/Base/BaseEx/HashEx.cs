using UtilZ.Dotnet.Ex.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Hash和MD5扩展使用类
    /// </summary>
    public static class HashEx
    {
        /// <summary>
        /// 获取二进制数据的Hash值
        /// </summary>
        /// <param name="str">要计算Hash值的字符串</param>
        /// <param name="hashType">HashType[默认值:SHA1Managed]</param>
        /// <returns>所得Hash值</returns>
        public static string GetHash(string str, MD5HashType hashType = MD5HashType.SHA1Managed)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return GetHash(Encoding.UTF8.GetBytes(str), hashType);
        }

        /// <summary>
        /// 获取二进制数据的Hash值
        /// </summary>
        /// <param name="data">要计算Hash值0的二进制数据</param>
        /// <param name="hashType">HashType[默认值:SHA1Managed]</param>
        /// <returns>所得Hash值</returns>
        public static string GetHash(byte[] data, MD5HashType hashType = MD5HashType.SHA1Managed)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (HashAlgorithm entry = HashEx.CreateHashEntry(hashType))
            {
                //计算指定Stream 对象的哈希值
                var arrbytHashValue = entry.ComputeHash(data);

                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                var md5HashValue = BitConverter.ToString(arrbytHashValue);

                //替换-为string.Empty(去年-)
                md5HashValue = md5HashValue.Replace("-", string.Empty);
                return md5HashValue;
            }
        }

        /// <summary>
        /// 获取二进制数据的Hash值
        /// </summary>
        /// <param name="data">要计算Hash值0的二进制数据</param>
        /// <param name="offset">字节数据中的偏移量,从该位置起使用数据</param>
        /// <param name="count">数组中用途数据的字节数</param>
        /// <param name="hashType">HashType[默认值:SHA1Managed]</param>
        /// <returns>所得Hash值</returns>
        public static string GetHash(byte[] data, int offset, int count, MD5HashType hashType = MD5HashType.SHA1Managed)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (HashAlgorithm entry = HashEx.CreateHashEntry(hashType))
            {
                //计算指定Stream 对象的哈希值
                var arrbytHashValue = entry.ComputeHash(data, offset, count);

                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                var md5HashValue = BitConverter.ToString(arrbytHashValue);

                //替换-为string.Empty(去年-)
                md5HashValue = md5HashValue.Replace("-", string.Empty);
                return md5HashValue;
            }
        }

        /// <summary>
        /// 获取一个文件的Hash值
        /// </summary>
        /// <param name="filePath">要计算Hash值的文件</param>
        /// <param name="hashType">HashType[默认值:SHA1Managed]</param>
        /// <returns>所得Hash值</returns>
        public static string GetFileHash(string filePath, MD5HashType hashType = MD5HashType.SHA1Managed)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Empty, filePath);
            }

            try
            {
                using (HashAlgorithm entry = HashEx.CreateHashEntry(hashType))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //计算指定Stream 对象的哈希值
                        var arrbytHashValue = entry.ComputeHash(fileStream);

                        //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                        var md5HashValue = BitConverter.ToString(arrbytHashValue);

                        //替换-为string.Empty(去年-)
                        md5HashValue = md5HashValue.Replace("-", string.Empty);
                        return md5HashValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception("获取文件的Hash值失败", ex);
            }
        }

        /// <summary>
        /// 获取指定目录中所有文件的Hash值,以目录树的形式在xml中存放每个文件的Hash
        /// </summary>
        /// <param name="targetDirectory">要创建hash树的目标目录</param>
        /// <param name="xmlPath">保存hash树xml文件的路径</param>
        /// <param name="hashType">HashType</param>
        public static void GetDirectoryFileHashTree(string targetDirectory, string xmlPath, MD5HashType hashType)
        {
            XDocument xdoc = new XDocument();
            DirectoryInfo dirInfo = new DirectoryInfo(targetDirectory);
            XElement root = new XElement("dir", string.Empty);

            root.Add(new XAttribute("value", dirInfo.Name));
            root.Add(new XAttribute("hashtype", hashType.ToString()));

            CreateHashTree(dirInfo, root, dirInfo.FullName, hashType);
            xdoc.Add(root);

            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }

            xdoc.Save(xmlPath);
        }

        /// <summary>
        /// 递归创建hash树
        /// </summary>
        /// <param name="dirInfo">创建hash树的DirectoryInfo</param>
        /// <param name="parentNode">父结点</param>
        /// <param name="baseDir">要创建hash树的基础路径</param>
        /// <param name="hashType">HashType</param>
        private static void CreateHashTree(DirectoryInfo dirInfo, XElement parentNode, string baseDir, MD5HashType hashType)
        {
            var fsytemFiles = dirInfo.GetFileSystemInfos();
            XElement currentNode = null;

            foreach (var sysFileInfo in fsytemFiles)
            {
                if (sysFileInfo is FileInfo)
                {
                    currentNode = new XElement("file");
                    currentNode.Add(new XAttribute("path", sysFileInfo.FullName.Substring(baseDir.Length + 1)));
                    currentNode.Add(new XAttribute("hash", GetFileHash(sysFileInfo.FullName, hashType)));
                }
                else if (sysFileInfo is DirectoryInfo)
                {
                    currentNode = new XElement("dir", string.Empty);
                    currentNode.Add(new XAttribute("value", dirInfo.Name));
                    CreateHashTree(sysFileInfo as DirectoryInfo, currentNode, baseDir, hashType);
                }
                else
                {
                    throw new Exception(string.Format("不能识别的类型{0}", sysFileInfo.GetType().FullName));
                }

                parentNode.Add(currentNode);
            }
        }

        /// <summary>
        /// 验证指定目录中所有文件的Hash值,以目录树中存放hash值的xml为准
        /// </summary>
        /// <param name="targetDirectory">要验证hash树的目标目录</param>
        /// <param name="hashXmlFile">Hash xml文件</param>
        /// <param name="hashType">MD5类型</param>
        /// <returns>与源一致,返回true,否则返回false</returns>
        public static bool ValidateDirectoryFileHashTree(string targetDirectory, string hashXmlFile, MD5HashType hashType)
        {
            try
            {
                string srcHashValue = string.Empty;
                string currentFileHashvalue = string.Empty;
                string filePath = string.Empty;

                XDocument hashXDoc = XDocument.Load(hashXmlFile);
                var fileNodes = (from file in hashXDoc.Descendants() where file.Name == "file" select file).ToList();

                foreach (var fileNode in fileNodes)
                {
                    filePath = Path.Combine(targetDirectory, fileNode.Attribute("path").Value);
                    if (!File.Exists(filePath))
                    {
                        return false;
                    }

                    srcHashValue = fileNode.Attribute("hash").Value;
                    currentFileHashvalue = GetFileHash(filePath, hashType);
                    if (currentFileHashvalue != srcHashValue)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据HashType创建对应的Hash实例
        /// </summary>
        /// <param name="hashType">HashType</param>
        /// <returns>Hash实例</returns>
        private static HashAlgorithm CreateHashEntry(MD5HashType hashType)
        {
            HashAlgorithm entry = null;
            switch (hashType)
            {
                case MD5HashType.HMACSHA1:
                    entry = new HMACSHA1();
                    break;
                //case MD5HashType.MACTripleDES:
                //    entry = new MACTripleDES();
                //    break;
                case MD5HashType.MD5CryptoServiceProvider:
                    entry = new MD5CryptoServiceProvider();
                    break;
                case MD5HashType.SHA1Managed:
                    entry = new SHA1Managed();
                    break;
                case MD5HashType.SHA256Managed:
                    entry = new SHA256Managed();
                    break;
                case MD5HashType.SHA384Managed:
                    entry = new SHA384Managed();
                    break;
                case MD5HashType.SHA512Managed:
                    entry = new SHA512Managed();
                    break;
            }
            return entry;
        }
    }

    /// <summary>
    /// HashType
    /// </summary>
    public enum MD5HashType
    {
        /// <summary>
        /// 使用SHA1哈希函数计算基于哈希值的消息验证代码(HMAC),同样的数据每次计算出的hash会是是不一样的
        /// </summary>
        [DisplayNameExAttribute("HMACSHA1", "使用SHA1哈希函数计算基于哈希值的消息验证代码(HMAC),同样的数据每次计算出的hash会是是不一样的")]
        HMACSHA1,

        ///// <summary>
        ///// 使用TripleDES计算输入数据CryptoStream的消息验证代码(MAC)
        ///// </summary>
        //[DisplayNameExAttribute("MACTripleDES", "使用TripleDES计算输入数据CryptoStream的消息验证代码(MAC")]
        //MACTripleDES,

        /// <summary>
        /// 使用加密服务提供程序(CSP)提供的实现，计算输入数据的MD5哈希值
        /// </summary>
        [DisplayNameExAttribute("MD5CryptoServiceProvider", "使用加密服务提供程序(CSP)提供的实现,计算输入数据的MD5哈希值")]
        MD5CryptoServiceProvider,

        /// <summary>
        /// 使用托管库计算输入数据的SHA1哈希值
        /// </summary>
        [DisplayNameExAttribute("SHA1Managed", "使用托管库计算输入数据的SHA1哈希值")]
        SHA1Managed,

        /// <summary>
        /// 使用托管库计算输入数据的SHA256哈希值
        /// </summary>
        [DisplayNameExAttribute("SHA256Managed", "使用托管库计算输入数据的SHA256哈希值")]
        SHA256Managed,

        /// <summary>
        /// 使用托管库计算输入数据的SHA384哈希值
        /// </summary>
        [DisplayNameExAttribute("SHA384Managed", "使用托管库计算输入数据的SHA384哈希值")]
        SHA384Managed,

        /// <summary>
        /// 使用托管库计算输入数据的SHA512哈希算法
        /// </summary>
        [DisplayNameExAttribute("SHA512Managed", "使用托管库计算输入数据的SHA512哈希算法")]
        SHA512Managed
    }
}
