using Notepad2.Applications.Controls;
using Notepad2.History;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using Notepad2.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Notepad2.Applications
{
    public class ApplicationViewModel : BaseViewModel
    {
        public List<NotepadWindow> NotepadWindows { get; set; }
        public ObservableCollection<WindowPreviewControl> WindowPreviews { get; set; }

        private NotepadWindow _focusedWindow;
        public NotepadWindow FocusedWindow
        {
            get => _focusedWindow;
            set => RaisePropertyChanged(ref _focusedWindow, value);
        }

        public WindowHistoryViewModel History { get; set; }

        public ICommand ShutdownApplicationCommand { get; set; }

        public ApplicationViewModel(string[] appArgs)
        {
            NotepadWindows = new List<NotepadWindow>();
            WindowPreviews = new ObservableCollection<WindowPreviewControl>();
            History = new WindowHistoryViewModel();
            History.OpenWindowCallback = CreateAndShowWindowFromViewModel;
            ParseParameters(appArgs);

            ShutdownApplicationCommand = new Command(ShutdownApp);
        }

        private void ShutdownApp()
        {
            ThisApplication.ShutdownApplication();
        }

        public void ParseParameters(string[] appArgs)
        {
            // opened the app normally.

            if (appArgs.Length > 0)
            {
                string parms = string.Join(" ", appArgs);
                string[] arguments = parms.Split('\"');
                CreateStartupBlankNotepadWindowAndPreview(arguments);
            }

            else
            {
                CreateStartupNotepadWindowAndPreview();
            }
        }

        public void ReopenLastWindow()
        {
            History.ReopenLastWindow();
        }


        // this code below is veeeery messy
        // if you want to clean it up... go ahead :))


        #region Simple methods

        public void OpenFileInNewWindow(string path)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(new string[] { path });
            AddNewNotepadAndPreviewWindowFromWindow(window);
        }

        public void OpenFilesInNewWindow(string[] paths)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(paths);
            AddWindow(window);
            ShowWindow(window);
        }

        #endregion

        public void FocusWindowFromDataContext(NotepadViewModel notepad)
        {
            NotepadWindow win = GetWindowFromPreviewDataContext(notepad);
            if (win != null)
                win.Focus();
        }

        #region Creating windows

        public NotepadWindow CreateBlankNotepadWindow()
        {
            NotepadWindow window = new NotepadWindow();
            SetupNotepadWindow(window);
            return window;
        }

        public NotepadWindow CreateNotepadWindowAndOpenFiles(string[] fileNames, bool loadGlobalTheme = false, bool loadWindowPosition = false)
        {
            NotepadWindow window = new NotepadWindow(fileNames);
            SetupNotepadWindow(window, loadGlobalTheme, loadWindowPosition);
            return window;
        }

        public NotepadWindow CreateStartupMainNotepadWindow(string[] fileNames, bool loadGlobalTheme = true, bool loadWindowPosition = true)
        {
            NotepadWindow window = new NotepadWindow(fileNames);
            window.AddStartupNotepad();
            SetupNotepadWindow(window, loadGlobalTheme, loadWindowPosition);
            return window;
        }

        public NotepadWindow CreateStartupMainNotepadWindow(bool loadGlobalTheme = true, bool loadWindowPosition = true)
        {
            NotepadWindow window = new NotepadWindow();
            window.AddStartupNotepad();
            SetupNotepadWindow(window, loadGlobalTheme, loadWindowPosition);
            return window;
        }

        public void CreateAndShowWindowFromViewModel(NotepadViewModel notepad)
        {
            NotepadWindow window = CreateWindowFromViewModel(notepad);
            AddNewNotepadAndPreviewWindowFromWindow(window);
        }

        #endregion

        #region Setting up windows

        public NotepadWindow CreateWindowFromViewModel(NotepadViewModel notepad)
        {
            NotepadWindow window = new NotepadWindow();
            window.Notepad = notepad;
            SetupNotepadWindow(window);
            return window;
        }

        public void SetupNotepadWindow(NotepadWindow window, bool loadGlobalTheme = false, bool loadWindowPosition = false)
        {
            SetupNotepadWindowCallbacks(window);
            window.LoadSettings(loadGlobalTheme, loadWindowPosition);
            window.CanSavePreferences = true;
        }

        public void SetupNotepadWindowCallbacks(NotepadWindow window)
        {
            window.WindowFocusedCallback = WindowFocused;
            window.WindowShownCallback = WindowShown;
            window.WindowClosedCallback = WindowClosed;
        }

        #endregion

        public void AddNewNotepadAndPreviewWindowFromWindow(NotepadWindow notepad)
        {
            WindowPreviewControl wpc = CreatePreviewControlFromDataContext(notepad.Notepad);
            AddPreviewWindow(wpc);
            SetupNotepadWindowCallbacks(notepad);
            AddWindow(notepad);
            ShowWindow(notepad);
        }

        public void CreateNotepadWindowAndPreview(string[] arguments)
        {
            NotepadWindow window = CreateStartupMainNotepadWindow(arguments);
            WindowPreviewControl wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public void CreateStartupNotepadWindowAndPreview()
        {
            NotepadWindow window = CreateStartupMainNotepadWindow();
            WindowPreviewControl wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public void CreateStartupBlankNotepadWindowAndPreview(string[] args)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(args, true, true);
            WindowPreviewControl wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public void CreateStartupBlankNotepadWindowAndPreview()
        {
            NotepadWindow window = CreateStartupMainNotepadWindow(true, false);
            WindowPreviewControl wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public WindowPreviewControl CreatePreviewControlFromDataContext(NotepadViewModel notepad)
        {
            WindowPreviewControl winPrev = new WindowPreviewControl(notepad);
            winPrev.FocusWindowCallback = FocusWindowFromDataContext;
            winPrev.CloseCallback = CloseWindowFromPreviewControl;
            return winPrev;
        }

        public WindowPreviewControl GetPreviewControlFromDataContext(NotepadViewModel notepad)
        {
            foreach (WindowPreviewControl control in WindowPreviews)
            {
                if (control.Notepad == notepad)
                    return control;
            }

            return null;
        }

        public NotepadWindow GetWindowFromPreviewDataContext(NotepadViewModel notepad)
        {
            foreach (NotepadWindow window in NotepadWindows)
            {
                if (window.Notepad == notepad)
                    return window;
            }

            return null;
        }

        public void CloseWindowFromPreviewControl(WindowPreviewControl wpc)
        {
            NotepadWindow nw = GetWindowFromPreviewDataContext(wpc.Notepad);
            if (nw != null)
                CloseWindowAndRemovePreviewFromPreview(wpc);
        }

        public void CloseWindowAndRemovePreviewFromPreview(WindowPreviewControl wpc)
        {
            RemovePreviewControl(wpc);
            NotepadWindow nw = GetWindowFromPreviewDataContext(wpc.Notepad);
            if (nw != null)
                CloseWindow(nw);
        }

        public void FullyCloseWindowFromDataContext(NotepadViewModel notepad)
        {
            if (notepad != null)
            {
                WindowPreviewControl wpc = GetPreviewControlFromDataContext(notepad);
                if (wpc != null)
                {
                    CloseWindowAndRemovePreviewFromPreview(wpc);
                }
            }
        }

        public void RemoveWindowAndPreviewFromWindow(NotepadWindow wind)
        {
            RemoveWindow(wind);
            WindowPreviewControl wpc = GetPreviewControlFromDataContext(wind.Notepad);
            if (wpc != null)
                RemovePreviewControl(wpc);
        }

        public void AddPreviewWindow(WindowPreviewControl prevWin)
        {
            WindowPreviews.Add(prevWin);
        }

        public void RemovePreviewControl(WindowPreviewControl prevWin)
        {
            WindowPreviews.Remove(prevWin);
        }

        public void AddWindow(NotepadWindow window)
        {
            NotepadWindows.Add(window);
        }

        public void RemoveWindow(NotepadWindow window)
        {
            NotepadWindows.Remove(window);
        }

        public void WindowFocused(NotepadWindow window)
        {
            if (window != null)
                FocusedWindow = window;
        }

        public void ShowWindow(NotepadWindow window)
        {
            window.Show();
        }

        public void CloseWindow(NotepadWindow window)
        {
            window.Close();
        }

        public void WindowShown(NotepadWindow window)
        {
            // not really used
        }

        public void WindowClosed(NotepadWindow window)
        {
            RemoveWindowAndPreviewFromWindow(window);

            int count = NotepadWindows.Count;
            if (count > 0)
            {
                History.WindowClosed(window.Notepad);

                NotepadWindows[count - 1]?.Focus();
            }
            else
                ThisApplication.ShutdownApplication();
        }
    }
}
