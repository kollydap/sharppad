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
        public Action<string> FileContentsChanged { get; set; }
        //public Action FilePathChanged { get; set; }

        public bool Running { get; set; }
        public bool Paused { get; set; }

        private DocumentModel Document { get; set; }

        public FileWatcher(DocumentModel doc)
        {
            Document = doc;
        }

        public void StartWatching()
        {
            Running = true;
            Task.Run(async () =>
            {
                while (Running)
                {
                    if (GlobalPreferences.ENABLE_FILE_WATCHER)
                    {
                        if (Document.FilePath.IsFile())
                        {
                            string content = File.ReadAllText(Document.FilePath);

                            if (content != Document.Text)
                            {
                                // Changed
                                FileContentsChanged?.Invoke(content);
                            }
                        }
                        // Breaks the special drag drop. not using.
                        //else
                        //{
                        //    FilePathChanged?.Invoke();
                        //}
                    }

                    while (Paused) { await Task.Delay(100); }

                    await Task.Delay(2000);
                }
            });
        }

        public void StopWatching()
        {
            Running = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        public void UnPause()
        {
            Paused = false;
        }
    }
}
