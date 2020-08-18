using Notepad2.Applications;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad2.Notepad.DragDropping
{
    public static class FileWatchers
    {
        // A prefix to identify files created by sharp pad (aka this program lol)
        public const string DRAG_SRC_PREFIX = "_SHRPDDRGPFX__";

        // Watches the Temp directory
        public static FileSystemWatcher TempWatcher;

        // For every logical drive (C, D, H, etc)
        public static List<FileSystemWatcher> DriveWatchers;

        // The currently being dropped document
        public static TextDocumentViewModel DroppingDocument;

        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        static FileWatchers()
        {
            DriveWatchers = new List<FileSystemWatcher>();

            TempWatcher = new FileSystemWatcher(Path.GetTempPath());
            TempWatcher.Filter = $"{DRAG_SRC_PREFIX}*.txt";
            TempWatcher.NotifyFilter = NotifyFilters.FileName;
            TempWatcher.IncludeSubdirectories = false;
            TempWatcher.EnableRaisingEvents = true;
            TempWatcher.Created += PrefixedFileCreatedInTempDirectory;
        }

        public static void DoingDragDrop(TextDocumentViewModel doc)
        {
            DroppingDocument = doc;
        }

        private static void PrefixedFileCreatedInTempDirectory(object sender, FileSystemEventArgs e)
        {
            ClearDriveWatchers();

            foreach (string drive in Directory.GetLogicalDrives())
            {
                FileSystemWatcher driveWatcher = new FileSystemWatcher(drive);
                driveWatcher.Filter = $"{DRAG_SRC_PREFIX}*.txt";
                driveWatcher.NotifyFilter = NotifyFilters.FileName;
                driveWatcher.IncludeSubdirectories = true;
                driveWatcher.EnableRaisingEvents = true;
                driveWatcher.Created += DriveWatcherDetectedPrefixedFileDropped;
                DriveWatchers.Add(driveWatcher);
            }

        }

        private static void DriveWatcherDetectedPrefixedFileDropped(object sender, FileSystemEventArgs e)
        {
            try
            {
                string droppedFilePath = e.FullPath;
                if (!string.IsNullOrEmpty(droppedFilePath.Trim()))
                {
                    string fileName = Path.GetFileName(droppedFilePath);
                    string realName = DragDropNameHelper.GetNonPrefixedFileName(fileName);
                    string realPath = Path.Combine(Path.GetDirectoryName(droppedFilePath), realName);

                    //// Renames the file
                    if (!File.Exists(realPath))
                    {
                        File.Move(droppedFilePath, realPath);

                        if (DroppingDocument != null && DroppingDocument.Document != null)
                        {
                            DroppingDocument.Document.FilePath = realPath;
                            DroppingDocument.HasMadeChanges = false;
                        }
                    }
                    else if (MessageBox.Show("File already exists. Override?", "File Exists", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.Delete(realPath);

                        File.Move(droppedFilePath, realPath);

                        if (DroppingDocument != null && DroppingDocument.Document != null)
                        {
                            DroppingDocument.Document.FilePath = realPath;
                            DroppingDocument.HasMadeChanges = false;
                        }
                    }
                    SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
                }

                ClearDriveWatchers();
            }
            catch { }
        }

        private static void ClearDriveWatchers()
        {
            try
            {
                for (int i = 0; i < DriveWatchers.Count; i++)
                {
                    FileSystemWatcher watcher = DriveWatchers[i];
                    watcher.Dispose();
                }

                DriveWatchers.Clear();
            }
            catch { }
        }
    }
}
