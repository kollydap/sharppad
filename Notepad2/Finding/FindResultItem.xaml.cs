using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad2.Finding
{
    /// <summary>
    /// Interaction logic for FindResultItem.xaml
    /// </summary>
    public partial class FindResultItem : UserControl
    {
        public Action<FindResultItem> HighlightCallback { get; set; }

        public FindResult Result { get; set; }

        public FindResultItem(FindResult result)
        {
            InitializeComponent();
            DataContext = result;
            Result = result;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HighlightCallback?.Invoke(this);
        }
    }
}
