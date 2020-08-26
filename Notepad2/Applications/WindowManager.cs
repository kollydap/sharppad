using Notepad2.CClipboard;
using Notepad2.Finding.NotepadItemFinding;
using Notepad2.Notepad.FileProperties;
using Notepad2.Views;
using System.Collections.Generic;

namespace Notepad2.Applications
{
    public static class WindowManager
    {
        // Collections of Windows
        public static List<NotepadWindow> NotepadWindows { get; set; }
        public static List<ItemSearchResultsWindow> SearchResultsWindows { get; set; }

        // Single Windows
        public static NotepadWindow FocusedWindow { get; set; }
        public static FilePropertiesWindow PropertiesView { get; private set; }
        public static WindowManagerView WindowPreviews { get; private set; }
        public static ClipboardWindow ClipboardWin { get; private set; }
        public static HelpWindow Help { get; private set; }


        static WindowManager()
        {
            NotepadWindows = new List<NotepadWindow>();
            WindowPreviews = new WindowManagerView();
            PropertiesView = new FilePropertiesWindow();
            ClipboardWin = new ClipboardWindow();
            Help = new HelpWindow();
        }
    }
}
