using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.RestFullBase.Demo
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
         ConcurrencyMode = ConcurrencyMode.Single,
         IncludeExceptionDetailInFaults = true,
         AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    internal class PersonInfoQueryService : IPersonInfoQuery
    {
        private static readonly Dictionary<string, User> _userDic = new Dictionary<string, User>();
        static PersonInfoQueryService()
        {
            var dt = DateTime.Now;
            _userDic.Add("1", new User() { Id = "1", Name = "yyy", Age = 23, Birthday = dt.AddDays(-255) });
            _userDic.Add("2", new User() { Id = "2", Name = "zzzz", Age = 32, Birthday = dt.AddDays(-152) });
            _userDic.Add("3", new User() { Id = "3", Name = "zf", Age = 18, Birthday = dt.AddDays(-1420) });
        }

        public User Get(string id)
        {
            if (_userDic.ContainsKey(id))
            {
                return _userDic[id];
            }
            else
            {
                return null;
            }
        }

        public int Post(User user)
        {
            if (_userDic.ContainsKey(user.Id))
            {
                return -1;
            }

            _userDic.Add(user.Id, user);
            return 0;
        }


        public int Put(User user)
        {
            if (_userDic.ContainsKey(user.Id))
            {
                _userDic[user.Id] = user;
                return 0;
            }

            return -1;
        }

        public int Delete(string id)
        {
            if (_userDic.ContainsKey(id))
            {
                _userDic.Remove(id);
                return 0;
            }

            return -1;
        }
    }
}
