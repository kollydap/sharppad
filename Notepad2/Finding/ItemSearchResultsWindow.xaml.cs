using Notepad2.Utilities;
using System.Windows;

namespace Notepad2.Finding
{
    /// <summary>
    /// Interaction logic for ItemSearchResultsWindow.xaml
    /// </summary>
    public partial class ItemSearchResultsWindow : Window
    {
        public ItemSearchResultsWindow()
        {
            InitializeComponent();

            Closing += ItemSearchResultsWindow_Closing;
        }

        private void ItemSearchResultsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void ForceClose()
        {
            Closing -= ItemSearchResultsWindow_Closing;
            Close();
        }

        public void ShowWindow()
        {
            Show();
        }
    }
}
