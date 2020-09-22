using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// Stream/Sample/Music/Recording/BASS_FX create flags to be used with BASS_StreamCreate(Int32, Int32, BASSFlag, STREAMPROC, IntPtr), 
    /// BASS_StreamCreateFile(String, Int64, Int64, BASSFlag), BASS_StreamCreateFileUser(BASSStreamSystem, BASSFlag, BASS_FILEPROCS, IntPtr), 
    /// BASS_StreamCreateURL(String, Int32, BASSFlag, DOWNLOADPROC, IntPtr), BASS_SampleCreate(Int32, Int32, Int32, Int32, BASSFlag),
    /// BASS_SampleLoad(String, Int64, Int32, Int32, BASSFlag), BASS_SAMPLE, BASS_MusicLoad(String, Int64, Int32, BASSFlag, Int32),
    /// BASS_RecordStart(Int32, Int32, BASSFlag, RECORDPROC, IntPtr) and BASS_FX_TempoCreate(Int32, BASSFlag), BASS_FX_ReverseCreate(Int32, Single, BASSFlag), 
    /// BASS_FX_BPM_DecodeGet(Int32, Double, Double, Int32, BASSFXBpm, BPMPROGRESSPROC, IntPtr), 
    /// BASS_FX_BPM_CallbackSet(Int32, BPMPROC, Double, Int32, BASSFXBpm, IntPtr) etc
    /// </summary>
    [Flags]
    public enum BASSFlag
    {
        /// <summary>
        /// BASS_AAC add-on: use 960 samples per frame
        /// </summary>
        BASS_AAC_FRAME960 = 0x1000,

        /// <summary>
        /// BASS_AAC add-on: downmatrix to stereo
        /// </summary>
        BASS_AAC_STEREO = 0x400000,




        /// <summary>
        /// BASS_AC3 add-on: downmix to stereo
        /// </summary>
        BASS_AC3_DOWNMIX_2 = 0x200,

        /// <summary>
        /// BASS_AC3 add-on: downmix to quad
        /// </summary>
        BASS_AC3_DOWNMIX_4 = 0x400,

        /// <summary>
        /// BASS_AC3 add-on: downmix to dolby
        /// </summary>
        BASS_AC3_DOWNMIX_DOLBY = 0x600,

        /// <summary>
        /// BASS_AC3 add-on: enable dynamic range compression
        /// </summary>
        BASS_AC3_DYNAMIC_RANGE = 0x800,

       




        /// <summary>
        /// BASSCD add-on: Include C2 error info. 296 bytes of C2 error info is returned after each 2352 bytes of audio (and optionally 96 bytes of sub-channel data). This flag cannot be used with the BASS_SAMPLE_FLOAT flag, and is ignored if the BASS_STREAM_DECODE flag is not used.
        /// The first 294 bytes contain the C2 error bits (one bit for each byte of audio), followed by a byte containing the logical OR of all 294 bytes, which can be used to quickly check if there were any C2 errors. The final byte is just padding.
        /// Note that if you request both sub-channel data and C2 error info, the C2 info will come before the sub-channel data!
        /// </summary>
        BASS_CD_C2ERRORS = 0x800,

        /// <summary>
        /// BASSCD add-on: Read sub-channel data. 
        /// 96 bytes of de-interleaved sub-channel data will be returned after each 2352 bytes of audio. 
        /// This flag can not be used with the BASS_SAMPLE_FLOAT flag, and is ignored if the BASS_STREAM_DECODE flag is not used.
        /// </summary>
        BASS_CD_SUBCHANNEL = 0x200,

        /// <summary>
        /// BASSCD add-on: Read sub-channel data, without using any hardware de-interleaving. 
        /// This is identical to the BASS_CD_SUBCHANNEL flag, except that the de-interleaving is always performed by BASSCD even if the drive is apparently capable of de-interleaving itself. 
        /// </summary>
        BASS_CD_SUBCHANNEL_NOHW = 0x400,




        /// <summary>
        /// BASSdsd add-on: Produce DSD-over-PCM data (with 0x05/0xFA markers). DSD-over-PCM data is 24-bit, so the BASS_SAMPLE_FLOAT flag is required
        /// </summary>
        BASS_DSD_DOP = 0x400,

        /// <summary>
        /// BASSdsd add-on: Produce raw DSD data instead of PCM. The DSD data is in blocks of 8 bits (1 byte) per-channel with the MSB being first/oldest. DSD data is not playable by BASS, so the BASS_STREAM_DECODE flag is required. 
        /// </summary>
        BASS_DSD_RAW = 0x200,








        /// <summary>
        /// BASS_FX add-on: If in use, then you can do other stuff while detection's in process. 
        /// </summary>
        BASS_FX_BPM_BKGRND = 1,

        /// <summary>
        /// BASS_FX add-on: If in use, then will auto multiply bpm by 2 (if BPM < MinBPM*2) 
        /// </summary>
        BASS_FX_BPM_MULT2 = 2,

        /// <summary>
        /// BASS_FX add-on: Free the source handle as well? 
        /// </summary>
        BASS_FX_FREESOURCE = 0x10000,

        /// <summary>
        /// BASS_FX add-on (AddOn.Fx.BassFx.BASS_FX_TempoCreate): Uses a cubic interpolation mode (recommended, default). 
        /// </summary>
        BASS_FX_TEMPO_ALGO_CUBIC = 0x400,

        /// <summary>
        /// BASS_FX add-on (AddOn.Fx.BassFx.BASS_FX_TempoCreate): Uses a linear interpolation mode (simple). 
        /// </summary>
        BASS_FX_TEMPO_ALGO_LINEAR = 0x200,

        /// <summary>
        /// BASS_FX add-on (AddOn.Fx.BassFx.BASS_FX_TempoCreate): 
        /// Uses a 8-tap band-limited Shannon interpolation (complex, but not much better than cubic). 
        /// </summary>
        BASS_FX_TEMPO_ALGO_SHANNON = 0x800,










        /// <summary>
        /// BASSMIDI add-on: Let the sound decay naturally (including reverb) instead of stopping it abruptly at the end of the file. 
        /// This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag). 
        /// </summary>
        BASS_MIDI_DECAYEND = 0x1000,

        /// <summary>
        /// BASSMIDI add-on: Let the old sound decay naturally (including reverb) when changing the position, including looping. 
        /// This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag), 
        /// and can also be used in BASS_ChannelSetPosition(Int32, Int64, BASSMode) calls to have it apply to particular position changes
        /// </summary>
        BASS_MIDI_DECAYSEEK = 0x4000,

        /// <summary>
        /// BASSMIDI add-on: Map the file into memory. This flag is ignored if the soundfont is packed as the sample data cannot be played directly from a mapping; it needs to be decoded. 
        /// This flag is also ignored if the file is too large to be mapped into memory. 
        /// </summary>
        BASS_MIDI_FONT_MMAP = 0x20000,

        /// <summary>
        /// BASSMIDI add-on: Ignore the soundfont's reverb and chorus settings. 
        /// If you would like to ignore the soundfont's reverb/chorus on only some presets/banks, then you can load 2 instances of it (one with BASS_MIDI_FONT_NOFX flag and one without), and then assign them as wanted in a BASS_MIDI_StreamSetFonts call. The 2 instances will share resources, so they won't really use any more memory than 1 instance.
        /// </summary>
        BASS_MIDI_FONT_NOFX = 0x80000,

        /// <summary>
        /// BASSMIDI add-on: Use bank 127 in the soundfont for XG drum kits. 
        /// When an XG drum kit is needed, bank 127 in soundfonts that have this flag set will be checked first, before falling back to bank 128 (the standard SF2 drum kit bank) if it is not available there.
        /// </summary>
        BASS_MIDI_FONT_XGDRUMS = 0x40000,

        /// <summary>
        /// BASSMIDI add-on: Do not remove empty space (containing no events) from the end of the file. 
        /// </summary>
        BASS_MIDI_NOCROP = 0x8000,

        /// <summary>
        /// BASSMIDI add-on: Disable the MIDI reverb/chorus processing. 
        /// This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag). 
        /// </summary>
        BASS_MIDI_NOFX = 0x2000,

        /// <summary>
        /// BASSMIDI add-on: Ignore system reset events (MIDI_EVENT_SYSTEM) when the system mode is unchanged. 
        /// This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag). 
        /// </summary>
        BASS_MIDI_NOSYSRESET = 0x800,

        /// <summary>
        /// BASSMIDI add-on: Only release the oldest instance upon a note off event (MIDI_EVENT_NOTE with velocity=0) when there are overlapping instances of the note. Otherwise all instances are released. 
        /// This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag). 
        /// </summary>
        BASS_MIDI_NOTEOFF1 = 0x10000,

        /// <summary>
        /// BASSMIDI add-on: Reduce 24-bit sample data to 16-bit before encoding. 
        /// </summary>
        BASS_MIDI_PACK_16BIT = 2,

        /// <summary>
        /// BASSMIDI add-on: Don't send a WAVE header to the encoder. If this flag is used then the sample format (mono 16-bit) must be passed to the encoder some other way, eg. via the command-line. 
        /// </summary>
        BASS_MIDI_PACK_NOHEAD = 1,

        /// <summary>
        /// BASSMIDI add-on: Use sinc interpolated sample mixing. This increases the sound quality, but also requires more CPU. Otherwise linear interpolation is used
        /// </summary>
        BASS_MIDI_SINCINTER = 0x800000,












        /// <summary>
        /// BASSmix add-on: buffer source data for BASS_Mixer_ChannelGetData(Int32, IntPtr, Int32) and BASS_Mixer_ChannelGetLevel(Int32). 
        /// </summary>
        BASS_MIXER_BUFFER = 0x2000,

        /// <summary>
        /// BASSmix add-on: downmix to stereo (or mono if mixer is mono) 
        /// </summary>
        BASS_MIXER_DOWNMIX = 0x400000,

        /// <summary>
        /// BASSmix add-on: end the stream when there are no sources 
        /// </summary>
        BASS_MIXER_END = 0x10000,

        /// <summary>
        /// BASSmix add-on: Limit mixer processing to the amount available from this source. 
        /// </summary>
        BASS_MIXER_LIMIT = 0x4000,

        /// <summary>
        /// BASSmix add-on: Matrix mixing 
        /// </summary>
        BASS_MIXER_MATRIX = 0x10000,

        /// <summary>
        /// BASSmix add-on: don't stall when there are no sources 
        /// </summary>
        BASS_MIXER_NONSTOP = 0x20000,

        /// <summary>
        /// BASSmix add-on: don't ramp-in the start 
        /// </summary>
        BASS_MIXER_NORAMPIN = 0x800000,

        /// <summary>
        /// BASSmix add-on: don't process the source 
        /// </summary>
        BASS_MIXER_PAUSE = 0x20000,

        /// <summary>
        /// BASSmix add-on: enable BASS_Mixer_ChannelGetPositionEx(Int32, BASSMode, Int32) support. 
        /// </summary>
        BASS_MIXER_POSEX = 0x2000,

        /// <summary>
        /// BASSmix add-on: resume a stalled mixer immediately upon new/unpaused source 
        /// </summary>
        BASS_MIXER_RESUME = 0x1000,











        /// <summary>
        /// Music: Use 3D functionality. This is ignored if BASS_DEVICE_3D wasn't specified when calling BASS_Init(Int32, Int32, BASSInit, IntPtr, IntPtr). 
        /// 3D streams must be mono (chans = 1). The SPEAKER flags can not be used together with this flag
        /// </summary>
        BASS_MUSIC_3D = 8,

        /// <summary>
        /// Music: Automatically free the music when it ends. This allows you to play a music and forget about it, as BASS will automatically free the music's resources when it has reached the end or when BASS_ChannelStop(Int32) (or BASS_Stop()) is called. 
        /// Note that some musics never actually end on their own (ie. without you stopping them).
        /// </summary>
        BASS_MUSIC_AUTOFREE = 0x40000,

        /// <summary>
        /// Music: Decode the music into sample data, without outputting it. 
        /// Use BASS_ChannelGetData(Int32, IntPtr, Int32) to retrieve decoded sample data. BASS_SAMPLE_SOFTWARE/3D/FX/AUTOFREE are ignored when using this flag, as are the SPEAKER flags.
        /// </summary>
        BASS_MUSIC_DECODE = 0x200000,

        /// <summary>
        /// Music: Use 32-bit floating-point sample data (see Floating-Point Channels for details). 
        /// WDM drivers or the BASS_STREAM_DECODE flag are required to use this flag
        /// </summary>
        BASS_MUSIC_FLOAT = 0x100,

        /// <summary>
        /// Music: Play .MOD file as FastTracker 2 would. 
        /// </summary>
        BASS_MUSIC_FT2MOD = 0x2000,

        /// <summary>
        /// Music: Apply FastTracker 2 panning to XM files. 
        /// </summary>
        BASS_MUSIC_FT2PAN = 0x2000,

        /// <summary>
        /// Music: Enable the old implementation of DirectX 8 effects. See the DX8 effect implementations section for details. 
        /// Use BASS_ChannelSetFX(Int32, BASSFXType, Int32) to add effects to the stream. Requires DirectX 8 or above.
        /// </summary>
        BASS_MUSIC_FX = 0x80,

        /// <summary>
        /// Music: Loop the music. This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag). 
        /// </summary>
        BASS_MUSIC_LOOP = 4,

        /// <summary>
        /// Music: Decode/play the mod music in mono, reducing the CPU usage (if it was originally stereo). 
        /// This flag is automatically applied if BASS_DEVICE_MONO was specified when calling BASS_Init(Int32, Int32, BASSInit, IntPtr, IntPtr). 
        /// </summary>
        BASS_MUSIC_MONO = 2,

        /// <summary>
        /// Music: Use non-interpolated mixing. This generally reduces the sound quality, but can be good for chip-tunes
        /// </summary>
        BASS_MUSIC_NONINTER = 0x10000,

        /// <summary>
        /// Music: Don't load the samples. This reduces the time taken to load the music, notably with MO3 files, 
        /// which is useful if you just want to get the name and length of the music without playing it. 
        /// </summary>
        BASS_MUSIC_NOSAMPLE = 0x100000,

        /// <summary>
        /// Music: Stop all notes when seeking (using BASS_ChannelSetPosition(Int32, Int64, BASSMode)). 
        /// </summary>
        BASS_MUSIC_POSRESET = 0x8000,

        /// <summary>
        /// Music: Stop all notes and reset bpm/etc when seeking. 
        /// </summary>
        BASS_MUSIC_POSRESETEX = 0x400000,

        /// <summary>
        /// Music: Calculate the playback length of the music, and enable seeking in bytes. This slightly increases the time taken to load the music, depending on how long it is. 
        /// In the case of musics that loop, the length until the loop occurs is calculated. Use BASS_ChannelGetLength(Int32, BASSMode) to retrieve the length
        /// </summary>
        BASS_MUSIC_PRESCAN = 0x20000,

        /// <summary>
        /// Music: Play .MOD file as ProTracker 1 would. 
        /// </summary>
        BASS_MUSIC_PT1MOD = 0x4000,

        /// <summary>
        /// Music: Use "normal" ramping (as used in FastTracker 2). 
        /// </summary>
        BASS_MUSIC_RAMP = 0x200,

        /// <summary>
        /// Music: Use "sensitive" ramping. 
        /// </summary>
        BASS_MUSIC_RAMPS = 0x400,

        /// <summary>
        /// Music: Sinc interpolated sample mixing. This increases the sound quality, but also requires quite a bit more processing.
        /// If neither this or the BASS_MUSIC_NONINTER flag is specified, linear interpolation is used
        /// </summary>
        BASS_MUSIC_SINCINTER = 0x800000,

        /// <summary>
        /// Music: Stop the music when a backward jump effect is played. This stops musics that never reach the end from going into endless loops. 
        /// Some MOD musics are designed to jump all over the place, so this flag would cause those to be stopped prematurely. If this flag is used together with the BASS_SAMPLE_LOOP flag, then the music would not be stopped but any BASS_SYNC_END sync would be triggered.
        /// </summary>
        BASS_MUSIC_STOPBACK = 0x80000,

        /// <summary>
        /// Music: Apply XMPlay's surround sound to the music (ignored in mono). 
        /// </summary>
        BASS_MUSIC_SURROUND = 0x800,

        /// <summary>
        /// Music: Apply XMPlay's surround sound mode 2 to the music (ignored in mono). 
        /// </summary>
        BASS_MUSIC_SURROUND2 = 0x1000,








        /// <summary>
        /// Recording: enabled automatic gain control (only available on certain devices, like iOS). 
        /// </summary>
        BASS_RECORD_AGC = 0x4000,

        /// <summary>
        /// Recording: enable echo cancellation (only available on certain devices, like iOS). 
        /// </summary>
        BASS_RECORD_ECHOCANCEL = 0x2000,

        /// <summary>
        /// Recording: Start the recording paused. Use BASS_ChannelPlay(Int32, Boolean) to start it. 
        /// </summary>
        BASS_RECORD_PAUSE = 0x8000,




       
        /// <summary>
        /// 0 = default create stream: 16 Bit, stereo, no Float, hardware mixing, no Loop, no 3D, no speaker assignments... 
        /// </summary>
        BASS_DEFAULT = 0,

        /// <summary>
        /// Use 8-bit resolution. If neither this or the BASS_SAMPLE_FLOAT flags are specified, then the stream is 16-bit. 
        /// </summary>
        BASS_SAMPLE_8BITS = 1,

        /// <summary>
        /// Use 32-bit floating-point sample data (see Floating-Point Channels for details). 
        /// WDM drivers or the BASS_STREAM_DECODE flag are required to use this flag
        /// </summary>
        BASS_SAMPLE_FLOAT = 0x100,

        /// <summary>
        /// Use 3D functionality. This is ignored if BASS_DEVICE_3D wasn't specified when calling BASS_Init(Int32, Int32, BASSInit, IntPtr, IntPtr). 
        /// 3D streams must be mono (chans = 1). The SPEAKER flags can not be used together with this flag
        /// </summary>
        BASS_SAMPLE_3D = 8,

        /// <summary>
        /// Enable the old implementation of DirectX 8 effects. See the DX8 effect implementations section for details. 
        /// Use BASS_ChannelSetFX(Int32, BASSFXType, Int32) to add effects to the stream. Requires DirectX 8 or above.
        /// </summary>
        BASS_SAMPLE_FX = 0x80,

        /// <summary>
        /// Loop the file. This flag can be toggled at any time using BASS_ChannelFlags(Int32, BASSFlag, BASSFlag). 
        /// </summary>
        BASS_SAMPLE_LOOP = 4,

        /// <summary>
        /// Decode/play the stream (MP3/MP2/MP1 only) in mono, reducing the CPU usage (if it was originally stereo). 
        /// This flag is automatically applied if BASS_DEVICE_MONO was specified when calling BASS_Init(Int32, Int32, BASSInit, IntPtr, IntPtr).
        /// </summary>
        BASS_SAMPLE_MONO = 2,

        /// <summary>
        /// Sample: muted at max distance (3D only) 
        /// </summary>
        BASS_SAMPLE_MUTEMAX = 0x20,

        /// <summary>
        /// Sample: override furthest from listener (3D only) 
        /// </summary>
        BASS_SAMPLE_OVER_DIST = 0x30000,

        /// <summary>
        /// Sample: override longest playing 
        /// </summary>
        BASS_SAMPLE_OVER_POS = 0x20000,

        /// <summary>
        /// Sample: override lowest volume 
        /// </summary>
        BASS_SAMPLE_OVER_VOL = 0x10000,

        /// <summary>
        /// Force the stream to not use hardware mixing
        /// </summary>
        BASS_SAMPLE_SOFTWARE = 0x10,

        /// <summary>
        /// Sample: uses the DX7 voice allocation & management 
        /// </summary>
        BASS_SAMPLE_VAM = 0x40,







        /// <summary>
        /// Center & LFE speakers (5.1, channel 5/6) 
        /// </summary>
        BASS_SPEAKER_CENLFE = 0x3000000,

        /// <summary>
        /// Center speaker only (5.1, channel 5) 
        /// </summary>
        BASS_SPEAKER_CENTER = 0x13000000,

        /// <summary>
        /// Front speakers (channel 1/2) 
        /// </summary>
        BASS_SPEAKER_FRONT = 0x1000000,

        /// <summary>
        /// Front Left speaker only (channel 1) 
        /// </summary>
        BASS_SPEAKER_FRONTLEFT = 0x11000000,

        /// <summary>
        /// Front Right speaker only (channel 2) 
        /// </summary>
        BASS_SPEAKER_FRONTRIGHT = 0x21000000,

        /// <summary>
        /// Speaker Modifier: left channel only 
        /// </summary>
        BASS_SPEAKER_LEFT = 0x10000000,

        /// <summary>
        /// LFE speaker only (5.1, channel 6) 
        /// </summary>
        BASS_SPEAKER_LFE = 0x23000000,



        /// <summary>
        /// Speakers Pair 1
        /// </summary>
        BASS_SPEAKER_PAIR1 = 0x1000000,

        /// <summary>
        /// Speakers Pair 2
        /// </summary>
        BASS_SPEAKER_PAIR2 = 0x2000000,

        /// <summary>
        /// Speakers Pair 3
        /// </summary>
        BASS_SPEAKER_PAIR3 = 0x3000000,

        /// <summary>
        /// Speakers Pair 4
        /// </summary>
        BASS_SPEAKER_PAIR4 = 0x4000000,

        /// <summary>
        /// Speakers Pair 5
        /// </summary>
        BASS_SPEAKER_PAIR5 = 0x5000000,

        /// <summary>
        /// Speakers Pair 6
        /// </summary>
        BASS_SPEAKER_PAIR6 = 0x6000000,

        /// <summary>
        /// Speakers Pair 7
        /// </summary>
        BASS_SPEAKER_PAIR7 = 0x7000000,

        /// <summary>
        /// Speakers Pair 8
        /// </summary>
        BASS_SPEAKER_PAIR8 = 0x8000000,

        /// <summary>
        /// Speakers Pair 9 
        /// </summary>
        BASS_SPEAKER_PAIR9 = 0x9000000,

        /// <summary>
        /// Speakers Pair 10
        /// </summary>
        BASS_SPEAKER_PAIR10 = 0xa000000,

        /// <summary>
        /// Speakers Pair 11
        /// </summary>
        BASS_SPEAKER_PAIR11 = 0xb000000,

        /// <summary>
        /// Speakers Pair 12
        /// </summary>
        BASS_SPEAKER_PAIR12 = 0xc000000,

        /// <summary>
        /// Speakers Pair 13
        /// </summary>
        BASS_SPEAKER_PAIR13 = 0xd000000,

        /// <summary>
        /// Speakers Pair 14
        /// </summary>
        BASS_SPEAKER_PAIR14 = 0xe000000,

        /// <summary>
        /// Speakers Pair 15
        /// </summary>
        BASS_SPEAKER_PAIR15 = 0xf000000,

        /// <summary>
        /// Rear/Side speakers (channel 3/4) 
        /// </summary>
        BASS_SPEAKER_REAR = 0x2000000,

        /// <summary>
        /// Rear Center speakers (7.1, channel 7/8) 
        /// </summary>
        BASS_SPEAKER_REAR2 = 0x4000000,

        /// <summary>
        /// Rear Center Left speaker only (7.1, channel 7) 
        /// </summary>
        BASS_SPEAKER_REAR2LEFT = 0x14000000,

        /// <summary>
        /// Rear Center Right speaker only (7.1, channel 8)
        /// </summary>
        BASS_SPEAKER_REAR2RIGHT = 0x24000000,

        /// <summary>
        /// Rear/Side Left speaker only (channel 3) 
        /// </summary>
        BASS_SPEAKER_REARLEFT = 0x12000000,

        /// <summary>
        /// Rear/Side Right speaker only (channel 4) 
        /// </summary>
        BASS_SPEAKER_REARRIGHT = 0x22000000,

        /// <summary>
        /// Speaker Modifier: right channel only 
        /// </summary>
        BASS_SPEAKER_RIGHT = 0x20000000,









       

        /// <summary>
        /// BASSmix add-on: The splitter's length and position is based on the splitter's (rather than the source's) channel count. 
        /// </summary>
        BASS_SPLIT_POS = 0x2000,

        /// <summary>
        /// BASSmix add-on: only read buffered data
        /// </summary>
        BASS_SPLIT_SLAVE = 0x1000,

        /// <summary>
        /// Automatically free the stream's resources when it has reached the end, or when BASS_ChannelStop(Int32) (or BASS_Stop()) is called.
        /// </summary>
        BASS_STREAM_AUTOFREE = 0x40000,

        /// <summary>
        /// Download and play the file in smaller chunks. 
        /// Uses a lot less memory than otherwise, but it's not possible to seek or loop the stream - once it's ended, the file must be opened again to play it again. This flag will automatically be applied when the file length is unknown. This flag also has the effect of resticting the download rate. This flag has no effect on "unbuffered" streams (buffer = false).
        /// </summary>
        BASS_STREAM_BLOCK = 0x100000,

        /// <summary>
        /// Decode the sample data, without outputting it. Use BASS_ChannelGetData(Int32, IntPtr, Int32) to retrieve decoded sample data. 
        /// BASS_SAMPLE_SOFTWARE/3D/FX/AUTOFREE are all ignored when using this flag, as are the SPEAKER flags.
        /// </summary>
        BASS_STREAM_DECODE = 0x200000,

        /// <summary>
        /// Enable pin-point accurate seeking (to the exact byte) on the MP3/MP2/MP1 stream. 
        /// This also increases the time taken to create the stream, due to the entire file being pre-scanned for the seek points. Note: BASS_STREAM_PRESCAN is ONLY needed for files with a VBR, files with a CBR are always accurate
        /// </summary>
        BASS_STREAM_PRESCAN = 0x20000,

        /// <summary>
        /// Restrict the download rate of the file to the rate required to sustain playback.         
        /// If this flag is not used, then the file will be downloaded as quickly as possible. This flag has no effect on "unbuffered" streams (buffer = false).
        /// </summary>
        BASS_STREAM_RESTRATE = 0x80000,

        /// <summary>
        /// Pass status info (HTTP/ICY tags) from the server to the DOWNLOADPROC callback during connection.         
        /// This can be useful to determine the reason for a failure.
        /// </summary>
        BASS_STREAM_STATUS = 0x800000,

        /// <summary>
        /// File is a Unicode (16-bit characters) filename 
        /// </summary>
        BASS_UNICODE = -2147483648,

        /// <summary>
        /// Use an async look-ahead cache
        /// </summary>
        BASS_ASYNCFILE = 0x40000000,

        /// <summary>
        /// BASSWV add-on: limit to stereo 
        /// </summary>
        BASS_WV_STEREO = 0x400000
    }





}
