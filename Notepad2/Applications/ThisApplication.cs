using Notepad2.CClipboard;
using Notepad2.InformationStuff;
using Notepad2.Notepad.DragDropping;
using Notepad2.Notepad.FileProperties;
using Notepad2.ViewModels;
using Notepad2.Views;
using System.Collections.Generic;
using System.Windows;

namespace Notepad2.Applications
{
    public static class ThisApplication
    {
        public static ApplicationViewModel App { get; private set; }

        public static void Startup(string[] args)
        {
            App = new ApplicationViewModel(args);
            WindowManager.WindowPreviews.ThisApp = App;
        }

        public static void CloseWindowFromDataContext(NotepadViewModel notepad)
        {
            Information.Show("Closing a window", "Windows");
            App.FullyCloseWindowFromDataContext(notepad);
        }

        public static void ShowWindowPreviewsWindow()
        {
            WindowManager.WindowPreviews.Show();
        }

        public static void OpenFileInNewWindow(string path)
        {
            Information.Show("Opening file in new window", "NewWindow");
            App.OpenFileInNewWindow(path);
        }

        public static void OpenNewBlankWindow()
        {
            App.CreateStartupBlankNotepadWindowAndPreview();
        }

        public static void ShutdownApplication()
        {
            Information.Show("Shutting down application", "App");
            ClipboardNotification.ShutdownListener();
            FileWatchers.Shutdown();
            Application.Current?.Shutdown();
        }

        public static void ReopenLastWindow()
        {
            Information.Show("Opening last window...", "Window");
            App.ReopenLastWindow();
        }

        public static void ShowClipboard()
        {
            WindowManager.ClipboardWin.ShowWindow();
        }

        public static void SetClipboardContext(ClipboardViewModel model)
        {
            WindowManager.ClipboardWin.Clipboard = model;
        }

        public static void ShowHelp()
        {
            WindowManager.Help.Show();
        }
    }
}
