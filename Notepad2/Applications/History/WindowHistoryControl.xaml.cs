using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad2.Applications.History
{
    /// <summary>
    /// Interaction logic for WindowHistoryControl.xaml
    /// </summary>
    public partial class WindowHistoryControl : UserControl
    {
        public WindowHistoryControlViewModel Model
        {
            get => this.DataContext as WindowHistoryControlViewModel;
            set => this.DataContext = value;
        }

        public WindowHistoryControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Model.ReopenWindow();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Model.ReopenWindow();
        }
    }
}
