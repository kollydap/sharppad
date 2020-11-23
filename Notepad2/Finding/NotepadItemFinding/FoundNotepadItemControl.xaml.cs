using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPad.Finding.NotepadItemFinding
{
    /// <summary>
    /// Interaction logic for FoundNotepadItemControl.xaml
    /// </summary>
    public partial class FoundNotepadItemControl : UserControl
    {
        public FoundNotepadItemControl()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (DataContext is FoundNotepadItemViewModel doc)
                {
                    doc.Select();
                    return;
                }
            }
        }
    }
}
