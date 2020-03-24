using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 表信息
    /// </summary>
    [Serializable]
    public class DBTableInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DBTableInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableInfo">表信息</param>
        public DBTableInfo(DBTableInfo tableInfo)
        {
            if (tableInfo == null)
            {
                throw new ArgumentNullException(nameof(tableInfo));
            }

            this.Name = tableInfo.Name;
            this.Comments = tableInfo.Comments;
            this.DbFieldInfos = tableInfo.DbFieldInfos;
            this.PriKeyFieldInfos = tableInfo.PriKeyFieldInfos;
            this.Indexs = tableInfo.Indexs;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="comments">注释</param>
        /// <param name="dbFieldInfos">表字段集合</param>
        /// <param name="priKeyFieldInfos">主键字段集合</param>
        /// <param name="indexs">索引集合</param>
        public DBTableInfo(string tableName, string comments, DBFieldInfoCollection dbFieldInfos, DBFieldInfoCollection priKeyFieldInfos, DBIndexInfoCollection indexs)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("表名不能为空或null", nameof(tableName));
            }

            if (dbFieldInfos == null)
            {
                throw new ArgumentNullException("表字段集合不能为null", nameof(dbFieldInfos));
            }

            this.Name = tableName;
            this.Comments = comments;
            this.DbFieldInfos = dbFieldInfos;
            this.PriKeyFieldInfos = priKeyFieldInfos;
            this.Indexs = indexs;
        }

        /// <summary>
        /// 表名
        /// </summary>
        [DisplayName("表名")]
        public virtual string Name { get; protected set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public virtual string Comments { get; protected set; }

        /// <summary>
        /// 表字段集合
        /// </summary>
        [Browsable(false)]
        public virtual DBFieldInfoCollection DbFieldInfos { get; protected set; }

        /// <summary>
        /// 主键字段集合
        /// </summary>
        [Browsable(false)]
        public virtual DBFieldInfoCollection PriKeyFieldInfos { get; protected set; }

        /// <summary>
        /// 索引集合
        /// </summary>
        [Browsable(false)]
        public virtual DBIndexInfoCollection Indexs { get; protected set; }

        /// <summary>
        /// 获取字段数
        /// </summary>
        [DisplayName("字段数")]
        public virtual int Count
        {
            get { return this.DbFieldInfos.Count; }
        }

        /// <summary>
        /// 获取主键列数
        /// </summary>
        [DisplayName("主键字段数")]
        public virtual int PriKeyCount
        {
            get { return this.PriKeyFieldInfos == null ? 0 : this.PriKeyFieldInfos.Count; }
        }

        /// <summary>
        /// 重写GetHashCode
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
