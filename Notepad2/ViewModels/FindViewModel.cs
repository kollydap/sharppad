using Notepad2.Finding;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using static Notepad2.Finding.CharacterFinder;

namespace Notepad2.ViewModels
{
    public class FindViewModel : BaseViewModel
    {
        public delegate void FindResultEventArgs(FindResult result);
        public event FindResultEventArgs OnNextTextFound;

        private int _selectedIndex;
        private FindResultItem _selectedItem;
        private string _findWhatText;
        private bool _matchCase;

        public ObservableCollection<FindResultItem> FoundItems { get; set; }
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => RaisePropertyChanged(ref _selectedIndex, value);
        }
        public FindResultItem SelectedItem
        {
            get => _selectedItem;
            set => RaisePropertyChanged(ref _selectedItem, value);
        }
        public string FindWhatText
        {
            get => _findWhatText;
            set => RaisePropertyChanged(ref _findWhatText, value);
        }
        public bool MatchCase
        {
            get => _matchCase;
            set => RaisePropertyChanged(ref _matchCase, value);
        }

        public DocumentModel Document { get; set; }

        public bool HasSearched { get; set; }

        public ICommand FindNextCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public FindViewModel(DocumentModel document)
        {
            FoundItems = new ObservableCollection<FindResultItem>();
            FindNextCommand = new Command(StartFind);
            CancelCommand = new Command(Close);
            Document = document;
        }

        public void StartFind()
        {
            ClearItems();
            FetchAllResults();
        }

        public void Close()
        {
            ClearItems();
            HasSearched = false;
            FindWhatText = "";
        }

        public void ClearItems()
        {
            FoundItems?.Clear();
        }

        public int ItemsCount
        {
            get => FoundItems.Count;
        }

        public void ResetSelection()
        {
            SelectedIndex = 0;
        }

        public void Reset()
        {
            ClearItems();
            HasSearched = false;
        }

        public void FetchAllResults()
        {
            if (Document != null)
            {
                string text = Document.Text;
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(FindWhatText))
                {
                    FindSettings settings = MatchCase ? FindSettings.CaseSensitive : FindSettings.None;
                    foreach (FindResult result in text.FindTextOccurrences(FindWhatText, settings))
                    {
                        result.PreviewFoundText = result.PreviewFoundText.Replace('\n', ' ');
                        FindResultItem fri = new FindResultItem(result)
                        {
                            HighlightCallback = Hightlight
                        };
                        FoundItems?.Add(fri);
                    }
                    HasSearched = true;
                    return;
                }
                MessageBox.Show("Notepad Text or ToFind text is empty");
            }
        }

        private void Hightlight(FindResultItem fri)
        {
            OnNextTextFound?.Invoke(fri.Result);
        }
    }
}
