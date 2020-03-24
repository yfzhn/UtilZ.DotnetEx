using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    /// <summary>
    /// 条件值生成接口参数
    /// </summary>
    internal class ConditionValueGeneratorPara
    {
        /// <summary>
        /// 条件值存放StringBuilder
        /// </summary>
        public StringBuilder SqlStringBuilder { get; private set; }

        /// <summary>
        /// 数据库参数字符
        /// </summary>
        public string ParaSign { get; private set; }

        /// <summary>
        /// 参数名-值字典集合
        /// </summary>
        public Dictionary<string, object> ParameterNameValueDic { get; private set; }

        /// <summary>
        /// 值列表
        /// </summary>
        public List<object> ValueList { get; set; }

        /// <summary>
        /// 比较运算符
        /// </summary>
        public CompareOperater Operater { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public DBFieldInfo FieldInfo { get; set; }

        /// <summary>
        /// 表别名
        /// </summary>
        public string TableAliaName { get; set; }

        /// <summary>
        /// 数据库字段值转换对象
        /// </summary>
        public IDBFiledValueConverter DBFiledValueConverter { get; set; }

        /// <summary>
        /// 参数索引
        /// </summary>
        private int _parameterIndex;

        public int GetParameterIndex()
        {
            return this._parameterIndex;
        }

        public void IncrementParameterIndex()
        {
            this._parameterIndex++;
        }

        /// <summary>
        /// 生成条件值
        /// </summary>
        /// <param name="sqlStringBuilder">sbSql</param>
        /// <param name="paraSign">数据库参数字符</param>
        /// <param name="parameterIndex">参数索引</param>
        /// <param name="parameterNameValueDic">参数名名称及对应的值字典集合[key:参数名称,含参数符号;value:参数值]</param>
        public ConditionValueGeneratorPara(StringBuilder sqlStringBuilder, string paraSign, 
            int parameterIndex, Dictionary<string, object> parameterNameValueDic)
        {
            this.SqlStringBuilder = sqlStringBuilder;
            this.ParaSign = paraSign;
            this.ParameterNameValueDic = parameterNameValueDic;
            this._parameterIndex = parameterIndex;
        }
    }
}
