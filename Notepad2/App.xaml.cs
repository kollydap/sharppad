using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mWnd;
            if (e.Args.Length > 0)
            {
                string parms = string.Join(" ", e.Args);
                string[] arguments = parms.Split('\"');
                mWnd = new MainWindow(arguments);
            }
            else
            {
                mWnd = new MainWindow();
            }

            MainWindow = mWnd;
            mWnd.Show();
            mWnd.LoadSettings();
        }
    }
}
