using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    internal class NotInRangeCompareOperaterWhereGenerator : CompareOperaterWhereGeneratorAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotInRangeCompareOperaterWhereGenerator()
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
            if (valueList == null || valueList.Count != 2)
            {
                throw new ArgumentException($"字段[{para.FieldInfo.FieldName}]条件值不符合预期,期望两个值");
            }

            para.SqlStringBuilder.Append($"{para.TableAliaName}.{para.FieldInfo.FieldName}");
            para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);

            string parameterName1 = this.CreateSqlParameterName(para.FieldInfo.FieldName, para.GetParameterIndex());
            para.IncrementParameterIndex();
            string parameterName2 = this.CreateSqlParameterName(para.FieldInfo.FieldName, para.GetParameterIndex());
            para.IncrementParameterIndex();

            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(para.Operater);
            para.SqlStringBuilder.Append(string.Format(compareOperaterAttribute.OperaterFormat, para.ParaSign + parameterName1, para.ParaSign + parameterName2));

            object value1 = valueList[0];
            object value2 = valueList[1];
            if (para.DBFiledValueConverter != null)
            {
                value1 = para.DBFiledValueConverter.Convert(value1);
                value2 = para.DBFiledValueConverter.Convert(value2);
            }

            para.ParameterNameValueDic.Add(parameterName1, value1);
            para.ParameterNameValueDic.Add(parameterName2, value2);
        }

        /// <summary>
        /// 生成条件值,无参数
        /// </summary>
        /// <param name="para">无SQL参数条件值生成接口参数</param>
        protected override void PrimitiveGenerateNoPara(ConditionValueNoSqlParaGeneratorPara para)
        {
            var valueList = para.ValueList;
            if (valueList == null || valueList.Count != 2)
            {
                throw new ArgumentException($"字段[{para.FieldInfo.FieldName}]条件值不符合预期,期望两个值");
            }

            para.SqlStringBuilder.Append($"{para.TableAliaName}.{para.FieldInfo.FieldName}");
            para.SqlStringBuilder.Append(DBConstant.BLACK_SPACE);

            object value1 = valueList[0];
            object value2 = valueList[1];
            if (para.DBFiledValueConverter != null)
            {
                value1 = para.DBFiledValueConverter.Convert(value1);
                value2 = para.DBFiledValueConverter.Convert(value2);
            }

            string formatValue1 = para.FieldValueFormator.FieldValueFormat(para.FieldInfo.FieldType, para.FieldInfo.DataType, value1);
            string formatValue2 = para.FieldValueFormator.FieldValueFormat(para.FieldInfo.FieldType, para.FieldInfo.DataType, value2);

            CompareOperaterAttribute compareOperaterAttribute = CompareOperaterHelper.GetCompareOperaterAttributeByCompareOperater(para.Operater);
            para.SqlStringBuilder.Append(string.Format(compareOperaterAttribute.OperaterFormat, formatValue1, formatValue2));
        }
    }
}
