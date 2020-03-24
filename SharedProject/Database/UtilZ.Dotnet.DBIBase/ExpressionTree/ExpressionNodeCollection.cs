using System;
using System.Collections;
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
    /// 表达式节点集合
    /// </summary>
    [Serializable]
    public class ExpressionNodeCollection : ICollection<ExpressionNode>
    {
        private readonly List<ExpressionNode> _nodeList = new List<ExpressionNode>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExpressionNodeCollection()
        {

        }

        /// <summary>
        /// 逻辑运算符
        /// </summary>
        public LogicOperaters LogicOperaters { get; set; } = LogicOperaters.AND;

        /// <summary>
        /// 获取集合中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return this._nodeList.Count;
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
        public void Add(ExpressionNode item)
        {
            this._nodeList.Add(item);
        }

        /// <summary>
        /// 从 System.Collections.Generic.ICollection`1 中移除所有项
        /// </summary>
        public void Clear()
        {
            this._nodeList.Clear();
        }

        /// <summary>
        /// 确定 System.Collections.Generic.ICollection`1 是否包含特定值
        /// </summary>
        /// <param name="item">要在 System.Collections.Generic.ICollection`1 中定位的对象</param>
        /// <returns>如果在 System.Collections.Generic.ICollection`1 中找到 item，则为 true；否则为 false</returns>
        public bool Contains(ExpressionNode item)
        {
            return this._nodeList.Contains(item);
        }

        /// <summary>
        /// 从特定的 System.Array 索引开始，将 System.Collections.Generic.ICollection`1 的元素复制到一个 System.Array中
        /// </summary>
        /// <param name="array">作为从 System.Collections.Generic.ICollection`1 复制的元素的目标的一维 System.Array。System.Array必须具有从零开始的索引</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此索引处开始进行复制</param>
        public void CopyTo(ExpressionNode[] array, int arrayIndex)
        {
            this._nodeList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 从 System.Collections.Generic.ICollection`1 中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要从 System.Collections.Generic.ICollection`1 中移除的对象</param>
        /// <returns>移除结果</returns>
        public bool Remove(ExpressionNode item)
        {
            return this._nodeList.Remove(item);
        }

        /// <summary>
        /// 返回循环访问 System.Collections.Generic.List`1 的枚举数
        /// </summary>
        /// <returns>循环访问 System.Collections.Generic.List`1 的枚举数</returns>
        public IEnumerator<ExpressionNode> GetEnumerator()
        {
            return this._nodeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 转换为带参数where语句
        /// </summary>
        /// <param name="tableFieldInfoDic">表字段字典集合[key:表名;value:[key:字段名;value:DBFieldInfo]]</param>
        /// <param name="tableAliaNameDic">表别名字典集合[key:列名;value:别名]</param>
        /// <param name="filedValueConverterCollection">数据库字段值转换对象集合</param>
        /// <param name="paraSign">SQL参数符号字符串</param>
        /// <param name="parameterIndex">参数索引</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        /// <returns>where语句</returns>
        public string ToWhere(Dictionary<string, Dictionary<string, DBFieldInfo>> tableFieldInfoDic,
            Dictionary<string, string> tableAliaNameDic, DBFiledValueConverterCollection filedValueConverterCollection,
            string paraSign, ref int parameterIndex, out Dictionary<string, object> parameterNameValueDic)
        {
            var sbSql = new StringBuilder();
            parameterNameValueDic = new Dictionary<string, object>();
            var para = new ConditionValueGeneratorPara(sbSql, paraSign, parameterIndex, parameterNameValueDic);
            this.CreateQueryFieldWhereSql(para, tableAliaNameDic, tableFieldInfoDic, this, filedValueConverterCollection);
            parameterIndex = para.GetParameterIndex();
            return sbSql.ToString();
        }

        private void CreateQueryFieldWhereSql(ConditionValueGeneratorPara para, Dictionary<string, string> tableAliaNameDic,
            Dictionary<string, Dictionary<string, DBFieldInfo>> tableFieldInfoDic, ExpressionNodeCollection conditionExpressionNodes,
            DBFiledValueConverterCollection dbFiledValueConverterCollection)
        {
            if (conditionExpressionNodes == null || conditionExpressionNodes.Count == 0)
            {
                return;
            }

            ExpressionNodeCollection childrenConditionExpressionNodes;
            DBFieldInfo dbFieldInfo;
            CompareOperaterAttribute compareOperaterAttribute;
            int lastIndex = conditionExpressionNodes.Count - 1;

            for (int i = 0; i < conditionExpressionNodes.Count; i++)
            {
                var expressionNode = conditionExpressionNodes.ElementAt(i);
                childrenConditionExpressionNodes = expressionNode.Children;
                if (childrenConditionExpressionNodes == null || childrenConditionExpressionNodes.Count == 0)
                {
                    //无子节点
                    dbFieldInfo = tableFieldInfoDic[expressionNode.TableName][expressionNode.FieldName];
                    para.DBFiledValueConverter = dbFiledValueConverterCollection.GetDBFiledValueConverter(dbFieldInfo);
                    para.FieldInfo = dbFieldInfo;
                    if (tableAliaNameDic.ContainsKey(expressionNode.TableName))
                    {
                        para.TableAliaName = tableAliaNameDic[expressionNode.TableName];
                    }
                    else
                    {
                        para.TableAliaName = expressionNode.TableName;
                    }

                    para.ValueList = expressionNode.ValueList;
                    para.Operater = expressionNode.Operater;

                    compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(expressionNode.Operater);
                    compareOperaterAttribute.ConditionValueGenerator.Generate(para);

                    if (i < lastIndex)
                    {
                        para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);
                        para.SqlStringBuilder.Append(conditionExpressionNodes.LogicOperaters.ToString());
                        para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);
                    }
                }
                else
                {
                    //有子节点     
                    para.SqlStringBuilder.Append('(');
                    this.CreateQueryFieldWhereSql(para, tableAliaNameDic, tableFieldInfoDic,
                        childrenConditionExpressionNodes, dbFiledValueConverterCollection);
                    para.SqlStringBuilder.Append(')');
                }
            }
        }

        /// <summary>
        /// 转换为无参where语句
        /// </summary>
        /// <param name="tableFieldInfoDic">表字段字典集合[key:表名;value:[key:字段名;value:DBFieldInfo]]</param>
        /// <param name="tableAliaNameDic">表别名字典集合[key:列名;value:别名]</param>
        /// <param name="filedValueConverterCollection">数据库字段值转换对象集合</param>
        /// <param name="fieldValueFormator">字段值格式化对象</param>
        /// <returns>where语句</returns>
        public string ToWhereNoParameter(Dictionary<string, Dictionary<string, DBFieldInfo>> tableFieldInfoDic, Dictionary<string, string> tableAliaNameDic,
            DBFiledValueConverterCollection filedValueConverterCollection, ISqlFieldValueFormator fieldValueFormator)
        {
            var sbSql = new StringBuilder();
            var para = new ConditionValueNoSqlParaGeneratorPara(sbSql, fieldValueFormator);
            this.CreateQueryFieldWhereSqlNoParameter(para, tableAliaNameDic,
                tableFieldInfoDic, this, filedValueConverterCollection);
            return sbSql.ToString();
        }

        private void CreateQueryFieldWhereSqlNoParameter(ConditionValueNoSqlParaGeneratorPara para,
           Dictionary<string, string> tableAliaNameDic, Dictionary<string, Dictionary<string, DBFieldInfo>> tableFieldInfoDic,
           ExpressionNodeCollection conditionExpressionNodes, DBFiledValueConverterCollection dbFiledValueConverterCollection)
        {
            if (conditionExpressionNodes == null || conditionExpressionNodes.Count == 0)
            {
                return;
            }

            ExpressionNodeCollection childrenConditionExpressionNodes;
            DBFieldInfo dbFieldInfo;
            CompareOperaterAttribute compareOperaterAttribute;
            int lastIndex = conditionExpressionNodes.Count - 1;

            for (int i = 0; i < conditionExpressionNodes.Count; i++)
            {
                var expressionNode = conditionExpressionNodes.ElementAt(i);
                childrenConditionExpressionNodes = expressionNode.Children;
                if (childrenConditionExpressionNodes == null || childrenConditionExpressionNodes.Count == 0)
                {
                    //无子节点
                    dbFieldInfo = tableFieldInfoDic[expressionNode.TableName][expressionNode.FieldName];
                    para.FieldInfo = dbFieldInfo;
                    para.DBFiledValueConverter = dbFiledValueConverterCollection.GetDBFiledValueConverter(dbFieldInfo);
                    if (tableAliaNameDic.ContainsKey(expressionNode.TableName))
                    {
                        para.TableAliaName = tableAliaNameDic[expressionNode.TableName];
                    }
                    else
                    {
                        para.TableAliaName = expressionNode.TableName;
                    }

                    para.ValueList = expressionNode.ValueList;
                    para.Operater = expressionNode.Operater;
                    compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(expressionNode.Operater);
                    compareOperaterAttribute.ConditionValueGenerator.GenerateNoPara(para);

                    if (i < lastIndex)
                    {
                        para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);
                        para.SqlStringBuilder.Append(conditionExpressionNodes.LogicOperaters.ToString());
                        para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);
                    }
                }
                else
                {
                    //有子节点     
                    para.SqlStringBuilder.Append('(');
                    this.CreateQueryFieldWhereSqlNoParameter(para, tableAliaNameDic,
                        tableFieldInfoDic, childrenConditionExpressionNodes, dbFiledValueConverterCollection);
                    para.SqlStringBuilder.Append(')');
                }
            }
        }
    }
}
