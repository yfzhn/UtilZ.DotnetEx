using System;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Base
{
    /// <summary>
    /// 属性设置值有效性验证基类
    /// </summary>
    [Serializable]
    public abstract class PropertyValueVerifyBase : IPropertyValueVerify
    {
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
