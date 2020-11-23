using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    internal class VoicePlotChannelData
    {
        /// <summary>
        /// 声道名称
        /// </summary>
        public string Name { get; private set; }

        private IComparable[] _globalViewData;
        /// <summary>
        /// 全局视图数据
        /// </summary>
        public IComparable[] GlobalViewData
        {
            get { return this._globalViewData; }
        }


        private IComparable[] _partViewData = null;
        /// <summary>
        /// 局部视图数据
        /// </summary>
        public IComparable[] PartViewData
        {
            get
            {
                if (this._partViewData == null)
                {
                    return this._globalViewData;
                }

                return this._partViewData;
            }
        }

        public VoicePlotChannelData(string name, IComparable[] globalViewData)
        {
            this.Name = name;
            this._globalViewData = globalViewData;
        }
    }
}
