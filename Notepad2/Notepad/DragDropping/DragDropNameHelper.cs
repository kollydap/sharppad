using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Notepad.DragDropping
{
    public static class DragDropNameHelper
    {
        // Adds the prefix
        public static string GetPrefixedFileName(string original)
        {
            return FileWatchers.DRAG_SRC_PREFIX + original;
        }

        // Removes the prefix
        public static string GetNonPrefixedFileName(string prefixedPath)
        {
            return prefixedPath.Remove(0, FileWatchers.DRAG_SRC_PREFIX.Length);
            //return prefixedPath.Replace(DRAG_SRC_PREFIX, "");
        }
    }
}
