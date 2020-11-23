using SharpPad.Utilities;
using System.Collections.ObjectModel;

namespace SharpPad.ByteAnalysis
{
    public class ByteAnalyserTextViewModel : BaseViewModel
    {
        public ObservableCollection<Character> Text { get; set; }

        public ByteAnalyserViewModel Analyser { get; set; }

        public ByteAnalyserTextViewModel()
        {
            Analyser = new ByteAnalyserViewModel();
        }
    }
}
