using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 数据库表字段信息
    /// </summary>
    [Serializable]
    public class DBFieldInfo
    {
        /// <summary>
        /// 所属表名
        /// </summary>
        [DisplayNameEx("表")]
        public virtual string OwerTableName { get; protected set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [DisplayNameEx("字段")]
        public virtual string FieldName { get; protected set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayNameEx("标题")]
        public virtual string Caption { get; protected set; }

        /// <summary>
        /// 数据库类型[字段数据库中对应的数据类型名称]
        /// </summary>
        [DisplayNameEx("数据库类型")]
        public virtual string DbTypeName { get; protected set; }

        /// <summary>
        /// 数据库字段对应于.net平台运行时数据类型
        /// </summary>
        [DisplayNameEx("数据类型")]
        public virtual Type DataType { get; protected set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        [DisplayNameEx("字段类型")]
        public virtual DBFieldType FieldType { get; protected set; }

        /// <summary>
        /// 是否允许为空值
        /// </summary>
        [DisplayNameEx("允许为空")]
        public virtual bool AllowNull { get; protected set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [DisplayNameEx("默认值")]
        public virtual object DefaultValue { get; protected set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DisplayNameEx("描述")]
        public virtual string Description { get; protected set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayNameEx("备注")]
        public virtual string Comments { get; protected set; }

        /// <summary>
        /// 是否是主键字段[true:主键字段;false:非主键字段]
        /// </summary>
        [DisplayNameEx("主键")]
        public virtual bool IsPriKey { get; protected set; }

        /// <summary>
        /// 字段可选枚举值映射字典集合
        /// </summary>
        [Browsable(false)]
        public virtual Dictionary<string, string> EnumMap { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DBFieldInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbFieldInfo">数据库表字段信息</param>
        public DBFieldInfo(DBFieldInfo dbFieldInfo)
        {
            this.OwerTableName = dbFieldInfo.OwerTableName;
            this.FieldName = dbFieldInfo.FieldName;
            this.DbTypeName = dbFieldInfo.DbTypeName;
            this.DataType = dbFieldInfo.DataType;
            this.Comments = dbFieldInfo.Comments;
            this.DefaultValue = dbFieldInfo.DefaultValue;
            this.AllowNull = dbFieldInfo.AllowNull;
            this.FieldType = dbFieldInfo.FieldType;
            this.IsPriKey = dbFieldInfo.IsPriKey;
            this.Caption = dbFieldInfo.Caption;
            this.Description = dbFieldInfo.Description;
            this.EnumMap = dbFieldInfo.EnumMap;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owerTableName">所属表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="dbTypeName">数据类型</param>
        /// <param name="dataType">数据库字段对应于.net平台的托管类型</param>
        /// <param name="comments">注释</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="allowNull">是否允许为空值</param>
        /// <param name="fieldType">运行时数据类型</param>
        /// <param name="isPriKey">是否是主键</param>
        public DBFieldInfo(string owerTableName, string fieldName, string dbTypeName, Type dataType,
            string comments, object defaultValue, bool allowNull, DBFieldType fieldType, bool isPriKey)
        {
            if (string.IsNullOrEmpty(owerTableName))
            {
                throw new ArgumentNullException(nameof(owerTableName));
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (string.IsNullOrEmpty(dbTypeName))
            {
                throw new ArgumentNullException(nameof(dbTypeName));
            }

            if (dataType == null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            this.OwerTableName = owerTableName;
            this.FieldName = fieldName;
            this.DbTypeName = dbTypeName;
            this.DataType = dataType;
            this.Comments = comments;
            this.DefaultValue = defaultValue;
            this.AllowNull = allowNull;
            this.FieldType = fieldType;
            this.IsPriKey = isPriKey;

            //解析备注
            this.ParseComments(comments, fieldName);
        }

        #region 解析备注
        //public void Test()
        //{
        //    var comments = @"[T:C极化][D:C极化(0:左;1:右;2:未知;允许各级单位自定义)][E:0:左;1:\;右;2:未知]";
        //    comments = @"[T:C极化][D:C极化(0:左;1:右;2:未知;允许各级单位自定义)][E:0:左;1:右;2:未\:\;知;3:XXX]";
        //    this.ParseComments(comments, "abc");
        //}

        /// <summary>
        /// 解析备注
        /// </summary>
        /// <param name="comments">字段备注</param>
        /// <param name="fieldName">字段名</param>
        private void ParseComments(string comments, string fieldName)
        {
            /************************************************
             * eg:[T:C极化][D:C极化(0:左;1:右;2:未知;允许各级单位自定义)][E:0:左;1:右;2:未知]
             * T:标题;=>[T:C极化]
             * D:描述;[D:C极化(0:左;1:右;2:未知)]
             * E:枚举值集合;[M:0:左;1:右;2:未知]
             * 
             * 备注规则:
             * * \:为转义符,如果字符串中有]符号,则需要转义;如果映射值中有;则需要转义
             * 有T必须存在D,M可选;如果D不存在,则默认将整个备注信息用于描述
             * 无T,则无D和M,默认字段名称为标题和描述
             * 
             * 解析规则:
             * 如果字段备注为空或null,则标题和描述皆为字段名;
             * 如果没有找到[T:,则全部为标题和描述;
             * 找[T:或[D:或[E:位置,两找到之后的第一个非转义的],自此为一对.以此从中解析出TDE值.
             * M和D可选,即可能没有
             ************************************************/


            //如果字段备注为空或null,则标题和描述皆为字段名
            if (string.IsNullOrEmpty(comments))
            {
                return;
            }

            try
            {
                //解析[T:
                var caption = this.ParseCaption(comments);
                if (string.IsNullOrEmpty(caption))
                {
                    caption = fieldName;
                }

                this.Caption = caption;

                //解析[D:
                this.Description = this.ParseDes(comments);

                //解析[E:
                this.EnumMap = this.ParseEnumMap(comments);
            }
            catch (Exception ex)
            {
                Loger.Debug(ex);
            }
        }

        private string ParseCaption(string comments)
        {
            string caption;
            const string CAPTION_FLAG = @"[T:";
            int startIndex = comments.IndexOf(CAPTION_FLAG);
            if (startIndex < 0)
            {
                //如果没有找到[T:,则全部为标题和描述
                caption = comments;
            }
            else
            {
                caption = this.CutParseStr(startIndex + CAPTION_FLAG.Length, comments);
            }

            return caption;
        }

        private string ParseDes(string comments)
        {
            const string DES_FLAG = @"[D:";
            var startIndex = comments.IndexOf(DES_FLAG);
            string description;
            if (startIndex < 0)
            {
                description = comments;
            }
            else
            {
                description = this.CutParseStr(startIndex + DES_FLAG.Length, comments);
            }

            return description;
        }

        private Dictionary<string, string> ParseEnumMap(string comments)
        {
            const string ENUM_MAP_FLAG = @"[E:";
            var startIndex = comments.IndexOf(ENUM_MAP_FLAG);
            if (startIndex < 0)
            {
                return null;
            }

            string enumMapStr = this.CutParseStr(startIndex + ENUM_MAP_FLAG.Length, comments);
            if (enumMapStr.Length <= 0)
            {
                return null;
            }

            char shiftCh = '\\';
            const char kvSplitCh = ':';
            List<string> kvStrList = this.ParseMapItemList(enumMapStr, shiftCh);
            return this.ParseMapItemTpMapDic(ref startIndex, shiftCh, kvSplitCh, kvStrList);
        }

        private Dictionary<string, string> ParseMapItemTpMapDic(ref int startIndex, char shiftCh, char kvSplitCh, List<string> kvStrList)
        {
            Dictionary<string, string> enumMap;
            char preCh;
            enumMap = new Dictionary<string, string>();
            foreach (var kvStr in kvStrList)
            {
                startIndex = -1;
                preCh = kvStr[0];
                for (int i = 1; i < kvStr.Length; i++)
                {
                    if (preCh == shiftCh)
                    {
                        preCh = kvStr[i];
                        continue;
                    }

                    if (kvStr[i] == kvSplitCh)
                    {
                        this.ParseMapItem(kvStr, i, enumMap);
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex == -1)
                {
                    Loger.Warn(string.Format("无效的枚举项映射[{0}],解析失败", kvStr));
                }
            }

            return enumMap;
        }

        private List<string> ParseMapItemList(string enumMapStr, char shiftCh)
        {
            var kvStrList = new List<string>();
            int startIndex;
            const char kvItemSplitCh = ';';

            //0:左;1:\;右;2:未知
            startIndex = 0;
            string itemStr;
            char preCh = enumMapStr[0];
            for (int i = 1; i < enumMapStr.Length; i++)
            {
                if (preCh == shiftCh)
                {
                    preCh = enumMapStr[i];
                    continue;
                }

                if (enumMapStr[i] == kvItemSplitCh)
                {
                    itemStr = enumMapStr.Substring(startIndex, i - startIndex);
                    itemStr = itemStr.Replace(@"\;", ";");//Replace转义回本身表达的含义
                    kvStrList.Add(itemStr);
                    i = i + 1;
                    startIndex = i;
                }

                preCh = enumMapStr[i];
            }

            itemStr = enumMapStr.Substring(startIndex);
            itemStr = itemStr.Replace(@"\;", ";");//Replace转义回本身表达的含义
            kvStrList.Add(itemStr);

            return kvStrList;
        }

        private void ParseMapItem(string kvStr, int splitIndex, Dictionary<string, string> enumMap)
        {
            //没有使用注释掉这段代码的方式,原因是可能存在转义符
            //var kvArr = kvStr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            //if (kvArr.Length >= 2)
            //{
            //    key = kvArr[0];
            //    value = kvArr[1];
            //}

            string key, value;
            key = kvStr.Substring(0, splitIndex);
            int valueLen = kvStr.Length - splitIndex - 1;
            int valueStartIndex = splitIndex + 1;
            if (valueLen > 0 && valueStartIndex < kvStr.Length)
            {
                value = kvStr.Substring(valueStartIndex, valueLen);
                value = value.Replace(@"\:", ":");//Replace转义回本身表达的含义
                enumMap[key] = value;
            }
            else
            {
                Loger.Warn(string.Format("无效的枚举项映射[{0}],解析失败", kvStr));
            }
        }

        /// <summary>
        /// 截取解析的字符串
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="comments">备注字符串</param>
        /// <returns>截取到的结果字符串</returns>
        private string CutParseStr(int startIndex, string comments)
        {
            string targetStr = null;
            if (startIndex >= 0)
            {
                for (int i = startIndex; i < comments.Length; i++)
                {
                    if (comments[i] == ']' && comments[i - 1] != '\\')
                    {
                        targetStr = comments.Substring(startIndex, i - startIndex);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(targetStr))
                {
                    targetStr = comments.Substring(startIndex);
                }

                targetStr = targetStr.Replace(@"\]", "]");//Replace转义回本身表达的含义
            }
            else
            {
                targetStr = comments;
            }

            return targetStr;
        }
        #endregion

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("字段名称:{0};数据类型:{1};备注:{2}", this.FieldName, this.DbTypeName, this.Comments);
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
        /// 重写Equals方法[相等返回true;不等返回false]
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>相等返回true;不等返回false</returns>
        public override bool Equals(object obj)
        {
            DBFieldInfo exObj = obj as DBFieldInfo;
            if (exObj == null)
            {
                return false;
            }

            if (!this.FieldName.Equals(exObj.FieldName))
            {
                return false;
            }

            if (!this.DbTypeName.Equals(exObj.DbTypeName))
            {
                return false;
            }

            return true;
        }
    }
}
