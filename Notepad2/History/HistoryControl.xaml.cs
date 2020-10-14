using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad2.History
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        public HistoryItemViewModel Model
        {
            get => this.DataContext as HistoryItemViewModel;
            set => this.DataContext = value;
        }

        public HistoryControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.ReopenFile();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Model.ReopenFile();
        }
    }
}
