using Notepad2.RecyclingBin;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Notepad2.Notepad
{
    public class NotepadItemViewModel : BaseViewModel
    {
        public Action<NotepadListItem> Close { get; set; }
        public Action<NotepadListItem> Open { get; set; }
        public Action<NotepadListItem> OpenInFileExplorer { get; set; }
        public Action<NotepadListItem> OpenInNewWindowCallback { get; set; }

        private TextDocumentViewModel _notepad;
        public TextDocumentViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public void ParseMenuCommands(int uids)
        {
            switch (uids)
            {
                case 0: Open?.Invoke(this); break;
                case 1: Close?.Invoke(this); break;
                case 2: OpenInFileExplorer?.Invoke(this); break;
                case 3: DeleteFile(); break;
            }
        }

        public void DeleteFile()
        {
            string fileName = Notepad.Document.FilePath;
            if (File.Exists(Notepad.Document.FilePath))
                Task.Run(() => RecycleBin.SilentSend(fileName));
            Close?.Invoke(this);
        }
    }
}
