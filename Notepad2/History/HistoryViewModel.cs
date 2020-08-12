using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Windows.Input;

namespace Notepad2.History
{
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<HistoryItemViewModel> HistoryItems { get; set; }

        public ICommand ReopenLastFileCommand { get; private set; }
        public ICommand ClearItemsCommand { get; private set; }

        public Action<TextDocumentViewModel> OpenFileCallback { get; set; }

        public HistoryViewModel()
        {
            HistoryItems = new ObservableCollection<HistoryItemViewModel>();
            ReopenLastFileCommand = new Command(ReopenLastFile);
            ClearItemsCommand = new Command(ClearItems);
        }

        public void Push(HistoryItemViewModel hc)
        {
            HistoryItems.Insert(0, hc);
        }

        private HistoryItemViewModel Pop()
        {
            HistoryItemViewModel hc = HistoryItems[0];
            HistoryItems.Remove(hc);
            return hc;
        }

        /// <summary>
        /// Pushes a file (that has just closed) to the history
        /// </summary>
        /// <param name="path"></param>
        public void FileClosed(TextDocumentViewModel notepad)
        {
            HistoryItemViewModel item = CreateHistoryItem(notepad);
            Push(item);
        }

        /// <summary>
        /// Opens the last closed file via callbacks
        /// </summary>
        public void ReopenLastFile()
        {
            if (HistoryItems.Count > 0)
            {
                HistoryItemViewModel hc = Pop();
                if (hc != null)
                    ReopenFile(hc);
            }
        }

        public void ReopenFile(HistoryItemViewModel hc)
        {
            OpenFileCallback?.Invoke(hc.TextDocument);
        }

        private void UserReopenFile(HistoryItemViewModel history)
        {
            HistoryItems.Remove(history);
            OpenFileCallback?.Invoke(history.TextDocument);
        }

        private HistoryItemViewModel CreateHistoryItem(TextDocumentViewModel notepad)
        {
            HistoryItemViewModel hc = new HistoryItemViewModel()
            {
                ReopenFileCallback = UserReopenFile
            };
            return hc;
        }

        public void ClearItems()
        {
            HistoryItems.Clear();
        }
    }
}
