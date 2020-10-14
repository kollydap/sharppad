using Notepad2.Utilities;

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
