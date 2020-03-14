using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.NativeMethod
{
    /// <summary>
    /// ICMP_ECHO_REPLY
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ICMP_ECHO_REPLY
    {
        /// <summary>
        /// 
        /// </summary>
        public int Address;

        /// <summary>
        /// 
        /// </summary>
        public int Status;

        /// <summary>
        /// 
        /// </summary>
        public int RoundTripTime;

        /// <summary>
        /// 
        /// </summary>
        public short DataSize;

        /// <summary>
        /// 
        /// </summary>
        public short Reserved;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr DataPtr;

        /// <summary>
        /// 
        /// </summary>
        public ICMP_OPTIONS Options;

        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 250)]
        public string Data;
    }
}
