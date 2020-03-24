using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Core
{
    public abstract partial class DBAccessAbs
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
        public int Update(string tableName, string priKeyColName, object priKeyValue, string colName, object value)
        {
            var priKeyColValues = new Dictionary<string, object>();
            priKeyColValues.Add(priKeyColName, priKeyValue);

            var colValues = new Dictionary<string, object>();
            colValues.Add(colName, value);

            return this.PrimitiveUpdate(tableName, priKeyColValues, colValues);
        }

        /// <summary>
        /// 更新记录,返回受影响的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyColName">主键名</param>
        /// <param name="priKeyValue">主键值</param>
        /// <param name="colValues">列名值字典</param>
        /// <returns>返回受影响的行数</returns>
        public int Update(string tableName, string priKeyColName, object priKeyValue, Dictionary<string, object> colValues)
        {
            Dictionary<string, object> priKeyColValues = new Dictionary<string, object>();
            priKeyColValues.Add(priKeyColName, priKeyValue);
            return this.PrimitiveUpdate(tableName, priKeyColValues, colValues);
        }

        /// <summary>
        /// 更新记录,返回受影响的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyColValues">主键列名值映射字典</param>
        /// <param name="colValues">修改列名值字典</param>
        /// <returns>返回受影响的行数</returns>
        public int Update(string tableName, Dictionary<string, object> priKeyColValues, Dictionary<string, object> colValues)
        {
            return this.PrimitiveUpdate(tableName, priKeyColValues, colValues);
        }

        /// <summary>
        /// 更新记录,返回受影响的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyColValues">主键列名值映射字典</param>
        /// <param name="colValues">修改列名值字典</param>
        /// <returns>返回受影响的行数</returns>
        private int PrimitiveUpdate(string tableName, Dictionary<string, object> priKeyColValues, Dictionary<string, object> colValues)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            using (var conInfo = new DbConnectionInfo(this._dbid, DBVisitType.W))
            {
                using (IDbCommand cmd = this.CreateCommand(conInfo.DbConnection))
                {
                    cmd.CommandText = this.GenerateSqlUpdate(tableName, priKeyColValues, colValues, cmd);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 生成SQL更新语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="priKeyValueDic">主键列名值映射字典</param>
        /// <param name="valuesDic">列名值字典</param>
        /// <param name="cmd">IDbCommand</param>
        /// <returns>SQL更新语句</returns>
        private string GenerateSqlUpdate(string tableName, Dictionary<string, object> priKeyValueDic,
            Dictionary<string, object> valuesDic, IDbCommand cmd)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("UPDATE ");
            sbSql.Append(tableName);
            sbSql.Append(" SET ");
            string paraSign = this.ParaSign;

            foreach (var keyValue in valuesDic)
            {
                sbSql.Append(keyValue.Key);
                sbSql.Append("=");
                sbSql.Append(paraSign);
                sbSql.Append(keyValue.Key);
                sbSql.Append(",");

                cmd.AddParameter(keyValue.Key, keyValue.Value);
            }

            sbSql = sbSql.Remove(sbSql.Length - 1, 1);

            if (priKeyValueDic != null && priKeyValueDic.Count > 0)
            {
                sbSql.Append(" WHERE ");
                foreach (var priKeyValue in priKeyValueDic)
                {
                    sbSql.Append(priKeyValue.Key);
                    sbSql.Append("=");
                    sbSql.Append(paraSign);
                    sbSql.Append(priKeyValue.Key);
                    sbSql.Append(",");

                    cmd.AddParameter(priKeyValue.Key, priKeyValue.Value);
                }

                sbSql = sbSql.Remove(sbSql.Length - 1, 1);
            }

            return sbSql.ToString();
        }
    }
}
