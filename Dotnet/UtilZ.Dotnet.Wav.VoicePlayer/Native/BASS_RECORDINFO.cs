using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class BASS_RECORDINFO
    {
        public BASSRecordInfo flags;
        public int formats;
        public int inputs;
        public bool singlein;
        public int freq;

        public BASS_RECORDINFO()
        {

        }

        public override string ToString()
        {
            return $"Inputs={this.inputs}, SingleIn={this.singlein}";
        }

        public BASSRecordFormat WaveFormat
        {
            get
            {
                return (((BASSRecordFormat)this.formats) & ((BASSRecordFormat)0xffffff));
            }
        }

        public int Channels
        {
            get
            {
                return (this.formats >> 0x18);
            }
        }

        public bool SupportsDirectSound
        {
            get
            {
                return ((this.flags & BASSRecordInfo.DSCAPS_EMULDRIVER) == BASSRecordInfo.DSCAPS_NONE);
            }
        }

        public bool IsCertified
        {
            get
            {
                return ((this.flags & BASSRecordInfo.DSCAPS_CERTIFIED) > BASSRecordInfo.DSCAPS_NONE);
            }
        }
    }
}
