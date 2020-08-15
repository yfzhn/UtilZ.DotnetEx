using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CoreWpfApp
{
    /// <summary>
    /// TestAutoCompleteTextBox222.xaml 的交互逻辑
    /// </summary>
    public partial class TestAutoCompleteTextBox222 : Window
    {
        public TestAutoCompleteTextBox222()
        {
            InitializeComponent();
        }


        private void Test()
        {
            this.Template.FindName("", this);
        }


        private TestAutoCompleteTextBox222VM VM
        {
            get { return (TestAutoCompleteTextBox222VM)this.DataContext; }
        }


        /// <summary>
        /// 上次选中项
        /// </summary>
        private ListItemModel _lastSelectedItem = null;
        /// <summary>
        /// TextChanged是否是因提示框选择而发生的更改,如果是则为true且TextChanged事件就不再触发,触发
        /// </summary>
        private bool _isSelectedItemChanged = false;
        private void txtKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var index = txtNamesListBox.Items.IndexOf(_lastSelectedItem);

                switch (e.Key)
                {
                    case Key.Enter:
                        this.VM.Items.Clear();
                        popup.IsOpen = false;
                        break;
                    case Key.Escape:
                        txtKey.Text = string.Empty;
                        return;
                    case Key.Up:
                        if (this.VM.Items.Count == 0 || !popup.IsOpen)
                        {
                            return;
                        }
                        if (index == -1 || index == 0)
                        {
                            index = txtNamesListBox.Items.Count - 1;
                        }
                        else
                        {
                            --index;
                        }

                        txtNamesListBox.SelectedIndex = index;
                        var upItem = txtNamesListBox.Items[txtNamesListBox.SelectedIndex] as ListItemModel;
                        if (_lastSelectedItem != null)
                        {
                            _lastSelectedItem.IsSelected = false;
                        }
                        upItem.IsSelected = true;
                        _lastSelectedItem = upItem;
                        break;
                    case Key.Down:
                        if (this.VM.Items.Count == 0 || !popup.IsOpen)
                        {
                            return;
                        }
                        ++index;
                        if (index == txtNamesListBox.Items.Count)
                        {
                            index = 0;
                        }

                        txtNamesListBox.SelectedIndex = index;
                        var downItem = txtNamesListBox.Items[txtNamesListBox.SelectedIndex] as ListItemModel;
                        if (_lastSelectedItem != null)
                        {
                            _lastSelectedItem.IsSelected = false;
                        }
                        downItem.IsSelected = true;
                        _lastSelectedItem = downItem;
                        break;
                    default:
                        _isSelectedItemChanged = false;
                        break;
                }
            }));
        }

        private void txtNamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isSelectedItemChanged = true;
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            var item = e.AddedItems[0] as ListItemModel;
            txtKey.Text = item.Text;
            txtKey.SelectionStart = txtKey.Text.Length;

        }

        private void txtKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSelectedItemChanged)
            {
                _isSelectedItemChanged = false;
                return;
            }

            if (this.VM.Items.Count == 0)
            {
                this.VM.InitItems();
            }
            if (popup != null && !popup.IsOpen)
            {
                popup.IsOpen = true;
            }
        }

        private void txtKey_GotFocus(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void txtKey_LostFocus(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }



        private void gridItem_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (_lastSelectedItem != null)
                {
                    _lastSelectedItem.IsSelected = false;
                }
            }));
        }

        private void gridItem_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var item = ((Grid)sender).DataContext as ListItemModel;
                if (_lastSelectedItem != null)
                {
                    _lastSelectedItem.IsSelected = false;
                }
                item.IsSelected = true;
                _lastSelectedItem = item;
            }));
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
            }
        }
    }

    internal class TestAutoCompleteTextBox222VM : NotifyPropertyChangedAbs
    {
        private ObservableCollection<ListItemModel> _items = new ObservableCollection<ListItemModel>();

        public ObservableCollection<ListItemModel> Items
        {
            get { return _items; }
            set { _items = value; }
        }



        public TestAutoCompleteTextBox222VM()
        {
            this.InitItems();
        }


        public void InitItems()
        {
            _items.Add(new ListItemModel("1dsfadsgsdfgsdfg"));
            _items.Add(new ListItemModel("2oklkkkkkkkkkkgjg"));
            _items.Add(new ListItemModel("3uyjghjnhgjfhfg"));
            _items.Add(new ListItemModel("4ljkghnklbcvxlbmcv"));
            _items.Add(new ListItemModel("5ghjffhn"));
        }
    }

    internal class ListItemModel : NotifyPropertyChangedAbs
    {
        private string _text = string.Empty;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                base.OnRaisePropertyChanged();
            }
        }
        private bool _isSelected = false;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                base.OnRaisePropertyChanged();
            }
        }

        public ListItemModel(String str)
        {
            Text = str;
        }
    }
}
