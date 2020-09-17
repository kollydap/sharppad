using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
