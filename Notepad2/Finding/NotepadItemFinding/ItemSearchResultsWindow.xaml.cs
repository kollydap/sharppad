using System.Windows;
using System.Windows.Input;

namespace SharpPad.Finding.NotepadItemFinding
{
    /// <summary>
    /// Interaction logic for ItemSearchResultsWindow.xaml
    /// </summary>
    public partial class ItemSearchResultsWindow : Window
    {
        public ItemSearchResultsWindow()
        {
            InitializeComponent();

            KeyDown += ItemSearchResultsWindow_KeyDown;
            Closing += ItemSearchResultsWindow_Closing;
        }

        private void ItemSearchResultsWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                case Key.Enter:
                    this.Close();
                    break;
            }
        }

        private void ItemSearchResultsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void ForceClose()
        {
            PreviewKeyDown -= ItemSearchResultsWindow_KeyDown;
            Closing -= ItemSearchResultsWindow_Closing;
            Close();
        }

        public void ShowWindow()
        {
            Show();
        }
    }
}
