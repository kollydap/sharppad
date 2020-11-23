using SharpPad.Utilities;
using SharpPad.WordCompletion.Words;
using System.Collections.ObjectModel;

namespace SharpPad.WordCompletion
{
    public class WorldCompletionViewModel : BaseViewModel
    {
        public ObservableCollection<WordItemControl> WordItems { get; set; }

        private WordItemControl _selectedWord;
        public WordItemControl SelectedWord
        {
            get => _selectedWord;
            set => RaisePropertyChanged(ref _selectedWord, value);
        }

        public WorldCompletionViewModel()
        {
            WordItems = new ObservableCollection<WordItemControl>();
        }

        public void AddWord(string word)
        {
            WordItemControl wic = new WordItemControl(word);
            AddWordItem(wic);
        }

        public void ClearWords()
        {
            WordItems.Clear();
        }

        public void AddWordItem(WordItemControl wordItem)
        {
            WordItems.Add(wordItem);
        }

        public void RemoveWordItem(WordItemControl wordItem)
        {
            WordItems.Remove(wordItem);
        }
    }
}
