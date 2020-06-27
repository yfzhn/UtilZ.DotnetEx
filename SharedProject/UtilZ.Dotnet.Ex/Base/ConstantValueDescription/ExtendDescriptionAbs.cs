using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.ConstantValueDescription
{
    /// <summary>
    /// 扩展描述基类
    /// </summary>
    public abstract class ExtendDescriptionAbs : IExtendDescription
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExtendDescriptionAbs()
        {

        }


        /// <summary>
        /// 获取扩展值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的名称</returns>
        public string GetName(object value, ValueDescriptionGroup group)
        {
            return this.PrimitiveGetName(value, group);
        }

        /// <summary>
        /// 获取扩展值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的名称</returns>
        protected abstract string PrimitiveGetName(object value, ValueDescriptionGroup group);


        /// <summary>
        /// 获取扩展值对应的描述
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的描述</returns>
        public string GetDescription(object value, ValueDescriptionGroup group)
        {
            return this.PrimitiveGetDescription(value, group);
        }

        /// <summary>
        /// 获取扩展值对应的描述
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的描述</returns>
        protected abstract string PrimitiveGetDescription(object value, ValueDescriptionGroup group);
    }
}
