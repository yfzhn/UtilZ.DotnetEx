using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.ConstantValueDescription
{
    /// <summary>
    /// 默认扩展描述类
    /// </summary>
    public class DefaultExtendDescription : ExtendDescriptionAbs
    {
        private readonly Func<object, ValueDescriptionGroup, string> _getNameFunc = null;
        private readonly Func<object, ValueDescriptionGroup, string> _getDescriptionFunc = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultExtendDescription()
            : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="getNameFunc">获取名称回调(第一个参数为值,第二个为值描述组,第三个为返回值)</param>
        /// <param name="getDescriptionFunc">获取描述回调(第一个参数为值,第二个为值描述组,第三个为返回值)</param>
        public DefaultExtendDescription(Func<object, ValueDescriptionGroup, string> getNameFunc, Func<object, ValueDescriptionGroup, string> getDescriptionFunc)
            : base()
        {
            this._getNameFunc = getNameFunc;
            this._getDescriptionFunc = getDescriptionFunc;
        }



        /// <summary>
        /// 获取扩展值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的名称</returns>
        protected override string PrimitiveGetName(object value, ValueDescriptionGroup group)
        {
            string name;
            if (this._getNameFunc == null)
            {
                if (group.GroupDescriptionAttribute != null)
                {
                    name = $"未知的{group.GroupDescriptionAttribute.DisplayName}\"{value}\"";
                }
                else
                {
                    name = $"未知的\"{value}\"";
                }
            }
            else
            {
                name = this._getNameFunc(value, group);
            }

            return name;
        }


        /// <summary>
        /// 获取扩展值对应的描述
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的描述</returns>
        protected override string PrimitiveGetDescription(object value, ValueDescriptionGroup group)
        {
            string des;
            if (this._getDescriptionFunc == null)
            {
                if (group.GroupDescriptionAttribute != null)
                {
                    des = $"未知的{group.GroupDescriptionAttribute.DisplayName}\"{value}\"";
                }
                else
                {
                    des = $"未知的\"{value}\"";
                }
            }
            else
            {
                des = this._getDescriptionFunc(value, group);
            }

            return des;
        }
    }
}
