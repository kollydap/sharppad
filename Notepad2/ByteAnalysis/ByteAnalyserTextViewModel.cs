using Notepad2.Utilities;
using System.Collections.ObjectModel;

namespace Notepad2.ByteAnalysis
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
