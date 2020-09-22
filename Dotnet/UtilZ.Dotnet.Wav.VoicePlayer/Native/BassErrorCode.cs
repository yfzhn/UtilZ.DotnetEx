using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// Bass错误码类
    /// </summary>
    public class BassErrorCode
    {
        private static Dictionary<int, string> _errorDesDic = new Dictionary<int, string>();
        /// <summary>
        /// 静态构造函数初始化
        /// </summary>
        static BassErrorCode()
        {
            FieldInfo[] fieldInfoArr = typeof(BassErrorCode).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type desAttType = typeof(DescriptionAttribute);
            DescriptionAttribute desAtt;
            int value;

            foreach (FieldInfo fieldInfo in fieldInfoArr)
            {
                value = (int)fieldInfo.GetValue(null);
                desAtt = (DescriptionAttribute)fieldInfo.GetCustomAttribute(desAttType);
                _errorDesDic.Add(value, desAtt.Description);
            }
        }

        #region 错误码
        /// <summary>
        /// All is OK
        /// </summary>
        [Description("All is OK")]
        public const int BASS_OK = 0;

        /// <summary>
        /// Memory error
        /// </summary>
        [Description("Memory error")]
        public const int BASS_ERROR_MEM = 1;

        /// <summary>
        /// Can't open the file
        /// </summary>
        [Description("Can't open the file")]
        public const int BASS_ERROR_FILEOPEN = 2;

        /// <summary>
        /// Can't find a free/valid driver 
        /// </summary>
        [Description("Can't find a free/valid driver")]
        public const int BASS_ERROR_DRIVER = 3;

        /// <summary>
        /// The sample buffer was lost
        /// </summary>
        [Description("The sample buffer was lost")]
        public const int BASS_ERROR_BUFLOST = 4;

        /// <summary>
        /// Invalid handle
        /// </summary>
        [Description("Invalid handle")]
        public const int BASS_ERROR_HANDLE = 5;

        /// <summary>
        /// Unsupported sample format
        /// </summary>
        [Description("Unsupported sample format")]
        public const int BASS_ERROR_FORMAT = 6;

        /// <summary>
        /// Invalid playback position
        /// </summary>
        [Description("Invalid playback position")]
        public const int BASS_ERROR_POSITION = 7;

        /// <summary>
        /// BASS_Init has not been successfully called
        /// </summary>
        [Description("BASS_Init has not been successfully called")]
        public const int BASS_ERROR_INIT = 8;

        /// <summary>
        /// BASS_Start has not been successfully called
        /// </summary>
        [Description("BASS_Start has not been successfully called")]
        public const int BASS_ERROR_START = 9;

        /// <summary>
        /// No CD in drive 
        /// </summary>
        [Description("No CD in drive")]
        public const int BASS_ERROR_NOCD = 12;

        /// <summary>
        /// Invalid track number
        /// </summary>
        [Description("Invalid track number")]
        public const int BASS_ERROR_CDTRACK = 13;

        /// <summary>
        /// Already initialized/paused/whatever
        /// </summary>
        [Description("Already initialized/paused/whatever")]
        public const int BASS_ERROR_ALREADY = 14;

        /// <summary>
        /// Not paused
        /// </summary>
        [Description("Not paused")]
        public const int BASS_ERROR_NOPAUSE = 16;

        /// <summary>
        /// Not an audio track
        /// </summary>
        [Description("Not an audio track")]
        public const int BASS_ERROR_NOTAUDIO = 17;

        /// <summary>
        /// Can't get a free channel
        /// </summary>
        [Description("Can't get a free channel")]
        public const int BASS_ERROR_NOCHAN = 18;

        /// <summary>
        /// An illegal type was specified
        /// </summary>
        [Description("An illegal type was specified")]
        public const int BASS_ERROR_ILLTYPE = 19;

        /// <summary>
        /// An illegal parameter was specified
        /// </summary>
        [Description("An illegal parameter was specified")]
        public const int BASS_ERROR_ILLPARAM = 20;

        /// <summary>
        /// No 3D support
        /// </summary>
        [Description("No 3D support")]
        public const int BASS_ERROR_NO3D = 21;

        /// <summary>
        /// No EAX support
        /// </summary>
        [Description("No EAX support")]
        public const int BASS_ERROR_NOEAX = 22;

        /// <summary>
        /// Illegal device number
        /// </summary>
        [Description("Illegal device number")]
        public const int BASS_ERROR_DEVICE = 23;

        /// <summary>
        /// Not playing
        /// </summary>
        [Description("Not playing")]
        public const int BASS_ERROR_NOPLAY = 24;

        /// <summary>
        /// Illegal sample rate
        /// </summary>
        [Description("Illegal sample rate")]
        public const int BASS_ERROR_FREQ = 25;

        /// <summary>
        /// The stream is not a file stream
        /// </summary>
        [Description("The stream is not a file stream")]
        public const int BASS_ERROR_NOTFILE = 27;

        /// <summary>
        /// No hardware voices available
        /// </summary>
        [Description("No hardware voices available")]
        public const int BASS_ERROR_NOHW = 29;

        /// <summary>
        /// The MOD music has no sequence data
        /// </summary>
        [Description("The MOD music has no sequence data")]
        public const int BASS_ERROR_EMPTY = 31;

        /// <summary>
        /// No internet connection could be opened
        /// </summary>
        [Description("No internet connection could be opened")]
        public const int BASS_ERROR_NONET = 32;

        /// <summary>
        /// Couldn't create the file
        /// </summary>
        [Description("Couldn't create the file")]
        public const int BASS_ERROR_CREATE = 33;

        /// <summary>
        /// Effects are not available
        /// </summary>
        [Description("Effects are not available")]
        public const int BASS_ERROR_NOFX = 34;

        /// <summary>
        /// The channel is playing
        /// </summary>
        [Description("The channel is playing")]
        public const int BASS_ERROR_PLAYING = 35;

        /// <summary>
        /// Requested data is not available
        /// </summary>
        [Description("Requested data is not available")]
        public const int BASS_ERROR_NOTAVAIL = 37;

        /// <summary>
        /// The channel is a 'decoding channel'
        /// </summary>
        [Description("The channel is a 'decoding channel'")]
        public const int BASS_ERROR_DECODE = 38;

        /// <summary>
        /// A sufficient DirectX version is not installed
        /// </summary>
        [Description("A sufficient DirectX version is not installed")]
        public const int BASS_ERROR_DX = 39;

        /// <summary>
        /// Connection timedout
        /// </summary>
        [Description("Connection timedout")]
        public const int BASS_ERROR_TIMEOUT = 40;

        /// <summary>
        /// Unsupported file format
        /// </summary>
        [Description("Unsupported file format")]
        public const int BASS_ERROR_FILEFORM = 41;

        /// <summary>
        /// Unavailable speaker
        /// </summary>
        [Description("Unavailable speaker")]
        public const int BASS_ERROR_SPEAKER = 42;

        /// <summary>
        /// Invalid BASS version (used by add-ons) 
        /// </summary>
        [Description("Invalid BASS version (used by add-ons)")]
        public const int BASS_ERROR_VERSION = 43;

        /// <summary>
        /// Codec is not available/supported
        /// </summary>
        [Description("Codec is not available/supported")]
        public const int BASS_ERROR_CODEC = 44;

        /// <summary>
        /// The channel/file has ended
        /// </summary>
        [Description("The channel/file has ended")]
        public const int BASS_ERROR_ENDED = 45;

        /// <summary>
        /// The device is busy (eg. in "exclusive" use by another process)
        /// </summary>
        [Description("The device is busy (eg. in \"exclusive\" use by another process)")]
        public const int BASS_ERROR_BUSY = 46;

        /// <summary>
        /// Some other mystery error
        /// </summary>
        [Description("Some other mystery error")]
        public const int BASS_ERROR_UNKNOWN = -1;

        /// <summary>
        /// BassWma: the file is protected
        /// </summary>
        [Description("BassWma: the file is protected")]
        public const int BASS_ERROR_WMA_LICENSE = 1000;

        /// <summary>
        /// BassWma: WM9 is required
        /// </summary>
        [Description("BassWma: WM9 is required")]
        public const int BASS_ERROR_WMA_WM9 = 1001;

        /// <summary>
        /// BassWma: access denied(user/pass is invalid)
        /// </summary>
        [Description("BassWma: access denied(user/pass is invalid)")]
        public const int BASS_ERROR_WMA_DENIED = 1002;

        /// <summary>
        /// BassWma: no appropriate codec is installed
        /// </summary>
        [Description("BassWma: no appropriate codec is installed")]
        public const int BASS_ERROR_WMA_CODEC = 1003;

        /// <summary>
        /// BassWma: individualization is needed
        /// </summary>
        [Description("BassWma: individualization is needed")]
        public const int BASS_ERROR_WMA_INDIVIDUAL = 1004;

        /// <summary>
        /// BassEnc: ACM codec selection cancelled
        /// </summary>
        [Description("BassEnc: ACM codec selection cancelled")]
        public const int BASS_ERROR_ACM_CANCEL = 2000;

        /// <summary>
        /// BassEnc: Access denied(invalid password)
        /// </summary>
        [Description("BassEnc: Access denied(invalid password)")]
        public const int BASS_ERROR_CAST_DENIED = 2100;

        /// <summary>
        /// BassVst: the given effect has no inputs and is probably a VST instrument and no effect
        /// </summary>
        [Description("BassVst: the given effect has no inputs and is probably a VST instrument and no effect")]
        public const int BASS_VST_ERROR_NOINPUTS = 3000;

        /// <summary>
        /// BassVst: the given effect has no outputs
        /// </summary>
        [Description("BassVst: the given effect has no outputs")]
        public const int BASS_VST_ERROR_NOOUTPUTS = 3001;

        /// <summary>
        /// BassVst: the given effect does not support realtime processing
        /// </summary>
        [Description("BassVst: the given effect does not support realtime processing")]
        public const int BASS_VST_ERROR_NOREALTIME = 3002;

        /// <summary>
        /// BASSWASAPI: no WASAPI available
        /// </summary>
        [Description("BASSWASAPI: no WASAPI available")]
        public const int BASS_ERROR_WASAPI = 5000;

        /// <summary>
        /// BASSWASAPI: buffer size is invalid
        /// </summary>
        [Description("BASSWASAPI: buffer size is invalid")]
        public const int BASS_ERROR_WASAPI_BUFFER = 5001;

        /// <summary>
        ///  BASSWASAPI: can't set category
        /// </summary>
        [Description("BASSWASAPI: can't set category")]
        public const int BASS_ERROR_WASAPI_CATEGORY = 5002;

        /// <summary>
        /// BASS_AAC: non-streamable due to MP4 atom order('mdat' before 'moov')
        /// </summary>
        [Description("BASS_AAC: non-streamable due to MP4 atom order('mdat' before 'moov')")]
        public const int BASS_ERROR_MP4_NOSTREAM = 6000;

        /// <summary>
        /// BASSWEBM: non-streamable WebM audio
        /// </summary>
        [Description(" BASSWEBM: non-streamable WebM audio")]
        public const int BASS_ERROR_WEBM_NOTAUDIO = 8000;

        /// <summary>
        /// BASSWEBM: invalid track number
        /// </summary>
        [Description("BASSWEBM: invalid track number")]
        public const int BASS_ERROR_WEBM_TRACK = 8001;
        #endregion





        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>错误信息</returns>
        public static string GetErrorInfo()
        {
            int errCode = NativeMethods.BASS_ErrorGetCode();
            return GetErrorInfo(errCode);
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="errCode">错误码</param>
        /// <returns>错误信息</returns>
        public static string GetErrorInfo(int errCode)
        {
            string errorInfo;
            if (_errorDesDic.TryGetValue(errCode, out errorInfo))
            {
                return errorInfo;
            }
            else
            {
                return $"未知的错误码{errCode}";
            }
        }
    }
}
