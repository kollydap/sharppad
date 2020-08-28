using Notepad2.Notepad;
using Notepad2.Utilities;
using System;

namespace Notepad2.History
{
    public class HistoryItemViewModel : BaseViewModel
    {
        private NotepadItemViewModel _textDocument;
        public NotepadItemViewModel TextDocument
        {
            get => _textDocument;
            set => RaisePropertyChanged(ref _textDocument, value);
        }

        public Action<HistoryItemViewModel> ReopenFileCallback { get; set; }

        public void ReopenFile()
        {
            ReopenFileCallback?.Invoke(this);
        }

        public HistoryItemViewModel()
        {

        }
    }
}
