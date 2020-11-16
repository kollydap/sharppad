using Notepad2.FileChangeWatcher;
using Notepad2.FileExplorer;
using Notepad2.Finding.TextFinding;
using Notepad2.InformationStuff;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System;
using System.IO;

namespace Notepad2.ViewModels
{
    /// <summary>
    /// Contains everything a notepad would contain; text, wrapping, font size, font, ect
    /// </summary>
    public class TextDocumentViewModel : BaseViewModel
    {
        private FormatViewModel _documentFormat;
        private DocumentViewModel _document;
        private FindReplaceViewModel _findResults;
        //private TextEditorLinesViewModel _linesCounter;
        private bool _hasMadeChanges;

        public FormatViewModel DocumentFormat
        {
            get => _documentFormat;
            set => RaisePropertyChanged(ref _documentFormat, value /*, Update*/);
        }

        public DocumentViewModel Document
        {
            get => _document;
            set => RaisePropertyChanged(ref _document, value /*, Update*/);
        }

        public FindReplaceViewModel FindResults
        {
            get => _findResults;
            set => RaisePropertyChanged(ref _findResults, value);
        }

        public bool HasMadeChanges
        {
            get => _hasMadeChanges;
            set => RaisePropertyChanged(ref _hasMadeChanges, value);
        }

        public DocumentWatcher Watcher { get; set; }

        ///// <summary>
        ///// Used for showing the lines count
        ///// </summary>
        //public TextEditorLinesViewModel LinesCounter
        //{
        //    get => _linesCounter;
        //    set => RaisePropertyChanged(ref _linesCounter, value);
        //}

        //public void Update()
        //{
        //    LinesCounter.Document = Document;
        //    LinesCounter.DocumentFormat = DocumentFormat;
        //    LinesCounter.UpdateDocuments(Document, DocumentFormat);
        //}

        public TextDocumentViewModel()
        {
            //LinesCounter = new TextEditorLinesViewModel();
            DocumentFormat = new FormatViewModel();
            Document = new DocumentViewModel();
            FindResults = new FindReplaceViewModel(Document);
            Watcher = new DocumentWatcher(Document);
            Watcher.FileContentsChanged = FileContentsChanged;
            Watcher.FileNameChanged = FileNameChanged;
            //Watcher.FilePathChanged = FilePathChangedToEmpty;
            Watcher.StartWatching();
            HasMadeChanges = false;
            Document.TextChanged = TextChanged;
        }

        public void FileContentsChanged()
        {
            // will only update the file if there's no changes made
            // meaning... it wont change your unsaved work.
            if (!HasMadeChanges)
            {
                if (Document.FilePath.IsFile())
                    Document.Text = NotepadActions.ReadFile(Document.FilePath);
                //HasMadeChanges = false;
            }
        }

        private void FileNameChanged(string newName)
        {
            Document.FileName = newName;
        }

        public void UpdateFileContents(bool displayHasMadeChanges = false)
        {
            if (Document.FilePath.IsFile())
            {
                FileInfo fInfo = new FileInfo(Document.FilePath);
                if (Document.FileSizeBytes != fInfo.Length)
                {
                    Document.Text = File.ReadAllText(Document.FilePath);
                    HasMadeChanges = displayHasMadeChanges;
                    Information.Show($"Refreshed the contents of [{Document.FileName}]", InfoTypes.FileIO);
                }
            }
        }

        private void TextChanged()
        {
            HasMadeChanges = true;
        }
    }
}
