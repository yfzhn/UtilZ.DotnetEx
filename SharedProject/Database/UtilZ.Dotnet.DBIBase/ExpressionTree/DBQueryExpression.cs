using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree
{
    /// <summary>
    /// 数据库查询表达式
    /// </summary>
    [Serializable]
    public class DBQueryExpression
    {
        /// <summary>
        /// 查询字段数列表
        /// </summary>
        public DBQueryFieldCollection QueryFields { get; set; } = new DBQueryFieldCollection();

        /// <summary>
        /// 查询条件表达式集合
        /// </summary>
        public ExpressionNodeCollection ConditionExpressionNodes { get; set; } = new ExpressionNodeCollection();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBQueryExpression()
        {

        }
    }
}
