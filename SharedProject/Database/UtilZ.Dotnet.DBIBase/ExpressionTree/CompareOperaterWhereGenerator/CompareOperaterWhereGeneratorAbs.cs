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
    /// 单个比较运算符条件语句生成器基类
    /// </summary>
    internal abstract class CompareOperaterWhereGeneratorAbs : ICompareOperaterWhereGenerator
    {
        /// <summary>
        /// 基类构造函数
        /// </summary>
        public CompareOperaterWhereGeneratorAbs()
        {

        }

        /// <summary>
        /// 生成条件值
        /// </summary>
        /// <param name="para">条件值生成接口参数</param>
        public void Generate(ConditionValueGeneratorPara para)
        {
            this.PrimitiveGenerate(para);
        }

        /// <summary>
        /// 生成条件值
        /// </summary>
        /// <param name="para">条件值生成接口参数</param>
        protected abstract void PrimitiveGenerate(ConditionValueGeneratorPara para);

        /// <summary>
        /// 创建SQL参数名称
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="parameterIndex">参数索引</param>
        /// <returns>SQL参数名称</returns>
        protected string CreateSqlParameterName(string fieldName, int parameterIndex)
        {
            return $"{fieldName}{parameterIndex}";
        }

        /// <summary>
        /// 生成条件值,无参数
        /// </summary>
        /// <param name="para">无SQL参数条件值生成接口参数</param>
        public void GenerateNoPara(ConditionValueNoSqlParaGeneratorPara para)
        {
            this.PrimitiveGenerateNoPara(para);
        }

        /// <summary>
        /// 生成条件值,无参数
        /// </summary>
        /// <param name="para">无SQL参数条件值生成接口参数</param>
        protected abstract void PrimitiveGenerateNoPara(ConditionValueNoSqlParaGeneratorPara para);
    }
}
