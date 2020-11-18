using Notepad2.Applications;
using Notepad2.FileExplorer;
using Notepad2.FileExplorer.ShellClasses;
using Notepad2.Finding.NotepadItemFinding;
using Notepad2.Finding.TextFinding;
using Notepad2.InformationStuff;
using Notepad2.Interfaces;
using Notepad2.Preferences;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static TheRThemes.ThemesController;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Notepad2.Views
{
    /// <summary>
    /// Interaction logic for NotepadWindow.xaml
    /// </summary>
    public partial class NotepadWindow : Window, IMainView
    {
        //
        //  e
        //  
        //  Yeah i know using behind code isn't very MVVM-ey, but idk how else to
        //  implement some of the stuff done here in using MVVM. Mainly noticable is
        //  drawing the line rectangle. I guess the find results could be moved to
        //  the view model... but i cant be bothered to atm lol.
        //  also, made by Kettlesimulator/tea-vs-coffee/quad5914/ther/TheR (all are me
        //  with different names xddddddddd shut)
        //  
        //  Have also been working on this app since about 7 months ago
        //  (https://www.reddit.com/r/csharp/comments/f9kiq3/made_a_very_basic_notepad_program_like_windows/)
        //  (started looking like the above... looks no where good then as it does now lol)
        //  i mainly started this to learn more about MVVM. back then i barely knew how to do databinding.
        //  now i know quite a lot thanks to making this, and have also learned how to avoid things i never knew
        //  existed like memory leaks, and even memory leaks in binding ;)))))) (idk why im still typing this)
        //  also i do this as a hobby because i find "upgrading" this notepad app extremely fun tbh
        //  even though i have no clue what to add next (kinda want convert it into a rich editor,
        //  not a plain textbox to allow formatting text to looks nice, but obviously not saving any
        //  of the formats because txt isn't a formateed doc. but eh it would be waay too hard to convert
        //  this app from what it is, into a rich editor, due to how the DocumentModel and stuff works).
        //  and its open source because why not. atleast some others might learn from it :))))
        //  
        //

        private Point MouseDownPoint { get; set; }

        /// <summary>
        /// Used for telling if a window can save to the properties file after it closes. this should
        /// only be enabled for the main window, of the last window opened.
        /// </summary>
        public bool CanSavePreferences { get; set; }

        private ItemSearchResultsWindow SearchResultsWindow { get; set; }

        public NotepadViewModel Notepad
        {
            get => this.DataContext as NotepadViewModel;
            set => this.DataContext = value;
        }

        // A few callback functions for telling the application
        // about what going on with different windows
        public Action<NotepadWindow> WindowFocusedCallback { get; set; }
        public Action<NotepadWindow> WindowShownCallback { get; set; }
        public Action<NotepadWindow> WindowClosedCallback { get; set; }

        #region Constructors

        public NotepadWindow(NewWinPrefs prefs = NewWinPrefs.OpenBlankWindow)
        {
            BeforeInitComponents();
            InitializeComponent();
            InitWindow();

            Task.Run(async () =>
            {
                await Task.Delay(GlobalPreferences.STARTUP_NOTEPAD_ACTIONS_DELAY_MS);
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    if (prefs == NewWinPrefs.OpenDefaultNotepadAfterLaunch)
                    {
                        AddStartupNotepad();
                    }
                });
            });
        }

        public NotepadWindow(string[] filePaths, NewWinPrefs prefs = NewWinPrefs.OpenFilesInParams, bool clearPath = false, bool useStartupDelay = true)
        {
            BeforeInitComponents();
            InitializeComponent();
            InitWindow();

            if (useStartupDelay)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(GlobalPreferences.STARTUP_NOTEPAD_ACTIONS_DELAY_MS);
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        LoadFiles(filePaths, clearPath);
                    });
                });
            }
            else
            {
                LoadFiles(filePaths, clearPath);
            }
        }

        private void LoadFiles(string[] paths, bool clearPath = false)
        {
            try
            {
                foreach (string path in paths)
                {
                    if (path.IsFile())
                    {
                        Notepad.OpenNotepadFromPath(path, true, clearPath: clearPath);
                    }
                    if (path.IsDirectory())
                    {
                        MessageBoxResult a =
                            MessageBox.Show(
                                "Open all files in this directory?",
                                "Open entire directory",
                                MessageBoxButton.YesNo);
                        if (a == MessageBoxResult.Yes)
                        {
                            try
                            {
                                foreach (string file in Directory.GetFiles(path))
                                {
                                    Notepad.OpenNotepadFromPath(file, true);
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(
                                    $"Error opening all files in a directory:  {e.Message}",
                                    "Error opening a directory");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Error opening files:  {e.Message}",
                    "Error opening files");
            }
        }

        #endregion

        #region Window Initialisation

        public void BeforeInitComponents()
        {
            Loaded += NotepadWindow_Loaded;
        }

        public void InitWindow()
        {
            Notepad = new NotepadViewModel(this);

            SearchResultsWindow = new ItemSearchResultsWindow()
            {
                DataContext = Notepad.ItemSearcher
            };

            InitialiseTreeFileExplorer();
        }

        public void LoadSettings(bool loadAndSetAppTheme = true, bool loadGlobalPreferencesG = true)
        {
            if (loadGlobalPreferencesG)
                PreferencesG.LoadFromPropertiesFile();
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            Notepad.LeftTabsExpanded = !Properties.Settings.Default.closeNLstOnStrt;
            Notepad.TopTabsExpanded = !Properties.Settings.Default.closeTopNLstOnStrt;
            showLineThing.IsChecked = Properties.Settings.Default.allowCaretLineOutline;
            if (loadAndSetAppTheme)
            {
                switch (Properties.Settings.Default.Theme)
                {
                    case 1: SetTheme(ThemeTypes.Light); break;
                    case 2: SetTheme(ThemeTypes.Dark); break;
                    case 3: SetTheme(ThemeTypes.ColourfulLight); break;
                    case 4: SetTheme(ThemeTypes.ColourfulDark); break;
                }
            }
        }

        #endregion

        public void AddStartupNotepad()
        {
            Notepad?.AddStartupItem();
        }

        private void NotepadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowShownCallback?.Invoke(this);
        }

        #region Text Editor and Finding

        public void DrawRectangleAtCaret()
        {
            try
            {
                if (MainTextBox != null && showLineThing.IsChecked == true)
                {
                    Rect p = MainTextBox.GetCaretLocation();
                    double offsetHeight = 1;
                    double scrollBarWidth = 18;
                    Thickness t = new Thickness(
                        0,
                        p.Y - offsetHeight,
                        scrollBarWidth,
                        MainTextBox.ActualHeight - p.Bottom - offsetHeight);
                    if (t.Top >= -1 && t.Bottom >= 0)
                    {
                        aditionalSelection.Visibility = Visibility.Visible;
                        aditionalSelection.Margin = t;
                    }
                    // dont remove this else statement otherwise the rendering
                    // goes absolutely mental for the entire app (black areas..)
                    else aditionalSelection.Visibility = Visibility.Collapsed;
                }
                // i think the same for this else statement too
                else aditionalSelection.Visibility = Visibility.Collapsed;
            }
            catch { Information.Show("Failed to draw Line Border on the Text Editor", "LineBorder"); }
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            DrawRectangleAtCaret();
            if (PreferencesG.CAN_ZOOM_EDITOR_CTRL_MWHEEL)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    int fontChange = e.Delta / 100;
                    if (Notepad.Notepad.DocumentFormat.Size > 1)
                        Notepad.Notepad.DocumentFormat.Size += fontChange;
                    if (Notepad.Notepad.DocumentFormat.Size == 1 && fontChange >= 1)
                        Notepad.Notepad.DocumentFormat.Size += fontChange;
                    e.Handled = true;
                }
            }
        }

        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        private void MainTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        private void ShowLineThing_Checked(object sender, RoutedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        private void MainTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        #endregion

        #region File Explorer

        private void InitialiseTreeFileExplorer()
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                DriveInfo.GetDrives().ToList().ForEach(drive =>
                {
                    fileExplorerTree.Items.Add(new FileSystemObjectInfo(drive));
                });
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to initialise File Explorer. Error: {e.Message}");
            }
        }

        #region Dragging items

        private void NotepadItemsListBox_ItemsDropped(object sender, DragEventArgs e)
        {
            if (Notepad != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] droppedItemArray)
                {
                    foreach (string path in droppedItemArray)
                    {
                        if (Directory.Exists(path))
                        {
                            if (MessageBox.Show(
                                "You dropped a folder. Open all files in this folder?",
                                "Open files in dropped folder",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                foreach (string file in Directory.GetFiles(path))
                                {
                                    try
                                    {
                                        Notepad.OpenNotepadFromPath(file);
                                    }
                                    catch (Exception ee) { Information.Show(ee.Message, "DroppedFile"); }
                                }
                            }
                        }

                        else if (ExplorerHelper.CheckPathIsShortcutFile(path, out string shortcutPath))
                        {
                            string shortcutLinkName = Path.GetFileName(shortcutPath);
                            MessageBoxResult results = MessageBox.Show(
                                $"You dropped a shortcut to a file. Open the path it goes to ({shortcutLinkName})? " +
                                $"or open the raw shortcut file (.lnk file)?",
                                "Open shortcut file or actual file",
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Yes);
                            if (results == MessageBoxResult.Yes)
                            {
                                Notepad.OpenNotepadFromPath(shortcutPath);
                            }
                            else if (results == MessageBoxResult.No)
                            {
                                Notepad.OpenNotepadFromPath(path);
                            }
                        }
                        else if (File.Exists(path))
                        {
                            Notepad.OpenNotepadFromPath(path);
                        }
                    }
                }
            }
        }

        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                MouseDownPoint = e.GetPosition(null);
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseDownPoint != e.GetPosition(null) && e.LeftButton == MouseButtonState.Pressed)
            {
                // Found that with treeviewitems, the sender's parent's datacontext is the FileSystemObjectInfo
                if (sender is Rectangle grip && grip.Parent is StackPanel fileItem)
                {
                    if (fileItem.DataContext is FileSystemObjectInfo file)
                    {
                        try
                        {
                            if (File.Exists(file.FileSystemInfo.FullName))
                            {
                                string[] path1 = new string[1] { file.FileSystemInfo.FullName };
                                DragDrop.DoDragDrop(
                                    this,
                                    new DataObject(DataFormats.FileDrop, path1),
                                    DragDropEffects.Copy);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        #endregion

        #endregion

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 0: SetTheme(ThemeTypes.Light); break;
                case 1: SetTheme(ThemeTypes.ColourfulLight); break;
                case 2: SetTheme(ThemeTypes.Dark); break;
                case 3: SetTheme(ThemeTypes.ColourfulDark); break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PreferencesG.SAVE_OPEN_UNCLOSED_FILES)
            {
                ThisApplication.SaveAllUnclosedFilesToStorageLocation(this);
            }

            if (CanSavePreferences)
            {
                try
                {
                    PreferencesG.CLOSE_NOTEPADLIST_BY_DEFAULT = !Notepad.LeftTabsExpanded;
                    Properties.Settings.Default.closeTopNLstOnStrt = !Notepad.TopTabsExpanded;
                    PreferencesG.SavePropertiesToFile();
                    if (WindowState == WindowState.Maximized)
                    {
                        // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                        Properties.Settings.Default.Height = RestoreBounds.Height;
                        Properties.Settings.Default.Width = RestoreBounds.Width;
                    }
                    else
                    {
                        Properties.Settings.Default.Height = this.Height;
                        Properties.Settings.Default.Width = this.Width;
                    }
                    switch (CurrentTheme)
                    {
                        case ThemeTypes.Light: Properties.Settings.Default.Theme = 1; break;
                        case ThemeTypes.Dark: Properties.Settings.Default.Theme = 2; break;
                        case ThemeTypes.ColourfulLight: Properties.Settings.Default.Theme = 3; break;
                        case ThemeTypes.ColourfulDark: Properties.Settings.Default.Theme = 4; break;
                    }
                    if (!this.Notepad.IsNotepadDocumentsNull())
                    {
                        if (Notepad.Notepad.DocumentFormat != null)
                        {
                            Properties.Settings.Default.DefaultFont = this.Notepad.Notepad.DocumentFormat.Family.ToString();
                            Properties.Settings.Default.DefaultFontSize = this.Notepad.Notepad.DocumentFormat.Size;
                        }
                    }
                    Properties.Settings.Default.allowCaretLineOutline = showLineThing.IsChecked ?? true;
                    Properties.Settings.Default.Save();
                }
                catch { }
                //Notepad?.ShutdownInformationHook();
            }

            WindowClosedCallback?.Invoke(this);
        }

        private void ChangeResolutionClick(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((FrameworkElement)sender).Uid))
            {
                case 0: Width = 1024; Height = 576 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 1: Width = 1152; Height = 648 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 2: Width = 1280; Height = 720 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 3: Width = 1706; Height = 960 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 4: Width = 1096; Height = 664; break;
            }
        }

        // this is kinda useless tbh but oh well
        // Clipboard thing
        private void OpenClipboardClick(object sender, MouseButtonEventArgs e)
        {
            Notepad.OurClipboard.ShowClipboardWindow();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            WindowFocusedCallback?.Invoke(this);
        }

        // idk why i dont add this to the NotepadViewModel... cant be bothered i guess xdddd
        public void HighlightFindResult(FindResult result, bool focusTextEditor = true)
        {
            try
            {
                MainTextBox.HighlightSearchResult(result, focusTextEditor);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to highlight text: {e.Message}");
            }
        }

        public void ScrollItemsIntoView()
        {
            if (NotepadItemsListBox.SelectedItem != null)
                NotepadItemsListBox.ScrollIntoView(NotepadItemsListBox.SelectedItem);
            if (TopNotepadItemsListBox.SelectedItem != null)
                TopNotepadItemsListBox.ScrollIntoView(TopNotepadItemsListBox.SelectedItem);
        }

        public void ShowItemsSearcherWindow()
        {
            SearchResultsWindow.ShowWindow();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ThisApplication.DeletePreviouslyUnclosedFiles();
        }

        public void ShowOrHideNotepadsList(bool show)
        {
            if (show)
            {
                DoubleAnimation widthAnim = new DoubleAnimation(225, TimeSpan.FromSeconds(GlobalPreferences.NOTEPADLIST_ANIMATION_SPEED_SEC));
                ThicknessAnimation marginAnim = new ThicknessAnimation(new Thickness(0, 0, 5, 0), TimeSpan.FromSeconds(GlobalPreferences.NOTEPADLIST_ANIMATION_SPEED_SEC));
                AnimationHelpers.SetAnimationRatios(widthAnim);

                NotepadsListGroupbox.BeginAnimation(MarginProperty, marginAnim);
                NotepadsListGroupbox.BeginAnimation(WidthProperty, widthAnim);
            }
            else
            {
                DoubleAnimation widthAnim = new DoubleAnimation(0, TimeSpan.FromSeconds(GlobalPreferences.NOTEPADLIST_ANIMATION_SPEED_SEC));
                ThicknessAnimation marginAnim = new ThicknessAnimation(new Thickness(0), TimeSpan.FromSeconds(GlobalPreferences.NOTEPADLIST_ANIMATION_SPEED_SEC));
                AnimationHelpers.SetAnimationRatios(widthAnim);

                NotepadsListGroupbox.BeginAnimation(MarginProperty, marginAnim);
                NotepadsListGroupbox.BeginAnimation(WidthProperty, widthAnim);
            }
        }

        public void ShowOrHideTopNotepadsList(bool show)
        {
            if (show)
            {
                DoubleAnimation heightAnim = new DoubleAnimation(52, TimeSpan.FromSeconds(GlobalPreferences.NOTEPADLIST_ANIMATION_SPEED_SEC));
                AnimationHelpers.SetAnimationRatios(heightAnim);

                TopNotepadListBorder.BeginAnimation(HeightProperty, heightAnim);
            }
            else
            {
                DoubleAnimation heightAnim = new DoubleAnimation(0, TimeSpan.FromSeconds(GlobalPreferences.NOTEPADLIST_ANIMATION_SPEED_SEC));
                AnimationHelpers.SetAnimationRatios(heightAnim);

                TopNotepadListBorder.BeginAnimation(HeightProperty, heightAnim);
            }
        }

        public void FocusFindInput()
        {
            findBox.Focus();
            findBox.SelectionStart = 0;
        }

        public void ReplaceEditorText(FindResult result, string replaceWith)
        {
            MainTextBox.ReplaceText(result, replaceWith);
        }

        private void findBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Notepad?.Notepad?.FindResults?.StartFind();
        }
    }
}
