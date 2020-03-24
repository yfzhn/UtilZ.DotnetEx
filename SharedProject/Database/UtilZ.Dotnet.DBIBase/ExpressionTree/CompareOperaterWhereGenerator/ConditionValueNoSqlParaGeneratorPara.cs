using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    /// <summary>
    /// 无SQL参数条件值生成接口参数
    /// </summary>
    internal class ConditionValueNoSqlParaGeneratorPara
    {
        /// <summary>
        /// 条件值存放StringBuilder
        /// </summary>
        public StringBuilder SqlStringBuilder { get; private set; }

        /// <summary>
        /// 字段值格式化对象
        /// </summary>
        public ISqlFieldValueFormator FieldValueFormator { get; private set; }

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
        /// 生成条件值
        /// </summary>
        /// <param name="sqlStringBuilder">sbSql</param>
        /// <param name="fieldValueFormator">字段值格式化对象</param>
        public ConditionValueNoSqlParaGeneratorPara(StringBuilder sqlStringBuilder, ISqlFieldValueFormator fieldValueFormator)
        {
            this.SqlStringBuilder = sqlStringBuilder;
            this.FieldValueFormator = fieldValueFormator;
        }
    }
}
