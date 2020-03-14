using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTVL辅助类[编码规则:Tag:4字节;Type:1字节;Length:4字节;Value:bytes]
    /// </summary>
    public partial class TTLVHelper
    {
        static TTLVHelper()
        {
            _ttlvAttributeType = typeof(TTLVAttribute);
            _ttlvIConverterTypeName = typeof(ITTLVConverter).FullName;
            _ienumerableTypeName = typeof(System.Collections.IEnumerable).FullName;
            _memberAcessFlags = BindingFlags.Public | BindingFlags.Instance;
            _codec = new TTLVNodeCodec();
        }

        private readonly static Type _ttlvAttributeType;
        private readonly static string _ttlvIConverterTypeName;
        private readonly static string _ienumerableTypeName;
        private readonly static BindingFlags _memberAcessFlags;
        private static ITTLVNodeCodec _codec;

        /// <summary>
        /// 设置节点编解码器
        /// </summary>
        /// <param name="codec">节点编解码器</param>
        public static void SetNodeCodec(ITTLVNodeCodec codec)
        {
            if (codec == null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            _codec = codec;
        }

        #region GetTypeStructTree

        private readonly static Dictionary<Type, TTLVStructNode> _typeStructTreeDic = new Dictionary<Type, TTLVStructNode>();
        private static TTLVStructNode GetTypeStructTree(Type type)
        {
            TTLVStructNode typeStructTree;
            if (_typeStructTreeDic.TryGetValue(type, out typeStructTree))
            {
                return typeStructTree;
            }

            lock (_typeStructTreeDic)
            {
                if (_typeStructTreeDic.TryGetValue(type, out typeStructTree))
                {
                    return typeStructTree;
                }

                typeStructTree = new TTLVStructNode(0, TypeCode.Empty, null, null);
                var allNodeDic = new Dictionary<Type, TTLVStructNode>();
                CreateTypeStructTree(allNodeDic, typeStructTree, type);
                _typeStructTreeDic[type] = typeStructTree;
                return typeStructTree;
            }
        }

        private static void CreateTypeStructTree(Dictionary<Type, TTLVStructNode> allNodeDic, TTLVStructNode parentNode, Type type)
        {
            CreateTypePropertyStructTree(allNodeDic, parentNode, type);//属性
            CreateTypeFieldStructTree(allNodeDic, parentNode, type);//字段
        }

        private static void CreateTypeFieldStructTree(Dictionary<Type, TTLVStructNode> allNodeDic, TTLVStructNode parentNode, Type type)
        {
            FieldInfo[] fieldInfoArr = type.GetFields(_memberAcessFlags);
            foreach (FieldInfo fieldInfo in fieldInfoArr)
            {
                object[] objAtts = fieldInfo.GetCustomAttributes(_ttlvAttributeType, true);
                if (objAtts.Length < 1)
                {
                    //未标记,忽略
                    continue;
                }

                var typeCode = Type.GetTypeCode(fieldInfo.FieldType);
                var ttlvAttribute = (TTLVAttribute)objAtts[0];
                var node = new TTLVStructNode(ttlvAttribute.Tag, typeCode, fieldInfo);

                if (typeCode == TypeCode.Object)
                {
                    if (ttlvAttribute.ConverterType != null)
                    {
                        //优先使用自定义转换
                        InitConverterNode(ttlvAttribute, node, fieldInfo.FieldType);
                    }
                    else
                    {
                        InitNoConverterNode(allNodeDic, fieldInfo, ttlvAttribute, node);
                    }
                }
                else
                {
                    node.TTLVType = TTLVType.BasicType;
                }

                parentNode.Childs.Add(node);
            }
        }

        private static void CreateTypePropertyStructTree(Dictionary<Type, TTLVStructNode> allNodeDic, TTLVStructNode parentNode, Type type)
        {
            PropertyInfo[] propertyInfoArr = type.GetProperties(_memberAcessFlags);
            foreach (PropertyInfo propertyInfo in propertyInfoArr)
            {
                object[] objAtts = propertyInfo.GetCustomAttributes(_ttlvAttributeType, true);
                if (objAtts.Length < 1)
                {
                    //未标记,忽略
                    continue;
                }

                var typeCode = Type.GetTypeCode(propertyInfo.PropertyType);
                var ttlvAttribute = (TTLVAttribute)objAtts[0];
                var node = new TTLVStructNode(ttlvAttribute.Tag, typeCode, propertyInfo, ttlvAttribute.Index);

                if (typeCode == TypeCode.Object)
                {
                    if (ttlvAttribute.ConverterType != null)
                    {
                        //优先使用自定义转换
                        InitConverterNode(ttlvAttribute, node, propertyInfo.PropertyType);
                    }
                    else
                    {
                        InitNoConverterNode(allNodeDic, propertyInfo, ttlvAttribute, node);
                    }
                }
                else
                {
                    node.TTLVType = TTLVType.BasicType;
                }

                parentNode.Childs.Add(node);
            }
        }

        private static void InitNoConverterNode(Dictionary<Type, TTLVStructNode> allNodeDic, PropertyInfo propertyInfo, TTLVAttribute ttlvAttribute, TTLVStructNode node)
        {
            switch (ttlvAttribute.PropertyType)
            {
                case TTLVPropertyType.Collection:
                    Type eleType;
                    TTLVType ttlvType;
                    TTLVCommon.GetCollectionElementType(propertyInfo.PropertyType, out eleType, out ttlvType);
                    node.TTLVType = ttlvType;
                    node.ElementType = eleType;
                    CreateTypeStructTree(allNodeDic, node, eleType);
                    break;
                case TTLVPropertyType.SingleObject:
                    if (allNodeDic.ContainsKey(propertyInfo.PropertyType))
                    {
                        //解决递归类型,即类型同嵌套自身类型
                        node.RefNode = allNodeDic[propertyInfo.PropertyType];
                    }
                    else
                    {
                        allNodeDic.Add(propertyInfo.PropertyType, node);
                        node.ElementType = propertyInfo.PropertyType;
                        CreateTypeStructTree(allNodeDic, node, propertyInfo.PropertyType);
                    }

                    node.TTLVType = TTLVType.Object;
                    break;
                default:
                    throw new NotImplementedException($"未实现的TTLVPropertyType[{ttlvAttribute.PropertyType.ToString()}]");
            }
        }

        private static void InitNoConverterNode(Dictionary<Type, TTLVStructNode> allNodeDic, FieldInfo fieldInfo, TTLVAttribute ttlvAttribute, TTLVStructNode node)
        {
            switch (ttlvAttribute.PropertyType)
            {
                case TTLVPropertyType.Collection:
                    Type eleType;
                    TTLVType ttlvType;
                    TTLVCommon.GetCollectionElementType(fieldInfo.FieldType, out eleType, out ttlvType);
                    node.TTLVType = ttlvType;
                    node.ElementType = eleType;
                    CreateTypeStructTree(allNodeDic, node, eleType);
                    break;
                case TTLVPropertyType.SingleObject:
                    if (allNodeDic.ContainsKey(fieldInfo.FieldType))
                    {
                        //解决递归类型,即类型同嵌套自身类型
                        node.RefNode = allNodeDic[fieldInfo.FieldType];
                    }
                    else
                    {
                        allNodeDic.Add(fieldInfo.FieldType, node);
                        node.ElementType = fieldInfo.FieldType;
                        CreateTypeStructTree(allNodeDic, node, fieldInfo.FieldType);
                    }

                    node.TTLVType = TTLVType.Object;
                    break;
                default:
                    throw new NotImplementedException($"未实现的TTLVPropertyType[{ttlvAttribute.PropertyType.ToString()}]");
            }
        }

        private static void InitConverterNode(TTLVAttribute ttlvAttribute, TTLVStructNode node, Type eleType)
        {
            if (ttlvAttribute.ConverterType.GetInterface(_ttlvIConverterTypeName) == null)
            {
                throw new ArgumentException(string.Format("转换器类型[{0}]未实现[{1}]接口", ttlvAttribute.ConverterType.FullName, _ttlvIConverterTypeName));
            }

            node.Converter = CreateTTLVConverter(ttlvAttribute);
            node.ConverterPara = ttlvAttribute.ConverterPara;
            node.TTLVType = TTLVType.Convert;
            node.ElementType = eleType;
        }

        private static Hashtable _htConverter = new Hashtable();
        private static ITTLVConverter CreateTTLVConverter(TTLVAttribute ttlvAttribute)
        {
            ITTLVConverter converter;
            var key = ttlvAttribute.ConverterType;
            if (_htConverter.ContainsKey(key))
            {
                converter = (ITTLVConverter)_htConverter[key];
            }
            else
            {
                converter = (ITTLVConverter)Activator.CreateInstance(ttlvAttribute.ConverterType);
                _htConverter[key] = converter;
            }

            return converter;
        }

        #endregion

        private static List<TTLVStructNode> GetTTLVStructNodeChilds(TTLVStructNode node)
        {
            List<TTLVStructNode> childs;
            if (node.RefNode != null)
            {
                childs = node.RefNode.Childs;
            }
            else
            {
                childs = node.Childs;
            }

            return childs;
        }



        /// <summary>
        /// 测试
        /// </summary>
        public static void Test()
        {
            try
            {

                //List<string> strList = new List<string>();
                //strList.Add("abc");
                //strList.Add(null);
                //strList.Add(string.Empty);
                //strList.Add("");
                //strList.Add("end");

                //string[] strArr = new string[] { "ycq", null, string.Empty, "zhn" };

                //string str = null;
                //object obj = str;
                //string str2 = Convert.ToString(obj);
                //var valueBuffer = System.Text.Encoding.UTF8.GetBytes(str2);

                TestGeneric();

                TestClass();

                TestStruct();


            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private static void TestGeneric()
        {
            try
            {
                var cmd1 = new CommandBaseT<Stu>(222, new Stu() { Addr = "四川", Leve = 24 });
                cmd1.Data = System.Text.Encoding.UTF8.GetBytes("hello word!三大一..");
                //cmd1.Data = null;
                byte[] buffer = cmd1.GenerateCommandData();

                var cmd2 = new CommandBaseT<Stu>();
                cmd2.Parse(buffer);
                string str = System.Text.Encoding.UTF8.GetString(cmd2.Data);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private static void TestClass()
        {

            var demo = new TTLVDemo();
            demo.PortList = new List<int>() { 1, 2, 3, 4, 5, 6 };
            demo.StringList = new List<string>() { "ycq", "yf" };
            demo.TimeList = new List<DateTime>() { DateTime.Parse("2019-03-25 18:00:00"), DateTime.Parse("2019-03-26 19:11:22") };
            demo.LongList = new List<long>() { 345345, 65786578, 675897689 };
            demo.ByteList = new List<byte>() { 1, 2, 3 };
            demo.ShortList = new List<short>() { 321, 654 };
            demo.CharList = new List<char>() { 'Z', 'Q', 'E' };
            demo.DoubleList = new List<double>() { 222.333d, 5555.678d };
            demo.SingleList = new List<float>() { 123.123f, 456.456f };
            demo.Sex = Sex.Z;
            demo.Sex2 = 123.123f;
            demo.Sex3 = 456.456453635d;
            demo.Sex4 = 345342564236543256;
            demo.Name = "白起";
            demo.Age = 31;
            demo.Person = new Person() { Addr = "天由", Leve = 23 };
            demo.Person.Pre = new Person { Addr = "Pre未名天", Leve = 99 };
            demo.Person.Next = new Person { Addr = "Next情怡", Leve = 22 };
            demo.Person.Next.Next = new Person { Addr = "Next222情怡", Leve = 32 };
            demo.Stu = new Stu() { Addr = "四川", Leve = 24 };
            demo.City = "成都";
            demo.PersonList = new List<Person>();
            demo.PersonList.Add(new Person() { Addr = "文莉1", Leve = 24 });
            demo.PersonList.Add(new Person() { Addr = "文莉2", Leve = 25 });
            demo.PersonList.Add(new Person() { Addr = "文莉3", Leve = 26 });

            demo.PersonArr = new Person[1] { new Person() { Addr = "文网页版3", Leve = 36 } };
            demo.Stu2 = new Stu() { Addr = "成都", Leve = 33 };

            var buffer = Encoding(demo);

            var result1 = Decoding<TTLVDemo>(buffer);

            var result2 = new TTLVDemo();
            Decoding(result2, buffer);


            using (var ms = new MemoryStream(buffer))
            {
                var result3 = Decoding<TTLVDemo>(ms, 0, ms.Length);


                var result4 = new TTLVDemo();
                ms.Position = 0;
                Decoding(result4, ms);
            }
        }

        private static void TestStruct()
        {
            var strx = new TTLVDemoStruct();
            strx.PortList = new List<int>() { 1, 2, 3, 4, 5, 6 };
            strx.Sex = Sex.Z;
            strx.Sex2 = 123.123f;
            strx.Sex3 = 456.456453635d;
            strx.Sex4 = 345342564236543256;
            strx.Name = "白起";
            strx.Age = 31;
            strx.Person = new Person() { Addr = "天由", Leve = 23 };
            strx.Age22 = 345;


            var buffer = Encoding(strx);

            var result1 = Decoding<TTLVDemoStruct>(buffer);

            ////var result2 = new TTLVDemoStruct();
            ////Decoding(buffer, ref result2);



            using (var ms = new MemoryStream(buffer))
            {
                var result3 = Decoding<TTLVDemoStruct>(ms);


                //    //var result4 = new TTLVDemoStruct();
                //    //ms.Position = 0;
                //    //Decoding(ms, ref result4);
            }
        }

    }
}
