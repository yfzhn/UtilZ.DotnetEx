using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 属性值验证接口
    /// </summary>
    public interface IPropertyValueVerify
    {
        /// <summary>
        /// 属性值有效性验证结果通知事件
        /// </summary>
        event EventHandler<PropertyValueVerifyArgs> PropertyValueVerifyResultNotify;
    }
}
