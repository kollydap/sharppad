using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Notepad2.Applications.History
{
    public class WindowHistoryViewModel : BaseViewModel
    {
        public ObservableCollection<WindowHistoryControlViewModel> HistoryItems { get; set; }

        public ICommand ReopenLastWindowCommand { get; private set; }
        public ICommand ClearItemsCommand { get; private set; }

        public Action<NotepadViewModel> OpenWindowCallback { get; set; }

        public WindowHistoryViewModel()
        {
            HistoryItems = new ObservableCollection<WindowHistoryControlViewModel>();
            ReopenLastWindowCommand = new Command(ReopenLastWindow);
            ClearItemsCommand = new Command(ClearItems);
        }

        private void Push(WindowHistoryControlViewModel hc)
        {
            HistoryItems.Insert(0, hc);
        }

        private WindowHistoryControlViewModel Pop()
        {
            if (HistoryItems.Count > 0)
            {
                WindowHistoryControlViewModel hc = HistoryItems[0];
                HistoryItems.Remove(hc);
                return hc;
            }
            return null;
        }

        /// <summary>
        /// Pushes a file (that has just closed) to the history
        /// </summary>
        /// <param name="path"></param>
        public void WindowClosed(NotepadViewModel notepad)
        {
            WindowHistoryControlViewModel item = CreateHistoryItem(notepad);
            Push(item);
        }

        /// <summary>
        /// Opens the last closed file via callbacks
        /// </summary>
        public void ReopenLastWindow()
        {
            WindowHistoryControlViewModel hc = Pop();
            if (hc != null)
                ReopenWindow(hc);
        }

        private void ReopenWindow(WindowHistoryControlViewModel hc)
        {
            OpenWindowCallback?.Invoke(hc.Notepad);
        }

        private void UserReopenWindow(WindowHistoryControlViewModel hc)
        {
            HistoryItems.Remove(hc);
            OpenWindowCallback?.Invoke(hc.Notepad);
        }

        private WindowHistoryControlViewModel CreateHistoryItem(NotepadViewModel notepad)
        {
            WindowHistoryControlViewModel hc = new WindowHistoryControlViewModel()
            {
                ReopenWindowCallback = UserReopenWindow,
                Notepad = notepad
            };
            return hc;
        }

        public void ClearItems()
        {
            HistoryItems.Clear();
        }
    }
}
