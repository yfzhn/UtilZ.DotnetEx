using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Interface
{
    /// <summary>
    /// 原始数据展示接口
    /// </summary>
    public interface IOriginalDataShow
    {
        /// <summary>
        /// 默认原始数据显示类型
        /// </summary>
        OriginalDataShowType DefaultOriginalDataShowType { get; set; }

        /// <summary>
        /// 获取或设置排除或忽略要展示的原始数据项集合
        /// </summary>
        OriginalDataShowTypeCollection IgnoreDataShowTypes { get; set; }

        /// <summary>
        /// 获取当前数据加载类型[true:二进制;false:文件]
        /// </summary>
        bool DataLoadType { get; }

        /// <summary>
        /// 获取或设置数据文件路径
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// 获取或设置数据
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// 清除当前显示
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 原始数据显示类型
    /// </summary>
    public enum OriginalDataShowType
    {
        /// <summary>
        /// ASC编码
        /// </summary>
        [DisplayNameExAttribute("ASC编码")]
        ASC,

        /// <summary>
        /// Unicode编码
        /// </summary>
        [DisplayNameExAttribute("Unicode编码")]
        Unicode,

        /// <summary>
        /// 二进制
        /// </summary>
        [DisplayNameExAttribute("二进制")]
        Bin,

        /// <summary>
        /// 十六进制
        /// </summary>
        [DisplayNameExAttribute("十六进制")]
        Hex
    }

    /// <summary>
    /// 原始数据显示类型集合
    /// </summary>
    public class OriginalDataShowTypeCollection : IEnumerable<OriginalDataShowType>
    {
        /// <summary>
        /// 数据项集合
        /// </summary>
        private readonly List<OriginalDataShowType> _items = new List<OriginalDataShowType>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items">数据项集合</param>
        public OriginalDataShowTypeCollection(IEnumerable<OriginalDataShowType> items)
        {
            this._items.AddRange(items);
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举数
        /// </summary>
        /// <returns>一个循环访问集合的枚举数</returns>
        public IEnumerator<OriginalDataShowType> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举数
        /// </summary>
        /// <returns>一个循环访问集合的枚举数</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
