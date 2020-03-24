using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 数据库表索引集合
    /// </summary>
    [Serializable]
    public class DBIndexInfoCollection : IEnumerable<DBIndexInfo>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items">数据库表索引集合</param>
        public DBIndexInfoCollection(IEnumerable<DBIndexInfo> items)
        {
            foreach (var item in items)
            {
                this._indexInfoDic.Add(item.FieldName.ToUpper(), item);
            }
        }

        /// <summary>
        /// 索引字典集合[key:索引名称;value:DBIndexInfo]
        /// </summary>
        private readonly Dictionary<string, DBIndexInfo> _indexInfoDic = new Dictionary<string, DBIndexInfo>();

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<DBIndexInfo> GetEnumerator()
        {
            return this._indexInfoDic.Values.GetEnumerator();
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
        public bool Contains(DBIndexInfo item)
        {
            return this._indexInfoDic.Values.Contains(item);
        }

        /// <summary>
        /// 确定是字段filedName否包含在集合中[true:包含;false:不包含]
        /// </summary>
        /// <param name="indexName">要在集合中查找的字段名称</param>
        /// <returns>true:包含;false:不包含</returns>
        public bool Contains(string indexName)
        {
            return this._indexInfoDic.ContainsKey(indexName);
        }

        /// <summary>
        /// 集合元素个数
        /// </summary>
        public int Count
        {
            get { return this._indexInfoDic.Count; }
        }

        /// <summary>
        /// 获取指定索引处的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>索引处的元素</returns>
        public DBIndexInfo this[int index]
        {
            get
            {
                if (this._indexInfoDic.Count == 0 ||
                    index < 0 ||
                    index >= this._indexInfoDic.Count)
                {
                    throw new ArgumentOutOfRangeException("索引超出范围。必须为非负值并小于集合大小", "index");
                }

                return this._indexInfoDic.ElementAt(index).Value;
            }
        }

        /// <summary>
        /// 获取指定索引处的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>索引处的元素</returns>
        public DBIndexInfo this[string index]
        {
            get
            {
                if (this._indexInfoDic.ContainsKey(index))
                {
                    return this._indexInfoDic[index];
                }
                else
                {
                    throw new KeyNotFoundException(string.Format("不存在名称为{0}的字段", index));
                }
            }
        }
    }
}
