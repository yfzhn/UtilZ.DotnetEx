using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotnetWinFormApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var list = UtilZ.Dotnet.Ex.Base.BaseEx.ConstantHelper.GetDisplayNameExAttributeItemList<XX>(false);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FTestWaveControl());
        }
    }

    public class XX
    {
        [UtilZ.Dotnet.Ex.Base.DisplayNameEx("代码")]
        public const string Code = "abc";

        public const int Age = 23;
    }
}
