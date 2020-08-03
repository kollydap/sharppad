using Notepad2.ViewModels;
using System;
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
        public TextDocumentViewModel Model
        {
            get => this.DataContext as TextDocumentViewModel;
            set => this.DataContext = value;
        }

        public Action<HistoryControl> ReopenFileCallback { get; set; }

        public HistoryControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReopenFileCallback?.Invoke(this);
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReopenFileCallback?.Invoke(this);
        }
    }
}
