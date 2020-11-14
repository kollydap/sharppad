using Notepad2.FileExplorer;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Notepad2.FileChangeWatcher
{
    /// <summary>
    /// A class for constantly checking if a file's contents 
    /// have changed, and calling a callback if so
    /// </summary>
    public class FileWatcher
    {
        // Doesn't have text as a param because that could be
        // very laggy if the file is very big 
        public Action FileContentsChanged { get; set; }
        public Action<string> FileNameChanged { get; set; }
        //public Action FilePathChanged { get; set; }

        public bool IsEnabled { get; set; }

        public DocumentModel Document { get; set; }

        public FileWatcher(DocumentModel doc)
        {
            Document = doc;
        }

        public void StartWatching()
        {
            IsEnabled = true;
            ApplicationFileWatcher.AddDocumentToWatcher(this);
        }

        public void StopWatching()
        {
            IsEnabled = false;
            ApplicationFileWatcher.RemoveDocumentFromWatcher(this);
        }
    }
}
