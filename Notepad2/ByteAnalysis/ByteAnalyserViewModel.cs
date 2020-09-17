using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.ByteAnalysis
{
    public class ByteAnalyserViewModel : BaseViewModel
    {
        private char _character;
        public char Character
        {
            get => _character;
            set => RaisePropertyChanged(ref _character, value);
        }

        public void Update(Character character)
        {

        }
    }
}
