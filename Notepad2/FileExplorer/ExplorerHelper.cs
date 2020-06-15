using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
