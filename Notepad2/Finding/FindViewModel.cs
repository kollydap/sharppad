using Notepad2.Finding;
using Notepad2.InformationStuff;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using static Notepad2.Finding.CharacterFinder;

namespace Notepad2.Finding
{
    public class FindViewModel : BaseViewModel
    {
        private string _findWhatText;
        private bool _matchCase;

        public ObservableCollection<FindResultItemViewModel> FoundItems { get; set; }

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

        public ICommand FindOccourancesCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public Action<FindResult> NextTextFoundCallback { get; set; }

        public FindViewModel(DocumentModel document)
        {
            FoundItems = new ObservableCollection<FindResultItemViewModel>();
            FindOccourancesCommand = new Command(StartFind);
            CancelCommand = new Command(Cancel);
            Document = document;
        }

        public void StartFind()
        {
            ClearItems();
            FetchAllResults();
        }

        public void Cancel()
        {
            ClearItems();
            HasSearched = false;
            FindWhatText = "";
        }

        public void ClearItems()
        {
            FoundItems?.Clear();
        }

        public void FetchAllResults()
        {
            FetchResultsInDocument(Document);
        }

        public void FetchResultsInDocument(DocumentModel doc)
        {
            if (doc != null)
            {
                string text = doc.Text;
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(FindWhatText))
                {
                    FindSettings settings = MatchCase ? FindSettings.CaseSensitive : FindSettings.None;
                    foreach (FindResult result in text.FindTextOccurrences(FindWhatText, settings))
                    {
                        result.PreviewFoundText = result.PreviewFoundText.Replace(Environment.NewLine, @"[\n]");
                        FindResultItemViewModel fri = new FindResultItemViewModel()
                        {
                            Result = result,
                            HighlightCallback = Hightlight
                        };
                        FoundItems.Add(fri);
                    }
                    HasSearched = true;
                    return;
                }
                Information.Show("Notepad Text or ToFind text is empty", "History");
            }
        }

        private void Hightlight(FindResult fri)
        {
            NextTextFoundCallback?.Invoke(fri);
        }
    }
}
