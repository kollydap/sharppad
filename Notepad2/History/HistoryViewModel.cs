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
        public ObservableCollection<HistoryControl> HistoryItems { get; set; }

        public ICommand ReopenLastFileCommand { get; private set; }
        public ICommand ClearItemsCommand { get; private set; }

        public Action<TextDocumentViewModel> OpenFileCallback { get; set; }

        public HistoryViewModel()
        {
            HistoryItems = new ObservableCollection<HistoryControl>();
            ReopenLastFileCommand = new Command(ReopenLastFile);
            ClearItemsCommand = new Command(ClearItems);
        }

        public void Push(HistoryControl hc)
        {
            HistoryItems.Insert(0, hc);
        }

        public HistoryControl Pop()
        {
            HistoryControl hc = HistoryItems[0];
            HistoryItems.Remove(hc);
            return hc;
        }

        /// <summary>
        /// Pushes a file (that has just closed) to the history
        /// </summary>
        /// <param name="path"></param>
        public void FileClosed(TextDocumentViewModel notepad)
        {
            HistoryControl item = CreateHistoryItem(notepad);
            Push(item);
        }

        /// <summary>
        /// Opens the last closed file via callbacks
        /// </summary>
        public void ReopenLastFile()
        {
            if (HistoryItems.Count > 0)
            {
                HistoryControl hc = Pop();
                if (hc != null)
                    ReopenFile(hc);
            }
        }

        public void ReopenFile(HistoryControl hc)
        {
            OpenFileCallback?.Invoke(hc.Model);
        }

        public void UserReopenFile(HistoryControl hc)
        {
            HistoryItems.Remove(hc);
            OpenFileCallback?.Invoke(hc.Model);
        }

        public HistoryControl CreateHistoryItem(TextDocumentViewModel notepad)
        {
            HistoryControl hc = new HistoryControl()
            {
                ReopenFileCallback = UserReopenFile,
                DataContext = notepad
            };
            return hc;
        }

        public void ClearItems()
        {
            HistoryItems.Clear();
        }
    }
}
