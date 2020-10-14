namespace Notepad2.Notepad.DragDropping
{
    public static class DragDropNameHelper
    {
        /// <summary>
        /// Adds the prefix to a FILE NAME not path
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string GetPrefixedFileName(string original)
        {
            return DragDropFileWatchers.DRAG_SRC_PREFIX + original;
        }

        /// <summary>
        /// Removes the prefix from a FILE NAME not path
        /// </summary>
        /// <param name="prefixedPath"></param>
        /// <returns></returns>
        public static string GetNonPrefixedFileName(string prefixedPath)
        {
            return prefixedPath.Remove(0, DragDropFileWatchers.DRAG_SRC_PREFIX.Length);
        }
    }
}
