using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = PartEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PartSelector, Type = typeof(Selector))]
    public class AutoCompleteTextBox : Control
    {
        #region ³£Á¿ 
        /// <summary>
        /// 
        /// </summary>
        public const string PartEditor = "PART_Editor";

        /// <summary>
        /// 
        /// </summary>
        public const string PartPopup = "PART_Popup";

        /// <summary>
        /// 
        /// </summary>
        public const string PartSelector = "PART_Selector";
        #endregion


        #region "Fields"
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(200));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplayMemberProperty = DependencyProperty.Register("DisplayMember", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IconPlacementProperty = DependencyProperty.Register("IconPlacement", typeof(IconPlacement), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(IconPlacement.Left));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(AutoCompleteTextBox));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LoadingContentProperty = DependencyProperty.Register("LoadingContent", typeof(object), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register("Provider", typeof(ISuggestionProvider), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.Register("CharacterCasing", typeof(CharacterCasing), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(CharacterCasing.Normal));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaxPopUpHeightProperty = DependencyProperty.Register("MaxPopUpHeight", typeof(int), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(600));

        /// <summary>
        /// 
        /// </summary>

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SuggestionBackgroundProperty = DependencyProperty.Register("SuggestionBackground", typeof(Brush), typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(Brushes.White));
        #endregion




        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));
        }


        #region "Properties"
        /// <summary>
        /// 
        /// </summary>
        public int MaxPopupHeight
        {
            get => (int)GetValue(MaxPopUpHeightProperty);
            set => SetValue(MaxPopUpHeightProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public BindingEvaluator BindingEvaluator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CharacterCasing CharacterCasing
        {
            get => (CharacterCasing)GetValue(CharacterCasingProperty);
            set => SetValue(CharacterCasingProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayMember
        {
            get => (string)GetValue(DisplayMemberProperty);
            set => SetValue(DisplayMemberProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public TextBox Editor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DispatcherTimer FetchTimer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public IconPlacement IconPlacement
        {
            get => (IconPlacement)GetValue(IconPlacementProperty);
            set => SetValue(IconPlacementProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility IconVisibility
        {
            get => (Visibility)GetValue(IconVisibilityProperty);

            set => SetValue(IconVisibilityProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Selector ItemsSelector { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get => ((DataTemplateSelector)(GetValue(ItemTemplateSelectorProperty)));
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public object LoadingContent
        {
            get => GetValue(LoadingContentProperty);
            set => SetValue(LoadingContentProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Popup Popup { get; set; }

        public bool PopupOpened { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public ISuggestionProvider Provider
        {
            get => (ISuggestionProvider)GetValue(ProviderProperty);
            set => SetValue(ProviderProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public SelectionAdapter SelectionAdapter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush SuggestionBackground
        {
            get => (Brush)GetValue(SuggestionBackgroundProperty);
            set => SetValue(SuggestionBackgroundProperty, value);
        }

        #endregion




        private bool _isUpdatingText;
        private bool _selectionCancelled;
        private SuggestionsAdapter _suggestionsAdapter;


        #region "Methods"
        /// <summary>
        /// 
        /// </summary>
        public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox act = null;
            act = d as AutoCompleteTextBox;
            if (act != null)
            {
                if (act.Editor != null & !act._isUpdatingText)
                {
                    act._isUpdatingText = true;
                    act.Editor.Text = act.BindingEvaluator.Evaluate(e.NewValue);
                    act._isUpdatingText = false;
                }
            }
        }

        private void ScrollToSelectedItem()
        {
            if (ItemsSelector is ListBox listBox && listBox.SelectedItem != null)
            {
                listBox.ScrollIntoView(listBox.SelectedItem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Editor = Template.FindName(PartEditor, this) as TextBox;
            Popup = Template.FindName(PartPopup, this) as Popup;
            ItemsSelector = Template.FindName(PartSelector, this) as Selector;
            BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));

            if (Editor != null)
            {
                Editor.TextChanged += OnEditorTextChanged;
                Editor.PreviewKeyDown += OnEditorKeyDown;
                Editor.LostFocus += OnEditorLostFocus;

                if (SelectedItem != null)
                {
                    _isUpdatingText = true;
                    Editor.Text = BindingEvaluator.Evaluate(SelectedItem);
                    _isUpdatingText = false;
                }

            }

            GotFocus += AutoCompleteTextBox_GotFocus;

            if (Popup != null)
            {
                Popup.StaysOpen = false;
                Popup.Opened += OnPopupOpened;
                Popup.Closed += OnPopupClosed;
            }
            if (ItemsSelector != null)
            {
                SelectionAdapter = new SelectionAdapter(ItemsSelector);
                SelectionAdapter.Commit += OnSelectionAdapterCommit;
                SelectionAdapter.Cancel += OnSelectionAdapterCancel;
                SelectionAdapter.SelectionChanged += OnSelectionAdapterSelectionChanged;
                ItemsSelector.PreviewMouseDown += ItemsSelector_PreviewMouseDown;
            }
        }
        private void ItemsSelector_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement)?.DataContext == null)
                return;
            if (!ItemsSelector.Items.Contains(((FrameworkElement)e.OriginalSource)?.DataContext))
                return;
            ItemsSelector.SelectedItem = ((FrameworkElement)e.OriginalSource)?.DataContext;
            OnSelectionAdapterCommit(sender, e);
            e.Handled = true;
        }
        private void AutoCompleteTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Editor?.Focus();
        }

        private string GetDisplayText(object dataItem)
        {
            if (BindingEvaluator == null)
            {
                BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));
            }
            if (dataItem == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(DisplayMember))
            {
                return dataItem.ToString();
            }
            return BindingEvaluator.Evaluate(dataItem);
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (SelectionAdapter != null)
            {
                if (IsDropDownOpen)
                    SelectionAdapter.HandleKeyDown(e);
                else
                    IsDropDownOpen = e.Key == Key.Down || e.Key == Key.Up;
            }
        }

        private void OnEditorLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                IsDropDownOpen = false;
            }
        }

        private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Editor.Text;
            if (_isUpdatingText)
                return;
            if (FetchTimer == null)
            {
                FetchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Delay) };
                FetchTimer.Tick += OnFetchTimerTick;
            }
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            SetSelectedItem(null);
            if (Editor.Text.Length > 0)
            {
                FetchTimer.IsEnabled = true;
                FetchTimer.Start();
            }
            else
            {
                IsDropDownOpen = false;
            }
        }

        private void OnFetchTimerTick(object sender, EventArgs e)
        {
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            if (Provider != null && ItemsSelector != null)
            {
                Filter = Editor.Text;
                if (_suggestionsAdapter == null)
                {
                    _suggestionsAdapter = new SuggestionsAdapter(this);
                }
                _suggestionsAdapter.GetSuggestions(Filter);
            }
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            this.PopupOpened = false;
            if (!_selectionCancelled)
            {
                OnSelectionAdapterCommit(sender, e);
            }
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            this.PopupOpened = true;
            _selectionCancelled = false;
            ItemsSelector.SelectedItem = SelectedItem;
        }

        private void OnSelectionAdapterCancel(object sender, EventArgs e)
        {
            _isUpdatingText = true;
            Editor.Text = SelectedItem == null ? Filter : GetDisplayText(SelectedItem);
            Editor.SelectionStart = Editor.Text.Length;
            Editor.SelectionLength = 0;
            _isUpdatingText = false;
            IsDropDownOpen = false;
            _selectionCancelled = true;
        }

        private void OnSelectionAdapterCommit(object sender, EventArgs e)
        {
            if (ItemsSelector.SelectedItem != null)
            {
                SelectedItem = ItemsSelector.SelectedItem;
                _isUpdatingText = true;
                Editor.Text = GetDisplayText(ItemsSelector.SelectedItem);
                SetSelectedItem(ItemsSelector.SelectedItem);
                _isUpdatingText = false;
                IsDropDownOpen = false;
            }
        }

        private void OnSelectionAdapterSelectionChanged(object sender, EventArgs e)
        {
            _isUpdatingText = true;
            Editor.Text = ItemsSelector.SelectedItem == null ? Filter : GetDisplayText(ItemsSelector.SelectedItem);
            Editor.SelectionStart = Editor.Text.Length;
            Editor.SelectionLength = 0;
            ScrollToSelectedItem();
            _isUpdatingText = false;
        }

        private void SetSelectedItem(object item)
        {
            _isUpdatingText = true;
            SelectedItem = item;
            _isUpdatingText = false;
        }
        #endregion

        #region "Nested Types"

        private class SuggestionsAdapter
        {

            #region "Fields"

            private readonly AutoCompleteTextBox _actb;

            private string _filter;
            #endregion

            #region "Constructors"

            public SuggestionsAdapter(AutoCompleteTextBox actb)
            {
                _actb = actb;
            }

            #endregion

            #region "Methods"

            public void GetSuggestions(string searchText)
            {
                _filter = searchText;
                _actb.IsLoading = true;
                // Do not open drop down if control is not focused
                if (_actb.IsKeyboardFocusWithin)
                    _actb.IsDropDownOpen = true;
                _actb.ItemsSelector.ItemsSource = null;
                ParameterizedThreadStart thInfo = GetSuggestionsAsync;
                Thread th = new Thread(thInfo);
                th.Start(new object[] { searchText, _actb.Provider });
            }

            private void DisplaySuggestions(IEnumerable suggestions, string filter)
            {
                if (_filter != filter)
                {
                    return;
                }
                _actb.IsLoading = false;
                _actb.ItemsSelector.ItemsSource = suggestions;
                // Close drop down if there are no items
                if (_actb.IsDropDownOpen)
                {
                    _actb.IsDropDownOpen = _actb.ItemsSelector.HasItems;
                }
            }

            private void GetSuggestionsAsync(object param)
            {
                if (param is object[] args)
                {
                    string searchText = Convert.ToString(args[0]);
                    if (args[1] is ISuggestionProvider provider)
                    {
                        IEnumerable list = provider.GetSuggestions(searchText);
                        _actb.Dispatcher.BeginInvoke(new Action<IEnumerable, string>(DisplaySuggestions), DispatcherPriority.Background, list, searchText);
                    }
                }
            }

            #endregion

        }

        #endregion

    }

}
