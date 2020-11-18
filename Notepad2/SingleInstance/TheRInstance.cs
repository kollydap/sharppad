using Notepad2.Applications;
using Notepad2.FileExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.SingleInstance
{
    public static class TheRInstance
    {
        public const string INSTANCE_FILE = "SingleInstance.txt";
        public static readonly string INSTANCE_PATH;

        public static FileSystemWatcher InstanceWatcher { get; set; }

        static TheRInstance()
        {
            INSTANCE_PATH = Path.Combine(ThisApplication.SharpPadFolderLocation, INSTANCE_FILE);

            if (!INSTANCE_PATH.IsFile())
            {
                File.WriteAllText(INSTANCE_PATH, "");
            }
        }

        public static void StartWatcher()
        {
            InstanceWatcher = new FileSystemWatcher(ThisApplication.SharpPadFolderLocation);
            InstanceWatcher.EnableRaisingEvents = true;
            InstanceWatcher.Changed += InstanceWatcher_Changed;
            InstanceWatcher.NotifyFilter = NotifyFilters.Size;
            InstanceWatcher.IncludeSubdirectories = false;
        }

        private static bool DoubleChanged { get; set; }

        private static void InstanceWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (DoubleChanged)
            {
                InformationStuff.Information.Show("Single Instance file Changed");
                DoubleChanged = false;
            }
            else
            {
                DoubleChanged = true;
            }
        }

        public static void StopWatcher()
        {
            InstanceWatcher.Dispose();
        }
    }
}
