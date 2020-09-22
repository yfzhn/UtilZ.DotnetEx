using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// 配置选项枚举BASS_SetConfigOptions
    /// </summary>
    public enum BASSConfig
    {
        /// <summary>
        /// The 3D algorithm for software mixed 3D channels.
        /// </summary>
        BASS_CONFIG_3DALGORITHM = 10,

        /// <summary>
        /// Asynchronous file reading buffer length.
        /// </summary>
        BASS_CONFIG_ASYNCFILE_BUFFER = 0x2d,

        /// <summary>
        /// Playback buffer length
        /// </summary>
        BASS_CONFIG_BUFFER = 0,

        /// <summary>
        /// Panning translation curve
        /// </summary>
        BASS_CONFIG_CURVE_PAN = 8,

        /// <summary>
        /// Volume translation curve.  
        /// </summary>
        BASS_CONFIG_CURVE_VOL = 7,

        /// <summary>
        /// Output device buffer length.
        /// </summary>
        BASS_CONFIG_DEV_BUFFER = 0x1b,

        /// <summary>
        /// Include a "Default" entry in the output device list?
        /// </summary>
        BASS_CONFIG_DEV_DEFAULT = 0x24,

        /// <summary>
        /// Do not stop an output device when nothing is playing?
        /// </summary>
        BASS_CONFIG_DEV_NONSTOP = 50,

        /// <summary>
        /// Pass 32-bit floating-point sample data to all DSP functions?
        /// </summary>
        BASS_CONFIG_FLOATDSP = 9,

        /// <summary>
        /// Global MOD music volume. 
        /// </summary>
        BASS_CONFIG_GVOL_MUSIC = 6,

        /// <summary>
        /// Global sample volume.  
        /// </summary>
        BASS_CONFIG_GVOL_SAMPLE = 4,

        /// <summary>
        /// Global stream volume.  
        /// </summary>
        BASS_CONFIG_GVOL_STREAM = 5,

        /// <summary>
        /// Number of existing HMUSIC / HRECORD / HSAMPLE / HSTREAM handles.  
        /// </summary>
        BASS_CONFIG_HANDLES = 0x29,

        /// <summary>
        /// Disable the use of Media Foundation?
        /// </summary>
        BASS_CONFIG_MF_DISABLE = 40,

        /// <summary>
        /// Play the audio from videos using Media Foundation?
        /// </summary>
        BASS_CONFIG_MF_VIDEO = 0x30,
        
        /// <summary>
        /// IT virtual channels.
        /// </summary>
        BASS_CONFIG_MUSIC_VIRTUAL = 0x16,

        /// <summary>
        /// Internet download buffer length.
        /// </summary>
        BASS_CONFIG_NET_BUFFER = 12,

        /// <summary>
        /// Use passive mode in FTP connections?
        /// </summary>
        BASS_CONFIG_NET_PASSIVE = 0x12,

        /// <summary>
        /// Process URLs in playlists?
        /// </summary>
        BASS_CONFIG_NET_PLAYLIST = 0x15,

        /// <summary>
        /// Maximum nested playlist processing depth.
        /// </summary>
        BASS_CONFIG_NET_PLAYLIST_DEPTH = 0x3b,

        /// <summary>
        /// Amount to pre-buffer before playing internet streams.
        /// </summary>
        BASS_CONFIG_NET_PREBUF = 15,

        /// <summary>
        /// Wait for pre-buffering when opening internet streams?
        /// </summary>
        BASS_CONFIG_NET_PREBUF_WAIT = 60,

        /// <summary>
        /// Time to wait for a server to deliver more data.
        /// </summary>
        BASS_CONFIG_NET_READTIMEOUT = 0x25,
  
        /// <summary>
        /// Time to wait for a server to respond to a connection request.  
        /// </summary>
        BASS_CONFIG_NET_TIMEOUT = 11,

        /// <summary>
        /// Pre-scan chained OGG files?  
        /// </summary>
        BASS_CONFIG_OGG_PRESCAN = 0x2f,

        /// <summary>
        /// Prevent channels being played when the output is paused?
        /// </summary>
        BASS_CONFIG_PAUSE_NOPLAY = 13,

        /// <summary>
        /// Recording buffer length.
        /// </summary>
        BASS_CONFIG_REC_BUFFER = 0x13,
     
        /// <summary>
        /// Default sample rate conversion quality.  
        /// </summary>
        BASS_CONFIG_SRC = 0x2b,

        /// <summary>
        /// Default sample rate conversion quality for samples.
        /// </summary>
        BASS_CONFIG_SRC_SAMPLE = 0x2c,

        /// <summary>
        /// Unicode device information? 
        /// </summary>
        BASS_CONFIG_UNICODE = 0x2a,

        /// <summary>
        /// Update period of playback buffers. 
        /// </summary>
        BASS_CONFIG_UPDATEPERIOD = 1,

        /// <summary>
        /// Number of update threads.
        /// </summary>
        BASS_CONFIG_UPDATETHREADS = 0x18,

        /// <summary>
        ///  File format verification length.
        /// </summary>
        BASS_CONFIG_VERIFY = 0x17,

        /// <summary>
        /// File format verification length for internet streams.  
        /// </summary>
        BASS_CONFIG_VERIFY_NET = 0x34,

        /// <summary>
        /// Enable speaker assignment with panning/balance control on Windows Vista and newer?
        /// </summary>
        BASS_CONFIG_VISTA_SPEAKERS = 0x26,
        
        /// <summary>
        /// Enable true play position mode on Windows Vista and newer?  
        /// </summary>
        BASS_CONFIG_VISTA_TRUEPOS = 30,

        /// <summary>
        /// Retain Windows mixer settings across sessions?
        /// </summary>
        BASS_CONFIG_WASAPI_PERSIST = 0x41,


        /***************************************************************************
         * 不适用桌面的配置项
         * BASS_CONFIG_AM_DISABLE  Disable the use of Android media codecs?
         * BASS_CONFIG_ANDROID_AAUDIO  Enable AAudio output on Android?
         * BASS_CONFIG_ANDROID_SESSIONID   Session ID to use for output on Android.
         * BASS_CONFIG_DEV_PERIOD  Output device update period.  
         * BASS_CONFIG_FLOAT   Floating-point sample data is supported?
         * BASS_CONFIG_IOS_SESSION Audio session configuration on iOS.  
         ***************************************************************************/


        /***************************************************************************
         * other config options may be supported by add-ons, see the documentation:
         * BASS_CONFIG_NET_PROXY = 0x11,
         * BASS_CONFIG_LIBSSL = 0x40,
         * BASS_CONFIG_NET_SEEK = 0x38,
         * BASS_CONFIG_SPLIT_BUFFER = 0x10610,
         * BASS_CONFIG_MIDI_AUTOFONT = 0x10402,
         * BASS_CONFIG_MIDI_COMPACT = 0x10400,
         * BASS_CONFIG_MIDI_DEFFONT = 0x10403,
         * BASS_CONFIG_MIDI_IN_PORTS = 0x10404,
         * BASS_CONFIG_MIDI_VOICES = 0x10401,
         * BASS_CONFIG_MIXER_BUFFER = 0x10601,
         * BASS_CONFIG_MIXER_FILTER = 0x10600,
         * BASS_CONFIG_MIXER_POSEX = 0x10602,
         * BASS_CONFIG_MP3_ERRORS = 0x23,
         * BASS_CONFIG_MP4_VIDEO = 0x10700,
         * BASS_CONFIG_DSD_FREQ = 0x10800,
         * BASS_CONFIG_DSD_GAIN = 0x10801,
         * BASS_CONFIG_ENCODE_ACM_LOAD = 0x10302,
         * BASS_CONFIG_ENCODE_CAST_PROXY = 0x10311,
         * BASS_CONFIG_ENCODE_CAST_TIMEOUT = 0x10310,
         * BASS_CONFIG_ENCODE_PRIORITY = 0x10300,
         * BASS_CONFIG_ENCODE_QUEUE = 0x10301,
         * BASS_CONFIG_CD_AUTOSPEED = 0x10202,
         * BASS_CONFIG_CD_CDDB_SERVER = 0x10204,
         * BASS_CONFIG_CD_FREEOLD = 0x10200,
         * BASS_CONFIG_CD_READ = 0x10205,
         * BASS_CONFIG_CD_RETRY = 0x10201,
         * BASS_CONFIG_CD_SKIPERROR = 0x10203,
         * BASS_CONFIG_CD_TIMEOUT = 0x10206,
         * BASS_CONFIG_AAC_MP4 = 0x10701,
         * BASS_CONFIG_AAC_PRESCAN = 0x10702,
         * BASS_CONFIG_AC3_DYNRNG = 0x10001,
         * BASS_CONFIG_NET_AGENT = 0x10,
         * BASS_CONFIG_WINAMP_INPUT_TIMEOUT = 0x10800,
         * BASS_CONFIG_WMA_ASYNC = 0x1010f,
         * BASS_CONFIG_WMA_BASSFILE = 0x10103,
         * BASS_CONFIG_WMA_NETSEEK = 0x10104,
         * BASS_CONFIG_WMA_PREBUF = 0x10101,
         * BASS_CONFIG_WMA_VIDEO = 0x10105
         ***************************************************************************/
    }
}
