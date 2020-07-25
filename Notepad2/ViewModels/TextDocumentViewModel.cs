using Notepad2.Finding;
using Notepad2.Notepad;
using Notepad2.Utilities;

namespace Notepad2.ViewModels
{
    /// <summary>
    /// Contains everything a notepad would contain; text, wrapping, font size, font, ect
    /// </summary>
    public class TextDocumentViewModel : BaseViewModel
    {
        private FormatModel _documentFormat;
        private DocumentModel _document;
        private FindViewModel _findResults;
        //private TextEditorLinesViewModel _linesCounter;
        private bool _hasMadeChanges;

        public FormatModel DocumentFormat
        {
            get => _documentFormat;
            set => RaisePropertyChanged(ref _documentFormat, value, Update);
        }

        public DocumentModel Document
        {
            get => _document;
            set => RaisePropertyChanged(ref _document, value, Update);
        }

        public FindViewModel FindResults
        {
            get => _findResults;
            set => RaisePropertyChanged(ref _findResults, value);
        }

        ///// <summary>
        ///// Used for showing the lines count
        ///// </summary>
        //public TextEditorLinesViewModel LinesCounter
        //{
        //    get => _linesCounter;
        //    set => RaisePropertyChanged(ref _linesCounter, value);
        //}

        public bool HasMadeChanges
        {
            get => _hasMadeChanges; set => RaisePropertyChanged(ref _hasMadeChanges, value);
        }

        public void Update()
        {
            //LinesCounter.Document = Document;
            //LinesCounter.DocumentFormat = DocumentFormat;
            //LinesCounter.UpdateDocuments(Document, DocumentFormat);
        }

        public TextDocumentViewModel()
        {
            //LinesCounter = new TextEditorLinesViewModel();
            DocumentFormat = new FormatModel();
            Document = new DocumentModel();
            FindResults = new FindViewModel(Document);
            HasMadeChanges = false;
            Document.TextChanged += TextChanged;
        }

        private void TextChanged(string newText)
        {
            HasMadeChanges = true;
        }
    }
}
