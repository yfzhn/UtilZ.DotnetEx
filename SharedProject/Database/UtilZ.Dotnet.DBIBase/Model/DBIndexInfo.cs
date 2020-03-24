using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 索引信息
    /// </summary>
    [Serializable]
    public class DBIndexInfo
    {
        /// <summary>
        /// 所属表名
        /// </summary>
        [DisplayNameEx("表")]
        public virtual string OwerTableName { get; protected set; }

        /// <summary>
        /// 索引名称
        /// </summary>
        [DisplayNameEx("索引名称")]
        public virtual string IndexName { get; protected set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [DisplayNameEx("字段")]
        public virtual string FieldName { get; protected set; }

        /// <summary>
        /// 详情信息
        /// </summary>
        //[DisplayNameEx("详情")]
        [Browsable(false)]
        public virtual string Detail { get; protected set; }

        /// <summary>
        /// 反序列化构造函数
        /// </summary>
        public DBIndexInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owerTableName">所属表名</param>
        /// <param name="indexName">索引名称</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="detail">详情信息</param>
        public DBIndexInfo(string owerTableName, string indexName, string fieldName, string detail)
        {
            this.OwerTableName = owerTableName;
            this.IndexName = indexName;
            this.FieldName = fieldName;
            this.Detail = detail;
        }
    }
}
