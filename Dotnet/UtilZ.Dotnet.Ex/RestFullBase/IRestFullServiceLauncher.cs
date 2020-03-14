using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// RestFullService启动接口
    /// </summary>
    public interface IRestFullServiceLauncher : IDisposable
    {
        /// <summary>
        /// 获取服务宿主
        /// </summary>
        ServiceHost ServiceHost { get; }

        /// <summary>
        /// 获取RestFullService状态[true:已启动;false:停止]
        /// </summary>
        bool Status { get; }

        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
    }
}
