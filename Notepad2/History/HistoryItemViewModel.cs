using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Notepad2.History
{
    public class HistoryItemViewModel : BaseViewModel
    {
        private TextDocumentViewModel _textDocument;
        public TextDocumentViewModel TextDocument
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
