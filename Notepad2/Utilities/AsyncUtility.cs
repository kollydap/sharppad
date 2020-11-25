using System;
using System.Threading.Tasks;
using System.Windows;

namespace SharpPad.Utilities
{
    public static class AsyncUtility
    {
        /// <summary>
        /// A helper for running asynchronous 
        /// code in the main thread
        /// </summary>
        public static void RunSync(Action method)
        {
            Application.Current?.Dispatcher?.Invoke(method);
        }

        /// <summary>
        /// A helper for running code
        /// in another thread
        /// </summary>
        public static void RunAsync(Action method)
        {
            Task.Run(method);
        }
    }
}
