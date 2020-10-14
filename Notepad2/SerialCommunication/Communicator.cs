using Notepad2.InformationStuff;
using System;
using System.IO.Ports;
using System.Threading;

namespace Notepad2.SerialCommunication
{
    public static class Communicator
    {
        public const string MAIN_APP_COM = "COM24";
        public const string ADDITIONAL_COM = "COM25";

        public static bool CheckMainAppOpen()
        {
            Information.Show("Checking if main app is open", "DEBUG");
            try
            {
                SerialPort sp = new SerialPort(ADDITIONAL_COM, 9600, Parity.None, 8, StopBits.One);
                sp.Open();
                Information.Show("Successfully opened additional com port", "DEBUG");
                sp.WriteTimeout = 100;
                sp.ReadTimeout = 100;
                try
                {
                    sp.Write("MAOPN");
                    Thread.Sleep(200);
                    return sp.ReadLine() == "T";
                }
                catch (TimeoutException) { Information.Show("FAILED to write to main app", "DEBUG"); return false; }
                catch (Exception e) { Information.Show($"Error: {e.Message}", "DEBUG"); return false; }
                finally { sp.Close(); }
            }
            catch (Exception e)
            {
                Information.Show(e.Message, "SerialComs");
                return false;
            }
        }

        public static void SendMessageToMainApp(string message)
        {
            try
            {
                SerialPort sp = new SerialPort(ADDITIONAL_COM, 9600, Parity.None, 8, StopBits.One);
                sp.Open();
                sp.WriteTimeout = 100;
                sp.ReadTimeout = 100;
                try { sp.WriteLine(message); }
                catch { }
                finally { sp.Close(); }
            }
            catch (Exception e)
            {
                Information.Show(e.Message, "SerialComs");
            }
        }

        public static void SendMessageToMainApp(string[] messages)
        {
            try
            {
                Information.Show("Sending array of messages to main app", "DEBUG");
                SerialPort sp = new SerialPort(ADDITIONAL_COM, 9600, Parity.None, 8, StopBits.One);
                sp.Open();
                sp.WriteTimeout = 100;
                sp.ReadTimeout = 100;
                try
                {
                    foreach (string message in messages)
                    {
                        sp.WriteLine(message);
                        Information.Show("Successfully wrote a message to main app", "DEBUG");
                    }
                }
                catch { }
                finally { sp.Close(); }
            }
            catch (Exception e)
            {
                Information.Show(e.Message, "SerialComs");
            }
        }
    }
}
