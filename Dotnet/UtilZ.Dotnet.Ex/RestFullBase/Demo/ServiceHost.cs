using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.RestFullBase.Demo
{
    internal class ServiceHost
    {
        public static void StartService()
        {
            PersonInfoQueryService service = new PersonInfoQueryService();
            Uri bassAddress = new Uri("http://127.0.0.1:7789/");
            using (WebServiceHost serviceHost = new WebServiceHost(service, bassAddress))
            {
                try
                {
                    const int MAX_BUFFER = 20971520;
                    var binding = new WebHttpBinding();
                    binding.TransferMode = TransferMode.Buffered;
                    binding.MaxBufferSize = MAX_BUFFER;
                    binding.MaxReceivedMessageSize = MAX_BUFFER;
                    binding.MaxBufferPoolSize = MAX_BUFFER;
                    binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                    binding.Security.Mode = WebHttpSecurityMode.None;
                    serviceHost.AddServiceEndpoint(typeof(IPersonInfoQuery), binding, bassAddress);

                    //跨域
                    var crossOriginBehavior = new EnableCrossOriginResourceSharingBehavior();
                    foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
                    {
                        endpoint.Behaviors.Add(crossOriginBehavior);
                    }

                    serviceHost.Opened += (s, e) =>
                    {
                        Console.WriteLine("服务已开启...http://127.0.0.1:7789/");
                    };

                    serviceHost.Closed += (s, e) =>
                    {
                        Console.WriteLine("服务已停止...");
                    };

                    serviceHost.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                Console.WriteLine("any key exit...");
                Console.ReadKey();
            }
        }
    }

    internal class EnableCrossOriginResourceSharingBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public EnableCrossOriginResourceSharingBehavior()
        {
            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            var requiredHeaders = new Dictionary<string, string>();

            requiredHeaders.Add("Access-Control-Allow-Origin", "*");
            requiredHeaders.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
            requiredHeaders.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type");

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CustomHeaderMessageInspector(requiredHeaders));
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        public override Type BehaviorType
        {
            get { return typeof(EnableCrossOriginResourceSharingBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new EnableCrossOriginResourceSharingBehavior();
        }
    }

    internal class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        Dictionary<string, string> requiredHeaders;
        public CustomHeaderMessageInspector(Dictionary<string, string> headers)
        {
            requiredHeaders = headers ?? new Dictionary<string, string>();
        }

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
            foreach (var item in requiredHeaders)
            {
                httpHeader.Headers.Add(item.Key, item.Value);
            }
        }
    }
}
