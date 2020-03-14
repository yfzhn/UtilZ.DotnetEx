using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferParaAnalyze
    {
        private const int _NONE = 0;
        private readonly TransferConfig _config;
        private int _rto = TransferConstant.DEFAULT_RTO;
        private int _mtu = TransferConstant.MTU_MIN;
        private NetTransferPara[] _netTransferParas;
        private readonly object _netTransferParasLock = new object();
        private int _netInfosIndex = 0;
        private readonly int _netInfoMaxCount;
        private bool _isDetectCompleted = false;
        private ushort _stepDetectCount = 0;
        private int _durationAdjustMtuCount = 0;
        private readonly int _durationAdjustMtuMaxCount;
        private void UpdateMtu(int mtu)
        {
            if (mtu > TransferConstant.MTU_MAX)
            {
                mtu = TransferConstant.MTU_MAX;
            }
            else if (mtu < TransferConstant.MTU_MIN)
            {
                mtu = TransferConstant.MTU_MIN;
            }

            this._mtu = mtu;
        }

        private void UpdateRto(int rto)
        {
            if (rto > this._config.RtoMax)
            {
                rto = this._config.RtoMax;
            }

            this._rto = rto;
        }

        public TransferParaAnalyze(TransferConfig config)
        {
            this._config = config;
            ushort detectStepLength = config.DetectStepLength;
            ushort stepDetectCount = config.StepDetectCount;

            int mtuRange = TransferConstant.MTU_MAX - TransferConstant.MTU_MIN;
            int count = mtuRange / detectStepLength;
            if (mtuRange % detectStepLength > 0)
            {
                count += 1;
            }

            this._netInfoMaxCount = count * config.StepDetectCount;
            this._netTransferParas = new NetTransferPara[this._netInfoMaxCount];
            for (int i = 0; i < this._netInfoMaxCount; i++)
            {
                this._netTransferParas[i] = new NetTransferPara();
            }

            this._durationAdjustMtuMaxCount = (mtuRange / config.MtuFineTuning) / 2;
        }

        public int GetMtu()
        {
            //return NetTransferConstant.MTU_MIN;
            lock (this._netTransferParasLock)
            {
                if (this._isDetectCompleted)
                {
                    //探测完成
                    this._durationAdjustMtuCount = 0;
                    return this._mtu;
                }

                //探测mtu
                if (this._stepDetectCount >= this._config.StepDetectCount)
                {
                    int tmpMtu = this._mtu + this._config.DetectStepLength;
                    if (tmpMtu > TransferConstant.MTU_MAX)
                    {
                        //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID, null, "调整mtu,会是计算值超出上限,不作调整");
                        this._isDetectCompleted = true;
                        return this._mtu;
                    }

                    //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID, null, $"调整mtu,{this._mtu}-{tmpMtu}");
                    this.UpdateMtu(tmpMtu);
                    this._stepDetectCount = 0;
                }

                this._stepDetectCount++;
                return this._mtu;
            }
        }

        public int GetRto()
        {
            lock (this._netTransferParasLock)
            {
                return this._rto;
            }
        }

        /// <summary>
        /// 下调mtu,并返回调整后的mtu
        /// </summary>
        /// <returns></returns>
        public int AdjustDownMtu()
        {
            lock (this._netTransferParasLock)
            {
                if (this._isDetectCompleted)
                {
                    this._durationAdjustMtuCount++;
                    if (this._durationAdjustMtuCount >= this._durationAdjustMtuMaxCount)
                    {
                        //连续调整mtu达到上限值,则重新探测
                        this.UpdateMtu(TransferConstant.MTU_MIN);
                        this._isDetectCompleted = false;
                        this._stepDetectCount = 0;
                    }
                    else
                    {
                        this.UpdateMtu(this._mtu - this._config.MtuFineTuning);
                    }
                }
                else
                {
                    //探测未完成,但是在默认rto值情况下mtu值已经超时了,之后的数据步进不再探测
                    //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID,"AdjustDownMtu-分析");
                    this.AnalyzeMtuAndRtoValue();
                }

                return this._mtu;
            }
        }

        /// <summary>
        /// 上调rto,并返回调整后的rto值
        /// </summary>
        /// <returns></returns>
        public int AdjustUpRto()
        {
            lock (this._netTransferParasLock)
            {
                if (this._isDetectCompleted)
                {
                    this.UpdateRto(this._rto + this._config.RtoFineTuning);
                }
                else
                {
                    //探测未完成,但是在默认rto值情况下mtu值已经超时了,之后的数据步进不再探测
                    //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID,"AdjustUpRto-分析");
                    this.AnalyzeMtuAndRtoValue();
                }

                return this._rto;
            }
        }

        public void RecordMtuAndRto(int mtu, int rto, bool isTimeout)
        {
            if (mtu <= 0 || rto <= 0)
            {
                return;
            }

            lock (this._netTransferParasLock)
            {
                try
                {
                    this._netTransferParas[this._netInfosIndex].Update(mtu, rto, isTimeout);
                    this._netInfosIndex++;

                    if (this._netInfosIndex >= this._netInfoMaxCount)
                    {
                        //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID,"RecordMtuAndRto-分析");
                        this.AnalyzeMtuAndRtoValue();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Error(ex, "RecordMtuAndRto异常");
                }
            }
        }

        private int _lastNoneAnalyzeItemCount = 0;

        private void AnalyzeMtuAndRtoValue()
        {
            try
            {
                IEnumerable<NetTransferPara> validTransferParas = this._netTransferParas.Where(w => { return w.HasValue; });
                if (validTransferParas.Count() <= 0)
                {
                    if (this._isDetectCompleted)
                    {
                        //已探测完成,但本次分析的可用项为0,则不进行分析,直接使用当前值
                        this._netInfosIndex = 0;
                        //Loger.Warn(NetTransferConstant.WARN_EVENT_ID, null, "已探测完成,但没有可用于分析的有效项");
                        return;
                    }

                    //未探测完成,强制进行分析,且一次传输成功的都没有,使用默认值
                    this.UpdateMtu(TransferConstant.MTU_MIN);
                    if (this._lastNoneAnalyzeItemCount > 0)
                    {
                        this.UpdateRto(TransferConstant.DEFAULT_RTO + this._lastNoneAnalyzeItemCount * this._config.RtoFineTuning);
                    }
                    else
                    {
                        this.UpdateRto(TransferConstant.DEFAULT_RTO);
                    }

                    this._netInfosIndex = 0;
                    //Loger.Warn(NetTransferConstant.WARN_EVENT_ID, null, $"第{this._lastNoneAnalyzeItemCount}次没有可用于分析的有效项,设置为默认最小值");
                    this._lastNoneAnalyzeItemCount++;
                    return;
                }

                //按mtu分组
                this._lastNoneAnalyzeItemCount = 0;
                IEnumerable<IGrouping<int, NetTransferPara>> groups = validTransferParas.OrderBy((t) => { return t.Timestamp; }).GroupBy(p => { return p.Mtu; });

                //groups = groups.Where(g => { return g.Count() >= this._config.DetectStepLength; });
                groups = groups.Where(g => { return g.Count() > 3; });
                if (groups.Count() == 0)
                {
                    //不作分析
                    //Loger.Warn($"符合分析条件的mtu组不存在,不作分析,使用当前值[{this._mtu}]");
                    this._netInfosIndex = 0;
                    return;
                }

                var evaluateResults = new List<MtuRtoEvaluateResult>();
                foreach (var group in groups)
                {
                    MtuRtoEvaluateResult evaluateResult = this.EvaluateMtuPerformance(group);
                    if (evaluateResult != null)
                    {
                        evaluateResults.Add(evaluateResult);
                    }
                }

                //取单位时间内传输最大值作为调整值
                var analyzeResult = evaluateResults.OrderByDescending(t => { return t.UnitTimeTrasnferSize; }).FirstOrDefault();
                if (analyzeResult != null && analyzeResult.UnitTimeTrasnferSize > 0)
                {
                    if (analyzeResult.TimeoutCount == 0)
                    {
                        //超时次数为0,向上微调mtu值
                        this.UpdateMtu(analyzeResult.Mtu + this._config.MtuFineTuning);
                    }
                    else
                    {
                        //有超时,向下微调mtu值
                        this.UpdateMtu(analyzeResult.Mtu + this._config.MtuFineTuning);
                    }

                    this.UpdateRto(analyzeResult.Rto);
                    //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID, null, $"分析mtu:{this._mtu},rto:{ this._rto}");
                }
                else
                {
                    //没有合适的值选用默认值
                    this.UpdateMtu(TransferConstant.MTU_MIN);
                    this.UpdateRto(TransferConstant.DEFAULT_RTO);
                    //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID, null, "分析无结果,使用默认值");
                }

                this._isDetectCompleted = true;
                this._netInfosIndex = 0;
            }
            catch (Exception ex)
            {
                Loger.Error(TransferConstant.ERROR_EVENT_ID, null, ex, "分析mtu以及rto值异常");
            }
        }

        private MtuRtoEvaluateResult EvaluateMtuPerformance(IGrouping<int, NetTransferPara> group)
        {
            int maxRto = 0;
            int successCount = 0, timeoutCount = 0;
            long tansferTotalLen = 0, totalRto = 0;
            long successTansferTotalLen = 0, successTotalRto = 0;
            foreach (var item in group)
            {
                tansferTotalLen += item.Mtu;
                totalRto += item.Rto;
                if (item.IsTimeout)
                {
                    //统计超时
                    timeoutCount++;
                    continue;
                }

                //统计成功
                successCount++;
                successTansferTotalLen += item.Mtu;
                successTotalRto += item.Rto;
                if (maxRto < item.Rto)
                {
                    maxRto = item.Rto;
                }
            }

            //统计传输性能指标
            float unitTimeTrasnferSize = (float)successTansferTotalLen / totalRto;
            int avgRto, mtu = group.ElementAt(0).Mtu;
            if (successCount > 0)
            {
                //计算成功的平均rto值,成功的平均值再和最大值取平均值
                avgRto = ((int)(successTotalRto / successCount) + maxRto) / 2;
                //Loger.Trace(NetTransferConstant.TRACE_EVENT_ID, null, $"mtu:{mtu},avgRto:{avgRto},总共:{group.Count() }次,成功:{successCount}次,超时:{timeoutCount}次");
                avgRto = (int)(avgRto * this._config.RtoAmplitude);//rto多乘以一个振幅,如果没有此值则容易超时
                return new MtuRtoEvaluateResult(unitTimeTrasnferSize, mtu, avgRto, timeoutCount);
            }
            else
            {
                //Loger.Warn(NetTransferConstant.WARN_EVENT_ID, null, $"mtu[{group.Key}]全部超时");
                return null;
            }
        }
    }

    internal class MtuRtoEvaluateResult
    {
        public float UnitTimeTrasnferSize { get; private set; }

        public int Mtu { get; private set; }

        public int Rto { get; private set; }

        public int TimeoutCount { get; private set; }

        public MtuRtoEvaluateResult(float unitTimeTrasnferSize, int mtu, int rto, int timeoutCount)
        {
            this.UnitTimeTrasnferSize = unitTimeTrasnferSize;
            this.Mtu = mtu;
            this.Rto = rto;
            this.TimeoutCount = timeoutCount;
        }
    }

    internal class NetTransferPara
    {
        public bool HasValue { get; private set; } = false;
        public bool IsTimeout { get; private set; } = false;
        public long Timestamp { get; private set; } = 0;
        public int Mtu { get; private set; } = 0;
        public int Rto { get; private set; } = 0;

        public NetTransferPara()
        {

        }

        public void Update(int mtu, int rto, bool isTimeout)
        {
            Timestamp = TimeEx.GetTimestamp();
            HasValue = true;
            this.IsTimeout = isTimeout;
            Mtu = mtu;
            Rto = rto;
        }
    }
}
