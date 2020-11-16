using Notepad2.Preferences;
using Notepad2.Utilities;
using System;

namespace Notepad2.Notepad
{
    /// <summary>
    /// A ViewModel containing common data a notepad document/file contains
    /// </summary>
    public class DocumentViewModel : BaseViewModel
    {
        // Used for telling the Text Document that text has changed
        internal Action TextChanged { private get; set; }

        private string _text;
        private string _filePath;
        private string _fileName;
        private long _fileSize;
        private int _wordCount;
        private bool _useWordCount;
        private bool _isReadOnly;
        private bool TempShouldAlterFileAttributes;

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
            set
            {
                if (value == "")
                    RaisePropertyChanged(ref _filePath, "(No path avaliable)");
                else
                    RaisePropertyChanged(ref _filePath, value);
            }
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

        /// <summary>
        /// Immidiately sets a file's attributes to say whether it's read only or not
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                RaisePropertyChanged(ref _isReadOnly, value);
                if (!TempShouldAlterFileAttributes)
                {
                    NotepadActions.SetFileReadOnly(FilePath, value);
                }
            }
        }

        /// <summary>
        /// If you're certain a file is or is not read only, use this instead of setting <see cref="IsReadOnly"/>, 
        /// because setting that will instantly alter a file's attributer, whereas this doesn't.
        /// </summary>
        /// <param name="isReadOnly"></param>
        public void PreviewSetReadOnly(bool isReadOnly)
        {
            TempShouldAlterFileAttributes = true;
            IsReadOnly = isReadOnly;
            TempShouldAlterFileAttributes = false;
        }

        public DocumentViewModel()
        {
            UseWordCount = PreferencesG.USE_WORD_COUNTER_BY_DEFAULT;
        }
    }
}
