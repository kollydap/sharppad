using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;

namespace Notepad2.Applications.History
{
    /// <summary>
    /// A ViewModel for a history item. Contains a Notepad Window's DataContext
    /// </summary>
    public class WindowHistoryControlViewModel : BaseViewModel
    {
        private NotepadViewModel _notepad;
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public Action<WindowHistoryControlViewModel> ReopenNotepadCallback { get; set; }

        public WindowHistoryControlViewModel() { }

        public WindowHistoryControlViewModel(NotepadViewModel notepad)
        {
            Notepad = notepad;
        }

        public void ReopenWindow()
        {
            ReopenNotepadCallback?.Invoke(this);
        }
    }
}
