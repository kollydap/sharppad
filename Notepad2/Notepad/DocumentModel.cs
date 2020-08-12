using Notepad2.Utilities;
using System;
using System.Runtime.Remoting.Messaging;

namespace Notepad2.Notepad
{
    public class DocumentModel : BaseViewModel
    {
        internal Action TextChanged { private get; set; } 

        private string _text;
        private string _filePath;
        private string _fileName;
        private long _fileSize;

        public string Text
        {
            get => _text;
            set
            {
                RaisePropertyChanged(ref _text, value);
                TextChanged?.Invoke();
                FileSizeBytes = value.Length;
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

        public long FileSizeBytes
        {
            get => _fileSize;
            set => RaisePropertyChanged(ref _fileSize, value);
        }
    }
}
