using System.Windows.Controls;

namespace Notepad2.InformationStuff
{
    /// <summary>
    /// Interaction logic for ErrorListItem.xaml
    /// </summary>
    public partial class InformationListItem : UserControl
    {
        public InformationListItem(InformationModel im)
        {
            InitializeComponent();
            DataContext = im;
        }
    }
}
