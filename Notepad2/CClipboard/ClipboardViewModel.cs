using Notepad2.Applications;
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

        public string ClipboardText
        {
            get => _clipboardText;
            set => RaisePropertyChanged(ref _clipboardText, value);
        }

        public ClipboardViewModel()
        {
            GetClipboard();
            ClipboardNotification.ClipboardUpdate += ClipboardNotification_ClipboardUpdate;
        }

        private void ClipboardNotification_ClipboardUpdate(object sender, EventArgs e)
        {
            GetClipboard();
        }

        public void GetClipboard()
        {
            object data = CustomClipboard.GetTextObject();
            if (data != null)
                ClipboardText = data.ToString();
            else
                ClipboardText = "";
        }

        public void ShowClipboardWindow()
        {
            ThisApplication.SetClipboardContext(this);
            ThisApplication.ShowClipboard();
        }

        public void ShutdownUpdaterHook()
        {
            // Application will shutdown the clipboard listener when the
            // app shuts down, because other windows will need the listener.
            //ClipboardNotification.ShutdownListener();
            ClipboardNotification.ClipboardUpdate -= ClipboardNotification_ClipboardUpdate;
        }
    }
}
