using Notepad2.Utilities;
using System;

namespace Notepad2.Notepad
{
    /// <summary>
    /// A ViewModel containing common data a notepad document/file contains
    /// </summary>
    public class DocumentModel : BaseViewModel
    {
        // Used for telling the Text Document that text has changed
        internal Action TextChanged { private get; set; }

        private string _text;
        private string _filePath;
        private string _fileName;
        private long _fileSize;
        private int _wordCount;
        private bool _useWordCount;

        /// <summary>
        /// The text of the document
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                RaisePropertyChanged(ref _text, value);
                TextChanged?.Invoke();
                FileSizeBytes = value.Length;
                if (UseWordCount)
                    WordCount = value.CollapseSpaces().Split(' ').Length;
            }
        }

        /// <summary>
        /// The path of the file on your computer
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => RaisePropertyChanged(ref _filePath, value);
        }

        /// <summary>
        /// The name of the file/notepad item
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => RaisePropertyChanged(ref _fileName, value);
        }

        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        public long FileSizeBytes
        {
            get => _fileSize;
            set => RaisePropertyChanged(ref _fileSize, value);
        }

        public int WordCount
        {
            get => _wordCount;
            set => RaisePropertyChanged(ref _wordCount, value);
        }

        public bool UseWordCount
        {
            get => _useWordCount;
            set => RaisePropertyChanged(ref _useWordCount, value);
        }

        public DocumentModel()
        {
            UseWordCount = true;
        }
    }
}
