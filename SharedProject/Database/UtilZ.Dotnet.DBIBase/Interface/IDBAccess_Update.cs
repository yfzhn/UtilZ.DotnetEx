using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.Interface
{
    public partial interface IDBAccess
    {
        /// <summary>
        /// 更新记录,返回受影响的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyColName">主键名</param>
        /// <param name="priKeyValue">主键值</param>
        /// <param name="colName">修改列名</param>
        /// <param name="value">修改列值</param>
        /// <returns>返回受影响的行数</returns>
        int Update(string tableName, string priKeyColName, object priKeyValue, string colName, object value);

        /// <summary>
        /// 更新记录,返回受影响的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyColName">主键名</param>
        /// <param name="priKeyValue">主键值</param>
        /// <param name="colValues">列名值字典</param>
        /// <returns>返回受影响的行数</returns>
        int Update(string tableName, string priKeyColName, object priKeyValue, Dictionary<string, object> colValues);

        /// <summary>
        /// 更新记录,返回受影响的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyColValues">主键列名值映射字典</param>
        /// <param name="colValues">修改列名值字典</param>
        /// <returns>返回受影响的行数</returns>
        int Update(string tableName, Dictionary<string, object> priKeyColValues, Dictionary<string, object> colValues);
    }
}
