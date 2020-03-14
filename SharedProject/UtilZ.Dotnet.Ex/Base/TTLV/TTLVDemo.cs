using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    internal struct TTLVDemoStruct
    {
        [TTLVAttribute(70, typeof(TTLVPrimitiveCollectionConverter))]
        public List<int> PortList { get; set; }

        [TTLVAttribute(50)]
        public Sex Sex { get; set; }

        [TTLVAttribute(51)]
        public float Sex2 { get; set; }

        [TTLVAttribute(52)]
        public double Sex3 { get; set; }

        [TTLVAttribute(53)]
        public long Sex4 { get; set; }

        [TTLVAttribute(5)]
        public string Name { get; set; }

        [TTLVAttribute(6)]
        public int Age { get; set; }

        [TTLVAttribute(7)]
        public Person Person { get; set; }

        [TTLVAttribute(8)]
        public int Age22;
    }

    internal class TTLVDemo
    {
        [TTLVAttribute(70, typeof(TTLVPrimitiveCollectionConverter))]
        public List<int> PortList { get; set; }

        [TTLVAttribute(72, typeof(TTLVPrimitiveCollectionConverter))]
        public List<string> StringList { get; set; }

        [TTLVAttribute(73, typeof(TTLVPrimitiveCollectionConverter))]
        public List<DateTime> TimeList { get; set; }

        [TTLVAttribute(74, typeof(TTLVPrimitiveCollectionConverter))]
        public List<long> LongList { get; set; }

        [TTLVAttribute(75, typeof(TTLVPrimitiveCollectionConverter))]
        public List<byte> ByteList { get; set; }

        [TTLVAttribute(76, typeof(TTLVPrimitiveCollectionConverter))]
        public List<short> ShortList { get; set; }

        [TTLVAttribute(77, typeof(TTLVPrimitiveCollectionConverter))]
        public List<char> CharList { get; set; }

        [TTLVAttribute(78, typeof(TTLVPrimitiveCollectionConverter))]
        public List<double> DoubleList { get; set; }

        [TTLVAttribute(79, typeof(TTLVPrimitiveCollectionConverter))]
        public List<Single> SingleList { get; set; }


        [TTLVAttribute(50)]
        public Sex Sex { get; set; }

        [TTLVAttribute(51)]
        public float Sex2;

        [TTLVAttribute(52)]
        public double Sex3;

        [TTLVAttribute(53)]
        public long Sex4;

        [TTLVAttribute(5)]
        public string Name { get; set; }

        [TTLVAttribute(6)]
        public int Age { get; set; }

        [TTLVAttribute(7)]
        public Person Person { get; set; }

        [TTLVAttribute(8, typeof(StuTTLVConvter))]
        public Stu Stu { get; set; }

        public string City { get; set; }


        [TTLVAttribute(9, false)]
        public List<Person> PersonList { get; set; }


        [TTLVAttribute(10, false)]
        public Person[] PersonArr { get; set; }

        [TTLVAttribute(11, typeof(TTLVSerializeConverter), null, TTLVSerializeType.Bin)]
        public Stu Stu2 { get; set; }

        public TTLVDemo()
        {

        }
    }

    internal enum Sex : byte
    {
        F = 1,

        M = 2,

        Z = 3
    }

    internal class Person
    {
        [TTLVAttribute(3)]
        public int Leve { get; set; }

        [TTLVAttribute(4)]
        public string Addr { get; set; }

        [TTLVAttribute(114)]
        public Person Next { get; set; }

        [TTLVAttribute(115)]
        public Person Pre { get; set; }
    }

    [Serializable]
    internal class Stu
    {
        [TTLVAttribute(1)]
        public int Leve { get; set; }

        [TTLVAttribute(2)]
        public string Addr { get; set; }

        public Stu()
        {

        }
    }

    internal class StuTTLVConvter : TTLVConverterBase
    {
        public StuTTLVConvter()
            : base()
        {

        }

        /// <summary>
        /// 目标对象转换为bytes
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">转换对象</param>
        /// <param name="targetType">目标属性或字段类型</param>
        /// <param name="converterPara">转换器参数</param>
        /// <returns>转换结果</returns>
        public override byte[] ConvertToBytes(int tag, object obj, Type targetType, object converterPara)
        {
            var stu = (Stu)obj;
            byte[] content = System.Text.Encoding.UTF8.GetBytes(stu.Addr);
            byte[] level = BitConverter.GetBytes(stu.Leve);
            byte[] buffer = new byte[level.Length + content.Length];
            Array.Copy(level, buffer, level.Length);
            Array.Copy(content, 0, buffer, level.Length, content.Length);
            return buffer;
        }

        /// <summary>
        /// 转换bytes到目标对象
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="buffer">byte[]</param>
        /// <param name="memberValue">目标属性或字段值</param>
        /// <param name="targetType">目标属性或字段类型</param>
        /// <param name="converterPara">转换器参数</param>
        /// <returns>解析结果对象</returns>
        public override object ConvertBackObject(int tag, byte[] buffer, object memberValue, Type targetType, object converterPara)
        {
            Stu stu = (Stu)memberValue;
            if (stu == null)
            {
                stu = new Stu();
            }

            stu.Leve = BitConverter.ToInt32(buffer, 0);
            stu.Addr = System.Text.Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
            return stu;
        }
    }











    /// <summary>
    /// 命令基类[TTLV_Tag范围(0-100)]
    /// </summary>
    [Serializable]
    internal abstract class CommandBase
    {
        public CommandBase()
        {

        }

        public CommandBase(Int32 cmd)
        {
            this.Cmd = cmd;
        }

        /// <summary>
        /// 命令字
        /// </summary>
        [TTLVAttribute(1)]
        public Int32 Cmd { get; protected set; }

        /// <summary>
        /// 生成命令数据
        /// </summary>
        /// <returns>命令数据</returns>
        public byte[] GenerateCommandData()
        {
            return TTLVHelper.Encoding(this);
        }

        /// <summary>
        /// 解析命令数据
        /// </summary>
        /// <param name="data">传输命令</param>
        public void Parse(byte[] data)
        {
            TTLVHelper.Decoding(this, data);
        }
    }

    /// <summary>
    /// 泛型命令基类
    /// </summary>
    [Serializable]
    internal class CommandBaseT<T> : CommandBase
    {
        [TTLVAttribute(101)]
        public T Value { get; set; }

        [TTLVAttribute(102, typeof(TTLVPrimitiveCollectionConverter))]
        public byte[] Data { get; set; }

        public CommandBaseT()
        {

        }

        public CommandBaseT(Int32 cmd, T value)
        {
            this.Cmd = cmd;
            this.Value = value;
        }
    }


}
