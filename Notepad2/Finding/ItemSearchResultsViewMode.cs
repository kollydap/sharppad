using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad2.Finding
{
    public class ItemSearchResultsViewMode : BaseViewModel
    {
        public ObservableCollection<FoundNotepadItemViewModel> FoundItems { get; set; }

        public ICommand ClearItemsCommand { get; }

        public Action<TextDocumentViewModel> SelectItemCallback { get; set; }

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

        public FoundNotepadItemViewModel CreateItem(TextDocumentViewModel notepad)
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

        public void Search(ICollection<TextDocumentViewModel> docs, string findText)
        {
            ClearItems();
            foreach (TextDocumentViewModel doc in docs)
            {
                if (doc.Document.FileName.ToLower().Contains(findText.ToLower()))
                {
                    AddItem(CreateItem(doc));
                }
            }
        }

        public void ClearItems()
        {
            FoundItems.Clear();
        }
    }
}
