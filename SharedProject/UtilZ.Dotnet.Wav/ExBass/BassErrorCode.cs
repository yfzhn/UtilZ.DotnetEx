using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.ExBass
{
    /// <summary>
    /// Bass错误码类
    /// </summary>
    public class BassErrorCode
    {
        /// <summary>
        /// Bass错误码集合表
        /// </summary>
        private static readonly Hashtable _htErrorCode = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 静态构造函数初始化
        /// </summary>
        static BassErrorCode()
        {
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_OK, "BASS_OK");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_MEM, "BASS_ERROR_MEM");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_FILEOPEN, "BASS_ERROR_FILEOPEN");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_DRIVER, "BASS_ERROR_DRIVER");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_BUFLOST, "BASS_ERROR_BUFLOST");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_HANDLE, "BASS_ERROR_HANDLE");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_FORMAT, "BASS_ERROR_FORMAT");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_POSITION, "BASS_ERROR_POSITION");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_INIT, "BASS_ERROR_INIT");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_START, "BASS_ERROR_START");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_SSL, "BASS_ERROR_SSL");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_ALREADY, "BASS_ERROR_ALREADY");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOCHAN, "BASS_ERROR_NOCHAN");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_ILLTYPE, "BASS_ERROR_ILLTYPE");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_ILLPARAM, "BASS_ERROR_ILLPARAM");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NO3D, "BASS_ERROR_NO3D");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOEAX, "BASS_ERROR_NOEAX");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_DEVICE, "BASS_ERROR_DEVICE");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOPLAY, "BASS_ERROR_NOPLAY");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_FREQ, "BASS_ERROR_FREQ");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOTFILE, "BASS_ERROR_NOTFILE");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOHW, "BASS_ERROR_NOHW");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_EMPTY, "BASS_ERROR_EMPTY");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NONET, "BASS_ERROR_NONET");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_CREATE, "BASS_ERROR_CREATE");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOFX, "BASS_ERROR_NOFX");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_NOTAVAIL, "BASS_ERROR_NOTAVAIL");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_DECODE, "BASS_ERROR_DECODE");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_DX, "BASS_ERROR_DX");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_TIMEOUT, "BASS_ERROR_TIMEOUT");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_FILEFORM, "BASS_ERROR_FILEFORM");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_SPEAKER, "BASS_ERROR_SPEAKER");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_VERSION, "BASS_ERROR_VERSION");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_CODEC, "BASS_ERROR_CODEC");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_ENDED, "BASS_ERROR_ENDED");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_BUSY, "BASS_ERROR_BUSY");
            BassErrorCode._htErrorCode.Add(BassErrorCode.BASS_ERROR_UNKNOWN, "BASS_ERROR_UNKNOWN");
        }

        #region 错误码
        /// <summary>
        /// 
        /// </summary>
        public const int BASS_OK = 0;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_MEM = 1;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_FILEOPEN = 2;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_DRIVER = 3;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_BUFLOST = 4;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_HANDLE = 5;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_FORMAT = 6;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_POSITION = 7;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_INIT = 8;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_START = 9;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_SSL = 10;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_ALREADY = 14;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOCHAN = 18;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_ILLTYPE = 19;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_ILLPARAM = 20;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NO3D = 21;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOEAX = 22;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_DEVICE = 23;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOPLAY = 24;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_FREQ = 25;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOTFILE = 27;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOHW = 29;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_EMPTY = 31;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NONET = 32;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_CREATE = 33;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOFX = 34;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_NOTAVAIL = 37;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_DECODE = 38;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_DX = 39;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_TIMEOUT = 40;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_FILEFORM = 41;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_SPEAKER = 42;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_VERSION = 43;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_CODEC = 44;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_ENDED = 45;

        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_BUSY = 46;
        
        /// <summary>
        /// 
        /// </summary>
        public const int BASS_ERROR_UNKNOWN = -1;
        #endregion

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>错误信息</returns>
        public static string GetErrorInfo()
        {
            int errCode = Bass.BASS_ErrorGetCode();
            return BassErrorCode.GetErrorInfo(errCode);
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="errCode">错误码</param>
        /// <returns>错误信息</returns>
        public static string GetErrorInfo(int errCode)
        {
            object value = BassErrorCode._htErrorCode[errCode];
            if (value == null)
            {
                return errCode.ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
