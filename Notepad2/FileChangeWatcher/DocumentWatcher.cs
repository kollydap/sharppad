using SharpPad.Notepad;
using System;

namespace SharpPad.FileChangeWatcher
{
    /// <summary>
    /// A class for constantly checking if a file's contents 
    /// have changed, and calling a callback if so
    /// </summary>
    public class DocumentWatcher
    {
        public bool IsEnabled { get; set; }

        public DocumentViewModel Document { get; set; }

        // Doesn't have text as a param because that could be
        // very laggy if the file is very big 
        public Action FileContentsChanged { get; set; }
        public Action<string> FileNameChanged { get; set; }

        //public Action FilePathChanged { get; set; }

        public DocumentWatcher(DocumentViewModel doc)
        {
            Document = doc;
        }

        public void StartWatching()
        {
            IsEnabled = true;
            ApplicationFileWatcher.AddDocumentToWatcher(this);
        }

        public void FullyStopWatching()
        {
            IsEnabled = false;
            ApplicationFileWatcher.RemoveDocumentFromWatcher(this);
        }
    }
}
