using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;

namespace Notepad2.Finding.NotepadItemFinding
{
    public class FoundNotepadItemViewModel : BaseViewModel
    {
        private TextDocumentViewModel _notepad;
        public TextDocumentViewModel Notepad
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
