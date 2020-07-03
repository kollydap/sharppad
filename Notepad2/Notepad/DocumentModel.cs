using Notepad2.Finding;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;

namespace Notepad2.Notepad
{
    public class DocumentModel : BaseViewModel
    {
        public delegate void TextChangedEventArgs(string newText);
        public event TextChangedEventArgs TextChanged;

        private string _text;
        private string _filePath;
        private string _fileName;
        private double _fileSize;

        public string Text
        {
            get => _text;
            set
            {
                RaisePropertyChanged(ref _text, value);
                TextChanged?.Invoke(value);
                FileSize = value.Length / 1000.0d;
            }
        }

        public string FilePath
        {
            get => _filePath;
            set => RaisePropertyChanged(ref _filePath, value);
        }

        public string FileName
        {
            get => _fileName;
            set => RaisePropertyChanged(ref _fileName, value);
        }

        public double FileSize
        {
            get => _fileSize;
            set => RaisePropertyChanged(ref _fileSize, value);
        }

        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(FileName) ||
                    string.IsNullOrEmpty(FilePath))
                    return true;

                return false;
            }
        }
    }
}
