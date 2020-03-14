using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 模型基类
    /// </summary>
    [Serializable]
    public abstract class NotifyPropertyChangedAbs : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        /// <summary>
        /// PropertyChanged
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性值改变事件
        /// </summary>
        /// <param name="propertyName">属性名</param>
#if NET4_0
        protected virtual void OnRaisePropertyChanged(string propertyName )
#else
        protected virtual void OnRaisePropertyChanged([CallerMemberName] string propertyName = null)
#endif
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 触发属性值改变事件
        /// </summary>
        /// <param name="e">改变属性信息</param>
        protected virtual void OnRaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyPropertyChangedAbs()
        {

        }
    }
}
