using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 排序信息
    /// </summary>
    [Serializable]
    public class DBOrderInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="orderFlag">排序标识[true:升序;false:降序]</param>
        public DBOrderInfo(string fieldName, bool orderFlag)
        {
            this.FieldName = fieldName;
            this.OrderFlag = orderFlag;
        }

        /// <summary>
        /// 反序列化构造函数-不得单独调用
        /// </summary>
        public DBOrderInfo()
        {

        }

        /// <summary>
        /// 字段名称
        /// </summary>
        [DisplayNameExAttribute("字段名称")]
        public virtual string FieldName { get; protected set; }

        /// <summary>
        /// 排序标识[true:升序;false:降序]
        /// </summary>
        [DisplayNameExAttribute("排序标识")]
        public virtual bool OrderFlag { get; protected set; }
    }
}
