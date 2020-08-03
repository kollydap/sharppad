using System.Windows.Media;

namespace Notepad2.Utilities
{
    public static class GlobalPreferences
    {
        public const int WINDOW_TITLEBAR_HEIGHT = 30;
        public const double ANIMATION_SPEED_SEC = 0.2;

        public const double WARN_FILE_SIZE_KB = 100.0d;
        public const double ALERT_FILE_SIZE_KB = 250.0d;
        public static Color WARN_FILE_TOO_BIG_COLOUR = Colors.Orange;
        public static Color ALERT_FILE_TOO_BIG_COLOUR = Colors.Red;

        //this is big for no reason other than why not lol
        public const int MAX_FONT_SIZE = 250;

        public static string[] PRESET_EXTENSIONS = new string[13]
        {
            ".txt",
            ".text",
            ".cs",
            ".c",
            ".cpp",
            ".h",
            ".xaml",
            ".xml",
            ".htm",
            ".html",
            ".css",
            ".js",
            ".exe"
        };
    }
}
