using SharpPad.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPad.Applications.Controls
{
    /// <summary>
    /// Interaction logic for WindowPreviewControl.xaml
    /// </summary>
    public partial class WindowPreviewControl : UserControl
    {
        public WindowPreviewControlViewModel Preview
        {
            get => this.DataContext as WindowPreviewControlViewModel;
            set => this.DataContext = value;
        }

        public WindowPreviewControl(NotepadViewModel notepad)
        {
            InitializeComponent();
            Preview.Notepad = notepad;
        }

        public WindowPreviewControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Preview.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Preview.FocusNotepad();
            }
        }
        private void FocusWindowClick(object sender, RoutedEventArgs e)
        {
            Preview.FocusNotepad();
        }
    }
}
