using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// ApiResult
    /// </summary>
    [DataContract]
    public class ApiResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ApiResultStatus Status { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public string Data { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiResult()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="data">数据</param>
        public ApiResult(ApiResultStatus status, string data)
        {
            this.Status = status;
            this.Data = data;
        }
    }
}
