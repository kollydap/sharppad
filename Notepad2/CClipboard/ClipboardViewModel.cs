using Notepad2.Utilities;
using System;

namespace Notepad2.CClipboard
{
    /// <summary>
    /// A ViewModel that automatically listens for clipboard changes
    /// and then sets them to a bindable string.
    /// <para>
    /// Info: Changes made to <see cref="ClipboardText"/> do not alter
    /// the actual clipboard, it is just a preview of the clipboard.
    /// </para>
    /// </summary>
    public class ClipboardViewModel : BaseViewModel
    {
        private string _clipboardText;
        private ClipboardWindow ClipboardWin;

        public string ClipboardText
        {
            get => _clipboardText;
            set => RaisePropertyChanged(ref _clipboardText, value);
        }

        public ClipboardViewModel()
        {
            ClipboardWin = new ClipboardWindow();
            ClipboardWin.DataContext = this;
            GetClipboard();
            ClipboardNotification.ClipboardUpdate += ClipboardNotification_ClipboardUpdate;
        }

        private void ClipboardNotification_ClipboardUpdate(object sender, EventArgs e)
        {
            GetClipboard();
        }

        public void GetClipboard()
        {
            object data = CustomClipboard.GetObject();
            if (data != null)
                ClipboardText = data.ToString();
            else
                ClipboardText = "[Error: Data is null or is not a string]";
        }

        public void ShowClipboardWindow()
        {
            ClipboardWin.ShowWindow();
        }

        public void ShutdownUpdaterHook()
        {
            ClipboardNotification.ClipboardUpdate -= ClipboardNotification_ClipboardUpdate;
        }
    }
}
