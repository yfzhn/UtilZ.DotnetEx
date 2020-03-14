using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    ///  char类型扩展方法类
    /// </summary>
    public static class CharEx
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static CharEx()
        {
            //中文标点符号字符串
            string chinesePunctuationStr = @"·！￥…（）— 【】、；‘：“，。、《》？";
            for (int i = 0; i < chinesePunctuationStr.Length; i++)
            {
                _chinesePunctuations.Add((int)chinesePunctuationStr[i]);
            }
        }

        /// <summary>
        /// 中文标点符号
        /// </summary>
        private static List<int> _chinesePunctuations = new List<int>();

        /// <summary>
        /// 中文汉字字符最小值
        /// </summary>
        private const int _chineseCharMinValue = 0x4e00;

        /// <summary>
        /// 中文汉字字符最大值
        /// </summary>
        private const int _chineseCharMaxValue = 0x9Fbd;

        /// <summary>
        /// 验证一个字符是否是中文字符[中文字符:返回true,否则返回false]
        /// </summary>
        /// <param name="ch">待验证的字符</param>
        /// <returns>中文字符:返回true,否则返回false</returns>
        public static bool IsChineseChar(this char ch)
        {
            int charValue = (int)ch;
            //如果是中文字符返回true
            if (_chinesePunctuations.Contains(charValue))
            {
                return true;
            }

            //如果是中文汉字返回true
            if (charValue >= _chineseCharMinValue && charValue <= _chineseCharMaxValue)
            {
                return true;
            }

            //默认返回false
            return false;
        }

        /// <summary>
        /// 验证一个字符是否属于26个字母[true:属于26个字母,false:非26个英文字母]
        /// </summary>
        /// <param name="ch">待验证的字符</param>
        /// <returns>true:属于26个字母,false:非26个英文字母</returns>
        public static bool IsLetter(char ch)
        {
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' || ch <= 'Z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 字母是否是大写字母[true:是,false:不是]
        /// </summary>
        /// <param name="ch">待验证的字符</param>
        /// <returns>true:是,false:不是</returns>
        public static bool IsLetterUpper(char ch)
        {
            if (ch >= 'A' && ch <= 'Z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 字母是否是小写字母[true:是,false:不是]
        /// </summary>
        /// <param name="ch">待验证的字符</param>
        /// <returns>true:是,false:不是</returns>
        public static bool IsLetterLower(char ch)
        {
            if (ch >= 'a' && ch <= 'z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
