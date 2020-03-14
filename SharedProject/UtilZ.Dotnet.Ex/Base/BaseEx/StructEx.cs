using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Struct扩展方法类
    /// </summary>
    public static class StructEx
    {
        /// <summary>
        /// 将结构体类型转换为字节数据
        /// </summary>
        /// <param name="obj">结构体类对象</param>
        /// <returns>字节数据</returns> 
        public static byte[] StructToBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);

            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(obj, structPtr, false);

            //从内存空间拷贝到byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);

            //释放内存空间
            Marshal.FreeHGlobal(structPtr);

            return bytes;

        }

        /// <summary>
        /// 将字节数组转换为结构体对象
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="bytes">字节数组</param>
        /// <returns>结构体对象</returns>
        public static object ByteToStruct<T>(byte[] bytes)
        {
            Type type = typeof(T);
            int size = Marshal.SizeOf(type);

            //分配结构体内存空间
            IntPtr ptr = Marshal.AllocHGlobal(size);

            //将byte数组拷贝到分配好的内存空间
            Marshal.Copy(bytes, 0, ptr, size);

            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(ptr, type);

            //释放内存空间
            Marshal.FreeHGlobal(ptr);

            return obj;
        }

        /// <summary>
        /// 结构体反射设置值[注:返回的结构体对象不是之前的对象,因为结构体是值类型]
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="value">要设置值的结构体对象</param>
        /// <param name="fieldValues">结构体对象要设置的字段及值字典集合</param>
        /// <returns>设置值后的结构体对象</returns>
        public static T StructReflectionSetValue<T>(T value, Dictionary<FieldInfo, object> fieldValues) where T : struct
        {
            if (fieldValues == null || fieldValues.Count == 0)
            {
                return value;
            }

            StructReflectionSetValueHelper<T> shellReference = new StructReflectionSetValueHelper<T>(value);
            TypedReference obj = TypedReference.MakeTypedReference(shellReference, new FieldInfo[] { shellReference.GetType().GetField("Value") });
            foreach (var kv in fieldValues)
            {
                if (kv.Key != null)
                {
                    kv.Key.SetValueDirect(obj, kv.Value);
                }
            }

            return shellReference.Value;
        }
    }

    /// <summary>
    /// 结果体反射设置值辅助类
    /// </summary>
    internal class StructReflectionSetValueHelper<T> where T : struct
    {
        /// <summary>
        /// 结构体对象
        /// </summary>
        public T Value;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">结构体对象</param>
        public StructReflectionSetValueHelper(T value)
        {
            this.Value = value;
        }
    }
}
