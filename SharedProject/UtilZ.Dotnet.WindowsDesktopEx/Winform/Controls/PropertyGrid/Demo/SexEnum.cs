using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo
{
    /// <summary>
    /// PropertyGrid枚举
    /// </summary>
    public enum SexEnum
    {
        /// <summary>
        /// M
        /// </summary>
        [DisplayNameExAttribute("男")]
        M,

        /// <summary>
        /// F
        /// </summary>
        //[NDisplayNameAttribute( "女")]
        F,

        /// <summary>
        /// O
        /// </summary>
        [DisplayNameExAttribute("中性")]
        O
    }
}
