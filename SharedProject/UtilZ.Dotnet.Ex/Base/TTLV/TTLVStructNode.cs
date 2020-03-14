using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// 
    /// </summary>
    internal class TTLVStructNode
    {
        public int Tag { get; private set; }

        public TTLVType TTLVType { get; set; }
        public TypeCode TypeCode { get; private set; }

        public ITTLVConverter Converter { get; set; }
        internal object ConverterPara { get; set; }

        private List<TTLVStructNode> _childs = new List<TTLVStructNode>();
        public List<TTLVStructNode> Childs
        {
            get
            {
                return _childs;
            }
        }

        /// <summary>
        /// 成员类型[true:属性;false:字段]
        /// </summary>
        private readonly bool _memberType;
        private readonly PropertyInfo _propertyInfo = null;
        private readonly FieldInfo _fieldInfo = null;
        private readonly object[] _index = null;

        public Type ElementType { get; set; }

        public TTLVStructNode RefNode { get; set; } = null;

        public TTLVStructNode(int tag, TypeCode typeCode, PropertyInfo propertyInfo, object[] index)
        {
            this.Tag = tag;
            this.TypeCode = typeCode;
            this._propertyInfo = propertyInfo;
            this._index = index;
            this._memberType = true;
        }

        public TTLVStructNode(int tag, TypeCode typeCode, FieldInfo fieldInfo)
        {
            this.Tag = tag;
            this.TypeCode = typeCode;
            this._fieldInfo = fieldInfo;
            this._memberType = false;
        }

        internal bool CheckCompatible(TypeCode valueBufferTypeCode)
        {
            return true;
        }


        public object GetValue(object obj)
        {
            object value;
            if (this._memberType)
            {
                value = this._propertyInfo.GetValue(obj, this._index);
            }
            else
            {
                value = this._fieldInfo.GetValue(obj);
            }

            return value;
        }

        public void SetValue(object obj, object value)
        {
            if (this._memberType)
            {
                this._propertyInfo.SetValue(obj, value, this._index);
            }
            else
            {
                this._fieldInfo.SetValue(obj, value);
            }
        }

        public Type GetMemberType()
        {
            if (this._memberType)
            {
                return this._propertyInfo.PropertyType;
            }
            else
            {
                return this._fieldInfo.FieldType;
            }
        }

        public object CreateMemberInstance()
        {
            Type type;
            if (this._memberType)
            {
                type = this._propertyInfo.PropertyType;
            }
            else
            {
                type = this._fieldInfo.FieldType;
            }

            TTLVCommon.CheckHasNoParaConstructor(type);
            return Activator.CreateInstance(type);
        }

        public override string ToString()
        {
            string memberName;
            if (this._memberType)
            {
                memberName = this._propertyInfo.Name;
            }
            else
            {
                memberName = this._fieldInfo.Name;
            }

            return $"{memberName}_{TTLVType.ToString()}_{TypeCode.ToString()}_{Tag}";
        }
    }
}
