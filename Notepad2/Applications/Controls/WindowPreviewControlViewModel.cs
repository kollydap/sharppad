using SharpPad.Utilities;
using SharpPad.ViewModels;
using System;

namespace SharpPad.Applications.Controls
{
    public class WindowPreviewControlViewModel : BaseViewModel
    {
        private NotepadViewModel _notepad;
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public Action<WindowPreviewControlViewModel> CloseNotepadCallback { get; set; }
        public Action<NotepadViewModel> FocusNotepadCallback { get; set; }

        public WindowPreviewControlViewModel(NotepadViewModel notepad)
        {
            Notepad = notepad;
        }

        public WindowPreviewControlViewModel() { }

        public void FocusNotepad()
        {
            FocusNotepadCallback?.Invoke(Notepad);
        }

        public void Close()
        {
            CloseNotepadCallback?.Invoke(this);
        }
    }
}
