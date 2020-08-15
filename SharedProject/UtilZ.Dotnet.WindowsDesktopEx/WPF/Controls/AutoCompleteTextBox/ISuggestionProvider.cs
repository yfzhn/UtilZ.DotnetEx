using System.Collections;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISuggestionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable GetSuggestions(string filter);
    }
}
