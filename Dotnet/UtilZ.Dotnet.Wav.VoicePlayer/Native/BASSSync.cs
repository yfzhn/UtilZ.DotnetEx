using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Flags]
    public enum BASSSync
    {
        /// <summary>
        /// Sync when the channel's device stops unexpectedly (eg. if it is disconnected/disabled). When this happens,
        /// it will not be possible to resume a recording but it may be possible to resume playback via BASS_Start once the device becomes available again.
        /// param : not used. data : not used
        /// </summary>
        BASS_SYNC_DEV_FAIL = 14,

        /// <summary>
        /// Sync when the sample format (sample rate and/or channel count) of the channel's device changes. 
        /// The new format is available from BASS_GetInfo.
        /// param : not used. data : not used.
        /// </summary>
        BASS_SYNC_DEV_FORMAT = 15,

        /// <summary>
        /// Sync when downloading of an internet (or "buffered" user file) stream is done.        
        /// param : not used. data : not used. 
        /// </summary>
        BASS_SYNC_DOWNLOAD = 7,

        /// <summary>
        /// Sync when a channel reaches the end, including when looping. Note that some MOD musics never reach the end; 
        /// they may jump to another position first. If the BASS_MUSIC_STOPBACK flag is used with a MOD music (through BASS_MusicLoad or BASS_ChannelFlags), 
        /// then this sync will also be called when a backward jump effect is played.        
        /// param : not used. data : 1 = the sync is triggered by a backward jump in a MOD music, otherwise not used. 
        /// </summary>
        BASS_SYNC_END = 2,

        /// <summary>
        /// Sync when a channel is freed. This can be useful when you need to release some resources associated with the channel. 
        /// Note that you will not be able to use any BASS functions with the channel in the callback, as the channel will no longer exist.        
        /// param : not used. data : not used. 
        /// </summary>
        BASS_SYNC_FREE = 8,

        /// <summary>
        /// Sync when metadata is received in a Shoutcast stream. The updated metadata is available from BASS_ChannelGetTags.        
        /// param : not used. data : not used. 
        /// </summary>
        BASS_SYNC_META = 4,

        /// <summary>
        /// Sync when the sync effect is used in a MOD music. The sync effect is E8x or Wxx for the XM/MTM/MOD formats, and S2x for the IT/S3M formats (where x = any value).        
        /// param : 0 = the position is passed to the callback (data : LOWORD = order, HIWORD = row), 1 = the value of x is passed to the callback (data : x value). 
        /// </summary>
        BASS_SYNC_MUSICFX = 3,

        /// <summary>
        /// Sync when an instrument (sample for the MOD/S3M/MTM formats) is played in a MOD music (not including retrigs).        
        /// param : LOWORD = instrument(1 = first), HIWORD = note(0 = c0...119 = b9, -1 = all).data : LOWORD = note, HIWORD = volume(0 - 64).
        /// </summary>
        BASS_SYNC_MUSICINST = 1,

        /// <summary>
        /// Sync when a MOD music reaches an order.row position.        
        /// param : LOWORD = order(0 = first, -1 = all), HIWORD = row(0 = first, -1 = all).data : LOWORD = order, HIWORD = row.
        /// </summary>
        BASS_SYNC_MUSICPOS = 10,

        /// <summary>
        /// Sync when a new logical bitstream begins in a chained OGG stream. Updated tags are available from BASS_ChannelGetTags.        
        /// param : not used. data : not used. 
        /// </summary>
        BASS_SYNC_OGG_CHANGE = 12,

        /// <summary>
        /// Sync when a channel reaches a position.        
        /// param : position in bytes (automatically rounded down to nearest sample). data : not used. 
        /// </summary>
        BASS_SYNC_POS = 0,

        /// <summary>
        /// Sync when a channel's position is set, including when looping/restarting.        
        /// param : not used. data : 0 = playback buffer is not flushed, 1 = playback buffer is flushed. 
        /// </summary>
        BASS_SYNC_SETPOS = 11,

        /// <summary>
        /// Sync when an attribute slide has ended.        
        /// param : not used. data : the attribute that has finished sliding. 
        /// </summary>
        BASS_SYNC_SLIDE = 5,

        /// <summary>
        /// Sync when playback of the channel is stalled/resumed.        
        /// param : not used. data : 0 = stalled, 1 = resumed. 
        /// </summary>
        BASS_SYNC_STALL = 6,



        //========下面定义的为other sync types may be supported by add-ons, see the documentation. ========

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_HLS_SEGMENT = 0x10300,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_CD_ERROR = 0x3e8,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_CD_SPEED = 0x3ea,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_CUE = 0x10001,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_EVENT = 0x10004,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_KEYSIG = 0x10007,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_LYRIC = 0x10002,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_MARKER = 0x10000,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_TEXT = 0x10003,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_TICK = 0x10005,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIDI_TIMESIG = 0x10006,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIXER_ENVELOPE = 0x10200,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIXER_ENVELOPE_NODE = 0x10201,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_MIXTIME = 0x40000000,


        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_ONETIME = -2147483648,



        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_WMA_CHANGE = 0x10100,

        /// <summary>
        /// 
        /// </summary>
        BASS_SYNC_WMA_META = 0x10101,

        /// <summary>
        /// 
        /// </summary>
        BASS_WINAMP_SYNC_BITRATE = 100
    }
}
