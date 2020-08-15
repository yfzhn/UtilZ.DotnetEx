using System.Windows;
using System.Windows.Data;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class BindingEvaluator : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(BindingEvaluator), new FrameworkPropertyMetadata(string.Empty));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        public BindingEvaluator(Binding binding)
        {
            ValueBinding = binding;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Binding ValueBinding { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        public string Evaluate(object dataItem)
        {
            DataContext = dataItem;
            SetBinding(ValueProperty, ValueBinding);
            return Value;
        }
    }
}