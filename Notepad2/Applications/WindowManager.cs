using SharpPad.CClipboard;
using SharpPad.Notepad.FileProperties;
using SharpPad.Preferences.Views;
using SharpPad.ViewModels;
using SharpPad.Views;
using System.Collections.Generic;

namespace SharpPad.Applications
{
    public static class WindowManager
    {
        // Collections of Windows
        public static List<NotepadWindow> NotepadWindows { get; set; }

        // Single Windows
        public static NotepadWindow FocusedWindow { get; set; }
        public static WindowManagerView WindowPreviews { get; private set; }
        public static FilePropertiesWindow PropertiesView { get; private set; }
        public static PreferencesWindow Preferences { get; private set; }
        public static ClipboardWindow ClipboardWin { get; private set; }
        public static HelpWindow Help { get; private set; }


        static WindowManager()
        {
            NotepadWindows = new List<NotepadWindow>();
            WindowPreviews = new WindowManagerView();
            PropertiesView = new FilePropertiesWindow();
            ClipboardWin = new ClipboardWindow();
            Preferences = new PreferencesWindow();
            Help = new HelpWindow();
        }

        /// <summary>
        /// Gets a Notepad window from a view model
        /// </summary>
        /// <param name="notepad"></param>
        /// <returns>null if there's no windows with the given viewmodel. else returns the Notepad window</returns>
        public static NotepadWindow GetNotepadWindowFromNotepad(NotepadViewModel notepad)
        {
            foreach (NotepadWindow window in NotepadWindows)
            {
                if (window.Notepad == notepad)
                    return window;
            }

            return null;
        }
    }
}
