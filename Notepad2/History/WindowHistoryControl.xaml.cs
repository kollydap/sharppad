using Notepad2.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad2.History
{
    /// <summary>
    /// Interaction logic for WindowHistoryControl.xaml
    /// </summary>
    public partial class WindowHistoryControl : UserControl
    {
        public NotepadViewModel Model
        {
            get => this.DataContext as NotepadViewModel;
            set => this.DataContext = value;
        }

        public Action<WindowHistoryControl> ReopenWindowCallback { get; set; }

        public WindowHistoryControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReopenWindowCallback?.Invoke(this);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReopenWindowCallback?.Invoke(this);
        }
    }
}
