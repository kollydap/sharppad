using SharpPad.FileExplorer;
using SharpPad.InformationStuff;
using SharpPad.Preferences;
using SharpPad.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SharpPad.FileChangeWatcher
{
    /// <summary>
    /// A class for watching multiple FileWatchers and checking if their associated
    /// document's contents have changed. <see cref="DocumentWatcher"/>s do not work on their own,
    /// they require this class.
    /// </summary>
    public static class ApplicationFileWatcher
    {
        private static List<DocumentWatcher> Watchers { get; set; }

        public static bool IsRunning { get; set; }

        static ApplicationFileWatcher()
        {
            Watchers = new List<DocumentWatcher>();
        }

        public static void AddDocumentToWatcher(DocumentWatcher d)
        {
            if (!Watchers.Contains(d))
                Watchers.Add(d);
        }

        public static void RemoveDocumentFromWatcher(DocumentWatcher d)
        {
            Watchers.Remove(d);
        }

        public static void StartDocumentWatcher()
        {
            IsRunning = true;
            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    if (GlobalPreferences.ENABLE_FILE_WATCHER)
                    {
                        CheckForChangedDocuments();
                    }

                    await Task.Delay(2000);
                }
            });
        }

        public static void StopDocumentWatcher()
        {
            IsRunning = false;
        }

        public static void CheckForChangedDocuments()
        {
            try
            {
                // for loop because async modifications stuff
                for (int i = 0; i < Watchers.Count; i++)
                {
                    DocumentWatcher watcher = Watchers[i];
                    if (watcher != null && watcher.IsEnabled && watcher.Document.FilePath.IsFile())
                    {
                        FileInfo fInfo = new FileInfo(watcher.Document.FilePath);
                        if (watcher.Document.FileSizeBytes != fInfo.Length)
                        {
                            watcher.FileContentsChanged?.Invoke();
                        }
                        if (PreferencesG.CHECK_FILENAME_CHANGES_IN_DOCUMENT_WATCHER && watcher.Document.FileName != fInfo.Name)
                        {
                            watcher.FileNameChanged?.Invoke(fInfo.Name);
                        }
                        watcher.Document.PreviewSetReadOnly(fInfo.IsReadOnly);
                    }
                }
            }
            catch (Exception e)
            {
                Information.Show($"Exception in File Watcher: {e.Message}", "FileWatcher");
            }
        }
    }
}
