using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 属性验证模型基类
    /// </summary>
    [Serializable]
    public abstract class BasePropertyValueVerifyModel : NotifyPropertyChangedAbs, IPropertyValueVerify
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePropertyValueVerifyModel()
        {

        }

        #region IPropertyValueVerify接口
        /// <summary>
        /// 属性值有效性验证结果通知事件
        /// </summary>
        public event EventHandler<PropertyValueVerifyArgs> PropertyValueVerifyResultNotify;

        /// <summary>
        /// 调用属性设置值有效性验证结果通知事件
        /// </summary>
        /// <param name="isValid">最新值的有效性[true:有效,false:无效]</param>
        /// <param name="errorMesage">当最新值无效时的错误提示消息</param>
        protected void OnRaisePropertyValueVerifyResultNotify(bool isValid, string errorMesage)
        {
            EventHandler<PropertyValueVerifyArgs> handler = PropertyValueVerifyResultNotify;
            if (handler != null)
            {
                handler(this, new PropertyValueVerifyArgs(isValid, errorMesage));
            }
        }
        #endregion
    }
}
