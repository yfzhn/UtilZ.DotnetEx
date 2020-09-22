using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Serializable]
    public class BASS_DEVICEINFO
    {
        internal BASS_DEVICEINFO_INTERNAL _internal;

        private string _driver = null;
        public string Driver
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_driver))
                {
                    byte[] buffer = Utils.GetIntPtrStringBytes(_internal.driver);
                    _driver = Encoding.Default.GetString(buffer);

                    //int num;
                    //_driver = Utils.IntPtrAsStringUtf8(_internal.driver, out num);
                }

                return _driver;
            }
        }

        public BASSDeviceInfo Flags
        {
            get { return _internal.flags; }
        }


        private string _name = string.Empty;
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    byte[] buffer = Utils.GetIntPtrStringBytes(_internal.name);
                    _name = Encoding.Default.GetString(buffer);

                    //int num;
                    //_name = Utils.IntPtrAsStringUtf8(_internal.name, out num);
                    //_name = Utils.IntPtrAsStringAnsi(_internal.name);
                    //_name = Utils.IntPtrAsStringUnicode(_internal.name);
                }

                return _name;
            }
        }




        public bool IsDefault
        {
            get
            {
                return ((this.Flags & BASSDeviceInfo.BASS_DEVICE_DEFAULT) > BASSDeviceInfo.BASS_DEVICE_NONE);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return ((this.Flags & BASSDeviceInfo.BASS_DEVICE_ENABLED) > BASSDeviceInfo.BASS_DEVICE_NONE);
            }
        }

        public bool IsInitialized
        {
            get
            {
                return ((this.Flags & BASSDeviceInfo.BASS_DEVICE_INIT) > BASSDeviceInfo.BASS_DEVICE_NONE);
            }
        }

        public BASSDeviceInfo status
        {
            get
            {
                return (BASSDeviceInfo)((int)this.Flags & 0xffffff);
            }
        }

        public BASSDeviceInfo type
        {
            get
            {
                return (BASSDeviceInfo)((int)this.Flags & -16777216);
            }
        }


        public int Device { get; private set; }

        internal BASS_DEVICEINFO(int device)
        {
            this.Device = device;
        }


        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return _internal.flags.ToString();
            }

            return this.Name;
        }
    }
}
