namespace Notepad2.Utilities
{
    public static class FileExtensionsHelper
    {
        public static string GetFileExtension(string fileName, string extUid)
        {
            string finalName = fileName;
            bool hasExtension = false;
            foreach (string extensionThing in GlobalPreferences.PRESET_EXTENSIONS)
            {
                if (fileName.Contains(extensionThing))
                {
                    hasExtension = true;
                    break;
                }
            }
            if (hasExtension)
            {
                string[] dotSplits = fileName.Split('.');
                int charsToRemoveCount = dotSplits[dotSplits.Length - 1].Length + 1;
                string fileNameNoExtension = fileName.Substring(0, fileName.Length - charsToRemoveCount);
                return fileNameNoExtension + extUid;
            }
            else
                return finalName += extUid;
        }
    }
}
