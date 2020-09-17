using System.Windows.Controls;

namespace Notepad2.ByteAnalysis
{
    /// <summary>
    /// Interaction logic for CharacterControl.xaml
    /// </summary>
    public partial class CharacterControl : UserControl
    {
        //public char Character
        //{
        //    get => _charBox.Text != null ? _charBox.Text[0]: (char)0;
        //    set => _charBox.Text = value.ToString();
        //}

        public Character Character
        {
            get => DataContext as Character;
            set => DataContext = value;
        }

        public CharacterControl()
        {
            InitializeComponent();
        }
    }
}
