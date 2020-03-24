using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    internal class NullCompareOperaterWhereGenerator : CompareOperaterWhereGeneratorAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NullCompareOperaterWhereGenerator()
            : base()
        {

        }

        private void GenerateNull(StringBuilder sbSql, DBFieldInfo dbFieldInfo, string tableAliaName, CompareOperater operater)
        {
            sbSql.Append($"{tableAliaName}.{dbFieldInfo.FieldName}");
            sbSql.Append(DBConstant.BLACK_SPACE);

            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(operater);
            sbSql.Append(compareOperaterAttribute.OperaterFormat);
        }

        /// <summary>
        /// 生成条件值
        /// </summary>
        /// <param name="para">条件值生成接口参数</param>
        protected override void PrimitiveGenerate(ConditionValueGeneratorPara para)
        {
            this.GenerateNull(para.SqlStringBuilder, para.FieldInfo, para.TableAliaName, para.Operater);
        }

        /// <summary>
        /// 生成条件值,无参数
        /// </summary>
        /// <param name="para">无SQL参数条件值生成接口参数</param>
        protected override void PrimitiveGenerateNoPara(ConditionValueNoSqlParaGeneratorPara para)
        {
            this.GenerateNull(para.SqlStringBuilder, para.FieldInfo, para.TableAliaName, para.Operater);
        }
    }
}
