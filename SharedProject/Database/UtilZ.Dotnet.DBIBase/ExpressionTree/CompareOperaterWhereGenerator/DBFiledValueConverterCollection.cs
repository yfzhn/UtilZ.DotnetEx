using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    /// <summary>
    /// 数据库字段值转换对象集合
    /// </summary>
    public class DBFiledValueConverterCollection : IEnumerable<KeyValuePair<string, IDBFiledValueConverter>>
    {
        /// <summary>
        /// 获取数据库字段值转换对象字典集合
        /// </summary>
        private readonly ConcurrentDictionary<string, IDBFiledValueConverter> _dbFiledValueConverterDic = new ConcurrentDictionary<string, IDBFiledValueConverter>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBFiledValueConverterCollection()
        {

        }

        /// <summary>
        /// 添加数据库字段值转换对象
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="dbFiledValueConverter">数据库字段值转换对象</param>
        public void AddConverter(string tableName, string fieldName, IDBFiledValueConverter dbFiledValueConverter)
        {
            this.PrimitiveAddConverter(tableName, fieldName, dbFiledValueConverter);
        }

        /// <summary>
        /// 添加数据库字段值转换对象
        /// </summary>
        /// <param name="fieldInfo">字段信息</param>
        /// <param name="dbFiledValueConverter">数据库字段值转换对象</param>
        public void AddConverter(DBFieldInfo fieldInfo, IDBFiledValueConverter dbFiledValueConverter)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            this.PrimitiveAddConverter(fieldInfo.OwerTableName, fieldInfo.FieldName, dbFiledValueConverter);
        }

        private void PrimitiveAddConverter(string tableName, string fieldName, IDBFiledValueConverter dbFiledValueConverter)
        {
            if (dbFiledValueConverter == null)
            {
                throw new ArgumentNullException(nameof(dbFiledValueConverter));
            }

            string key = this.CreateKey(tableName, fieldName);
            this._dbFiledValueConverterDic[key] = dbFiledValueConverter;
        }


        private string CreateKey(string tableName, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            return $"{tableName}_{fieldName}";
        }

        /// <summary>
        /// 获取数据库字段值转换对象
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>数据库字段值转换对象</returns>
        public IDBFiledValueConverter GetDBFiledValueConverter(string tableName, string fieldName)
        {
            return this.PrimitiveGetDBFiledValueConverter(tableName, fieldName);
        }

        /// <summary>
        /// 获取数据库字段值转换对象
        /// </summary>
        /// <param name="fieldInfo">字段信息</param>
        /// <returns>数据库字段值转换对象</returns>
        public IDBFiledValueConverter GetDBFiledValueConverter(DBFieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            return this.PrimitiveGetDBFiledValueConverter(fieldInfo.OwerTableName, fieldInfo.FieldName);
        }

        private IDBFiledValueConverter PrimitiveGetDBFiledValueConverter(string tableName, string fieldName)
        {
            string key = this.CreateKey(tableName, fieldName);
            IDBFiledValueConverter dbFiledValueConverter;
            _dbFiledValueConverterDic.TryGetValue(key, out dbFiledValueConverter);
            return dbFiledValueConverter;
        }

        /// <summary>
        /// 返回循环访问 System.Collections.Generic.List`1 的枚举数
        /// </summary>
        /// <returns>循环访问 System.Collections.Generic.List`1 的枚举数</returns>
        public IEnumerator<KeyValuePair<string, IDBFiledValueConverter>> GetEnumerator()
        {
            return this._dbFiledValueConverterDic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
