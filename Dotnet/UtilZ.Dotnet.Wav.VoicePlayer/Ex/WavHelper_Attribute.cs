using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    //WavHelper-Attribute
    public partial class WavHelper
    {
        #region Attribute
        /// <summary>
        /// Retrieves the value of a channel's attribute. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to get the value of... one of the following.</param>
        /// <returns>Pointer to a variable to receive the attribute value</returns>
        public static float ChannelGetAttribute(int handle, BASSAttribute attribute)
        {
            float value = default(float);
            bool result = NativeMethods.BASS_ChannelGetAttribute(handle, attribute, ref value);
            //If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. 
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return value;
        }

        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="size">The size of the attribute data... 0 = get the size of the attribute without getting the data. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int ChannelGetAttributeEx(int handle, BASSAttribute attribute, IntPtr value, int size)
        {
            int result = NativeMethods.BASS_ChannelGetAttributeEx(handle, attribute, value, size);
            //If successful, the size of the attribute data is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code. 
            if (result == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }


        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values</param>
        public static void ChannelSetAttribute(int handle, BASSAttribute attribute, float value)
        {
            bool result = NativeMethods.BASS_ChannelSetAttribute(handle, attribute, value);
            //If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. 
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }


        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to set the value of... one of the following.
        /// BASS_ATTRIB_SCANINFO Scanned info. (HSTREAM only)  
        /// other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute data</param>
        /// <param name="size">The size of the attribute data</param>
        public static void ChannelSetAttributeEx(int handle, BASSAttribute attribute, IntPtr value, int size)
        {
            bool result = NativeMethods.BASS_ChannelSetAttributeEx(handle, attribute, value, size);
            //If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. 
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }


        /// <summary>
        /// Slides a channel's attribute from its current value to a new value
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM, HMUSIC, or HRECORD</param>
        /// <param name="attribute">The attribute to slide the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="time">The length of time (in milliseconds) that it should take for the attribute to reach the value</param>
        public static void ChannelSlideAttribute(int handle, BASSAttribute attribute, float value, int time)
        {
            bool result = NativeMethods.BASS_ChannelSlideAttribute(handle, attribute, value, time);
            //If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Checks if an attribute (or any attribute) of a sample, stream, or MOD music is sliding
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="attribute">The attribute to check for sliding... one of the following, or 0 for any attribute.
        /// BASS_ATTRIB_EAXMIX EAX wet/dry mix.  
        /// BASS_ATTRIB_FREQ Sample rate.
        /// BASS_ATTRIB_PAN Panning/balance position.
        /// BASS_ATTRIB_VOL Volume level.  
        /// BASS_ATTRIB_MUSIC_AMPLIFY Amplification level. (HMUSIC only)
        /// BASS_ATTRIB_MUSIC_BPM BPM. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_PANSEP Pan separation level. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_PSCALER Position scaler. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_SPEED Speed. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_VOL_CHAN A channel volume level. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_VOL_GLOBAL Global volume level. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_VOL_INST An instrument/sample volume level. (HMUSIC)
        /// other attributes may be supported by add-ons, see the documentation.</param>
        /// <returns>If the attribute is sliding, then TRUE is returned, else FALSE is returned.</returns>
        public static bool ChannelIsSliding(int handle, BASSAttribute attribute)
        {
            return NativeMethods.BASS_ChannelIsSliding(handle, attribute);
        }






        /// <summary>
        /// get channel volume
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <returns>The channel volume The volume level... 0 (silent) to 1 (max)</returns>
        public static float ChannelGetVolume(int handle)
        {
            return ChannelGetAttribute(handle, BASSAttribute.BASS_ATTRIB_VOL);
        }

        /// <summary>
        /// set channel volume
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="volume">The channel volume The volume level... 0 (silent) to 1 (max)</param>
        public static void ChannelSetVolume(int handle, float volume)
        {
            ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_VOL, volume);
        }






        //public static int ChannelGetSpeed(int handle)
        //{
        //    return (int)ChannelGetAttribute(handle, BASSAttribute.BASS_ATTRIB_MUSIC_SPEED);
        //}
        //public static void ChannelSetSpeed(int handle, int speed)
        //{
        //    ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_MUSIC_SPEED, speed);
        //}

        //public static int ChannelGetSpeed(int handle)
        //{
        //    return (int)ChannelGetAttribute(handle, BASSAttribute.BASS_ATTRIB_TEMPO);
        //}
        //public static void ChannelSetSpeed(int handle, int speed)
        //{
        //    ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_TEMPO, speed);
        //}


        public static float ChannelGetSpeed(int handle)
        {
            return ChannelGetAttribute(handle, BASSAttribute.BASS_ATTRIB_FREQ);
        }
        public static void ChannelSetSpeed(int handle, float outputSampleRate)
        {
            ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_FREQ, outputSampleRate);
        }

        //private const int _PLAY_SPEED_ZOOM_MULIT = 200;
        ///// <summary>
        ///// get channel play speed
        ///// </summary>
        ///// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        ///// <returns>Speed</returns>
        //public static int ChannelGetSpeed(int handle)
        //{
        //    int speed;
        //    float outputSampleRate = ChannelGetAttribute(handle, BASSAttribute.BASS_ATTRIB_FREQ);
        //    float offset = outputSampleRate / _outputSampleRate;
        //    if (outputSampleRate < _outputSampleRate)
        //    {
        //        speed = (int)((1 - offset) * (0 - _PLAY_SPEED_ZOOM_MULIT));
        //    }
        //    else if (outputSampleRate > _outputSampleRate)
        //    {
        //        speed = (int)((offset - 1) * _PLAY_SPEED_ZOOM_MULIT);
        //    }
        //    else
        //    {
        //        speed = 0;
        //    }

        //    return speed;
        //}

        ///// <summary>
        ///// set channel play speed
        ///// </summary>
        ///// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        ///// <param name="speed">The speed... -200 (min) to 200 (max),-200或0 = 原速;小于0减速;大于0加速</param>
        //public static void ChannelSetSpeed(int handle, int speed)
        //{
        //    if (speed > _PLAY_SPEED_ZOOM_MULIT || speed + _PLAY_SPEED_ZOOM_MULIT < 0)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(speed), $"值只能-{_PLAY_SPEED_ZOOM_MULIT} (min) to {_PLAY_SPEED_ZOOM_MULIT} (max),-{_PLAY_SPEED_ZOOM_MULIT}或0 = 原速;小于0减速;大于0加速");
        //    }

        //    float offset = (float)Math.Abs(speed) / _PLAY_SPEED_ZOOM_MULIT;
        //    float outputSampleRate;
        //    if (speed < 0)
        //    {
        //        outputSampleRate = _outputSampleRate * (1 - offset);
        //    }
        //    else if (speed == 0)
        //    {
        //        outputSampleRate = _outputSampleRate;
        //    }
        //    else
        //    {
        //        outputSampleRate = _outputSampleRate * (1 + offset);
        //    }

        //    ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_FREQ, outputSampleRate);
        //}
        #endregion
    }
}
