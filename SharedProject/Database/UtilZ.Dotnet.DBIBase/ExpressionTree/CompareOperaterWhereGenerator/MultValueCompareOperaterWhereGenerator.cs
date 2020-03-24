using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    internal class MultValueCompareOperaterWhereGenerator : CompareOperaterWhereGeneratorAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MultValueCompareOperaterWhereGenerator()
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

            para.SqlStringBuilder.Append($"{para.TableAliaName}.{para.FieldInfo.FieldName}");
            para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);

            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(para.Operater);
            para.SqlStringBuilder.Append(compareOperaterAttribute.OperaterFormat);

            para.SqlStringBuilder.Append("(");
            string parameterName;
            int lastIndex = valueList.Count - 1;
            object value;

            for (int i = 0; i < valueList.Count; i++)
            {
                parameterName = base.CreateSqlParameterName(para.FieldInfo.FieldName, para.GetParameterIndex());
                para.IncrementParameterIndex();
                para.SqlStringBuilder.Append(para.ParaSign);
                para.SqlStringBuilder.Append(parameterName);

                value = valueList[i];
                if (para.DBFiledValueConverter != null)
                {
                    value = para.DBFiledValueConverter.Convert(value);
                }

                para.ParameterNameValueDic.Add(parameterName, value);

                if (i < lastIndex)
                {
                    para.SqlStringBuilder.Append(",");
                }
            }

            para.SqlStringBuilder.Append(")");
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

            para.SqlStringBuilder.Append($"{para.TableAliaName}.{para.FieldInfo.FieldName}");
            para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);

            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(para.Operater);
            para.SqlStringBuilder.Append(compareOperaterAttribute.OperaterFormat);

            para.SqlStringBuilder.Append("(");
            int lastIndex = valueList.Count - 1;
            object value;
            string formatValue;

            for (int i = 0; i < valueList.Count; i++)
            {
                value = valueList[i];
                if (para.DBFiledValueConverter != null)
                {
                    value = para.DBFiledValueConverter.Convert(value);
                }

                formatValue = para.FieldValueFormator.FieldValueFormat(para.FieldInfo.FieldType, para.FieldInfo.DataType, value);
                para.SqlStringBuilder.Append(formatValue);

                if (i < lastIndex)
                {
                    para.SqlStringBuilder.Append(",");
                }
            }

            para.SqlStringBuilder.Append(")");
        }
    }
}
