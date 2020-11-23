using SharpPad.Utilities;
using SharpPad.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SharpPad.Applications.History
{
    /// <summary>
    /// A class for dealing with the history of Notepads (Aka the Notepad windows' DataContext). 
    /// It deals only with datacontexts so this is MVVM friendly.
    /// </summary>
    public class WindowHistoryViewModel : BaseViewModel
    {
        public ObservableCollection<WindowHistoryControlViewModel> HistoryItems { get; set; }

        public ICommand ReopenLastWindowCommand { get; private set; }
        public ICommand ClearItemsCommand { get; private set; }

        public Action<NotepadViewModel> OpenWindowCallback { get; set; }

        public WindowHistoryViewModel()
        {
            HistoryItems = new ObservableCollection<WindowHistoryControlViewModel>();
            ReopenLastWindowCommand = new Command(ReopenLastNotepad);
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
        /// Pushes a Notepad View's DataContext (that has just closed) to the history
        /// </summary>
        /// <param name="path"></param>
        public void PushNotepad(NotepadViewModel notepad)
        {
            WindowHistoryControlViewModel item = CreateHistoryItem(notepad);
            Push(item);
        }

        /// <summary>
        /// Opens the last closed file via callbacks
        /// </summary>
        public void ReopenLastNotepad()
        {
            WindowHistoryControlViewModel hc = Pop();
            if (hc != null)
                ReopenNotepad(hc);
        }

        private void ReopenNotepad(WindowHistoryControlViewModel hc)
        {
            OpenWindowCallback?.Invoke(hc.Notepad);
        }

        private void UserReopenNotepad(WindowHistoryControlViewModel hc)
        {
            HistoryItems.Remove(hc);
            OpenWindowCallback?.Invoke(hc.Notepad);
        }

        private WindowHistoryControlViewModel CreateHistoryItem(NotepadViewModel notepad)
        {
            WindowHistoryControlViewModel hc = new WindowHistoryControlViewModel()
            {
                ReopenNotepadCallback = UserReopenNotepad,
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
