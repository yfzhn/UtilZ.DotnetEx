using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// 表达式节点
    /// </summary>
    [Serializable]
    public class ExpressionNode
    {
        /// <summary>
        /// 比较运算符
        /// </summary>
        public CompareOperater Operater { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 参数值列表[一个或多个值]
        /// </summary>
        public List<object> ValueList { get; set; } = new List<object>();

        /// <summary>
        /// 子节点集合
        /// </summary>
        public ExpressionNodeCollection Children { get; set; } = new ExpressionNodeCollection();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExpressionNode()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operater">比较运算符</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        protected ExpressionNode(CompareOperater operater, string tableName, string fieldName)
        {
            this.Operater = operater;
            this.TableName = tableName;
            this.FieldName = fieldName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operater">比较运算符</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">参数值</param>
        public ExpressionNode(CompareOperater operater, string tableName, string fieldName, object value)
            : this(operater, tableName, fieldName)
        {
            this.ValueList.Add(value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operater">比较运算符</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="valueList">参数值</param>
        public ExpressionNode(CompareOperater operater, string tableName, string fieldName, IEnumerable<object> valueList)
            : this(operater, tableName, fieldName)
        {
            this.ValueList.AddRange(valueList);
        }
    }
}
