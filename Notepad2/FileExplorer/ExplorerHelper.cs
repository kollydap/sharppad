using Shell32;
using System.IO;

namespace Notepad2.FileExplorer
{
    public static class ExplorerHelper
    {
        public static bool IsFile(this string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public static bool IsDirectory(this string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        public static bool IsDrive(this string path)
        {
            return !string.IsNullOrEmpty(path) && path.Length < 3;
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
