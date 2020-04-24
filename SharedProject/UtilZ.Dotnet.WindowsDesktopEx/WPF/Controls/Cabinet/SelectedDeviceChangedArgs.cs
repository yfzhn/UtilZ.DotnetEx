using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 选中设备改变事件
    /// </summary>
    public class SelectedDeviceChangedArgs : EventArgs
    {
        /// <summary>
        /// 前一次选中的设备
        /// </summary>
        public CabinetDevice OldDevice { get; private set; }

        /// <summary>
        /// 当前选中的设备
        /// </summary>
        public CabinetDevice NewDevice { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oldDevice">前一次选中的设备</param>
        /// <param name="newDevice">当前选中的设备</param>
        public SelectedDeviceChangedArgs(CabinetDevice oldDevice, CabinetDevice newDevice)
        {
            this.OldDevice = oldDevice;
            this.NewDevice = newDevice;
        }
    }
}
