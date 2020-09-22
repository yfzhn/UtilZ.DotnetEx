using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    //WavHelper-Config
    public partial class WavHelper
    {
        #region Config
        /// <summary>
        /// Retrieves the value of a config option. 
        /// </summary>
        /// <param name="option">The option to get the value of... one of the following.</param>
        /// <returns>If successful, the value of the requested config option is returned, else throw WavException. </returns>
        public static int BASS_GetConfig(BASSConfig option)
        {
            int result = NativeMethods.BASS_GetConfig(option);
            //If successful, the value of the requested config option is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code
            if (result == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

        /// <summary>
        /// Retrieves the value of a config option. 
        /// </summary>
        /// <param name="option">The option to get the value of... one of the following.</param>
        /// <returns>the value of the requested config option is returned </returns>
        public static bool BASS_GetConfigBool(BASSConfig option)
        {
            return NativeMethods.BASS_GetConfigBool(option);
        }

        /// <summary>
        /// Retrieves the value of a config option. 
        /// </summary>
        /// <param name="option">The option to get the value of... one of the following.</param>
        /// <returns>If successful, the value of the requested config option is returned, else throw WavException. </returns>
        public static string BASS_GetConfigString(BASSConfig option)
        {
            IntPtr result = NativeMethods.BASS_GetConfigPtr(option | ((BASSConfig)(-2147483648)));
            //If successful, the value of the requested config option is returned, else NULL is returned. NULL may also be a valid setting with some config options, in which case the error code should be used to confirm whether it's an error. Use BASS_ErrorGetCode to get the error code. 
            if (result == IntPtr.Zero)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return Utils.IntPtrAsStringUnicode(result);
        }






        /// <summary>
        /// Sets the value of a config option. 
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following.</param>
        /// <param name="newvalue">The new option setting. See the option's documentation for details on the possible values. </param>
        public static void BASS_SetConfig(BASSConfig option, bool newvalue)
        {
            bool result = NativeMethods.BASS_SetConfig(option, newvalue);
            //If successful, the value of the requested config option is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Sets the value of a config option. 
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following.</param>
        /// <param name="newvalue">The new option setting. See the option's documentation for details on the possible values. </param>
        public static void BASS_SetConfig(BASSConfig option, int newvalue)
        {
            bool result = NativeMethods.BASS_SetConfig(option, newvalue);
            //If successful, the value of the requested config option is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Sets the value of a pointer config option
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following</param>
        /// <param name="newvalue">The new option setting. See the option's documentation for details on the possible values. </param>
        public static void BASS_SetConfigPtr(BASSConfig option, IntPtr newvalue)
        {
            bool result = NativeMethods.BASS_SetConfigPtr(option, newvalue);
            //If successful, the value of the requested config option is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Sets the value of a pointer config option
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following</param>
        public static bool BASS_SetConfigString(BASSConfig option, string newvalue)
        {
            return NativeMethods.BASS_SetConfigStringUnicode(option | ((BASSConfig)(-2147483648)), newvalue);
        }
        #endregion
    }
}
