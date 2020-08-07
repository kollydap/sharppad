using Notepad2.Interfaces;
using System.Windows;

namespace Notepad2.Notepad.FileProperties
{
    /// <summary>
    /// Interaction logic for FilePropertiesWindow.xaml
    /// </summary>
    public partial class FilePropertiesWindow : Window, IView
    {
        public FilePropertiesViewModel Properties
        {
            get => this.DataContext as FilePropertiesViewModel;
            set => this.DataContext = value;
        }

        public FilePropertiesWindow()
        {
            InitializeComponent();
            Closing += FilePropertiesWindow_Closing;

            Properties = new FilePropertiesViewModel(this);
        }

        private void FilePropertiesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void HideView()
        {
            this.Hide();
        }

        public void ShowView()
        {
            this.Show();
        }
    }
}
