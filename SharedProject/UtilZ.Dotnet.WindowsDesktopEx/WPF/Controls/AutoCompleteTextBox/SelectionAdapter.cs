using System;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectionAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        public SelectionAdapter(Selector selector)
        {
            SelectorControl = selector;
            SelectorControl.PreviewMouseUp += OnSelectorMouseDown;
        }



        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Cancel;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Commit;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler SelectionChanged;


        /// <summary>
        /// 
        /// </summary>
        public Selector SelectorControl { get; set; }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void HandleKeyDown(KeyEventArgs key)
        {
            //Debug.WriteLine(key.Key);
            switch (key.Key)
            {
                case Key.Down:
                    IncrementSelection();
                    break;
                case Key.Up:
                    DecrementSelection();
                    break;
                case Key.Enter:
                    Commit?.Invoke(this, new EventArgs());

                    break;
                case Key.Escape:
                    Cancel?.Invoke(this, new EventArgs());

                    break;
                case Key.Tab:
                    Commit?.Invoke(this, new EventArgs());

                    break;
                default:
                    return;
            }
            key.Handled = true;
        }

        private void DecrementSelection()
        {
            if (SelectorControl.SelectedIndex == -1)
            {
                SelectorControl.SelectedIndex = SelectorControl.Items.Count - 1;
            }
            else
            {
                SelectorControl.SelectedIndex -= 1;
            }

            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void IncrementSelection()
        {
            if (SelectorControl.SelectedIndex == SelectorControl.Items.Count - 1)
            {
                SelectorControl.SelectedIndex = -1;
            }
            else
            {
                SelectorControl.SelectedIndex += 1;
            }

            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void OnSelectorMouseDown(object sender, MouseButtonEventArgs e)
        {
            Commit?.Invoke(sender, e);
            e.Handled = true;
        }
    }

}