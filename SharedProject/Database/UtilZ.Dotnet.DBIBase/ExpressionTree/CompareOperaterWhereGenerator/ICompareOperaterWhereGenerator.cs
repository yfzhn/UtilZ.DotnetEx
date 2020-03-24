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
    /// 单个比较运算符条件语句生成器接口
    /// </summary>
    internal interface ICompareOperaterWhereGenerator
    {
        /// <summary>
        /// 生成条件值
        /// </summary>
        /// <param name="conditionValueGeneratorPara">条件值生成接口参数</param>
        void Generate(ConditionValueGeneratorPara conditionValueGeneratorPara);

        /// <summary>
        /// 生成条件值,无参数
        /// </summary>
        /// <param name="conditionValueNoSqlParaGeneratorPara">无SQL参数条件值生成接口参数</param>
        void GenerateNoPara(ConditionValueNoSqlParaGeneratorPara conditionValueNoSqlParaGeneratorPara);
    }
}
