using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;

namespace Notepad2.Applications.History
{
    public class WindowHistoryControlViewModel : BaseViewModel
    {
        private NotepadViewModel _notepad;
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public Action<WindowHistoryControlViewModel> ReopenWindowCallback { get; set; }

        public WindowHistoryControlViewModel() { }

        public WindowHistoryControlViewModel(NotepadViewModel notepad)
        {
            Notepad = notepad;
        }

        public void ReopenWindow()
        {
            ReopenWindowCallback?.Invoke(this);
        }
    }
}
