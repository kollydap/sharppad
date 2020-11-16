using Notepad2.InformationStuff;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Notepad2.Finding.TextFinding
{
    public class FindReplaceViewModel : BaseViewModel
    {
        private int _position;
        private string _findWhatText;
        private string _replaceWithText;
        private bool _matchCase;
        private bool _matchWholeWord;

        public ObservableCollection<FindResult> FoundItems { get; set; }

        public int Position
        {
            get => _position;
            set
            {
                if (_position < 1 && Count < 1)
                {
                    HasSearched = false;
                }

                RaisePropertyChanged(ref _position, value);
            }
        }

        public string FindWhatText
        {
            get => _findWhatText;
            set => RaisePropertyChanged(ref _findWhatText, value, StartFind);
        }

        public string ReplaceWithText
        {
            get => _replaceWithText;
            set => RaisePropertyChanged(ref _replaceWithText, value);
        }

        public bool MatchCase
        {
            get => _matchCase;
            set => RaisePropertyChanged(ref _matchCase, value, StartFind);
        }

        public bool MatchWholeWord
        {
            get => _matchWholeWord;
            set => RaisePropertyChanged(ref _matchWholeWord, value, StartFind);
        }

        public int Count
        {
            get => FoundItems.Count;
        }

        public DocumentViewModel Document { get; set; }

        public bool HasSearched { get; set; }

        public ICommand FindOccourancesCommand { get; set; }
        public ICommand HighlightNextMatchCommand { get; set; }
        public ICommand HighlightPreviousMatchCommand { get; set; }
        public ICommand ReplaceNextCommand { get; set; }
        public ICommand ReplaceAllCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public Action<FindResult, bool> HighlightResultCallback { get; set; }
        public Action<FindResult, string> ReplaceTextCallback { get; set; }
        public Action<bool> SetFindViewIsVisibleCallback { get; set; }

        public FindReplaceViewModel(DocumentViewModel document)
        {
            FoundItems = new ObservableCollection<FindResult>();
            FindOccourancesCommand = new Command(StartFind);
            HighlightNextMatchCommand = new Command(HighlightNextMatch);
            HighlightPreviousMatchCommand = new Command(HighlightPreviousMatch);
            ReplaceNextCommand = new Command(ReplaceNext);
            ReplaceAllCommand = new Command(ReplaceAll);
            CancelCommand = new Command(Cancel);
            Document = document;
            Position = 0;
        }

        public void StartFind()
        {
            Position = 0;
            ClearItems();
            FetchAllResults();
        }

        public void HighlightNextMatch()
        {
            if (Count < 1)
            {
                Position = 0;
                return;
            }

            if (Position < Count)
            {
                Position += 1;
            }
            else
            {
                Position = 1;
            }

            HighlightResultAtIndex(Position);
        }

        public void HighlightPreviousMatch()
        {
            if (Count < 1)
            {
                Position = 0;
                return;
            }

            if (Position > 1)
            {
                Position -= 1;
            }
            else
            {
                Position = Count;
            }

            HighlightResultAtIndex(Position);
        }

        public void ReplaceNext()
        {
            ReplaceNext(false);
        }

        public void ReplaceNext(bool isReplacingAll = false)
        {
            int index = Position - 1;
            if (Count > 0 && index >= 0)
            {
                FindResult result = FoundItems[index];
                string replaceWith = ReplaceWithText;

                ReplaceTextCallback?.Invoke(result, replaceWith);

                FoundItems.Remove(result);

                for (int i = index; i < FoundItems.Count; i++)
                {
                    IncrementFindResult(FoundItems[i], replaceWith.Length - result.WordLength);
                }

                if (Position <= Count)
                {
                    if (!isReplacingAll)
                        HighlightResultAtIndex(Position);
                }
                else
                {
                    Position = 0;
                }
            }
        }

        public void ReplaceAll()
        {
            int count = Count;
            if (count > 1)
            {
                Position = 1;
                for (int i = 0; i < count; i++)
                {
                    ReplaceNext(true);
                }
            }

            Position = 0;
        }

        public void IncrementFindResult(FindResult result, int amount)
        {
            result.StartIndex += amount;
        }

        public void Cancel()
        {
            Position = 0;
            ClearItems();
            FindWhatText = "";

            SetFindViewIsVisibleCallback?.Invoke(false);
        }

        public void HighlightResultAtIndex(int position)
        {
            int index = position - 1;
            if (Count > 0 && index >= 0 && index < Count)
            {
                FindResult result = FoundItems[index];
                HighlightResultCallback?.Invoke(result, true);
                SetFindViewIsVisibleCallback?.Invoke(true);
            }
        }

        public void ClearItems()
        {
            FoundItems?.Clear();
            HasSearched = false;
        }

        public void FetchAllResults()
        {
            FetchResultsInDocument(Document);
        }

        public void FetchResultsInDocument(DocumentViewModel doc)
        {
            if (doc != null)
            {
                string text = doc.Text;
                if (!text.IsEmpty() && !FindWhatText.IsEmpty())
                {
                    // combine the enums using the OR thing
                    // if match whole word and case are true, it ends up being 2 | 1 which is 3
                    // can extract either of the settings by doing settings & FindSettings.MatchCase...
                    FindSettings settings =
                        (MatchWholeWord ? FindSettings.MatchWholeWord : FindSettings.None) |
                        (MatchCase ? FindSettings.MatchCase : FindSettings.None);
                    foreach (FindResult result in text.FindTextOccurrences(FindWhatText, settings))
                    {
                        FoundItems.Add(result);
                    }
                    HasSearched = true;
                    return;
                }
                Information.Show("Notepad Text or ToFind text is empty", "History");
            }
        }
    }
}
