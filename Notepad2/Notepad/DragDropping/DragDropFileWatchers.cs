using Microsoft.Win32;
using Notepad2.InformationStuff;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad2.Notepad.DragDropping
{
    public static class DragDropFileWatchers
    {
        // A prefix to identify files created by sharp pad (aka this program lol)
        public const string DRAG_SRC_PREFIX = "_SHRPDDRGPFX__";

        // Watches the Temp directory
        public static FileSystemWatcher TempWatcher;

        // For every logical drive (C, D, H, etc)
        public static List<FileSystemWatcher> DriveWatchers;

        // The currently being dropped document
        public static TextDocumentViewModel DroppingDocument;

        private struct FileRenameContainer
        {
            public string From { get; set; }
            public string To { get; set; }

            public FileRenameContainer(string from, string to)
            {
                From = from;
                To = to;
            }
        }

        public static int QueCooldown { get; set; }
        private static bool QueRunning;
        private static int QueCountdown;
        private static List<FileRenameContainer> FileRenamingQue { get; set; }

        [DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        static DragDropFileWatchers()
        {
            DriveWatchers = new List<FileSystemWatcher>();
            FileRenamingQue = new List<FileRenameContainer>();

            TempWatcher = new FileSystemWatcher(Path.GetTempPath());
            TempWatcher.Filter = $"{DRAG_SRC_PREFIX}*.txt";
            TempWatcher.NotifyFilter = NotifyFilters.FileName;
            TempWatcher.IncludeSubdirectories = false;
            TempWatcher.EnableRaisingEvents = true;
            TempWatcher.Created += PrefixedFileCreatedInTempDirectory;

            QueCooldown = 3000;
            StartRenameQue();
        }

        /// <summary>
        /// Just used to initialise the static class and get the que up and running
        /// </summary>
        public static void Initialise()
        {

        }

        public static void StartRenameQue()
        {
            Task.Run(async() =>
            {
                QueCountdown = QueCooldown;
                QueRunning = true;
                while (QueRunning)
                {
                    QueCountdown -= 1000;

                    if (QueCountdown <= 0)
                    {
                        SaveFilesInQue();
                        ResetQueCountdown();
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public static void StopRenameQue()
        {
            QueRunning = false;
        }

        public static void ResetQueCountdown()
        {
            QueCountdown = QueCooldown;
        }

        public static void SaveFilesInQue()
        {
            try
            {
                foreach (FileRenameContainer rename in FileRenamingQue)
                {
                    File.Move(rename.From, rename.To);
                }

                FileRenamingQue.Clear();
            }
            catch { }
        }

        public static void Shutdown()
        {
            TempWatcher.Created -= PrefixedFileCreatedInTempDirectory;
            StopRenameQue();
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
                        FileRenamingQue.Add(new FileRenameContainer(droppedFilePath, realPath));
                        ResetQueCountdown();

                        if (DroppingDocument != null && DroppingDocument.Document != null)
                        {
                            DroppingDocument.Document.FilePath = realPath;
                            DroppingDocument.HasMadeChanges = false;
                        }
                    }
                    else if (MessageBox.Show("File already exists. Override?", "File Exists", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.Delete(realPath);

                        Task.Run(async () =>
                        {
                            await Task.Delay(GlobalPreferences.FILEWATCHER_FILE_RENAME_DELAY_MS);
                            File.Move(droppedFilePath, realPath);
                        });

                        if (DroppingDocument != null && DroppingDocument.Document != null)
                        {
                            DroppingDocument.Document.FilePath = realPath;
                            DroppingDocument.HasMadeChanges = false;
                        }
                    }
                    //SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
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
                    watcher.Created -= DriveWatcherDetectedPrefixedFileDropped;
                    watcher.Dispose();
                }

                DriveWatchers.Clear();
            }
            catch { }
            finally
            {
                if (DriveWatchers != null)
                    DriveWatchers.Clear();
            }
        }
    }
}
