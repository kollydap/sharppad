using System.Windows;

namespace Notepad2.Applications
{
    /// <summary>
    /// Interaction logic for WindowsPrviewWindow.xaml
    /// </summary>
    public partial class WindowsPrviewWindow : Window
    {
        public ApplicationViewModel ThisApp
        {
            get => this.DataContext as ApplicationViewModel;
            set => this.DataContext = value;
        }

        public WindowsPrviewWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
