using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Popup扩展辅助类
    /// </summary>
    public class PopupEx
    {
        /// <summary>
        /// 判定Popup是否打开
        /// </summary>
        /// <param name="popupLastCloseTime"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static bool HasOpen(DateTime? popupLastCloseTime, double interval = 300d)
        {
            if (popupLastCloseTime.HasValue)
            {
                var ts = DateTime.Now - popupLastCloseTime.Value;
                //const double interval = 200d;
                const double PRE = 0.0001d;
                if (ts.TotalMilliseconds - interval < PRE)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
