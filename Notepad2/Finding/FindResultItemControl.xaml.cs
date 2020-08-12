using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad2.Finding
{
    /// <summary>
    /// Interaction logic for FindResultItemControl.xaml
    /// </summary>
    public partial class FindResultItemControl : UserControl
    {
        public FindResultItemViewModel ResultModel
        {
            get => this.DataContext as FindResultItemViewModel;
        }

        public FindResultItemControl()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                ResultModel.Highlight();
        }
    }
}
