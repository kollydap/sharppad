using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notepad2.Applications.Controls
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
                Preview.FocusWindow();
            }
        }
        private void FocusWindowClick(object sender, RoutedEventArgs e)
        {
            Preview.FocusWindow();
        }
    }
}
