using Microsoft.Win32;
using Notepad2.Applications;
using Notepad2.CClipboard;
using Notepad2.FileExplorer;
using Notepad2.Finding;
using Notepad2.History;
using Notepad2.InformationStuff;
using Notepad2.Interfaces;
using Notepad2.Notepad;
using Notepad2.Preferences;
using Notepad2.Preferences.Views;
using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace Notepad2.ViewModels
{
    public class NotepadViewModel : BaseViewModel
    {
        #region Private Fields

        private TextDocumentViewModel _notepad;
        private TextDocumentViewModel _selectedNotepadItem;
        private FindViewModel _find;
        private int _selectedIndex;
        private bool _notepadAvaliable;
        private bool _findExpanded;

        #endregion

        #region Public Properties

        /// <summary>
        /// Holds a list of <see cref="TextDocumentViewModel"/>, for use in a ListBox
        /// (the one on the left side of the program)
        /// <para>
        /// Call this, "the list on the left"
        /// </para>
        /// </summary>
        public ObservableCollection<TextDocumentViewModel> NotepadItems { get; set; }

        /// <summary>
        /// The selected index of the currently selected <see cref="TextDocumentViewModel"/>
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => RaisePropertyChanged(ref _selectedIndex, value);
        }

        /// <summary>
        /// The currently selected <see cref="TextDocumentViewModel"/>
        /// </summary>
        public TextDocumentViewModel SelectedNotepadItem
        {
            get => _selectedNotepadItem;
            set => RaisePropertyChanged(ref _selectedNotepadItem, value, () =>
            {
                Notepad = value;
                UpdateSelectedNotepad();
            });
        }

        /// <summary>
        /// Says whether a notepad is avaliable (aka, if there's any 
        /// <see cref="TextDocumentViewModel"/>s in the list on the left)
        /// </summary>
        public bool NotepadAvaliable
        {
            get => _notepadAvaliable;
            set => RaisePropertyChanged(ref _notepadAvaliable, value);
        }

        /// <summary>
        /// Says whether the Find panel is expanded or not
        /// </summary>
        public bool FindExpanded
        {
            get => _findExpanded;
            set => RaisePropertyChanged(ref _findExpanded, value, FocusFind);
        }

        /// <summary>
        /// The currently selected <see cref="TextDocumentViewModel"/>
        /// </summary>
        public TextDocumentViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        /// <summary>
        /// A ViewModel for the Finding Panel for finding text within the selected Notepad
        /// </summary>
        public FindViewModel Find
        {
            get => _find;
            set => RaisePropertyChanged(ref _find, value);
        }

        public InformationViewModel InformationView { get; set; }

        //unexpected communism
        /// <summary>
        /// A ViewModel used for binding to the clipboard
        /// </summary>
        public ClipboardViewModel OurClipboard { get; set; }

        /// <summary>
        /// A ViewModel for dealing/altering with application 
        /// properties/preferences in an easily accessible way
        /// </summary>
        public PreferencesViewModel Preference { get; set; }

        /// <summary>
        /// A ViewModel for dealing with the history of recently closed files.
        /// </summary>
        public HistoryViewModel History { get; set; }

        /// <summary>
        /// An interface for providing a few functions to be executed on the notepad window,
        /// without having an instance to the window. So this is MVVM friendly i think
        /// </summary>
        public IMainView View { get; }

        #endregion

        #region Commands

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand OpenDirectoryCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand CloseSelectedNotepadCommand { get; }
        public ICommand CloseAllNotepadsCommand { get; }
        public ICommand MoveItemCommand { get; }
        public ICommand PrintFileCommand { get; }
        public ICommand AutoShowFindMenuCommand { get; }
        public ICommand ShowHelpCommand { get; }

        public ICommand NewWindowCommand { get; }
        public ICommand ReopenLastWindowCommand { get; }
        public ICommand CloseWindowCommand { get; }
        public ICommand CloseViewWithCheckCommand { get; }
        public ICommand CloseAllWindowsCommand { get; }

        public ICommand ShowWindowManagerCommand { get; }

        public ICommand SortListCommand { get; }


        #endregion

        #region Constructor

        public NotepadViewModel(IMainView view)
        {
            View = view;

            History = new HistoryViewModel();
            Preference = new PreferencesViewModel();
            OurClipboard = new ClipboardViewModel();
            InformationView = new InformationViewModel();
            NotepadItems = new ObservableCollection<TextDocumentViewModel>();

            SetupInformationHook();
            History.OpenFileCallback = ReopenNotepadFromHistory;

            NewCommand = new Command(AddDefaultNotepadItem);
            OpenCommand = new Command(OpenNotepadFromFileExplorer);
            OpenDirectoryCommand = new Command(OpenNotepadsFromDirectoryExplorer);
            SaveCommand = new Command(SaveSelectedNotepad);
            SaveAsCommand = new Command(SaveSelectedNotepadAs);
            SaveAllCommand = new Command(SaveAllNotepadItems);
            CloseSelectedNotepadCommand = new Command(CloseSelectedNotepad);
            CloseAllNotepadsCommand = new Command(CloseAllNotepads);
            MoveItemCommand = new CommandParam<string>(MoveItem);
            PrintFileCommand = new Command(PrintFile);
            AutoShowFindMenuCommand = new Command(OpenFindWindow);
            ShowHelpCommand = new Command(ThisApplication.ShowHelp);

            NewWindowCommand = new Command(NewWindow);
            ReopenLastWindowCommand = new Command(ReopenLastWindow);
            CloseWindowCommand = new Command(CloseWindow);
            CloseViewWithCheckCommand = new Command(CloseWindowWithCheck);
            CloseAllWindowsCommand = new Command(CloseAllWindow);
            ShowWindowManagerCommand = new Command(ShowWindowManager);

            SortListCommand = new CommandParam<string>(SortItems);

            Information.Show("Notepad Window loaded", InfoTypes.Information);
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sorts all of the notepad documents/items in a certain way
        /// </summary>
        /// <param name="sortBy">fn to sort by file name, or fs to sort by the file size</param>
        public void SortItems(string sortBy)
        {
            switch (sortBy.ToString())
            {
                case "fn":
                    {
                        List<TextDocumentViewModel> sortedNames = NotepadItems.OrderBy(a => a.Document.FileName).ToList();
                        int index = SelectedIndex;
                        NotepadItems.Clear();
                        foreach (TextDocumentViewModel item in sortedNames)
                        {
                            NotepadItems.Add(item);
                        }
                        SelectedIndex = index;
                    }
                    break;
                case "fs":
                    {
                        List<TextDocumentViewModel> sortedSizes = NotepadItems.OrderBy(a => a.Document.FileSizeBytes).ToList();
                        int index = SelectedIndex;
                        NotepadItems.Clear();
                        foreach (TextDocumentViewModel item in sortedSizes)
                        {
                            NotepadItems.Add(item);
                        }
                        SelectedIndex = index;
                    }
                    break;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns whether the currently selected 
        /// <see cref="TextDocumentViewModel"/> is null
        /// </summary>
        /// <returns>a bool. xdddd</returns>
        public bool IsNotepadNull()
        {
            return Notepad == null || Notepad.Document == null || Notepad.DocumentFormat == null;
        }

        /// <summary>
        /// Prints (on A4 Paper) a page to your printer. Shows a dialog to specify settings (untested)
        /// </summary>
        public void PrintFile()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument
                {
                    PagePadding = new Thickness(50)
                };

                flowDocument.Blocks.Add(new Paragraph(new Run(Notepad.Document.Text)));
                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocument).DocumentPaginator, "Printed from SharpPad");
            }
        }

        /// <summary>
        /// Returns whether any Notepad Items have been edited or not
        /// </summary>
        /// <returns></returns>
        public bool HasAnyNotepadMadeChanges()
        {
            foreach (TextDocumentViewModel nli in NotepadItems)
            {
                if (nli != null && nli is TextDocumentViewModel fivm)
                    if (fivm != null && fivm.HasMadeChanges)
                        return true;
            }
            return false;
        }

        #endregion

        // Notepad Functions

        #region Adding Notepads

        /// <summary>
        /// Adds a new default Notepad Item
        /// </summary>
        public void AddDefaultNotepadItem()
        {
            AddNotepadItem(CreateDefaultStyleNotepadItem("", $"new{NotepadItems.Count}.txt", null), true);
        }

        /// <summary>
        /// Adds a <see cref="TextDocumentViewModel"/> to the list on the left.
        /// </summary>
        /// <param name="nli">A <see cref="TextDocumentViewModel"/> to be added</param>
        public void AddNotepadItem(TextDocumentViewModel nli, bool selectItem = false)
        {
            if (nli != null)
            {
                nli.HasMadeChanges = false;
                NotepadItems.Add(nli);
                if (selectItem)
                    SelectedNotepadItem = nli;
                Information.Show($"Added FileItem: {nli.Document.FileName}", InfoTypes.FileIO);
            }
        }

        /// <summary>
        /// Adds a normal notepad item, and then selects it 
        /// (for simplicity and quick access... maybe)
        /// </summary>
        public void AddStartupItem()
        {
            TextDocumentViewModel nli = CreateDefaultStyleNotepadItem("", $"new{NotepadItems.Count}.txt", null);
            AddNotepadItem(nli);
            SelectedNotepadItem = nli;
        }

        public void AddNotepadFromViewModel(TextDocumentViewModel notepad, bool selectItem = true)
        {
            SetupNotepadItemCallbacks(notepad);
            AddNotepadItem(notepad);
            if (selectItem)
                SelectedNotepadItem = notepad;
        }

        #endregion

        #region Closing Notepads

        /// <summary>
        /// if the given <see cref="TextDocumentViewModel"/> is inside 
        /// the list on the left, this will remove it.
        /// </summary>
        /// <param name="nli">The <see cref="TextDocumentViewModel"/> to be removed</param>
        public void CloseNotepadItem(TextDocumentViewModel nli)
        {
            CloseNotepadItem(nli, true);
        }

        /// <summary>
        /// if the given <see cref="TextDocumentViewModel"/> is inside 
        /// the list on the left, this will remove it.
        /// </summary>
        /// <param name="nli">The <see cref="TextDocumentViewModel"/> to be removed</param>
        public void CloseNotepadItem(TextDocumentViewModel nli, bool checkHasSavedFile = true)
        {
            if (checkHasSavedFile && nli.HasMadeChanges)
            {
                if (MessageBox.Show(
                    "You have unsaved work. Do you want to save it/them?",
                    "Unsaved Work",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information,
                    MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    SaveNotepad(nli);
                }
            }
            //AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemCLOSE);
            RemoveNotepadItem(nli);
            History.PushFile(nli);
            Information.Show($"Removed FileItem: [{nli.Document.FileName}]", InfoTypes.FileIO);
            UpdateSelectedNotepad();
        }

        // NotepadListItems would've had a shutdown method for
        // getting rid of static handlers or other things...
        public void RemoveNotepadItem(TextDocumentViewModel nli)
        {
            //nli?.Shutdown();
            NotepadItems?.Remove(nli);
        }

        public void ShutdownAllNotepadItems()
        {
            //if (NotepadItems != null)
            //{
            //    foreach (TextDocumentViewModel nli in NotepadItems)
            //    {
            //        nli.Shutdown();
            //    }
            //}
        }

        /// <summary>
        /// If a Notepad Item is selected, this will close it.
        /// </summary>
        private void CloseSelectedNotepad()
        {
            if (SelectedNotepadItem != null)
            {
                CloseNotepadItem(SelectedNotepadItem);
            }
        }

        /// <summary>
        /// Closes every single Notepad Item inside the list on the left
        /// </summary>
        private void CloseAllNotepads()
        {
            ShutdownAllNotepadItems();
            NotepadItems.Clear();
            Information.Show($"Cleared {NotepadItems.Count} NotepadItems", InfoTypes.FileIO);
            Notepad = null;
        }

        #endregion

        #region Create

        /// <summary>
        /// Gets the font from the properties. this is automatically set as the last notepad 
        /// window's last notepad document's font when the application closes (i think lol)
        /// </summary>
        /// <returns></returns>
        public FontFamily GetDefaultFont()
        {
            return
                !string.IsNullOrEmpty(Properties.Settings.Default.DefaultFont)
                    ? new FontFamily(Properties.Settings.Default.DefaultFont)
                    : new FontFamily("Consolas");
        }

        /// <summary>
        /// Gets the font size from properties. this is automatically set as the last notepad 
        /// window's last notepad document's font when the application closes (i think lol)
        /// </summary>
        /// <returns></returns>
        public double GetDefaultFontSize()
        {
            double fSize = Properties.Settings.Default.DefaultFontSize;
            return fSize > 0 ? fSize : 14;
        }

        /// <summary>
        /// Creates and returns a completely default <see cref="TextDocumentViewModel"/>, with preset fonts,
        /// fontsizes, etc, ready to be added to the list on the left.
        /// </summary>
        /// <param name="text">The text the Notepad Item will have</param>
        /// <param name="itemName">The header/file name of the Notepad Item</param>
        /// <param name="itemPath">The file path of the Notepad Item</param>
        /// <returns></returns>
        public TextDocumentViewModel CreateDefaultStyleNotepadItem(string text, string itemName, string itemPath)
        {
            FormatModel fm = new FormatModel()
            {
                Family = GetDefaultFont(),
                Size = GetDefaultFontSize(),
                IsWrapped = PreferencesG.WRAP_TEXT_BY_DEFAULT
            };

            return CreateNotepadItem(text, itemName, itemPath, fm);
        }

        /// <summary>
        /// Creates a <see cref="TextDocumentViewModel"/> with the given text, name, path and text formats
        /// </summary>
        /// <param name="text">The text the Notepad Item will have</param>
        /// <param name="fileName">The header/name of the Notepad Item</param>
        /// <param name="filePath">The file path of the Notepad Item</param>
        /// <param name="fm">The styles the Notepad Item will have</param>
        /// <returns></returns>
        public TextDocumentViewModel CreateNotepadItem(string text, string fileName, string filePath, FormatModel fm)
        {
            TextDocumentViewModel fivm = new TextDocumentViewModel();
            fivm.Document.Text = text;
            fivm.Document.FileName = fileName;
            fivm.Document.FilePath = filePath;
            fivm.DocumentFormat = fm;

            fivm.FindResults.NextTextFoundCallback = OnNextTextFound;
            fivm.HasMadeChanges = false;
            SetupNotepadItemCallbacks(fivm);

            return fivm;
        }

        /// kettlesimulator made this xd
        /// <summary>
        /// Sets up the callback functions for a notepad document 
        /// (e.g, the ones to close the item or open in another window).
        /// </summary>
        /// <param name="nli"></param>
        public void SetupNotepadItemCallbacks(TextDocumentViewModel nli)
        {
            nli.Close = CloseNotepadItem;
            nli.OpenInNewWindowCallback = OpenNotepadItemInNewWindow;
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Sets the selected <see cref="TextDocumentViewModel"/> using 
        /// the selected <see cref="TextDocumentViewModel"/>, and also other stuff
        /// </summary>
        private void UpdateSelectedNotepad()
        {
            TextDocumentViewModel item = SelectedNotepadItem;
            if (item != null)
            {
                OpenNotepadItem(item);
                Find = item.FindResults;
            }
            UpdateOthers();
        }

        /// <summary>
        /// Updates stuff
        /// </summary>
        public void UpdateOthers()
        {
            NotepadAvaliable = NotepadItems.Count > 0;
            if (NotepadItems.Count == 0)
                Notepad = null;
        }

        #endregion

        // File IO

        #region Opening

        public void OpenNotepadItem(TextDocumentViewModel notepadItem)
        {
            if (notepadItem != null)
            {
                Notepad = notepadItem;
            }
        }

        public void OpenNotepadFromFileExplorer()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Select Files to open",
                    Multiselect = true
                };

                if (ofd.ShowDialog() == true)
                {
                    int i;
                    for (i = 0; i < ofd.FileNames.Length; i++)
                    {
                        string paths = ofd.FileNames[i];
                        OpenNotepadFromPath(paths, true);
                    }
                    Information.Show($"Opened {i} files", InfoTypes.FileIO);
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from f.explorer"); }
        }

        public void OpenNotepadsFromDirectoryExplorer()
        {
            System.Windows.Forms.FolderBrowserDialog ofd =
                new System.Windows.Forms.FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int i;
                if (ofd.SelectedPath.IsDirectory())
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
                            string[] files = Directory.GetFiles(ofd.SelectedPath);
                            for (i = 0; i < files.Length; i++)
                            {
                                string file = files[i];
                                OpenNotepadFromPath(file);
                            }
                            Information.Show($"Opened {i} files", InfoTypes.FileIO);
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

        /// <summary>
        /// Reads the text from a file path and creates and adds a new notepad document
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectFile"></param>
        public void OpenNotepadFromPath(string path, bool selectFile = false)
        {
            try
            {
                if (File.Exists(path))
                {
                    FileInfo fInfo = new FileInfo(path);
                    if(fInfo.Length > GlobalPreferences.MAX_FILE_SIZE)
                    {
                        MessageBox.Show("File size too big: Cannot open because a file this big would be extremely laggy");
                        return;
                    }

                    if (fInfo.Length > GlobalPreferences.WARN_FILE_SIZE_BYTES && 
                        MessageBox.Show(
                            "The file is very big in size and might lag the program. Continue to open?",
                            "File very big. Open anyway?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
                        return;

                    string text = NotepadActions.ReadFile(path);
                    TextDocumentViewModel item = CreateDefaultStyleNotepadItem(text, Path.GetFileName(path), path);
                    AddNotepadItem(item);

                    if (fInfo.IsReadOnly)
                        item.HasMadeChanges = true;

                    Information.Show($"Opened file: [{path}]", InfoTypes.FileIO);
                    if (selectFile && item != null)
                        SelectedNotepadItem = item;
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from path"); }
        }

        public void ReopenNotepadFromHistory(TextDocumentViewModel notepad)
        {
            if (notepad != null)
            {
                Information.Show($"Reopening [{notepad.Document.FileName}] from history", "History");
                AddNotepadFromViewModel(notepad);
            }
        }

        #endregion

        #region Saving

        public void SaveNotepad(TextDocumentViewModel fivm)
        {
            try
            {
                if (File.Exists(fivm.Document.FilePath))
                {
                    string oldFilePath = fivm.Document.FilePath;
                    string oldFileName = Path.GetFileName(fivm.Document.FilePath);
                    string extension = Path.GetExtension(fivm.Document.FilePath);
                    string folderName = Path.GetDirectoryName(fivm.Document.FilePath);
                    string newFileName =
                        Path.HasExtension(Path.Combine(folderName, fivm.Document.FileName))
                        ? fivm.Document.FileName
                        : fivm.Document.FileName + extension;
                    string newFilePath = Path.Combine(folderName, newFileName);
                    if (fivm.Document.FilePath != newFilePath)
                    {
                        if (MessageBox.Show($"You are about to overwrite {oldFileName} " +
                            $"with {newFileName}, do you want to continue?", "Overrite file?",
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (SaveFile(fivm.Document.FilePath, fivm.Document.Text))
                            {
                                fivm.HasMadeChanges = false;
                            }

                            File.Move(fivm.Document.FilePath, newFilePath);
                            fivm.Document.FileName = newFileName;
                            fivm.Document.FilePath = newFilePath;

                            Information.Show($"Renamed [{oldFileName}] to [{newFileName}] successfully", "File IO");
                        }
                    }
                    else
                    {
                        if (SaveFile(fivm.Document.FilePath, fivm.Document.Text))
                        {
                            fivm.HasMadeChanges = false;
                        }

                        fivm.Document.FileName = newFileName;
                        fivm.Document.FilePath = newFilePath;
                    }
                }
                else
                    SaveNotepadAs(fivm);
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving a (manual) notepad item"); }
        }

        public void SaveNotepadAs(TextDocumentViewModel fivm)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter =
                        "Plain Text (.txt)|*.txt|" +
                        "Text..? ish (.text)|*.text|" +
                        "C# File (.cs)|*.cs|" +
                        "C File (.c)|*.c|" +
                        "C++ File (.cpp)|*.cpp|" +
                        "C/C++ Header File (.h)|*.h|" +
                        "XAML File (.xaml)|*.xaml|" +
                        "XML File (.xml)|*.xml|" +
                        "HTM File (.htm)|*.htm|" +
                        "HTML File (.html)|*.html|" +
                        "CSS File (.css)|*.css|" +
                        "JS File (.js)|*.js|" +
                        "EXE File (.exe)|*.exe|" +
                        "All files|*.*";
                sfd.Title = "Select Files to save";
                sfd.FileName = fivm.Document.FileName;
                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == true)
                {
                    string newFilePath = sfd.FileName;
                    if (SaveFile(newFilePath, fivm.Document.Text))
                    {
                        fivm.HasMadeChanges = false;
                    }

                    fivm.Document.FileName = Path.GetFileName(newFilePath);
                    fivm.Document.FilePath = newFilePath;
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving (manual) notepaditem as..."); }
        }

        public void SaveSelectedNotepad()
        {
            if (NotepadAvaliable)
            {
                Information.Show($"Attempted to save [{Notepad.Document.FileName}]", InfoTypes.FileIO);
                try
                {
                    if (!IsNotepadNull())
                    {
                        if (File.Exists(Notepad.Document.FilePath)) SaveNotepad(Notepad);
                        else SaveNotepadAs(Notepad);
                    }
                }
                catch (Exception e) { Information.Show(e.Message, "Error while saving currently selected notepad"); }
                UpdateSelectedNotepad();
            }
            else
            {
                Information.Show($"Failed to save notepad: none exist", InfoTypes.FileIO);
            }
        }

        public void SaveSelectedNotepadAs()
        {
            try
            {
                if (!IsNotepadNull() && NotepadAvaliable)
                    SaveNotepadAs(Notepad);
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving currently selected notepad as..."); }
        }

        public void SaveAllNotepadItems()
        {
            if (NotepadAvaliable)
                foreach (TextDocumentViewModel nli in NotepadItems)
                {
                    if (nli is TextDocumentViewModel fivm)
                        SaveNotepad(fivm);
                }
        }

        /// <summary>
        /// Saves the text to the given path
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="text">The text to be saved to the file</param>
        /// <returns></returns>
        public bool SaveFile(string path, string text)
        {
            try
            {
                if (path.IsFile())
                {
                    FileInfo fInfo = new FileInfo(path);
                    if (!fInfo.IsReadOnly)
                    {
                        NotepadActions.SaveFile(path, text);
                        Information.Show($"Successfully saved [{path}]", InfoTypes.FileIO);
                        return true;
                    }
                    else
                    {
                        Information.Show($"File [{path}] is read only. Cannot save.", "File IO");
                        return false;
                    }
                }
                else
                {
                    NotepadActions.SaveFile(path, text);
                    Information.Show($"Successfully saved [{path}]", InfoTypes.FileIO);
                    return true;
                }
            }
            catch (Exception e)
            {
                Information.Show(e.Message, "Error while saving text to file.");
                return false;
            }
        }

        #endregion

        // Other

        #region Finding Text

        public void FocusFind()
        {
            View.FocusFindInput(FindExpanded);
        }

        public void OpenFindWindow()
        {
            FindExpanded = !FindExpanded;
        }

        public void HighlightText(FindResult result)
        {
            View.HighlightFindResult(result);
        }

        private void OnNextTextFound(FindResult result)
        {
            HighlightText(result);
        }

        #endregion

        #region Moving Items

        public void MoveItem(string direction)
        {
            if (SelectedNotepadItem != null)
            {
                if (direction.ToString() == "up")
                    MoveSelectedItemUp();

                if (direction.ToString() == "down")
                    MoveSelectedItemDown();
            }
        }

        public void MoveSelectedItemUp()
        {
            if (SelectedIndex > 0)
            {
                int newIndex = SelectedIndex - 1;
                MoveControl(SelectedIndex, newIndex);
                View.ScrollItemsIntoView();
            }
        }

        public void MoveSelectedItemDown()
        {
            if (SelectedIndex + 1 < NotepadItems.Count)
            {
                MoveControl(SelectedIndex, SelectedIndex + 1);
                View.ScrollItemsIntoView();
            }
        }

        public void MoveControl(int oldIndex, int newIndex)
        {
            if (NotepadItems.Count > 0 &&
                oldIndex >= 0 &&
                oldIndex < NotepadItems.Count &&
                newIndex >= 0 &&
                newIndex < NotepadItems.Count)
            {
                TextDocumentViewModel item = NotepadItems[oldIndex];
                NotepadItems.Remove(item);
                NotepadItems.Insert(newIndex, item);
                SelectedIndex = newIndex;
            }
        }

        #endregion

        #region InfoStatusErrors

        private void Information_InformationAdded(InformationModel e)
        {
            InformationView.AddInformation(e);
        }

        // ther

        #endregion

        #region Windows

        private void NewWindow()
        {
            ThisApplication.OpenNewBlankWindow();
        }

        private void ShowWindowManager()
        {
            ThisApplication.ShowWindowPreviewsWindow();
        }

        public void ReopenLastWindow()
        {
            if (PreferencesG.CAN_REOPEN_WIN_WITH_CTRL_SHIFT_T)
                ThisApplication.ReopenLastWindow();
        }

        public void CloseWindow()
        {
            if (PreferencesG.CAN_CLOSE_WIN_WITH_CTRL_W)
            {
                Shutdown();
                ThisApplication.CloseWindowFromDataContext(this);
            }
        }

        public void CloseWindowWithCheck()
        {
            if (MessageBox.Show(
                "Close Window?", 
                "Close", 
                MessageBoxButton.OKCancel, 
                MessageBoxImage.Information, 
                MessageBoxResult.OK) == MessageBoxResult.OK)
            {
                Shutdown();
                ThisApplication.CloseWindowFromDataContext(this);
            }
        }

        public void CloseAllWindow()
        {
            // easier and faster than manually closing all windows.
            if (PreferencesG.CAN_CLOSE_WIN_WITH_CTRL_W)
                ThisApplication.ShutdownApplication();
        }

        /// <summary>
        /// Opens a notepad document in a new window
        /// </summary>
        /// <param name="nli"></param>
        public void OpenNotepadItemInNewWindow(TextDocumentViewModel nli)
        {
            if (nli != null)
            {
                if (File.Exists(nli.Document.FilePath))
                {
                    ThisApplication.OpenFileInNewWindow(nli.Document.FilePath);
                    Information.Show($"Opened [{nli.Document.FileName}] in another window", InfoTypes.Information);
                    CloseNotepadItem(nli, false);
                }
                else
                {
                    string tempFilePath = Path.Combine(Path.GetTempPath(), nli.Document.FileName);
                    File.WriteAllText(tempFilePath, nli.Document.Text);
                    ThisApplication.OpenFileInNewWindow(tempFilePath);
                    Information.Show($"Opened [{nli.Document.FileName}] in another window", InfoTypes.Information);
                    CloseNotepadItem(nli, false);
                    File.Delete(tempFilePath);
                }
            }
        }

        #endregion

        #region Hooks and Shutting down

        public void SetupInformationHook()
        {
            Information.InformationAdded += Information_InformationAdded;
        }

        /// <summary>
        /// Unattach the static event from the information thingy. i think
        /// this stops a memory leak occouring.
        /// </summary>
        public void ShutdownInformationHook() //idec this method name is bad xd
        {
            Information.InformationAdded -= Information_InformationAdded;
        }

        public void Shutdown()
        {
            ShutdownInformationHook();
            OurClipboard.ShutdownUpdaterHook();
        }

        #endregion
    }
}

// eee over 1000 lines xd