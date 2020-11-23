using System.Windows;

namespace SharpPad.ByteAnalysis
{
    /// <summary>
    /// Interaction logic for ByteAnalyserTextWindow.xaml
    /// </summary>
    public partial class ByteAnalyserTextWindow : Window
    {
        public ByteAnalyserTextViewModel ViewModel
        {
            get => DataContext as ByteAnalyserTextViewModel;
            set => DataContext = value;
        }

        public ByteAnalyserTextWindow()
        {
            InitializeComponent();
        }
    }
}
