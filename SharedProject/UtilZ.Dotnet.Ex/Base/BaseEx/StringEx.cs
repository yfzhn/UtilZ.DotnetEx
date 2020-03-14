using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 字符串类型扩展方法类
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// 字符串扩展方法:十进制或是十六进制的数值字符串转换为uint
        /// </summary>
        /// <param name="str">十进制或是十六进制的数值字符</param>
        /// <param name="value">转换出的uint</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool TryParseUInt32(this string str, out uint value)
        {
            value = 0;
            try
            {
                value = str.ParseUInt32();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 字符串扩展方法:十进制或是十六进制的数值字符串转换为uint
        /// </summary>
        /// <param name="str">十进制或是十六进制的数值字符串</param>
        /// <returns>转换出的uint</returns>
        public static uint ParseUInt32(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new Exception("入参字符串不能为空");
            }

            uint value = 0;
            string hxRegStr = @"^0x([\d,a-f])+$";
            string intRegStr = @"^\d+$";

            if (Regex.IsMatch(str, hxRegStr))
            {
                //如果命令编码为16进制的字符串，将其由16进制转字符串换为十进制数字
                value = Convert.ToUInt32(str, 16);
            }
            else if (Regex.IsMatch(str, intRegStr))
            {
                //命令编码为10进制字符串，将其由十进制字符串转换为十进制数字
                value = Convert.ToUInt32(str, 10);
            }
            else
            {
                throw new Exception(string.Format("字符串:{0}不是有效的十进制或十六进制数值字符串,转换失败", str));
            }
            return value;
        }

        /// <summary>
        /// 字符串扩展方法:十进制或是十六进制的数值字符串转换为uint
        /// </summary>
        /// <param name="str">十进制或是十六进制的数值字符</param>
        /// <param name="value">转换出的int</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool TryParseInt32(this string str, out int value)
        {
            value = 0;
            try
            {
                value = str.ParseInt32();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 字符串扩展方法:十进制或是十六进制的数值字符串转换为uint
        /// </summary>
        /// <param name="str">十进制或是十六进制的数值字符串</param>
        /// <returns>转换出的int</returns>
        public static int ParseInt32(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new Exception("入参字符串不能为空");
            }

            int value = 0;
            string hxRegStr = @"^0x([\d,a-f])+$";
            string intRegStr = @"^\-?\d+$";

            if (Regex.IsMatch(str, hxRegStr))
            {
                //如果命令编码为16进制的字符串，将其由16进制转字符串换为十进制数字
                value = Convert.ToInt32(str, 16);
            }
            else if (Regex.IsMatch(str, intRegStr))
            {
                //命令编码为10进制字符串，将其由十进制字符串转换为十进制数字
                value = Convert.ToInt32(str, 10);
            }
            else
            {
                throw new Exception(string.Format("字符串:{0}不是有效的十进制或十六进制数值字符串,转换失败", str));
            }
            return value;
        }

        /// <summary>
        /// 计算字符串中包含的中文字符数
        /// </summary>
        /// <param name="str">待计算的字符串</param>
        /// <returns>字符串中包含的中文字符数</returns>
        public static int CalculateChineseCharCount(this string str)
        {
            //已计算的中文字符个数
            int chineseCharCount = 0;
            //NExtendObject.DisorderLoop(str, (ch) =>
            //{
            //    if (NExtendChar.IsChineseChar(ch))
            //    {
            //        chineseCharCount++;
            //    }

            //    return true;
            //});

            for (int i = 0; i < str.Length; i++)
            {
                if (CharEx.IsChineseChar(str[i]))
                {
                    chineseCharCount++;
                }
            }

            return chineseCharCount;

            //if (str == null)
            //{
            //    throw new ArgumentNullException(NExtendObject.GetVarName(xx => str));
            //}

            ////空字符串返回0
            //if (string.IsNullOrEmpty(str))
            //{
            //    return 0;
            //}

            ////已计算的中文字符个数
            //int chineseCharCount = 0;
            //int length = str.Length / 2;
            //if (str.Length % 2 != 0)
            //{
            //    length += 1;
            //}

            //*************************************************
            // *    fp              pl      pr              rp
            // * |-->|              |<--|-->|              |<--|
            // * |______________________|______________________|
            // **************************************************/
            //int fp = 0;
            //int pl = length - 1;
            //int pr = 0;
            //int rp = str.Length - 1;
            ////fp,pl,pr,rp为四个方向移动的索引指针

            //for (fp = 0, pr = length; fp < length; fp++, pr++)
            //{
            //    if (fp <= pl && NExtendChar.IsChineseChar(str[fp]))
            //    {
            //        chineseCharCount++;
            //    }

            //    if (pl > fp)
            //    {
            //        if (NExtendChar.IsChineseChar(str[pl]))
            //        {
            //            chineseCharCount++;
            //        }

            //        pl--;
            //    }

            //    if (pr <= rp && NExtendChar.IsChineseChar(str[pr]))
            //    {
            //        chineseCharCount++;
            //    }

            //    if (rp > pr)
            //    {
            //        if (NExtendChar.IsChineseChar(str[rp]))
            //        {
            //            chineseCharCount++;
            //        }

            //        rp--;
            //    }
            //}

            //return chineseCharCount;
        }

        /// <summary>
        /// 字符串转换为十六进制的二进制数据
        /// </summary>
        /// <param name="str">待转换的字符串</param>
        /// <returns>十六进制格式的二进制数据</returns>
        public static byte[] ToHexadecimaBytes(this string str)
        {
            byte[] findBuffer = null;
            str = str.Replace(" ", string.Empty);
            if (str.Length % 2 != 0)
            {
                throw new ArgumentException("待转换的字符串长度不能为奇数");
            }

            findBuffer = new byte[str.Length / 2];
            for (int i = 0; i < findBuffer.Length; i++)
            {
                findBuffer[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }

            return findBuffer;
        }

        /// <summary>
        /// 字符串的字母是否全大写[true:全大写;false:大小写混合]
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>true:全大写;false:大小写混合</returns>
        public static bool IsAllUpper(this string str)
        {
            //bool result = true;
            //NExtendObject.DisorderLoop(str, (ch) =>
            //{
            //    if (NExtendChar.IsLetter(ch) && NExtendChar.IsLetterLower(ch))
            //    {
            //        result = false;
            //        return false;
            //    } 

            //    return true;
            //});

            //return result;

            char ch;
            for (int i = 0; i < str.Length; i++)
            {
                ch = str[i];
                if (CharEx.IsLetter(ch) && CharEx.IsLetterLower(ch))
                {
                    return false;
                }
            }

            return true;

            //if (string.IsNullOrEmpty(str))
            //{
            //    throw new ArgumentNullException(NExtendObject.GetVarName(xx => str));
            //}

            ////已计算的中文字符个数
            //bool result = true;
            //int length = str.Length / 2;
            //if (str.Length % 2 != 0)
            //{
            //    length += 1;
            //}

            //*************************************************
            // *    fp              pl      pr              rp
            // * |-->|              |<--|-->|              |<--|
            // * |______________________|______________________|
            // **************************************************/
            //int fp = 0;
            //int pl = length - 1;
            //int pr = 0;
            //int rp = str.Length - 1;

            //char ch;
            ////fp,pl,pr,rp为四个方向移动的索引指针

            //for (fp = 0, pr = length; fp < length; fp++, pr++)
            //{
            //    if (fp <= pl)
            //    {
            //        ch = str[fp];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterLower(ch))
            //        {
            //            result = false;
            //            break;
            //        }
            //    }

            //    if (pl > fp)
            //    {
            //        ch = str[pl];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterLower(ch))
            //        {
            //            result = false;
            //            break;
            //        }

            //        pl--;
            //    }

            //    if (pr <= rp)
            //    {
            //        ch = str[pr];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterLower(ch))
            //        {
            //            result = false;
            //            break;
            //        }
            //    }

            //    if (rp > pr)
            //    {
            //        ch = str[rp];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterLower(ch))
            //        {
            //            result = false;
            //            break;
            //        }

            //        rp--;
            //    }
            //}

            //return result;
        }

        /// <summary>
        /// 字符串的字母是否全小写[true:全小写;false:大小写混合]
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>true:全小写;false:大小写混合</returns>
        public static bool IsAllLower(this string str)
        {
            char ch;
            for (int i = 0; i < str.Length; i++)
            {
                ch = str[i];
                if (CharEx.IsLetter(ch) && CharEx.IsLetterUpper(ch))
                {
                    return false;
                }
            }

            return true;

            //bool result = true;
            //NExtendObject.DisorderLoop(str, (ch) =>
            //{
            //    if (NExtendChar.IsLetter(ch) && NExtendChar.IsLetterUpper(ch))
            //    {
            //        result = false;
            //        return false;
            //    }

            //    return true;
            //});

            //return result;

            //if (string.IsNullOrEmpty(str))
            //{
            //    throw new ArgumentNullException(NExtendObject.GetVarName(xx => str));
            //}

            ////已计算的中文字符个数
            //bool result = true;
            //int length = str.Length / 2;
            //if (str.Length % 2 != 0)
            //{
            //    length += 1;
            //}

            //*************************************************
            // *    fp              pl      pr              rp
            // * |-->|              |<--|-->|              |<--|
            // * |______________________|______________________|
            // **************************************************/
            //int fp = 0;
            //int pl = length - 1;
            //int pr = 0;
            //int rp = str.Length - 1;

            //char ch;
            ////fp,pl,pr,rp为四个方向移动的索引指针

            //for (fp = 0, pr = length; fp < length; fp++, pr++)
            //{
            //    if (fp <= pl)
            //    {
            //        ch = str[fp];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterUpper(ch))
            //        {
            //            result = false;
            //            break;
            //        }
            //    }

            //    if (pl > fp)
            //    {
            //        ch = str[pl];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterUpper(ch))
            //        {
            //            result = false;
            //            break;
            //        }

            //        pl--;
            //    }

            //    if (pr <= rp)
            //    {
            //        ch = str[pr];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterUpper(ch))
            //        {
            //            result = false;
            //            break;
            //        }
            //    }

            //    if (rp > pr)
            //    {
            //        ch = str[rp];
            //        if (NExtendString.IsLetter(ch) && NExtendString.IsLetterUpper(ch))
            //        {
            //            result = false;
            //            break;
            //        }

            //        rp--;
            //    }
            //}

            //return result;
        }
    }
}
