using Notepad2.Notepad;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;

namespace Notepad2.Finding.NotepadItemFinding
{
    public class FoundNotepadItemViewModel : BaseViewModel
    {
        private NotepadItemViewModel _notepad;
        public NotepadItemViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public Action<FoundNotepadItemViewModel> SelectNotepadCallback { get; set; }

        public void Select()
        {
            SelectNotepadCallback?.Invoke(this);
        }
    }
}
