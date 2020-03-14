using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 传输配置
    /// </summary>
    public class TransferConfig
    {
        /// <summary>
        /// 收到的文件临时存放目录
        /// </summary>
        public string LocalFileDirectory { get; set; } = @"NetTransferFile";

        /// <summary>
        /// 传输数据线程数,最大值32,为了社会和谐建议不要大于4
        /// </summary>
        public byte TransferThreadCount { get; set; } = 2;

        /// <summary>
        /// 解析数据最大线程数,建议值为[传输数据线程数+2]
        /// </summary>
        public int ParseDataMaxThreadCount { get; set; } = 4;

        /// <summary>
        /// 解析数据超时时长,当超过此值时,会多开一个线程用于解析
        /// </summary>
        public int ProReceiveDataAddThreadTimeout { get; internal set; } = 500;

        /// <summary>
        /// 探测步长,默认值1024
        /// 值建议不要太小也不要太大,因udp发包最大值为64k,如果太大则要花费更多的时间去微调到接近完美值;
        /// 如果值太小,则探测过程要花费更长的时间
        /// </summary>
        public ushort DetectStepLength { get; set; } = 1024;

        /// <summary>
        /// 每一步探测次数
        /// </summary>
        public ushort StepDetectCount { get; set; } = 10;

        /// <summary>
        /// mtu微调值,不可大于探测步长值
        /// 1).值越小多次调整后越接近完美值,但所需时间更长;
        /// 2).值越大调整速度越快,但离完美值会有一定的偏差;
        /// 3).建议为探测步长的三分之一
        /// </summary>
        public ushort MtuFineTuning { get; set; } = 342;

        /// <summary>
        /// rto微调值,默认值100
        /// 1).值越小多次调整后越接近完美值,但所需时间更长;
        /// 2).值越大调整速度越快,但离完美值会有一定的偏差;
        /// </summary>
        public ushort RtoFineTuning { get; set; } = 100;

        /// <summary>
        /// rto振幅倍数,建议1.5
        /// 作用于rto分析的结果再乘以振幅倍数得到真正的rto分析值,如果没有此值则容易超时
        /// </summary>
        public float RtoAmplitude { get; set; } = 1.5f;

        /// <summary>
        /// rto最大值
        /// </summary>
        public int RtoMax { get; set; } = 10000;

        /// <summary>
        /// 同一个mtu值超时重试最大次,默认为3
        /// </summary>
        public int MtuRepeatMaxCount { get; set; } = 3;

        /// <summary>
        /// 超时心跳倍数[接收检测超时时长=发送超时/(传输线程数*超时心跳倍数)],值越大,心跳检测频率越高,高优先级传输整数速度越慢
        /// </summary>
        public int TimeoutHeartMul { get; set; } = 5;

        /// <summary>
        /// 网络配置
        /// </summary>
        public NetConfig NetConfig { get; set; } = new NetConfig();

        /// <summary>
        /// 构造函数
        /// </summary>
        public TransferConfig()
        {

        }

        /// <summary>
        /// 验证参数有效性,验证不通过抛出异常
        /// </summary>
        public void Validate()
        {
            if (this.TransferThreadCount > TransferConstant.PARALLEL_THREAD_MAX_COUN)
            {
                throw new ArgumentOutOfRangeException(nameof(this.TransferThreadCount), $"并行下载数据最大线程数值过大,不能超过{TransferConstant.PARALLEL_THREAD_MAX_COUN}");
            }

            if (this.TransferThreadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.TransferThreadCount), "传输数据线程数过小");
            }

            if (this.ParseDataMaxThreadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.ParseDataMaxThreadCount), "解析数据最大线程数过小");
            }

            if (this.ParseDataMaxThreadCount < this.TransferThreadCount)
            {
                throw new ArgumentOutOfRangeException(nameof(this.ParseDataMaxThreadCount), "解析数据最大线程数不能小于传输数据线程数");
            }

            if (string.IsNullOrWhiteSpace(this.LocalFileDirectory))
            {
                throw new ArgumentNullException(nameof(this.LocalFileDirectory), "收到的文件临时存放目录不能为null或空");
            }

            if (this.DetectStepLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.DetectStepLength), "探测步长值过小");
            }

            if (this.DetectStepLength >= TransferConstant.MTU_MAX - TransferConstant.MTU_MIN)
            {
                throw new ArgumentOutOfRangeException(nameof(this.DetectStepLength), "探测步长值过大");
            }

            if (this.StepDetectCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.StepDetectCount), "每一步探测数量值过小");
            }

            if (this.MtuFineTuning < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.MtuFineTuning), "mtu微调值过小");
            }

            if (this.MtuFineTuning >= this.DetectStepLength)
            {
                throw new ArgumentOutOfRangeException(nameof(this.MtuFineTuning), "mtu微调值不能大于过探测步长值");
            }

            if (this.RtoFineTuning < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.RtoFineTuning), "rto微调值过小");
            }

            if (this.RtoAmplitude - 1 < 0.01)
            {
                throw new ArgumentOutOfRangeException(nameof(this.RtoAmplitude), "rto振幅倍数过小");
            }

            if (this.RtoMax < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(this.RtoMax), "rto最大值过小");
            }

            if (this.MtuRepeatMaxCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.MtuRepeatMaxCount), "同一个mtu值超时重试最大次过小");
            }

            if (this.TimeoutHeartMul < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(this.TimeoutHeartMul), "超时心跳倍数值过小");
            }

            if (this.NetConfig == null)
            {
                throw new ArgumentNullException(nameof(this.NetConfig), "网络配置不能为null");
            }
            else
            {
                this.NetConfig.Validate();
            }
        }

        /// <summary>
        /// 深拷贝一份当前对象
        /// </summary>
        /// <returns>当前对象副本</returns>
        public TransferConfig Clone()
        {
            return ObjectEx.DeepCopy<TransferConfig>(this);
        }
    }
}