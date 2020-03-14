using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.NativeMethod
{
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ICMP_OPTIONS
    {
        /// <summary>
        /// 
        /// </summary>
        public byte Ttl;

        /// <summary>
        /// 
        /// </summary>
        public byte Tos;

        /// <summary>
        /// 
        /// </summary>
        public byte Flags;

        /// <summary>
        /// 
        /// </summary>
        public byte OptionsSize;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr OptionsData;
    }
}
