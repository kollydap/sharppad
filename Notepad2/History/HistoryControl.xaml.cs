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
