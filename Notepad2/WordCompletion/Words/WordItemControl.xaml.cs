using System.Windows.Controls;

namespace Notepad2.WordCompletion.Words
{
    /// <summary>
    /// Interaction logic for WordItemControl.xaml
    /// </summary>
    public partial class WordItemControl : UserControl
    {
        public string Text
        {
            get => txt.Text;
            set => txt.Text = value;
        }

        public WordItemControl()
        {
            InitializeComponent();
        }

        public WordItemControl(string word)
        {
            InitializeComponent();
            Text = word;
        }
    }
}
