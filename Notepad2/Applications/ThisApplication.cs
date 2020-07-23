using Notepad2.CClipboard;
using Notepad2.InformationStuff;
using Notepad2.ViewModels;
using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad2.Applications
{
    public static class ThisApplication
    {
        public static ApplicationViewModel App { get; set; }
        public static WindowsPrviewWindow WindowPreviews { get; set; }

        public static void Startup(string[] args)
        {
            App = new ApplicationViewModel(args);
            WindowPreviews = new WindowsPrviewWindow();
            WindowPreviews.ThisApp = App;
        }

        public static void CloseWindowFromDataContext(NotepadViewModel notepad)
        {
            Information.Show("Closing a window", "Windows");
            App.FullyCloseWindowFromDataContext(notepad);
        }

        public static void ShowWindowPreviewsWindow()
        {
            WindowPreviews.Show();
        }

        public static void OpenFileInNewWindow(string path)
        {
            Information.Show("Opening file in new window", "NewWindow");
            App.OpenFileInNewWindow(path);
        }

        public static void ShutdownApplication()
        {
            Information.Show("Shutting down application", "App");
            ClipboardNotification.ShutdownListener();
            Application.Current?.Shutdown();
        }
    }
}
