using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator
{
    internal class CompareOperaterHelper
    {
        private readonly static ReadOnlyDictionary<CompareOperater, CompareOperaterAttribute> _map;
        static CompareOperaterHelper()
        {
            var fields = typeof(CompareOperater).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type attriType = typeof(CompareOperaterAttribute);
            var map = fields.ToDictionary(
            k =>
            {
                return (CompareOperater)k.GetValue(null);
            },
            v =>
            {
                return ((CompareOperaterAttribute)v.GetCustomAttributes(attriType, true)[0]);
            });
            _map = new ReadOnlyDictionary<CompareOperater, CompareOperaterAttribute>(map);
        }

        /// <summary>
        /// 根据比较运算符获取比较运算符特性
        /// </summary>
        /// <param name="operater">参数名称</param>
        /// <returns>数据库字段特性</returns>
        public static CompareOperaterAttribute GetCompareOperaterAttributeByCompareOperater(CompareOperater operater)
        {
            if (_map.ContainsKey(operater))
            {
                return _map[operater];
            }
            else
            {
                throw new ArgumentException($"无效的比较运算符[{operater.ToString()}]");
            }
        }
    }
}
