using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// RestFullService启动器
    /// </summary>
    public class RestFullServiceLauncher<T> : IRestFullServiceLauncher
    {
        private readonly Uri _bassAddress;
        private readonly WebServiceHost _serviceHost;

        /// <summary>
        /// 获取服务宿主
        /// </summary>
        public ServiceHost ServiceHost
        {
            get
            {
                return this._serviceHost;
            }
        }

        private bool _status = false;
        /// <summary>
        /// 获取RestFullService状态[true:已启动;false:停止]
        /// </summary>
        public bool Status
        {
            get { return _status; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bassAddress">服务基地址</param>
        /// <param name="serviceInstace">服务实例</param>
        public RestFullServiceLauncher(Uri bassAddress, T serviceInstace)
        {
            this._bassAddress = bassAddress;
            this._serviceHost = new WebServiceHost(serviceInstace, bassAddress);
        }

        private void ServiceThreadMethod()
        {
            const int MAX_BUFFER = 20971520;
            var binding = new WebHttpBinding();
            binding.TransferMode = TransferMode.Buffered;
            binding.MaxBufferSize = MAX_BUFFER;
            binding.MaxReceivedMessageSize = MAX_BUFFER;
            binding.MaxBufferPoolSize = MAX_BUFFER;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            binding.Security.Mode = WebHttpSecurityMode.None;

            this._serviceHost.AddServiceEndpoint(typeof(T), binding, this._bassAddress);

            //跨域
            var crossOriginBehavior = new CrossDomainExtensionBehavior();
            foreach (ServiceEndpoint endpoint in _serviceHost.Description.Endpoints)
            {
                endpoint.Behaviors.Add(crossOriginBehavior);
            }

            this._serviceHost.Opened += (s, e) =>
            {
                Loger.Info("服务启动成功");
                this._status = true;
            };

            this._serviceHost.Closed += (s, e) =>
            {
                this._status = false;
                Loger.Info("服务已关闭");
            };

            this._serviceHost.Open();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            //注:此处如果不用另外一个线程去启动,之后服务处理将会在主线程上,微软的坑啊.
            Task.Factory.StartNew(this.ServiceThreadMethod);
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            this._serviceHost.Close();
        }

        /// <summary>
        /// IDisposable接口
        /// </summary>
        public void Dispose()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Loger.Error(ex, "Dispose异常");
            }
        }
    }
}
