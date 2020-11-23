using SharpPad.Notepad;
using SharpPad.Utilities;

namespace SharpPad.ViewModels
{
    public class RichNotepadViewModel : BaseViewModel
    {
        private FormatViewModel _documentFormat;
        private DocumentViewModel _document;
        public FormatViewModel DocumentFormat
        {
            get => _documentFormat;
            set => RaisePropertyChanged(ref _documentFormat, value);
        }
        public DocumentViewModel Document
        {
            get => _document;
            set => RaisePropertyChanged(ref _document, value);
        }

        public RichNotepadViewModel()
        {
            DocumentFormat = new FormatViewModel();
            Document = new DocumentViewModel();
        }

        public void SetNotepad(TextDocumentViewModel fivm)
        {
            this.DocumentFormat = fivm.DocumentFormat;
            this.Document = fivm.Document;
        }
    }
}
