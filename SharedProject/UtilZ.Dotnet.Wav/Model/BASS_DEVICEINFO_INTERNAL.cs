using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 设备信息
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct BASS_DEVICEINFO_INTERNAL
    {
        /// <summary>
        /// 
        /// </summary>
        public IntPtr name;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr driver;

        /// <summary>
        /// 
        /// </summary>
        public BASSDeviceInfo flags;
    }
}
