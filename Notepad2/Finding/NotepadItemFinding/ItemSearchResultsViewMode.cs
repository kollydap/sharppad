using SharpPad.Notepad;
using SharpPad.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SharpPad.Finding.NotepadItemFinding
{
    public class ItemSearchResultsViewMode : BaseViewModel
    {
        public ObservableCollection<FoundNotepadItemViewModel> FoundItems { get; set; }

        public ICommand ClearItemsCommand { get; }

        public Action<NotepadItemViewModel> SelectItemCallback { get; set; }

        public ItemSearchResultsViewMode()
        {
            FoundItems = new ObservableCollection<FoundNotepadItemViewModel>();
            ClearItemsCommand = new Command(ClearItems);
        }

        public void AddItem(FoundNotepadItemViewModel doc)
        {
            FoundItems.Add(doc);
        }

        public void RemoveItem(FoundNotepadItemViewModel doc)
        {
            FoundItems.Remove(doc);
        }

        public FoundNotepadItemViewModel CreateItem(NotepadItemViewModel notepad)
        {
            FoundNotepadItemViewModel item = new FoundNotepadItemViewModel()
            {
                Notepad = notepad
            };

            SetupCallbacks(item);

            return item;
        }

        public void SetupCallbacks(FoundNotepadItemViewModel item)
        {
            item.SelectNotepadCallback = Select;
        }

        public void Select(FoundNotepadItemViewModel item)
        {
            if (item != null)
                if (item.Notepad != null)
                    SelectItemCallback?.Invoke(item.Notepad);
        }

        public void Search(ICollection<NotepadItemViewModel> docs, string findText)
        {
            if (!findText.IsEmpty())
            {
                ClearItems();
                foreach (NotepadItemViewModel doc in docs)
                {
                    if (doc.Notepad.Document.FileName.ToLower().Contains(findText.ToLower()))
                    {
                        AddItem(CreateItem(doc));
                    }
                }
            }
        }

        public void ClearItems()
        {
            FoundItems.Clear();
        }
    }
}
