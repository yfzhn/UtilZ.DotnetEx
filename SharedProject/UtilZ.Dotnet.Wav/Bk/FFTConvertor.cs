using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Bk
{
    internal class FFTConvertor
    {
        public static FFTResult EffectiveValueConvert2(short[] datas)
        {
            short lmaxValue = 0, lminValue = 0;
            for (int i = 0; i < datas.Length; i++)
            {
                lmaxValue = Math.Max(datas[i], lmaxValue);
                lminValue = Math.Min(datas[i], lminValue);
            }

            FFTResult fftRet = new FFTResult();
            fftRet.Amplitude = lmaxValue;
            fftRet.EffectiveValue = lminValue;
            return fftRet;
        }
    }

    /// <summary>
    /// 快速傅氏变换算法结果
    /// </summary>
    public struct FFTResult
    {
        /// <summary>
        /// 实部
        /// </summary>
        public double Real;

        /// <summary>
        /// 虚部
        /// </summary>
        public double Virtual;

        /// <summary>
        /// 幅值
        /// </summary>
        public double Amplitude;

        /// <summary>
        /// 有效值
        /// </summary>
        public double EffectiveValue;
    }
}
