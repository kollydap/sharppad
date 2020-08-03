using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Notepad2.History
{
    public class WindowHistoryViewModel : BaseViewModel
    {
        public ObservableCollection<WindowHistoryControl> HistoryItems { get; set; }

        public ICommand ReopenLastWindowCommand { get; private set; }
        public ICommand ClearItemsCommand { get; private set; }

        public Action<NotepadViewModel> OpenWindowCallback { get; set; }

        public WindowHistoryViewModel()
        {
            HistoryItems = new ObservableCollection<WindowHistoryControl>();
            ReopenLastWindowCommand = new Command(ReopenLastWindow);
            ClearItemsCommand = new Command(ClearItems);
        }

        private void Push(WindowHistoryControl hc)
        {
            HistoryItems.Insert(0, hc);
        }

        private WindowHistoryControl Pop()
        {
            if (HistoryItems.Count > 0)
            {
                WindowHistoryControl hc = HistoryItems[0];
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
            WindowHistoryControl item = CreateHistoryItem(notepad);
            Push(item);
        }

        /// <summary>
        /// Opens the last closed file via callbacks
        /// </summary>
        public void ReopenLastWindow()
        {
            WindowHistoryControl hc = Pop();
            if (hc != null)
                ReopenWindow(hc);
        }

        private void ReopenWindow(WindowHistoryControl hc)
        {
            OpenWindowCallback?.Invoke(hc.Model);
        }

        private void UserReopenWindow(WindowHistoryControl hc)
        {
            HistoryItems.Remove(hc);
            OpenWindowCallback?.Invoke(hc.Model);
        }

        private WindowHistoryControl CreateHistoryItem(NotepadViewModel notepad)
        {
            WindowHistoryControl hc = new WindowHistoryControl()
            {
                ReopenWindowCallback = UserReopenWindow,
                Model = notepad
            };
            return hc;
        }

        public void ClearItems()
        {
            HistoryItems.Clear();
        }
    }
}
