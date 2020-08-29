using Notepad2.Applications.Controls;
using Notepad2.Applications.History;
using Notepad2.Preferences;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using Notepad2.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Notepad2.Applications
{
    /// <summary>
    /// Not technically an MVVM view model because it directly interacts with window and usercontrol 
    /// objects/instances, however this class manages windows and previews/history of windows. This class 
    /// mainly deals with Creating, Showing, finding window objects using ViewModels, Closing windows and pushing to history, 
    /// and receiving callbacks from history to re-create windows (by creating new windows with the old DataContext, not re-using
    /// previously closed window objects due to some bugs with the instances still existing in memory, maybe due to GC not collecting idk)
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

            ShutdownApplicationCommand = new Command(ShutdownApp);

            ParseParameters(appArgs);
        }

        // There's a lot of methods below here. it might seem 
        // like it's unnessesary but it kinda isn't.


        #region Creating windows and Previews

        /// <summary>
        /// (ONLY USE WHEN STARTING APPLICATION) Creates a Notepad window and adds an item. 
        /// Loads the theme and preferences by default, meaning  if you create this window but changed 
        /// the theme before this will change it back. Only use when starting up appplication.
        /// </summary>
        /// <param name="loadAndSetAppTheme"></param>
        /// <param name="loadGlobalPreferencesG"></param>
        /// <returns></returns>
        public NotepadWindow CreateApplicationStartupNotepadWindow(bool loadAndSetAppTheme = true, bool loadGlobalPreferencesG = true)
        {
            NotepadWindow window = new NotepadWindow(NewWinPrefs.OpenDefaultNotepadAfterLaunch);
            SetupNotepadWindow(window, loadAndSetAppTheme, loadGlobalPreferencesG);
            return window;
        }

        /// <summary>
        /// Creates a Notepad window using the given view model as it's DataContext
        /// </summary>
        /// <param name="notepad"></param>
        /// <returns></returns>
        public NotepadWindow CreateWindowFromViewModel(NotepadViewModel notepad)
        {
            NotepadWindow window = new NotepadWindow();
            window.Notepad = notepad;
            SetupNotepadWindow(window);
            return window;
        }

        /// <summary>
        /// Creates a Preview item using the given notepad view model
        /// </summary>
        /// <param name="notepad"></param>
        /// <returns></returns>
        public WindowPreviewControlViewModel CreatePreviewControlFromDataContext(NotepadViewModel notepad)
        {
            WindowPreviewControlViewModel winPrev = new WindowPreviewControlViewModel(notepad);
            winPrev.FocusNotepadCallback = FocusWindowFromDataContext;
            winPrev.CloseNotepadCallback = CloseWindowFromPreviewControl;
            return winPrev;
        }

        /// <summary>
        /// Create a completely blank Notepad window with no notepad items
        /// </summary>
        /// <returns></returns>
        public NotepadWindow CreateBlankNotepadWindow()
        {
            NotepadWindow window = new NotepadWindow();
            SetupNotepadWindow(window);
            return window;
        }

        /// <summary>
        /// Create Notepad window with a default item
        /// </summary>
        /// <returns></returns>
        public NotepadWindow CreateNotepadWindowWithItem()
        {
            NotepadWindow window = new NotepadWindow();
            window.AddStartupNotepad();
            SetupNotepadWindow(window);
            return window;
        }

        /// <summary>
        /// Create a Notepad window and open multiple files. does not load theme and position by default
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="loadAndSetAppTheme"></param>
        /// <param name="loadWindowPosition"></param>
        /// <returns></returns>
        public NotepadWindow CreateNotepadWindowAndOpenFiles(
            string[] fileNames, 
            bool loadAndSetAppTheme = false, 
            bool loadWindowPosition = false, 
            bool clearPath = false, 
            bool useStartupDelay = false)
        {
            NotepadWindow window = new NotepadWindow(fileNames, clearPath: clearPath, useStartupDelay: useStartupDelay);
            SetupNotepadWindow(window, loadAndSetAppTheme, loadWindowPosition);
            return window;
        }

        /// <summary>
        /// Creates and shows a Notepad Window using the given view model.
        /// </summary>
        /// <param name="notepad"></param>
        public void CreateAndShowWindowFromViewModel(NotepadViewModel notepad)
        {
            notepad.SetupInformationHook();
            NotepadWindow window = CreateWindowFromViewModel(notepad);
            AddNewNotepadAndPreviewWindowFromWindow(window);
        }

        /// <summary>
        /// (ONLY USE WHEN STARTING APPLICATION) Creates and shows a startup Notepad window.
        /// </summary>
        public void CreateAndShowStartupNotepadWindowAndPreview()
        {
            NotepadWindow window = CreateApplicationStartupNotepadWindow();
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewItem(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        /// <summary>
        /// (ONLY USE WHEN STARTING APPLICATION) Creates and shows a Notepad window which loads the theme/
        /// perferences, and opens the files' paths given in the parameters
        /// </summary>
        /// <param name="args"></param>
        public void CreateAndShowApplicationStartupNotepadWindowAndPreview(string[] args)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(args, true, true);
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewItem(wpc);
            AddWindow(window);
            ShowWindow(window);
        }


        /// <summary>
        /// (ONLY USE WHEN STARTING APPLICATION) Creates and shows a startup Notepad window and opens the previously unclosed files
        /// </summary>
        public void CreateAndShowStartupNotepadWindowAndPreviewAndOpenUnclosedFiles()
        {
            NotepadWindow window = CreateApplicationStartupNotepadWindow();
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewItem(wpc);
            AddWindow(window);
            ShowWindow(window);

            if (PreferencesG.SAVE_OPEN_UNCLOSED_FILES)
            {
                try
                {
                    List<string> filesToOpen = ThisApplication.GetPreviouslyUnclosedFiles();
                    foreach (string file in filesToOpen)
                    {
                        window.Notepad.OpenNotepadFromPath(file, false, true, true);
                    }
                }
                catch { }
                finally
                {
                    ThisApplication.DeletePreviouslyUnclosedFiles();
                }
            }
        }

        /// <summary>
        /// (ONLY USE WHEN STARTING APPLICATION) Creates and shows a Notepad window which loads the theme/
        /// perferences, and opens the files' paths given in the parameters. Also opens the previously unclosed files
        /// </summary>
        /// <param name="args"></param>
        public void CreateAndShowApplicationStartupNotepadWindowAndPreviewAndOpenUnclosedFiles(string[] args)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(args, true, true, useStartupDelay: true);
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);

            AddPreviewItem(wpc);
            AddWindow(window);
            ShowWindow(window);

            if (PreferencesG.SAVE_OPEN_UNCLOSED_FILES)
            {
                try
                {
                    List<string> filesToOpen = ThisApplication.GetPreviouslyUnclosedFiles();
                    foreach (string file in filesToOpen)
                    {
                        window.Notepad.OpenNotepadFromPath(file);
                    }
                }
                catch { }
                finally
                {
                    ThisApplication.DeletePreviouslyUnclosedFiles();
                }
            }
        }

        /// <summary>
        /// Creates and shows a Notepad window with no items. Loads
        /// them theme and preferences.
        /// </summary>
        public void CreateAndShowBlankNotepadWindowAndPreview()
        {
            NotepadWindow window = CreateBlankNotepadWindow();
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewItem(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        /// <summary>
        /// Creates and shows a Notepad window with no items. Loads
        /// them theme and preferences.
        /// </summary>
        public void CreateAndShowNotepadWindowAndPreviewWithDefaultItem()
        {
            NotepadWindow window = CreateNotepadWindowWithItem();
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(window.Notepad);
            AddPreviewItem(wpc);
            AddWindow(window);
            ShowWindow(window);
        }

        #endregion

        #region Setting up windows

        /// <summary>
        /// Sets up a Notepad window's callbacks, does not load the theme by default but does load
        /// the preferencesG by default.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="loadAndSetAppTheme"></param>
        /// <param name="loadGlobalPreferenesG"></param>
        public void SetupNotepadWindow(NotepadWindow window, bool loadAndSetAppTheme = false, bool loadGlobalPreferenesG = true)
        {
            SetupNotepadWindowCallbacksAndProperties(window);
            window.LoadSettings(loadAndSetAppTheme, loadGlobalPreferenesG);
            window.CanSavePreferences = true;
        }

        /// <summary>
        /// Sets up a Notepad window's callbacks (focusing, closed, etc)
        /// </summary>
        /// <param name="window"></param>
        public void SetupNotepadWindowCallbacksAndProperties(NotepadWindow window)
        {
            window.WindowFocusedCallback = OnWindowFocused;
            window.WindowShownCallback = OnWindowShown;
            window.WindowClosedCallback = OnWindowClosed;
            window.Notepad.View = window;
        }

        #endregion

        #region Getting Windows and Previews

        /// <summary>
        /// Shows a given Notepad window
        /// </summary>
        /// <param name="notepad"></param>
        public void AddNewNotepadAndPreviewWindowFromWindow(NotepadWindow notepad)
        {
            WindowPreviewControlViewModel wpc = CreatePreviewControlFromDataContext(notepad.Notepad);
            AddPreviewItem(wpc);
            SetupNotepadWindowCallbacksAndProperties(notepad);
            AddWindow(notepad);
            ShowWindow(notepad);
        }

        /// <summary>
        /// Gets a preview item using a Notepad window's view model
        /// </summary>
        /// <param name="notepad"></param>
        /// <returns>null if there's no windows with the given viewmodel. else returns the preview</returns>
        public WindowPreviewControlViewModel GetPreviewControlFromDataContext(NotepadViewModel notepad)
        {
            foreach (WindowPreviewControlViewModel control in WindowPreviews)
            {
                if (control.Notepad == notepad)
                    return control;
            }

            return null;
        }

        /// <summary>
        /// Gets a Notepad window from a view model
        /// </summary>
        /// <param name="notepad"></param>
        /// <returns>null if there's no windows with the given viewmodel. else returns the Notepad window</returns>
        public NotepadWindow GetWindowFromPreviewDataContext(NotepadViewModel notepad)
        {
            foreach (NotepadWindow window in WindowManager.NotepadWindows)
            {
                if (window.Notepad == notepad)
                    return window;
            }

            return null;
        }

        #endregion

        #region Closing Windows and Previews

        public void CloseWindowFromPreviewControl(WindowPreviewControlViewModel wpc)
        {
            CloseWindowAndRemovePreviewFromPreview(wpc);
        }

        public void CloseWindowAndRemovePreviewFromPreview(WindowPreviewControlViewModel wpc)
        {
            RemovePreviewItem(wpc);
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
                RemovePreviewItem(wpc);
        }

        #endregion

        #region Useful methods

        public void ParseParameters(string[] appArgs)
        {
            // opened the app normally.

            if (appArgs.Length > 0)
            {
                string parms = string.Join(" ", appArgs);
                string[] arguments = parms.Split('\"');
                CreateAndShowApplicationStartupNotepadWindowAndPreviewAndOpenUnclosedFiles(arguments);
            }

            else
            {
                CreateAndShowStartupNotepadWindowAndPreviewAndOpenUnclosedFiles();
            }
        }

        public void OpenFileInNewWindow(string path, bool clearPath = false, bool useStartupDelay = false)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(new string[] { path }, clearPath: clearPath, useStartupDelay: useStartupDelay);
            AddNewNotepadAndPreviewWindowFromWindow(window);
        }

        public void OpenFilesInNewWindow(string[] paths)
        {
            NotepadWindow window = CreateNotepadWindowAndOpenFiles(paths);
            AddWindow(window);
            ShowWindow(window);
        }

        public void FocusWindowFromDataContext(NotepadViewModel notepad)
        {
            NotepadWindow win = GetWindowFromPreviewDataContext(notepad);
            if (win != null)
                win.Focus();
        }

        #endregion

        #region Other methods

        public void ReopenLastWindow()
        {
            History.ReopenLastNotepad();
        }

        public void AddPreviewItem(WindowPreviewControlViewModel prevWin)
        {
            WindowPreviews.Add(prevWin);
        }

        public void RemovePreviewItem(WindowPreviewControlViewModel prevWin)
        {
            WindowPreviews.Remove(prevWin);
        }

        public void AddWindow(NotepadWindow window)
        {
            WindowManager.NotepadWindows.Add(window);
        }

        public void RemoveWindow(NotepadWindow window)
        {
            WindowManager.NotepadWindows.Remove(window);
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
                WindowManager.FocusedWindow = window;
        }

        public void OnWindowShown(NotepadWindow window)
        {
            // not really used
        }

        public void OnWindowClosed(NotepadWindow window)
        {
            window?.Notepad?.Shutdown();
            RemoveWindowAndPreviewFromWindow(window);

            int windowsCount = WindowManager.NotepadWindows.Count;
            if (windowsCount > 0)
            {
                History.PushNotepad(window.Notepad);

                WindowManager.NotepadWindows[windowsCount - 1]?.Focus();
            }
            else
            {
                ThisApplication.ShutdownApplication();
            }
        }

        /// <summary>
        /// Will write any unclosed files to the 
        /// </summary>
        public void CheckForAnyOpenFilesInEveryWindowAndWriteToUnsavedFilesStorage()
        {
            foreach(NotepadWindow window in WindowManager.NotepadWindows)
            {
                ThisApplication.SaveAllUnclosedFilesToStorageLocation(window);
            }
        }

        #endregion

        private void ShutdownApp()
        {
            ThisApplication.ShutdownApplication();
        }
    }
}
