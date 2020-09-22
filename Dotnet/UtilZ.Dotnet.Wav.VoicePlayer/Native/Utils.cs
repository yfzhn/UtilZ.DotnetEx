using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    internal class Utils
    {
        public static string IntPtrAsStringUnicode(IntPtr unicodePtr)
        {
            if (unicodePtr != IntPtr.Zero)
            {
                return Marshal.PtrToStringUni(unicodePtr);
            }

            return null;
        }




        public static string IntPtrAsStringUtf8(IntPtr utf8Ptr, out int len)
        {
            len = 0;
            if (utf8Ptr != IntPtr.Zero)
            {
                len = IntPtrNullTermLength(utf8Ptr);
                if (len != 0)
                {
                    byte[] destination = new byte[len];
                    Marshal.Copy(utf8Ptr, destination, 0, len);
                    return Encoding.UTF8.GetString(destination, 0, len);
                }
            }
            return null;
        }

        public static byte[] GetIntPtrStringBytes(IntPtr utf8Ptr)
        {
            if (utf8Ptr == IntPtr.Zero)
            {
                return null;
            }

            int len = IntPtrNullTermLength(utf8Ptr);
            if (len == 0)
            {
                return null;
            }

            byte[] destination = new byte[len];
            Marshal.Copy(utf8Ptr, destination, 0, len);
            return destination;
        }

        private static int IntPtrNullTermLength(IntPtr p)
        {
            int length = 0;
            while (Marshal.ReadByte(p, length) != 0)
            {
                length++;
            }

            return length;
        }






        public static string IntPtrAsStringAnsi(IntPtr ansiPtr)
        {
            int num;
            return IntPtrAsStringUtf8orLatin1(ansiPtr, out num);
        }

        public static string IntPtrAsStringUtf8orLatin1(IntPtr utf8Ptr, out int len)
        {
            len = 0;
            if (utf8Ptr != IntPtr.Zero)
            {
                len = IntPtrNullTermLength(utf8Ptr);
                if (len != 0)
                {
                    byte[] destination = new byte[len];
                    Marshal.Copy(utf8Ptr, destination, 0, len);
                    //string str = BassNet.UseBrokenLatin1Behavior ? Encoding.Default.GetString(destination, 0, len) : Encoding.GetEncoding("latin1").GetString(destination, 0, len);
                    string str = Encoding.GetEncoding("latin1").GetString(destination, 0, len);
                    try
                    {
                        string str2 = new UTF8Encoding(false, true).GetString(destination, 0, len);
                        if (str2.Length < str.Length)
                        {
                            return str2;
                        }
                    }
                    catch
                    {
                    }
                    return str;
                }
            }
            return null;
        }


        public static int MakeLong(int lowWord, int highWord)
        {
            return ((highWord << 0x10) | (lowWord & 0xffff));
        }


        public static string IntPtrAsStringUtf8(IntPtr utf8Ptr)
        {
            if (utf8Ptr != IntPtr.Zero)
            {
                int length = IntPtrNullTermLength(utf8Ptr);
                if (length != 0)
                {
                    byte[] destination = new byte[length];
                    Marshal.Copy(utf8Ptr, destination, 0, length);
                    return Encoding.UTF8.GetString(destination, 0, length);
                }
            }
            return null;
        }



    }
}
