using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 属性值验证结果通知事件参数
    /// </summary>
    [Serializable]
    public class PropertyValueVerifyArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isValid">最新值的有效性[true:有效,false:无效]</param>
        /// <param name="errorMesage">当最新值无效时的错误提示消息</param>
        public PropertyValueVerifyArgs(bool isValid, string errorMesage)
        {
            this.IsValid = isValid;
            this.ErrorMesage = errorMesage;
        }

        /// <summary>
        /// 获取最新值的有效性[true:有效,false:无效]
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// 获取当最新值无效时的错误提示消息
        /// </summary>
        public string ErrorMesage { get;private set; }
    }
}
