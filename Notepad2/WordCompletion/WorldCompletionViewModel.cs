using Notepad2.Utilities;
using Notepad2.WordCompletion.Words;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.WordCompletion
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
