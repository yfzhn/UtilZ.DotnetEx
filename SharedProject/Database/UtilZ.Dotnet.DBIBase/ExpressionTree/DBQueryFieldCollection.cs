using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// 数据库查询字段集合
    /// </summary>
    [Serializable]
    public class DBQueryFieldCollection : ICollection<DBQueryField>
    {
        private readonly List<DBQueryField> _list = new List<DBQueryField>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBQueryFieldCollection()
        {

        }

        /// <summary>
        /// 获取集合中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return this._list.Count;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示集合是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 将某项添加到集合中
        /// </summary>
        /// <param name="item"> 要添加到 System.Collections.Generic.ICollection`1 的对象</param>
        public void Add(DBQueryField item)
        {
            this._list.Add(item);
        }

        /// <summary>
        /// 从 System.Collections.Generic.ICollection`1 中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要从 System.Collections.Generic.ICollection`1 中移除的对象</param>
        /// <returns>移除结果</returns>
        public bool Remove(DBQueryField item)
        {
            return this._list.Remove(item);
        }

        /// <summary>
        /// 从 System.Collections.Generic.ICollection`1 中移除所有项
        /// </summary>
        public void Clear()
        {
            this._list.Clear();
        }

        /// <summary>
        /// 确定 System.Collections.Generic.ICollection`1 是否包含特定值
        /// </summary>
        /// <param name="item">要在 System.Collections.Generic.ICollection`1 中定位的对象</param>
        /// <returns>如果在 System.Collections.Generic.ICollection`1 中找到 item，则为 true；否则为 false</returns>
        public bool Contains(DBQueryField item)
        {
            return this._list.Contains(item);
        }

        /// <summary>
        /// 从特定的 System.Array 索引开始，将 System.Collections.Generic.ICollection`1 的元素复制到一个 System.Array中
        /// </summary>
        /// <param name="array">作为从 System.Collections.Generic.ICollection`1 复制的元素的目标的一维 System.Array。System.Array必须具有从零开始的索引</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此索引处开始进行复制</param>
        public void CopyTo(DBQueryField[] array, int arrayIndex)
        {
            this._list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 返回循环访问 System.Collections.Generic.List`1 的枚举数
        /// </summary>
        /// <returns>循环访问 System.Collections.Generic.List`1 的枚举数</returns>
        public IEnumerator<DBQueryField> GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 转换为查询字段SQL语句
        /// </summary>
        /// <param name="tableAliaNameDic">表别名字典集合[key:列名;value:别名]</param>
        /// <returns>查询字段SQL语句</returns>
        public string ToQueryFieldSql(Dictionary<string, string> tableAliaNameDic)
        {
            StringBuilder sbSql = new StringBuilder();
            var queryFields = this;
            var lastDBQueryField = queryFields.Last();
            string aliaName;
            foreach (var queryField in queryFields)
            {
                if (tableAliaNameDic.ContainsKey(queryField.TableName))
                {
                    aliaName = tableAliaNameDic[queryField.TableName];
                    sbSql.Append(aliaName + ".");
                }
                else
                {
                    sbSql.Append(queryField.TableName + ".");
                }

                sbSql.Append(queryField.FiledName);
                if (!string.IsNullOrWhiteSpace(queryField.Alias))
                {
                    sbSql.Append(" AS ");
                    sbSql.Append(queryField.Alias);
                }

                if (queryField != lastDBQueryField)
                {
                    sbSql.Append(",");
                }
            }

            return sbSql.ToString();
        }
    }
}
