using SharpPad.InformationStuff;
using SharpPad.Utilities;
using Shell32;
using System.Diagnostics;
using System.IO;

namespace SharpPad.FileExplorer
{
    public static class ExplorerHelper
    {
        public static bool IsFile(this string path)
        {
            return !path.IsEmpty() && File.Exists(path);
        }

        public static bool IsDirectory(this string path)
        {
            return !path.IsEmpty() && Directory.Exists(path);
        }

        public static bool IsDrive(this string path)
        {
            return !path.IsEmpty() && path.Length < 3;
        }

        /// <summary>
        /// Checks whether a path is a valid path and can exist as a file. Will only return true
        /// if the file doesn't already exist, and if the path can actually be a real file.
        /// C:\idk\h.txt will return true
        /// </summary>
        /// <param name="possiblePath"></param>
        /// <returns></returns>
        public static bool CanNonExistantButPossiblePathExist(this string possiblePath)
        {
            if (!possiblePath.IsEmpty() && possiblePath.Contains("\\"))
            {
                if (File.Exists(possiblePath)) return false;
                try
                {
                    File.WriteAllText(possiblePath, "t");
                    File.Delete(possiblePath);
                    return true;
                }
                catch { return false; }
            }

            return false;
        }

        public static void OpenInFileExplorer(this string path)
        {
            if (path.IsFile())
            {
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = string.Format("/e, /select, \"{0}\"", path)
                };
                Information.Show("Opening File Explorer at selected file location", InfoTypes.FileIO);
                Process.Start(info);
            }

            else Information.Show("FilePath Doesn't Exist", "FilePath null");
        }

        /// <summary>
        /// Checks if the path is a shortcut to a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckPathIsShortcutFile(string path, out string shortcutPath)
        {
            string possiblePath = GetShortcutTargetFolder(path);
            if (File.Exists(possiblePath))
            {
                shortcutPath = possiblePath;
                return true;
            }

            shortcutPath = "";
            return false;
        }

        /// <summary>
        /// Gets the root path of a shortcut
        /// </summary>
        /// <param name="shortcutFilename"></param>
        /// <returns></returns>
        public static string GetShortcutTargetFolder(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                try
                {
                    ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                    return link.Path;
                }
                catch { }
                return string.Empty;
            }

            return string.Empty;
        }
    }
}
