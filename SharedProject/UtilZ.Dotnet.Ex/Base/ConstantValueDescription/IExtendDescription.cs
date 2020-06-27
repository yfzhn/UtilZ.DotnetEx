using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.ConstantValueDescription
{
    /// <summary>
    /// 扩展描述接口
    /// </summary>
    public interface IExtendDescription
    {
        /// <summary>
        /// 获取扩展值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的名称</returns>
        string GetName(object value, ValueDescriptionGroup group);

        /// <summary>
        /// 获取扩展值对应的描述
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="group">描述组</param>
        /// <returns>值对应的描述</returns>
        string GetDescription(object value, ValueDescriptionGroup group);
    }
}