using System;
using System.Collections;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class SuggestionProvider : ISuggestionProvider
    {

        private readonly Func<string, IEnumerable> _method;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public SuggestionProvider(Func<string, IEnumerable> method)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable GetSuggestions(string filter)
        {
            return _method(filter);
        }
    }
}