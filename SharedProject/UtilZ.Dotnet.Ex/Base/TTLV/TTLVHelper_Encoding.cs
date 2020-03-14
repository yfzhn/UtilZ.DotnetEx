using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    //Encoding
    public partial class TTLVHelper
    {
        #region Encoding
        /// <summary>
        /// 转换为bytes
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <returns>转换结果bytes</returns>
        public static byte[] Encoding(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Type type = obj.GetType();
            var ttlvTypeStructTree = GetTypeStructTree(type);

            List<byte> buffer = new List<byte>();
            PrimitiveEncoding(buffer, ttlvTypeStructTree.Childs, obj);
            return buffer.ToArray();
        }

        private static void PrimitiveEncoding(List<byte> buffer, List<TTLVStructNode> childs, object obj)
        {
            foreach (var node in childs)
            {
                var value = node.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                switch (node.TTLVType)
                {
                    case TTLVType.BasicType:
                        EncodingBasicType(buffer, node, value);
                        break;
                    case TTLVType.Convert:
                        EncodingConvert(buffer, node, value);
                        break;
                    case TTLVType.Object:
                        EncodingObject(buffer, node, value);
                        break;
                    case TTLVType.Array:
                    case TTLVType.IList:
                        EncodingCollection(buffer, node, value);
                        break;
                    default:
                        throw new NotImplementedException(string.Format("未知类型", node.TTLVType.ToString()));
                }
            }
        }

        private static void EncodingCollection(List<byte> buffer, TTLVStructNode node, object collectionObj)
        {
            List<byte> buffer2 = new List<byte>();
            var collection = (IEnumerable)collectionObj;
            foreach (var item in collection)
            {
                EncodingObject(buffer2, node, item);
            }

            AddTTLVNodeValue(buffer, node, buffer2.ToArray());
        }

        private static void EncodingObject(List<byte> buffer, TTLVStructNode node, object objValue)
        {
            List<byte> buffer2 = new List<byte>();
            var childs = GetTTLVStructNodeChilds(node);
            PrimitiveEncoding(buffer2, childs, objValue);
            AddTTLVNodeValue(buffer, node, buffer2.ToArray());
        }

        private static void EncodingConvert(List<byte> buffer, TTLVStructNode node, object objValue)
        {
            var valueBuffer = node.Converter.ConvertToBytes(node.Tag, objValue, node.ElementType, node.ConverterPara);
            AddTTLVNodeValue(buffer, node, valueBuffer);
        }

        private static void EncodingBasicType(List<byte> buffer, TTLVStructNode node, object objValue)
        {
            byte[] valueBuffer = TTLVCommon.ConvertToBytes(node.TypeCode, objValue);
            AddTTLVNodeValue(buffer, node, valueBuffer);
        }

        private static void AddTTLVNodeValue(List<byte> buffer, TTLVStructNode node, byte[] valueBuffer)
        {
            _codec.WriteNode(buffer, node.Tag, node.TypeCode, valueBuffer);
            //buffer.AddRange(BitConverter.GetBytes(node.Tag));
            //buffer.Add((byte)node.TypeCode);
            //buffer.AddRange(BitConverter.GetBytes(valueBuffer.Length));
            //buffer.AddRange(valueBuffer);
        }
        #endregion
    }
}
