using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls;

namespace CoreWpfApp
{
    /// <summary>
    /// TestAutoCompleteTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class TestAutoCompleteTextBox : Window
    {
        public TestAutoCompleteTextBox()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = (TestAutoCompleteTextBoxVM)this.DataContext;
        }

        private void PART_Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb.Text = ((TextBox)sender).Text;
        }
    }


    internal class TestAutoCompleteTextBoxVM : NotifyPropertyChangedAbs
    {
        private ObservableCollection<Person> _people;
        private Person _selectedPerson;

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                base.OnRaisePropertyChanged();
            }
        }

        public ObservableCollection<Person> People
        {
            get => _people;
            private set
            {
                _people = value;
                base.OnRaisePropertyChanged();
            }
        }


        private long _freq = 1000L;
        public long Freq
        {
            get { return _freq; }
            set
            {
                _freq = value;
                base.OnRaisePropertyChanged();
            }
        }

        public TestAutoCompleteTextBoxVM()
        {
            People = new ObservableCollection<Person>()
            {
                new Person() { Name = "Chris Lee", State = StateFactory.FromAbbreviation("NY"), Color = WebColorsFactory.FromColor(Colors.DarkGoldenrod) },
                new Person() { Name = "David Smith", State = StateFactory.FromAbbreviation("MS"), Color = WebColorsFactory.FromColor(Colors.Blue) },
                new Person() { Name = "Jane Allen", State = StateFactory.FromAbbreviation("AL"), Color = WebColorsFactory.FromColor(Colors.Aqua) },
                new Person() { Name = "John Doe", State = StateFactory.FromAbbreviation("TN"), Color = WebColorsFactory.FromColor(Colors.AliceBlue) },
                new Person() { Name = "Maria Hernandez", State = StateFactory.FromAbbreviation("CA"), Color = WebColorsFactory.FromColor(Colors.ForestGreen) },
            };

            SelectedPerson = People[0];
        }
    }


    public class FreqSuggestionProvider : ISuggestionProvider
    {
        private readonly List<string> _srcList;
        public FreqSuggestionProvider()
        {
            _srcList = new List<string>()
            {
                "123",
                "124",
                "125",
                "258",
                "256",
                "7778",
                "7896",
            };
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            //System.Threading.Thread.Sleep(1000);
            return
                _srcList
                    .Where(freq => freq.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) > -1)
                    .ToList();
        }
    }




    public class StateSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<State> ListOfStates { get; set; }

        public State GetExactSuggestion(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                ListOfStates
                    .FirstOrDefault(state => string.Equals(state.Name, filter, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<State> GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            //System.Threading.Thread.Sleep(1000);
            return
                ListOfStates
                    .Where(state => state.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) > -1)
                    .ToList();
        }

        IEnumerable ISuggestionProvider.GetSuggestions(string filter)
        {
            return GetSuggestions(filter);
        }

        public StateSuggestionProvider()
        {
            var states = StateFactory.CreateStateList();
            ListOfStates = states;
        }
    }

    public enum MatchKind
    {
        StartsWith,
        Contains,
        EndsWith,
        Exact,
    }

    public class WebColorsSuggestionProvider : NotifyPropertyChangedAbs, ISuggestionProvider
    {
        public ObservableCollection<WebColor> WebColors { get; private set; }

        private bool _allowEmptyFilter;
        private bool _ignoreCase;
        private string _lastFilter;
        private int _maxSuggestionCount;
        private MatchKind _matchKind;
        private StringComparison _comparison;
        private Func<string, string, bool> _matchPredicate;

        public bool AllowEmptyFilter
        {
            get { return _allowEmptyFilter; }
            set
            {
                _allowEmptyFilter = value;
                base.OnRaisePropertyChanged();
            }
        }

        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set
            {
                _ignoreCase = value;
                _comparison = value ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
                base.OnRaisePropertyChanged();
            }
        }

        public string LastFilter
        {
            get { return _lastFilter; }
            set
            {
                _lastFilter = value;
                base.OnRaisePropertyChanged();
            }
        }

        public int MaxSuggestionCount
        {
            get { return _maxSuggestionCount; }
            set
            {
                _maxSuggestionCount = value;
                base.OnRaisePropertyChanged();
            }
        }

        public MatchKind MatchKind
        {
            get { return _matchKind; }
            set
            {
                if (_matchKind == value)
                {
                    return;
                }

                _matchKind = value;
                base.OnRaisePropertyChanged();

                switch (value)
                {
                    case MatchKind.StartsWith:
                        _matchPredicate = StartsWith;
                        break;
                    case MatchKind.EndsWith:
                        _matchPredicate = EndsWith;
                        break;
                    case MatchKind.Exact:
                        _matchPredicate = Exact;
                        break;
                    case MatchKind.Contains:
                    default:
                        _matchPredicate = Contains;
                        break;
                }
            }
        }

        public IEnumerable GetSuggestions(string filter)
        {
            LastFilter = filter;
            if (string.IsNullOrWhiteSpace(filter))
            {
                if (!AllowEmptyFilter)
                    return null;

                return WebColors
                        .Take(MaxSuggestionCount)
                        .ToList();
            }

            return
                WebColors
                    .Where(x => _matchPredicate(x.Name, filter))
                    .Take(MaxSuggestionCount)
                    .ToList();
        }

        private bool Contains(string source, string value)
        {
            if (source == null || value == null) return false;
            return source.IndexOf(value, _comparison) > -1;
        }

        private bool EndsWith(string source, string value)
        {
            if (source == null || value == null) return false;
            return source.EndsWith(value, _comparison);
        }

        private bool Exact(string source, string value)
        {
            return string.Equals(source, value, _comparison);
        }

        private bool StartsWith(string source, string value)
        {
            if (source == null || value == null) return false;
            return source.StartsWith(value, _comparison);
        }

        public WebColorsSuggestionProvider()
        {
            IgnoreCase = true;
            MatchKind = MatchKind.Contains;
            MaxSuggestionCount = 10;
            WebColors = new ObservableCollection<WebColor>(WebColorsFactory.Create());
        }
    }



    public class Person : NotifyPropertyChangedAbs
    {
        private State _state;
        private string _name;
        private WebColor _webColor;

        [System.ComponentModel.DataAnnotations.Required]
        public State State
        {
            get => _state;
            set
            {
                _state = value;
                base.OnRaisePropertyChanged();
            }
        }

        [System.ComponentModel.DataAnnotations.Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                base.OnRaisePropertyChanged();
            }
        }

        public WebColor Color
        {
            get => _webColor;
            set
            {
                _webColor = value;
                base.OnRaisePropertyChanged();
            }
        }
    }

    public static class WebColorsFactory
    {
        private static readonly Lazy<List<WebColor>> webColors = new Lazy<List<WebColor>>(GetWebColors);

        private static List<WebColor> GetWebColors()
        {
            var data = GetConstants(typeof(Colors));
            return data.Select(kvp => new WebColor(kvp.Value, kvp.Key)).ToList();
        }

        public static IList<WebColor> Create()
        {
            return webColors.Value;
        }

        static List<KeyValuePair<string, Color>> GetConstants(Type sourceType)
        {
            var properties = sourceType
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.PropertyType == typeof(Color));
            var list = new List<KeyValuePair<string, Color>>();
            MethodInfo getMethod = null;
            foreach (var info in properties)
            {
                if (getMethod == null)
                    getMethod = info.GetGetMethod();
                var c = (Color)info.GetValue(null, null);
                list.Add(new KeyValuePair<string, Color>(info.Name, c));
            }
            return list;
        }

        public static WebColor FromColor(Color color)
        {
            return webColors.Value.FirstOrDefault(c => c.Color == color);
        }

        public static WebColor FromName(string name)
        {
            return webColors.Value.FirstOrDefault(c => string.Equals(name, c.Name, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [DebuggerDisplay("WebColor = {Name} {HexCode}")]
    public class WebColor : NotifyPropertyChangedAbs
    {
        private string _name;
        private string _hexCode;
        private string _decimalCode;
        private Color _color;
        private SolidColorBrush _brush;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                base.OnRaisePropertyChanged();
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                Brush = new SolidColorBrush(value);
                HexCode = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
                DecimalCode = $"{value.R}, {value.G}, {value.B}";
                base.OnRaisePropertyChanged();
            }
        }

        public SolidColorBrush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                base.OnRaisePropertyChanged();
            }
        }

        public string HexCode
        {
            get { return _hexCode; }
            private set
            {
                _hexCode = value;
                base.OnRaisePropertyChanged();
            }
        }

        public string DecimalCode
        {
            get { return _decimalCode; }
            private set
            {
                _decimalCode = value;
                base.OnRaisePropertyChanged();
            }
        }

        public WebColor()
        { }

        public WebColor(Color c, string name = null)
        {
            Name = name ?? c.GetType().Name;
            Color = c;
        }
    }


    /// <summary>
    /// Sort color by HSB
    /// </summary>
    public class WebColorComparer : IComparer<WebColor>
    {
        private static readonly Comparer<float> floatComparer = Comparer<float>.Default;

        public static System.Drawing.Color ToDrawingColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public int Compare(WebColor x, WebColor y)
        {
            if (x == null)
                return y == null ? 0 : 1;
            if (y == null)
                return -1;

            var color1 = ToDrawingColor(x.Color);
            var color2 = ToDrawingColor(y.Color);

            int result = floatComparer.Compare(color1.A, color2.A);
            if (result != 0) return result;

            result = floatComparer.Compare(color1.GetHue(), color2.GetHue());
            if (result != 0) return result;

            result = floatComparer.Compare(color1.GetSaturation(), color2.GetSaturation());
            if (result != 0) return result;

            result = floatComparer.Compare(color1.GetBrightness(), color2.GetBrightness());
            if (result != 0) return result;

            return 0;
        }
    }

    [DebuggerDisplay("State = {Name} ({Abbreviation})")]
    public class State
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }

    /// <summary>
    /// Return a static list of all U.S. states
    /// </summary>
    /// <returns></returns>
    public static class StateFactory
    {
        private static Lazy<List<State>> states = new Lazy<List<State>>(GetStateList);

        private static List<State> GetStateList()
        {
            return new List<State>
            {
                new State { Abbreviation = "AL", Name = "Alabama" },
                new State { Abbreviation = "AK", Name = "Alaska" },
                new State { Abbreviation = "AZ", Name = "Arizona" },
                new State { Abbreviation = "AR", Name = "Arkansas" },
                new State { Abbreviation = "CA", Name = "California" },
                new State { Abbreviation = "CO", Name = "Colorado" },
                new State { Abbreviation = "CT", Name = "Connecticut" },
                new State { Abbreviation = "DE", Name = "Delaware" },
                new State { Abbreviation = "FL", Name = "Florida" },
                new State { Abbreviation = "GA", Name = "Georgia" },
                new State { Abbreviation = "HI", Name = "Hawaii" },
                new State { Abbreviation = "ID", Name = "Idaho" },
                new State { Abbreviation = "IL", Name = "Illinois" },
                new State { Abbreviation = "IN", Name = "Indiana" },
                new State { Abbreviation = "IA", Name = "Iowa" },
                new State { Abbreviation = "KS", Name = "Kansas" },
                new State { Abbreviation = "KY", Name = "Kentucky" },
                new State { Abbreviation = "LA", Name = "Louisiana" },
                new State { Abbreviation = "ME", Name = "Maine" },
                new State { Abbreviation = "MD", Name = "Maryland" },
                new State { Abbreviation = "MA", Name = "Massachusetts" },
                new State { Abbreviation = "MI", Name = "Michigan" },
                new State { Abbreviation = "MN", Name = "Minnesota" },
                new State { Abbreviation = "MS", Name = "Mississippi" },
                new State { Abbreviation = "MO", Name = "Missouri" },
                new State { Abbreviation = "MT", Name = "Montana" },
                new State { Abbreviation = "NE", Name = "Nebraska" },
                new State { Abbreviation = "NV", Name = "Nevada" },
                new State { Abbreviation = "NH", Name = "New Hampshire" },
                new State { Abbreviation = "NJ", Name = "New Jersey" },
                new State { Abbreviation = "NM", Name = "New Mexico" },
                new State { Abbreviation = "NY", Name = "New York" },
                new State { Abbreviation = "NC", Name = "North Carolina" },
                new State { Abbreviation = "ND", Name = "North Dakota" },
                new State { Abbreviation = "OH", Name = "Ohio" },
                new State { Abbreviation = "OK", Name = "Oklahoma" },
                new State { Abbreviation = "OR", Name = "Oregon" },
                new State { Abbreviation = "PA", Name = "Pennsylvania" },
                new State { Abbreviation = "RI", Name = "Rhode Island" },
                new State { Abbreviation = "SC", Name = "South Carolina" },
                new State { Abbreviation = "SD", Name = "South Dakota" },
                new State { Abbreviation = "TN", Name = "Tennessee" },
                new State { Abbreviation = "TX", Name = "Texas" },
                new State { Abbreviation = "UT", Name = "Utah" },
                new State { Abbreviation = "VT", Name = "Vermont" },
                new State { Abbreviation = "VA", Name = "Virginia" },
                new State { Abbreviation = "WA", Name = "Washington" },
                new State { Abbreviation = "WV", Name = "West Virginia" },
                new State { Abbreviation = "WI", Name = "Wisconsin" },
                new State { Abbreviation = "WY", Name = "Wyoming" },
            };
        }

        public static IList<State> CreateStateList()
        {
            return states.Value;
        }

        private static Lazy<Dictionary<string, State>> abbrLookup =
            new Lazy<Dictionary<string, State>>(GetAbbrLookup);

        private static Dictionary<string, State> GetAbbrLookup()
        {
            return states.Value.ToDictionary(x => x.Abbreviation, StringComparer.CurrentCultureIgnoreCase);
        }

        public static State FromAbbreviation(string abbreviation)
        {
            if (abbrLookup.Value.TryGetValue(abbreviation, out State value))
                return value;

            return null;
        }
    }
}
