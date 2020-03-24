using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.EF
{
    internal class EFConnectionFactory : IDbConnectionFactory
    {
        public EFConnectionFactory()
        {

        }

        public System.Data.Common.DbConnection CreateConnection(string nameOrConnectionString)
        {
            return null;
        }
    }
}
