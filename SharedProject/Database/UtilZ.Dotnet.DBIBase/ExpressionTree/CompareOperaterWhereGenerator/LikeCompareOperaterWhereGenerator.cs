using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    internal class LikeCompareOperaterWhereGenerator : CompareOperaterWhereGeneratorAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LikeCompareOperaterWhereGenerator()
            : base()
        {

        }

        /// <summary>
        /// 生成条件值
        /// </summary>
        /// <param name="para">条件值生成接口参数</param>
        protected override void PrimitiveGenerate(ConditionValueGeneratorPara para)
        {
            var valueList = para.ValueList;
            if (valueList == null || valueList.Count == 0)
            {
                return;
            }

            if (valueList.Count != 1)
            {
                throw new ArgumentException($"字段[{para.FieldInfo.FieldName}]条件值不符合预期,期望一个值");
            }

            para.SqlStringBuilder.Append($"{para.TableAliaName}.{para.FieldInfo.FieldName}");
            para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);

            string parameterName = this.CreateSqlParameterName(para.FieldInfo.FieldName, para.GetParameterIndex());
            para.IncrementParameterIndex();
            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(para.Operater);
            para.SqlStringBuilder.Append(string.Format(compareOperaterAttribute.OperaterFormat, para.ParaSign + parameterName));

            object value = valueList[0];
            if (para.DBFiledValueConverter != null)
            {
                value = para.DBFiledValueConverter.Convert(value);
            }

            para.ParameterNameValueDic.Add(parameterName, value);
        }

        /// <summary>
        /// 生成条件值,无参数
        /// </summary>
        /// <param name="para">无SQL参数条件值生成接口参数</param>
        protected override void PrimitiveGenerateNoPara(ConditionValueNoSqlParaGeneratorPara para)
        {
            var valueList = para.ValueList;
            if (valueList == null || valueList.Count == 0)
            {
                return;
            }

            if (valueList.Count != 1)
            {
                throw new ArgumentException($"字段[{para.FieldInfo.FieldName}]条件值不符合预期,期望一个值");
            }

            object value = valueList[0];
            if (para.DBFiledValueConverter != null)
            {
                value = para.DBFiledValueConverter.Convert(value);
            }

            para.SqlStringBuilder.Append($"{para.TableAliaName}.{para.FieldInfo.FieldName}");
            para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);

            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(para.Operater);
            para.SqlStringBuilder.Append(string.Format(compareOperaterAttribute.OperaterFormat, value));
        }
    }
}
