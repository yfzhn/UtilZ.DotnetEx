using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Interface
{
    /// <summary>
    /// 数据库接口
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// 判断表是否存在[存在返回true,不存在返回false]
        /// </summary>
        /// <param name="tableName">表名[表名区分大小写的数据库:Oracle,SQLite]</param>
        /// <returns>存在返回true,不存在返回false</returns>
        bool ExistTable(string tableName);

        /// <summary>
        /// 判断表中是否存在字段[存在返回true,不存在返回false]
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>存在返回true,不存在返回false</returns>
        bool ExistField(string tableName, string fieldName);

        /// <summary>
        /// 获取表二进制字段名称集合
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>字段信息集合</returns>
        List<string> GetTableBinaryFieldInfo(string tableName);

        #region 获取表的字段信息
        /// <summary>
        /// 获取表的字段信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>字段信息集合</returns>
        List<DBFieldInfo> GetTableFieldInfoList(string tableName);

        /// <summary>
        /// 查询主键字段集合
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>主键列名集合</returns>
        List<string> QueryPriKeyField(string tableName);

        /// <summary>
        /// 获取字段的公共语言运行时类型字典集合
        /// </summary>        
        /// <param name="cols">列集合</param>
        /// <returns>字段的公共语言运行时类型字典集合</returns>
        Dictionary<string, DBFieldType> GetFieldDbClrFieldType(DataColumnCollection cols);
        #endregion

        #region 获取表信息
        /// <summary>
        /// 获取当前用户有权限的所有表集合
        /// </summary>
        /// <param name="getFieldInfo">是否获取字段信息[true:获取字段信息;false:不获取;默认不获取]</param>
        /// <returns>当前用户有权限的所有表集合</returns>
        List<DBTableInfo> GetTableInfoList(bool getFieldInfo = false);

        /// <summary>
        /// 获取表信息[表不存在返回null]
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="getFieldInfo">是否获取字段信息[true:获取字段信息;false:不获取;默认不获取]</param>
        /// <returns>表信息</returns>
        DBTableInfo GetTableInfoByName(string tableName, bool getFieldInfo = false);
        #endregion

        /// <summary>
        /// 获取表索引信息集合
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>表索引信息集合</returns>
        DBIndexInfoCollection GetTableIndexs(string tableName);

        /// <summary>
        /// 获取数据库版本信息
        /// </summary>
        /// <returns>数据库版本信息</returns>
        DataBaseVersionInfo GetDataBaseVersion();

        /// <summary>
        /// 获取数据库系统时间
        /// </summary>
        /// <returns>数据库系统时间</returns>
        DateTime GetDataBaseSysTime();

        /// <summary>
        /// 获取当前登录用户名
        /// </summary>
        /// <returns>当前登录用户名</returns>
        string GetLoginUserName();

        /// <summary>
        /// 获取数据库名称
        /// </summary>
        /// <returns>数据库名称</returns>
        string GetDatabaseName();

        /// <summary>
        /// 获取数据库属性信息
        /// </summary>
        /// <returns>数据库属性信息</returns>
        DatabasePropertyInfo GetDatabasePropertyInfo();
    }
}
