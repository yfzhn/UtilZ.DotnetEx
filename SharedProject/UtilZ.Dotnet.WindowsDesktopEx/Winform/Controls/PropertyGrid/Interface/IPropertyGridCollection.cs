using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface
{
    /// <summary>
    /// 表格集合显示集合接口
    /// </summary>
    public interface IPropertyGridCollection
    {
        /// <summary>
        /// 获取集合显示信息
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        string GetCollectionDisplayInfo(string propertyName);

        /// <summary>
        /// 获取此集合编辑器可以包含的数据类型
        /// </summary>
        /// <returns>此集合可以包含的数据类型的数组</returns>
        Type[] GetCreateNewItemTypes();

        /// <summary>
        /// 获取允许的最大实例数[小于1无限制]
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>许的最大实例数</returns>
        int GetObjectsInstanceMaxCount(string propertyName);

        /// <summary>
        /// 获取集合是否允许删除项
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">要移除的值</param>
        /// <returns>true 如果允许此值从集合中删除;否则为 false。 默认实现始终返回 true</returns>
        bool GetCanRemoveInstance(string propertyName, object value);

        /// <summary>
        /// 集合编辑完成通知
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        void CollectionEditCompletedNotify(string propertyName);
    }

    /// <summary>
    /// 表格集合显示项接口
    /// </summary>
    public interface IPropertyGridCollectionItem
    {
        /// <summary>
        /// 获取集合项显示信息
        /// </summary>
        /// <returns>集合项显示信息</returns>
        string GetItemInfo();
    }
}
