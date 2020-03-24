using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// 数据库查询字段
    /// </summary>
    [Serializable]
    public class DBQueryField
    {
        /// <summary>
        /// 表名
        /// </summary>
        [DisplayNameEx("表名")]
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [DisplayNameEx("字段")]
        public string FiledName { get; set; }

        /// <summary>
        /// 返回结果别名,为空时不使用别名
        /// </summary>
        [DisplayNameEx("返回结果别名")]
        public string Alias { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBQueryField()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="filedName">字段名</param>
        /// <param name="alias">返回结果别名</param>
        public DBQueryField(string tableName, string filedName, string alias = null)
        {
            this.TableName = tableName;
            this.FiledName = filedName;
            this.Alias = alias;
        }
    }
}
