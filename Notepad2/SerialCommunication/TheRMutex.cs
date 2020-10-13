using Notepad2.InformationStuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.SerialCommunication
{
    public static class TheRMutex
    {
        public delegate void MessageArgs(ApplicationMessage messageType, string message);
        public static event MessageArgs OnMessageReceived;

        private static SerialPort MainAppPort;

        static TheRMutex()
        {

        }

        public static void SetAsMainApp()
        {
            OpenMainAppCommunication();
            SetupMessageReceiver();
        }

        public static void OpenMainAppCommunication()
        {
            MainAppPort = new SerialPort(Communicator.MAIN_APP_COM, 9600, Parity.None, 8, StopBits.One);

            try
            {
                MainAppPort.Open();
            }
            catch { }
        }

        public static void SetupMessageReceiver()
        {
            if (MainAppPort?.IsOpen == true)
            {
                MainAppPort.DataReceived -= MainAppPort_DataReceived;
                MainAppPort.DataReceived += MainAppPort_DataReceived;
            }
        }

        private static void MainAppPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = MainAppPort.ReadExisting();

            if (File.Exists(data))
            {
                OnMessageReceived?.Invoke(ApplicationMessage.OpenFiles, data);
            }

            else if (data == "MAOPN")
            {
                try
                {
                    MainAppPort.Write("T");
                }

                catch{ }
            }
        }

        internal static void MainAppClosing()
        {
            if (MainAppPort != null)
            {
                if (MainAppPort.IsOpen)
                {
                    try
                    {
                        MainAppPort.Close();
                    }
                    catch { }
                }
            }
        }
    }
}
