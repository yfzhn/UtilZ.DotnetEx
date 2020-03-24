using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UtilZ.Dotnet.DBIBase.Config
{
    internal class DefaultDatabaseConfigParser : DatabaseConfigParserAbs
    {
        private readonly Dictionary<string, PropertyInfo> _propertyDic;
        public DefaultDatabaseConfigParser()
            : base()
        {
            this._propertyDic = typeof(DatabaseConfig).GetProperties().ToDictionary(t => { return t.Name.ToLower(); });
        }

        /// <summary>
        /// 解析数据库配置项
        /// </summary>
        /// <param name="ele">xml配置节点</param>
        /// <returns>数据库配置对象集合</returns>
        public override IEnumerable<DatabaseConfig> Parse(XElement ele)
        {
            var configList = new List<DatabaseConfig>();
            var dbBConfigItem = new DatabaseConfig();
            var configPropertyAtts = ele.Attributes();
            string tagName;
            PropertyInfo propertyInfo;
            object value;

            foreach (var configPropertyAtt in configPropertyAtts)
            {
                tagName = configPropertyAtt.Name.LocalName.Trim().ToLower();
                if (!this._propertyDic.ContainsKey(tagName))
                {
                    //不识别的标签,忽略
                    continue;
                }

                propertyInfo = this._propertyDic[tagName];
                value = configPropertyAtt.Value;
                if (Type.GetTypeCode(propertyInfo.PropertyType) != TypeCode.String)
                {
                    value = Convert.ChangeType(value, propertyInfo.PropertyType);
                }

                propertyInfo.SetValue(dbBConfigItem, value, null);
            }

            configList.Add(dbBConfigItem);
            return configList;
        }
    }
}
