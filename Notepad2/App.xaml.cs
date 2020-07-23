using Notepad2.Applications;
using Notepad2.ViewModels;
using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Notepad2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThisApplication.Startup(e.Args);
        }

        // i think this fixes an issue with the clipboard going completely mad when spamming it
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is COMException comException && comException.ErrorCode == -2147221040)
                e.Handled = true;
        }
    }
}
