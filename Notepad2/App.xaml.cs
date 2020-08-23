using Notepad2.Applications;
using Notepad2.Views;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Notepad2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // My attempt at making the application single instance only,
        // and opening a window on that instance. idk how to get the
        // instance though which is the problem... so rip.

        //[DllImport("user32", CharSet = CharSet.Unicode)]
        //static extern IntPtr FindWindow(string cls, string win);

        //[DllImport("user32")]
        //static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        //public Mutex mMutex;

        //public App()
        //{
        //    SingleInstanceCheck();
        //    Exit += App_Exit;
        //}

        //private void App_Exit(object sender, ExitEventArgs e)
        //{
        //    mMutex?.Dispose();
        //    mMutex.Close();
        //}

        //public void SingleInstanceCheck()
        //{
        //    bool useSingleWindow = true;
        //    if (useSingleWindow)
        //    {
        //        bool isOnlyInstance = false;
        //        mMutex = new Mutex(true, "Notepad2", out isOnlyInstance);
        //        if (!isOnlyInstance)
        //        {
        //            MessageBox.Show("Another instance exists! Adding this instance's window to that one.");
        //            IntPtr winHandle = FindWindow(null, "SharpPad");
        //            // winHandle is null because it's on another thread or something.
        //            if (winHandle != IntPtr.Zero)
        //            {
        //                HwndTarget target = new HwndTarget(winHandle);
        //                HwndSource fromHwnd = HwndSource.FromHwnd(winHandle);
        //                if (fromHwnd.RootVisual is Window window)
        //                {
        //                    MessageBox.Show($"Window fetched: " +
        //                        $"Title: {window.Title}\n");
        //                    if (window is NotepadWindow win)
        //                    {
        //                        MessageBox.Show("Window is notepad window:" +
        //                            $"Items count: {win.Notepad.NotepadItems.Count}");
        //                    }
        //                }
        //            }
        //            Shutdown();
        //        }
        //    }
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThisApplication.Startup(e.Args);
        }

        // i think this fixes an issue with the clipboard going completely mad when spamming it
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //  && comException.ErrorCode == -2147221040
            Debug.Write($"Dispatcher Exception:: {e.Exception.Message}");
            if (e.Exception is COMException comException)
                e.Handled = true;
        }
    }
}
