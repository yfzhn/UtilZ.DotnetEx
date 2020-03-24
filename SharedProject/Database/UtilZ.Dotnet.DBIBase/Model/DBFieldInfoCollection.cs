using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 数据库表字段信息集合
    /// </summary>
    [Serializable]
    public class DBFieldInfoCollection : IEnumerable<DBFieldInfo>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items">数据库表字段信息集合</param>
        public DBFieldInfoCollection(IEnumerable<DBFieldInfo> items)
        {
            if (items == null || items.Count() == 0)
            {
                return;
            }

            foreach (var item in items)
            {
                this._fieldInfoDic.Add(item.FieldName.ToUpper(), item);
            }
        }

        /// <summary>
        /// 数据库表字段信息集合
        /// </summary>
        private readonly Dictionary<string, DBFieldInfo> _fieldInfoDic = new Dictionary<string, DBFieldInfo>();

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<DBFieldInfo> GetEnumerator()
        {
            return this._fieldInfoDic.Values.GetEnumerator();
        }

        /// <summary>
        /// System.Collections.IEnumerable.GetEnumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 确定元素item是否包含在集合中[true:包含;false:不包含]
        /// </summary>
        /// <param name="item">要在集合中查找的对象</param>
        /// <returns>true:包含;false:不包含</returns>
        public bool Contains(DBFieldInfo item)
        {
            return this._fieldInfoDic.Values.Contains(item);
        }

        /// <summary>
        /// 确定是字段filedName否包含在集合中[true:包含;false:不包含]
        /// </summary>
        /// <param name="filedName">要在集合中查找的字段名称</param>
        /// <returns>true:包含;false:不包含</returns>
        public bool Contains(string filedName)
        {
            return this._fieldInfoDic.ContainsKey(filedName);
        }

        /// <summary>
        /// 集合元素个数
        /// </summary>
        public int Count
        {
            get { return this._fieldInfoDic.Count; }
        }

        /// <summary>
        /// 获取指定索引处的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>索引处的元素</returns>
        public DBFieldInfo this[int index]
        {
            get
            {
                if (this._fieldInfoDic.Count == 0 ||
                    index < 0 ||
                    index >= this._fieldInfoDic.Count)
                {
                    throw new ArgumentOutOfRangeException("索引超出范围。必须为非负值并小于集合大小", "index");
                }

                return this._fieldInfoDic.ElementAt(index).Value;
            }
        }

        /// <summary>
        /// 获取指定索引处的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>索引处的元素</returns>
        public DBFieldInfo this[string index]
        {
            get
            {
                if (this._fieldInfoDic.ContainsKey(index))
                {
                    return this._fieldInfoDic[index];
                }
                else
                {
                    throw new KeyNotFoundException(string.Format("不存在名称为{0}的字段", index));
                }
            }
        }
    }
}
