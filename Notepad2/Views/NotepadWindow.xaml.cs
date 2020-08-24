using Notepad2.FileExplorer;
using Notepad2.FileExplorer.ShellClasses;
using Notepad2.Finding;
using Notepad2.InformationStuff;
using Notepad2.Interfaces;
using Notepad2.Preferences;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TheRThemes;
using static TheRThemes.ThemesController;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Notepad2.Views
{
    /// <summary>
    /// Interaction logic for NotepadWindow.xaml
    /// </summary>
    public partial class NotepadWindow : Window, IMainView
    {
        private Point MouseDownPoint { get; set; }

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
            if (prefs == NewWinPrefs.OpenDefaultNotepadAfterLaunch)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(10);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Notepad.AddStartupItem();
                    });
                });
            }
        }

        public NotepadWindow(string[] filePaths, NewWinPrefs prefs = NewWinPrefs.OpenFilesInParams)
        {
            BeforeInitComponents();
            InitializeComponent();
            InitWindow();
            try
            {
                foreach (string path in filePaths)
                {
                    if (path.IsFile())
                    {
                        Notepad.OpenNotepadFromPath(path, true);
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
                PreferencesG.LoadFromProperties();
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            ListExpander.IsExpanded = !Properties.Settings.Default.closeNLstOnStrt;
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

        #region Notepad items list

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

                        if (ExplorerHelper.CheckPathIsShortcutFile(path, out string shortcutPath))
                        {
                            string shortcutLinkName = Path.GetFileName(shortcutPath);
                            MessageBoxResult results = MessageBox.Show(
                                $"You dropped a shortcut to a file. Open the path it goes to ({shortcutLinkName})? " +
                                $"or open the raw shortcut file (.lnk file)?",
                                "Open shortcut file or actual file",
                                MessageBoxButton.YesNoCancel);
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

        #endregion

        #region Text Editor and Finding

        public void DrawRectangleAtCaret()
        {
            if (MainTextBox != null && showLineThing.IsChecked == true)
            {
                Rect p = MainTextBox.GetCaretLocation();
                double offsetHeight = 1;
                Thickness t = new Thickness(0, p.Y - offsetHeight, 17, MainTextBox.ActualHeight - p.Bottom - offsetHeight); ;
                if (t.Top >= 0 && t.Bottom >= 0)
                {
                    aditionalSelection.Visibility = Visibility.Visible;
                    aditionalSelection.Margin = t;
                }
                else aditionalSelection.Visibility = Visibility.Collapsed;
            }
            else aditionalSelection.Visibility = Visibility.Collapsed;
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

        #endregion

        #region File Explorer

        private void InitialiseTreeFileExplorer()
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                DriveInfo.GetDrives().ToList().ForEach(drive =>
                {
                    treeView.Items.Add(new FileSystemObjectInfo(drive));
                });
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to initialise File Explorer. Error: {e.Message}");
            }
        }

        #region Dragging items

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

        public void SetTheme(ThemeTypes theme)
        {
            ThemesController.SetTheme(theme);
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 0: ThemesController.SetTheme(ThemeTypes.Light); break;
                case 1: ThemesController.SetTheme(ThemeTypes.ColourfulLight); break;
                case 2: ThemesController.SetTheme(ThemeTypes.Dark); break;
                case 3: ThemesController.SetTheme(ThemeTypes.ColourfulDark); break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Notepad.HasAnyNotepadMadeChanges())
            {
                MessageBoxResult mbr = MessageBox.Show(
                    "You have unsaved work. Do you want to save it/them?",
                    "Unsaved Work",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Information);

                if (mbr == MessageBoxResult.Yes)
                {
                    Notepad.SaveAllNotepadItems();
                }
                if (mbr == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (CanSavePreferences)
            {
                try
                {
                    PreferencesG.CLOSE_NOTEPADLIST_BY_DEFAULT = !ListExpander.IsExpanded;
                    PreferencesG.SaveToProperties();
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
                    if (!this.Notepad.IsNotepadNull())
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

        private void FindInputBox_Loaded(object sender, RoutedEventArgs e)
        {
            findInputBox.Focus();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            WindowFocusedCallback?.Invoke(this);
        }

        // idk why i dont add this to the NotepadViewModel... cant be bothered i guess xdddd
        public void HighlightFindResult(FindResult result)
        {
            try
            {
                if (findList.ItemsSource is ObservableCollection<FindResultItemViewModel> items)
                {
                    if (findList.SelectedItem is FindResultItemViewModel fri)
                    {
                        int itemIndex = findList.SelectedIndex;
                        items.Remove(fri);
                        items.Insert(itemIndex, fri);
                        MainTextBox.HighlightSearchResult(result);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to highlight text: {e.Message}");
            }
        }

        public void FocusFindInput(bool focusOrNot)
        {
            if (focusOrNot)
                findInputBox.Focus();
            else
                MainTextBox.Focus();
        }

        public void ScrollItemsIntoView()
        {
            if (NotepadItemsListBox.SelectedItem != null)
                NotepadItemsListBox.ScrollIntoView(NotepadItemsListBox.SelectedItem);
        }

        public void ShowItemsSearcherWindow()
        {
            SearchResultsWindow.ShowWindow();
        }
    }
}
