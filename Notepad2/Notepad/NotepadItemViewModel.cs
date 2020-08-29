using Notepad2.Applications;
using Notepad2.CClipboard;
using Notepad2.FileExplorer;
using Notepad2.RecyclingBin;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Notepad2.Notepad
{
    public class NotepadItemViewModel : BaseViewModel
    {
        private TextDocumentViewModel _notepad;
        public TextDocumentViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public ICommand CloseFileCommand { get; }
        public ICommand OpenInFileExplorerCommand { get; }
        public ICommand OpenInNewWindowCommand { get; }
        public ICommand DeleteFileCommand { get; }
        public ICommand RefreshContentsCommand { get; }
        public ICommand ShowPropertiesCommand { get; }
        public ICommand CopyPathCommand { get; }

        public Action<NotepadItemViewModel> RemoveNotepadCallback { get; set; }
        public Action<NotepadItemViewModel> OpenInNewWindowCallback { get; set; }
        // true = up, false = down
        public Action<bool> MoveItemCallback { get; set; }

        public NotepadItemViewModel()
        {
            CloseFileCommand = new Command(Remove);
            OpenInFileExplorerCommand = new Command(OpenInFileExplorer);
            OpenInNewWindowCommand = new Command(OpenInAnotherWindow);
            DeleteFileCommand = new Command(DeleteFile);
            RefreshContentsCommand = new Command(RefreshContents);
            ShowPropertiesCommand = new Command(ShowProperties);
            CopyPathCommand = new Command(CopyPath);
        }

        public void Remove()
        {
            RemoveNotepadCallback?.Invoke(this);
        }

        public void OpenInFileExplorer()
        {
            Notepad?.Document?.FilePath.OpenInFileExplorer();
        }

        public void DeleteFile()
        {
            string fileName = Notepad.Document.FilePath;
            if (Notepad?.Document?.FilePath.IsFile() == true)
                Task.Run(() => RecycleBin.SilentSend(fileName));
            Remove();
        }

        public void SetFileExtension(string extensionID)
        {
            Notepad.Document.FileName = 
                FileExtensionsHelper.GetFileExtension(Notepad.Document.FileName, extensionID);
        }

        public void OpenInAnotherWindow()
        {
            OpenInNewWindowCallback?.Invoke(this);
        }

        public void ShowProperties()
        {
            if (Notepad?.Document != null)
            {
                WindowManager.PropertiesView.Properties.Show();
                if (Notepad.Document.FilePath.IsFile())
                {
                    if (!Notepad.HasMadeChanges)
                        WindowManager.PropertiesView.Properties.FetchProperties(Notepad.Document.FilePath);
                    else
                        WindowManager.PropertiesView.Properties.FetchFromDocument(Notepad.Document);
                }
                else
                {
                    WindowManager.PropertiesView.Properties.FetchFromDocument(Notepad.Document);
                }
            }
        }

        public void RefreshContents()
        {
            Notepad.UpdateFileContents();
        }

        public void CopyPath()
        {
            if (Notepad?.Document?.FilePath.IsFile() == true)
            {
                CustomClipboard.SetTextObject(Notepad.Document.FilePath);
            }
        }

        public void MoveItemUp()
        {
            MoveItemCallback?.Invoke(true);
        }

        public void MoveItemDown()
        {
            MoveItemCallback?.Invoke(false);
        }
    }
}
