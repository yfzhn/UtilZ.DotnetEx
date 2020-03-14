using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// 跨域扩展行为
    /// </summary>
    public class CrossDomainExtensionBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CrossDomainExtensionBehavior()
        {

        }

        /// <summary>
        /// 空方法
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        /// <summary>
        /// 空方法
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {

        }

        /// <summary>
        /// ApplyDispatchBehavior
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="endpointDispatcher"></param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            var requiredHeaders = new Dictionary<string, string>();

            requiredHeaders.Add("Access-Control-Allow-Origin", "*");
            requiredHeaders.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
            requiredHeaders.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type");

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CrossDomainHeaderMessageInspector(requiredHeaders));
        }

        /// <summary>
        /// 空方法
        /// </summary>
        /// <param name="endpoint"></param>
        public void Validate(ServiceEndpoint endpoint)
        {

        }

        /// <summary>
        /// BehaviorType
        /// </summary>
        public override Type BehaviorType
        {
            get { return typeof(CrossDomainExtensionBehavior); }
        }

        /// <summary>
        /// CreateBehavior
        /// </summary>
        /// <returns></returns>
        protected override object CreateBehavior()
        {
            return new CrossDomainExtensionBehavior();
        }
    }
}
