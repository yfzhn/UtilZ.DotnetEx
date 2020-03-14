using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.RestFullBase.Demo
{
    [ServiceContract(Name = "PersonInfoQueryService")]
    internal interface IPersonInfoQuery
    {
        [OperationContract]
        [WebGet(UriTemplate = "PersonInfoQuery/{id}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        User Get(string id);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "PersonInfoQuery/user",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        int Post(User user);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "PersonInfoQuery/user",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        int Put(User user);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            UriTemplate = "PersonInfoQuery/{id}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        int Delete(string id);
    }

    [DataContract]
    internal class User
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public byte Age { get; set; }

        [DataMember]
        public DateTime Birthday { get; set; }

        public User()
        {

        }
    }
}
