using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    //Decoding
    public partial class TTLVHelper
    {
        #region Decoding
        /// <summary>
        /// 解析数据[支持类和结构体]
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="buffer">要解析的bytes</param>
        /// <param name="beginIndex">解析数据起始索引(小于0表示从头开始)</param>
        /// <param name="length">解析数据长度(小于0表示从起始到末尾)</param>
        /// <returns>解析结果</returns>
        public static T Decoding<T>(byte[] buffer, int beginIndex = -1, int length = -1) where T : new()
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            using (var ms = new MemoryStream(buffer))
            {
                if (beginIndex > 0)
                {
                    ms.Position = beginIndex;
                }

                if (length < 0)
                {
                    length = buffer.Length;
                }

                var ttlvTypeStructTree = GetTypeStructTree(typeof(T));
                var br = new BinaryReader(ms);
                object obj = new T();//注:此处使用object他传递使用使用ref是为了支持结构类型.
                PrimitiveDecoding(br, length, ref obj, ttlvTypeStructTree.Childs);
                return (T)obj;
            }
        }

        /// <summary>
        /// 解析数据到指定对象体中
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="obj">数据存储对象</param>
        /// <param name="buffer">要解析的bytes</param>
        /// <param name="beginIndex">解析数据起始索引(小于0表示从头开始)</param>
        /// <param name="length">解析数据长度(小于0表示从起始到末尾)</param>
        public static void Decoding<T>(T obj, byte[] buffer, int beginIndex = -1, int length = -1) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            using (var ms = new MemoryStream(buffer))
            {
                if (beginIndex > 0)
                {
                    ms.Position = beginIndex;
                }

                if (length < 0)
                {
                    length = buffer.Length;
                }

                var ttlvTypeStructTree = GetTypeStructTree(obj.GetType());
                var br = new BinaryReader(ms);
                object obj2 = obj;
                PrimitiveDecoding(br, length, ref obj2, ttlvTypeStructTree.Childs);
            }
        }

        /// <summary>
        /// 解析数据[支持类和结构体]
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="stream">要解析的数据流</param>
        /// <param name="position">流解析起始位置(小于0表示从头开始)</param>
        /// <param name="endPosition">流解析结束位置(小于0表示从起始到末尾)</param>
        /// <returns>解析结果</returns>
        public static T Decoding<T>(Stream stream, long position = -1, long endPosition = -1) where T : new()
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (position < 0)
            {
                position = 0;
            }

            if (stream.Position != position)
            {
                stream.Position = position;
            }

            if (endPosition < 0)
            {
                endPosition = stream.Length;
            }

            var ttlvTypeStructTree = GetTypeStructTree(typeof(T));
            var br = new BinaryReader(stream);
            object obj = new T();//注:此处使用object他传递使用使用ref是为了支持结构类型.
            PrimitiveDecoding(br, endPosition, ref obj, ttlvTypeStructTree.Childs);
            return (T)obj;
        }

        /// <summary>
        /// 解析数据到指定对象体中
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="obj">数据存储对象</param>
        /// <param name="stream">要解析的数据流</param>
        /// <param name="position">流解析起始位置(小于0表示从头开始)</param>
        /// <param name="endPosition">流解析结束位置(小于0表示从起始到末尾)</param>
        public static void Decoding<T>(T obj, Stream stream, long position = -1, long endPosition = -1) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (position < 0)
            {
                position = 0;
            }

            if (stream.Position != position)
            {
                stream.Position = position;
            }

            if (endPosition < 0)
            {
                endPosition = stream.Length;
            }

            var ttlvTypeStructTree = GetTypeStructTree(obj.GetType());
            var br = new BinaryReader(stream);
            object obj2 = obj;
            PrimitiveDecoding(br, endPosition, ref obj2, ttlvTypeStructTree.Childs);
        }

        /// <summary>
        /// 解析数据到指定对象体中
        /// </summary>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <param name="obj">数据存储对象</param>
        /// <param name="br">BinaryReader</param>
        /// <param name="position">流解析起始位置(小于0表示从头开始)</param>
        /// <param name="endPosition">流解析结束位置(小于0表示从起始到末尾)</param>
        public static void Decoding<T>(T obj, BinaryReader br, long position = -1, long endPosition = -1) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (br == null)
            {
                throw new ArgumentNullException(nameof(br));
            }

            if (position < 0)
            {
                position = 0;
            }

            if (br.BaseStream.Position != position)
            {
                br.BaseStream.Position = position;
            }

            if (endPosition < 0)
            {
                endPosition = br.BaseStream.Length;
            }

            var ttlvTypeStructTree = GetTypeStructTree(obj.GetType());
            object obj2 = obj;
            PrimitiveDecoding(br, endPosition, ref obj2, ttlvTypeStructTree.Childs);
        }

        private static void PrimitiveDecoding(BinaryReader br, long endPosition, ref object obj, List<TTLVStructNode> childs)
        {
            var nodeInfo = new TTLVNodeInfo();
            while (br.BaseStream.Position < endPosition)
            {
                //int tag = br.ReadInt32();
                //TypeCode valueBufferTypeCode = (TypeCode)br.ReadByte();
                //int length = br.ReadInt32();
                _codec.ReadNodeInfo(br, nodeInfo);
                int length = nodeInfo.Length;
                TypeCode valueBufferTypeCode = nodeInfo.Type;

                TTLVStructNode node = childs.Find(t => { return t.Tag == nodeInfo.Tag; });
                if (node == null || !node.CheckCompatible(valueBufferTypeCode))
                {
                    br.BaseStream.Position += length;
                    continue;
                }

                switch (node.TTLVType)
                {
                    case TTLVType.BasicType:
                        DecodingBasicType(br, ref obj, node, length, valueBufferTypeCode);
                        break;
                    case TTLVType.Convert:
                        DecodingConvert(br, ref obj, node, length);
                        break;
                    case TTLVType.Object:
                        DecodingObject(br, ref obj, node, length);
                        break;
                    case TTLVType.Array:
                        DecodingArray(br, ref obj, node, length);
                        break;
                    case TTLVType.IList:
                        DecodingIList(br, ref obj, node, length);
                        break;
                    default:
                        throw new NotImplementedException(string.Format("未知类型", node.TTLVType.ToString()));
                }
            }
        }

        private static void DecodingIList(BinaryReader br, ref object obj, TTLVStructNode node, int valueLength)
        {
            var list = (IList)node.GetValue(obj);
            if (list == null)
            {
                list = (IList)node.CreateMemberInstance();
                SetTTLVNodeValue(obj, node, list);
            }

            DecodingCollection(list, br, node, valueLength);
        }

        private static void DecodingArray(BinaryReader br, ref object obj, TTLVStructNode node, int valueLength)
        {
            var list = new List<object>();
            DecodingCollection(list, br, node, valueLength);
            Array array = Array.CreateInstance(node.ElementType, list.Count);
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(list[i], i);
            }

            SetTTLVNodeValue(obj, node, array);
        }

        private static void DecodingCollection(IList list, BinaryReader br, TTLVStructNode node, int valueLength)
        {
            TTLVCommon.CheckHasNoParaConstructor(node.ElementType);
            var endPosition = br.BaseStream.Position + valueLength;
            var nodeInfo = new TTLVNodeInfo();

            while (br.BaseStream.Position < endPosition)
            {
                //int tag = br.ReadInt32();
                //TypeCode valueBufferTypeCode = (TypeCode)br.ReadByte();
                //int length = br.ReadInt32();
                _codec.ReadNodeInfo(br, nodeInfo);

                object instance = Activator.CreateInstance(node.ElementType);
                PrimitiveDecoding(br, br.BaseStream.Position + nodeInfo.Length, ref instance, node.Childs);
                list.Add(instance);
            }

            br.BaseStream.Position = endPosition;
        }

        private static void DecodingObject(BinaryReader br, ref object obj, TTLVStructNode node, int valueLength)
        {
            var obj2 = node.GetValue(obj);
            if (obj2 == null)
            {
                obj2 = node.CreateMemberInstance();
                SetTTLVNodeValue(obj, node, obj2);
            }

            var endPosition = br.BaseStream.Position + valueLength;
            var childs = GetTTLVStructNodeChilds(node);
            PrimitiveDecoding(br, endPosition, ref obj2, childs);
            br.BaseStream.Position = endPosition;
        }

        private static void DecodingConvert(BinaryReader br, ref object obj, TTLVStructNode node, int valueLength)
        {
            byte[] valueBuffer = new byte[valueLength];
            br.Read(valueBuffer, 0, valueBuffer.Length);
            object memberValue = node.GetValue(obj);
            object value = node.Converter.ConvertBackObject(node.Tag, valueBuffer, memberValue, node.GetMemberType(), node.ConverterPara);
            SetTTLVNodeValue(obj, node, value);
        }

        private static void DecodingBasicType(BinaryReader br, ref object obj, TTLVStructNode node, int valueLength, TypeCode valueBufferTypeCode)
        {
            byte[] valueBuffer = new byte[valueLength];
            br.Read(valueBuffer, 0, valueBuffer.Length);
            object value = TTLVCommon.ConvterBack(valueBuffer, valueBufferTypeCode, 0, valueLength);
            SetTTLVNodeValue(obj, node, value);
        }

        private static void SetTTLVNodeValue(object obj, TTLVStructNode node, object value)
        {
            node.SetValue(obj, value);
        }
        #endregion
    }
}
