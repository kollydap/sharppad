using Notepad2.Applications.Controls;
using Notepad2.Applications.History;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using Notepad2.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Notepad2.Applications
{
    /// <summary>
    /// Not technically an MVVM view model because it directly interacts
    /// with window objects, but this class manages windows. This class mainly deals
    /// with finding window objects using ViewModels, Closing windows and pushing to history, and
    /// receiving callbacks from history to re-create windows (aka creating new windows with the old DataContext)
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        /// <summary>
        /// The colletcion of active/opened window as preview items
        /// </summary>
        public ObservableCollection<WindowPreviewControlViewModel> WindowPreviews { get; set; }

        /// <summary>
        /// The History
        /// </summary>
        public WindowHistoryViewModel History { get; set; }

        public ICommand ShutdownApplicationCommand { get; set; }

        public ApplicationViewModel(string[] appArgs)
        {
            WindowPreviews = new ObservableCollection<WindowPreviewControlViewModel>();
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
            History.ReopenLastNotepad();
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

        public NotepadWindow CreateNotepadWindowAndOpenFiles(string[] fileNames, bool loadAndSetAppTheme = false, bool loadWindowPosition = false)
        {
            NotepadWindow window = new NotepadWindow(fileNames);
            SetupNotepadWindow(window, loadAndSetAppTheme, loadWindowPosition);
            return window;
        }

        public NotepadWindow CreateStartupMainNotepadWindow(string[] fileNames, bool loadAndSetAppTheme = true, bool loadWindowPosition = true)
        {
            NotepadWindow window = new NotepadWindow(fileNames);
            window.AddStartupNotepad();
            SetupNotepadWindow(window, loadAndSetAppTheme, loadWindowPosition);
            return window;
        }

        public NotepadWindow CreateStartupMainNotepadWindow(bool loadAndSetAppTheme = true, bool loadGlobalPreferencesG = true)
        {
            NotepadWindow window = new NotepadWindow();
            SetupNotepadWindow(window, loadAndSetAppTheme, loadGlobalPreferencesG);
            window.AddStartupNotepad();
            return window;
        }

        public void CreateAndShowWindowFromViewModel(NotepadViewModel notepad)
        {
            notepad.SetupInformationHook();
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

        public void SetupNotepadWindow(NotepadWindow window, bool loadAndSetAppTheme = false, bool loadGlobalPreferenesG = true)
        {
            SetupNotepadWindowCallbacks(window);
            window.LoadSettings(loadAndSetAppTheme, loadGlobalPreferenesG);
            window.CanSavePreferences = true;
        }

        public void SetupNotepadWindowCallbacks(NotepadWindow window)
        {
            window.WindowFocusedCallback = OnWindowFocused;
            window.WindowShownCallback = OnWindowShown;
            window.WindowClosedCallback = OnWindowClosed;
        }

        #endregion

        public void AddNewNotepadAndPreviewWindowFromWindow(NotepadWindow notepad)
        {
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(notepad.Notepad);
            AddPreviewWindow(wpc);
            SetupNotepadWindowCallbacks(notepad);
            AddWindow(notepad);
            ShowWindow(notepad);
        }

        public void CreateNotepadWindowAndPreview(string[] arguments)
        {
            NotepadWindow window = CreateStartupMainNotepadWindow(arguments);
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public void CreateStartupNotepadWindowAndPreview()
        {
            NotepadWindow window = CreateStartupMainNotepadWindow();
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public void CreateStartupBlankNotepadWindowAndPreview(string[] args)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(args, true, true);
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public void CreateStartupBlankNotepadWindowAndPreview()
        {
            NotepadWindow window = CreateStartupMainNotepadWindow(true, true);
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewWindow(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        public WindowPreviewControlViewModel CreatePreviewControlFromDataContext(NotepadViewModel notepad)
        {
            WindowPreviewControlViewModel winPrev = new WindowPreviewControlViewModel(notepad);
            winPrev.FocusNotepadCallback = FocusWindowFromDataContext;
            winPrev.CloseNotepadCallback = CloseWindowFromPreviewControl;
            return winPrev;
        }

        public WindowPreviewControlViewModel GetPreviewControlFromDataContext(NotepadViewModel notepad)
        {
            foreach (WindowPreviewControlViewModel control in WindowPreviews)
            {
                if (control.Notepad == notepad)
                    return control;
            }

            return null;
        }

        public NotepadWindow GetWindowFromPreviewDataContext(NotepadViewModel notepad)
        {
            foreach (NotepadWindow window in ThisApplication.NotepadWindows)
            {
                if (window.Notepad == notepad)
                    return window;
            }

            return null;
        }

        public void CloseWindowFromPreviewControl(WindowPreviewControlViewModel wpc)
        {
            NotepadWindow nw = GetWindowFromPreviewDataContext(wpc.Notepad);
            if (nw != null)
                CloseWindowAndRemovePreviewFromPreview(wpc);
        }

        public void CloseWindowAndRemovePreviewFromPreview(WindowPreviewControlViewModel wpc)
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
                WindowPreviewControlViewModel wpc = GetPreviewControlFromDataContext(notepad);
                if (wpc != null)
                {
                    CloseWindowAndRemovePreviewFromPreview(wpc);
                }
            }
        }

        public void RemoveWindowAndPreviewFromWindow(NotepadWindow wind)
        {
            RemoveWindow(wind);
            WindowPreviewControlViewModel wpc = GetPreviewControlFromDataContext(wind.Notepad);
            if (wpc != null)
                RemovePreviewControl(wpc);
        }

        public void AddPreviewWindow(WindowPreviewControlViewModel prevWin)
        {
            WindowPreviews.Add(prevWin);
        }

        public void RemovePreviewControl(WindowPreviewControlViewModel prevWin)
        {
            WindowPreviews.Remove(prevWin);
        }

        public void AddWindow(NotepadWindow window)
        {
            ThisApplication.NotepadWindows.Add(window);
        }

        public void RemoveWindow(NotepadWindow window)
        {
            ThisApplication.NotepadWindows.Remove(window);
        }

        public void ShowWindow(NotepadWindow window)
        {
            window.Show();
        }

        public void CloseWindow(NotepadWindow window)
        {
            window.Close();
        }

        public void OnWindowFocused(NotepadWindow window)
        {
            if (window != null)
                ThisApplication.FocusedWindow = window;
        }

        public void OnWindowShown(NotepadWindow window)
        {
            // not really used
        }

        public void OnWindowClosed(NotepadWindow window)
        {
            window?.Notepad?.Shutdown();
            RemoveWindowAndPreviewFromWindow(window);

            int windowsCount = ThisApplication.NotepadWindows.Count;
            if (windowsCount > 0)
            {
                History.PushNotepad(window.Notepad);

                ThisApplication.NotepadWindows[windowsCount - 1]?.Focus();
            }
            else
            {
                ThisApplication.ShutdownApplication();
            }
        }
    }
}
