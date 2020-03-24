using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBMySql.Core
{
    [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
    public class MySqlClientFactoryZ : DbProviderFactory, IServiceProvider
    {
        private Type _dbServicesType;
        private FieldInfo _mySqlDbProviderServicesInstance;

        public MySqlClientFactoryZ()
        {

        }

        public override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new MySqlConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType != this.GetDbServicesType())
            {
                return null;
            }

            FieldInfo mySqlDbProviderServicesInstanceFieldInfo = this.GetMySqlDbProviderServicesInstanceFieldInfo();
            if (mySqlDbProviderServicesInstanceFieldInfo == null)
            {
                return null;
            }

            return mySqlDbProviderServicesInstanceFieldInfo.GetValue(null);
        }

        private Type GetDbServicesType()
        {
            return (this._dbServicesType ?? (this._dbServicesType = Type.GetType("System.Data.Common.DbProviderServices, System.Data.Entity, \n\t\t\t\t\t\t\t\t\t\t\t\tVersion=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false)));
        }

        private FieldInfo GetMySqlDbProviderServicesInstanceFieldInfo()
        {
            if (this._mySqlDbProviderServicesInstance == null)
            {
                string fullName = Assembly.GetExecutingAssembly().FullName;
                string str2 = fullName.Replace("MySql.Data", "MySql.Data.Entity.EF6");
                string str3 = fullName.Replace("MySql.Data", "MySql.Data.Entity.EF5");
                Type type = Type.GetType(string.Format("MySql.Data.MySqlClient.MySqlProviderServices, {0}", str3), false);
                if (type == null)
                {
                    fullName = string.Format("MySql.Data.MySqlClient.MySqlProviderServices, {0}", str2);
                    type = Type.GetType(fullName, false);
                    if (type == null)
                    {
                        throw new DllNotFoundException(fullName);
                    }
                }

                this._mySqlDbProviderServicesInstance = type.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            }

            return this._mySqlDbProviderServicesInstance;
        }
    }
}
