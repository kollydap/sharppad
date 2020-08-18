using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Applications.Controls
{
    public class WindowPreviewControlViewModel : BaseViewModel
    {
        private NotepadViewModel _notepad;
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public Action<WindowPreviewControlViewModel> CloseCallback { get; set; }
        public Action<NotepadViewModel> FocusWindowCallback { get; set; }

        public WindowPreviewControlViewModel(NotepadViewModel notepad)
        {
            Notepad = notepad;
        }

        public WindowPreviewControlViewModel()
        {

        }

        public void FocusWindow()
        {
            FocusWindowCallback?.Invoke(Notepad);
        }

        public void Close()
        {
            CloseCallback?.Invoke(this);
        }
    }
}
