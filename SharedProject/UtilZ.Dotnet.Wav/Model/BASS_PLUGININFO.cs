using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 插件信息结构体
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct BASS_PLUGININFO
    {
        /// <summary>
        /// Plugin version, in the same form as given by BASS_GetVersion
        /// </summary>
        public int Version;

        /// <summary>
        /// Number of supported formats
        /// </summary>
        public int Formatc;

        /// <summary>
        /// The array of supported formats. The array contains formatc elements.
        /// </summary>
        public BASS_PLUGINFORM[] Formats;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatc">Number of supported formats</param>
        /// <param name="version">Plugin version, in the same form as given by BASS_GetVersion</param>
        /// <param name="Formats">The array of supported formats. The array contains formatc elements</param>
        internal BASS_PLUGININFO(int formatc, int version, BASS_PLUGINFORM[] Formats)
        {
            this.Version = version;
            this.Formatc = Formats.Length;
            this.Formats = Formats;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Version, this.Formatc);
        }
    }

    /// <summary>
    /// 插件格式信息结构体
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct BASS_PLUGINFORM
    {
        /// <summary>
        /// The channel type, as would appear in the BASS_CHANNELINFO structure
        /// </summary>
        public int CType;

        /// <summary>
        /// Format description
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;

        /// <summary>
        /// File extension filter, in the form of "*.ext1;*.ext2;...". 
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Exts;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Name">Format description</param>
        /// <param name="Extensions">File extension filter, in the form of "*.ext1;*.ext2;...". </param>
        /// <param name="ChannelType">The channel type, as would appear in the BASS_CHANNELINFO structure</param>
        public BASS_PLUGINFORM(string Name, string Extensions, int ChannelType)
        {
            this.CType = ChannelType;
            this.Name = Name;
            this.Exts = Extensions;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0}|{1}", this.Name, this.Exts);
        }
    }
}
