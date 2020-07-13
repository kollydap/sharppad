using Microsoft.Win32;
using Notepad2.CClipboard;
using Notepad2.FileExplorer;
using Notepad2.Finding;
using Notepad2.History;
using Notepad2.InformationStuff;
using Notepad2.Notepad;
using Notepad2.Preferences;
using Notepad2.Preferences.Views;
using Notepad2.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields

        private NotepadViewModel _notepad;
        private NotepadListItem _selectedNotepadItem;
        private FindViewModel _find;
        private int _selectedIndex;
        private bool _notepadAvaliable;
        private bool _findExpanded;

        #endregion

        #region Public Fields

        /// <summary>
        /// Holds a list of <see cref="NotepadListItem"/>, for use in a ListBox
        /// (the one on the left side of the program)
        /// <para>
        /// Call this, "the list on the left"
        /// </para>
        /// </summary>
        public ObservableCollection<NotepadListItem> NotepadItems { get; set; }

        /// <summary>
        /// Holds a list of information, statuses and errors 
        /// for use in the list at the bottom of the program
        /// </summary>
        public ObservableCollection<InformationModel> InfoStatusErrorsItems { get; set; }

        /// <summary>
        /// The selected index of the currently selected <see cref="NotepadListItem"/>
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => RaisePropertyChanged(ref _selectedIndex, value);
        }

        /// <summary>
        /// The currently selected <see cref="NotepadListItem"/>
        /// </summary>
        public NotepadListItem SelectedNotepadItem
        {
            get => _selectedNotepadItem;
            set => RaisePropertyChanged(ref _selectedNotepadItem, value, () =>
            {
                Notepad = value?.Notepad;
                UpdateSelectedNotepad();
            });
        }

        /// <summary>
        /// Says whether a notepad is avaliable (aka, if there's any 
        /// <see cref="NotepadListItem"/>s in the list on the left)
        /// </summary>
        public bool NotepadAvaliable
        {
            get => _notepadAvaliable;
            set => RaisePropertyChanged(ref _notepadAvaliable, value);
        }

        public bool FindExpanded
        {
            get => _findExpanded;
            set => RaisePropertyChanged(ref _findExpanded, value, FocusFindInputCallback);
        }

        /// <summary>
        /// The currently selected <see cref="NotepadViewModel"/>
        /// </summary>
        public NotepadViewModel Notepad
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

        /// <summary>
        /// A ViewModel for dealing with showing help
        /// </summary>
        public HelpViewModel Help { get; set; }

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

        #endregion

        #region Commands

        public ICommand NewCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand OpenDirectoryCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand SaveAllCommand { get; private set; }
        public ICommand CloseSelectedNotepadCommand { get; private set; }
        public ICommand CloseAllNotepadsCommand { get; private set; }
        public ICommand OpenInNewWindowCommand { get; private set; }
        public ICommand PrintFileCommand { get; private set; }
        public ICommand ClearInfoItemsCommand { get; private set; }
        public ICommand AutoShowFindMenuCommand { get; private set; }

        #endregion

        #region Callbacks 

        /// <summary>
        /// A callback, used for animating <see cref="NotepadListItem"/>s
        /// being added to the list on the left
        /// </summary>
        public Action<NotepadListItem, AnimationFlag> AnimateAddCallback { get; set; }

        /// <summary>
        /// A callback from the <see cref="FindTextWindow"/>, used for telling the MainWindow to 
        /// highlight a specific region of text contained within a <see cref="FindResult"/>
        /// </summary>
        public Action<FindResult> HightlightTextCallback { get; set; }

        /// <summary>
        /// A callback to focus either the maintextbox or the find input
        /// </summary>
        public Action<bool> FocusFindInputCallback { get; set; }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            History = new HistoryViewModel();
            Preference = new PreferencesViewModel();
            Help = new HelpViewModel();
            OurClipboard = new ClipboardViewModel();
            NotepadItems = new ObservableCollection<NotepadListItem>();
            InfoStatusErrorsItems = new ObservableCollection<InformationModel>();

            Information.InformationAdded += Information_InformationAdded;
            History.OpenFileCallback = ReopenNotepadFromHistory;

            NewCommand = new Command(NewNotepad);
            OpenCommand = new Command(OpenNotepadFileFromFileExplorer);
            OpenDirectoryCommand = new Command(OpenDirectory);
            SaveCommand = new Command(SaveCurrentNotepad);
            SaveAsCommand = new Command(SaveCurrentNotepadAs);
            SaveAllCommand = new Command(SaveAllNotepadItems);
            CloseSelectedNotepadCommand = new Command(CloseSelectedNotepad);
            CloseAllNotepadsCommand = new Command(CloseAllNotepads);
            OpenInNewWindowCommand = new Command(OpenInNewWindow);
            PrintFileCommand = new Command(PrintFile);
            AutoShowFindMenuCommand = new Command(OpenFindWindow);
            ClearInfoItemsCommand = new Command(ClearInfoItems);

            Information.Show("Program loaded", InfoTypes.Information);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns whether the currently selected 
        /// <see cref="NotepadViewModel"/> is null or not
        /// </summary>
        /// <returns>a bool. xdddd</returns>
        public bool CheckNotepadNull()
        {
            return Notepad == null || Notepad.Document == null || Notepad.DocumentFormat == null;
        }

        /// <summary>
        /// Prints (on A4 Paper) a page to your printer. Shows a dialog to specify settings
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
        public bool CheckHasMadeChanges()
        {
            foreach (NotepadListItem nli in NotepadItems)
            {
                if (nli != null && nli.DataContext is NotepadViewModel fivm)
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
        public void NewNotepad()
        {
            AddNotepadItem(CreateDefaultStyleNotepadItem("", $"new{NotepadItems.Count}.txt", null));
        }

        /// <summary>
        /// Adds a <see cref="NotepadListItem"/> to the list on the left.
        /// </summary>
        /// <param name="nli">A <see cref="NotepadListItem"/> to be added</param>
        public void AddNotepadItem(NotepadListItem nli)
        {
            if (nli != null)
            {
                nli.Notepad.HasMadeChanges = false;
                NotepadItems.Add(nli);
                Information.Show($"Added FileItem: {nli.Notepad.Document.FileName}", InfoTypes.FileIO);
                AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemOPEN);
            }
        }

        /// <summary>
        /// Adds a normal notepad item, and then selects it 
        /// (for simplicity and quick access... maybe)
        /// </summary>
        public void AddStartupItem()
        {
            NotepadListItem nli = CreateDefaultStyleNotepadItem("", $"new{NotepadItems.Count}.txt", null);
            AddNotepadItem(nli);
            SelectedNotepadItem = nli;
        }

        public void AddNotepadFromVM(NotepadViewModel notepad)
        {
            NotepadListItem nli = CreateNotepadItemFromVM(notepad);
            AddNotepadItem(nli);
            SelectedNotepadItem = nli;
        }

        #endregion

        #region Closing Notepads

        /// <summary>
        /// if the given <see cref="NotepadListItem"/> is inside 
        /// the list on the left, this will remove it.
        /// </summary>
        /// <param name="nli">The <see cref="NotepadListItem"/> to be removed</param>
        public void CloseNotepadItem(NotepadListItem nli)
        {
            if (nli.Notepad.HasMadeChanges)
            {
                MessageBoxResult mbr = MessageBox.Show(
                    "You have unsaved work. Do you want to save it/them?",
                    "Unsaved Work",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (mbr == MessageBoxResult.Yes)
                    SaveNotepad(nli.Notepad);
            }
            //AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemCLOSE);
            NotepadItems.Remove(nli);
            History.FileClosed(nli.Notepad);
            Information.Show($"Removed FileItem: {nli.Notepad.Document.FileName}", InfoTypes.FileIO);
            UpdateSelectedNotepad();
        }

        /// <summary>
        /// If a Notepad Item is selected, this will close it.
        /// </summary>
        private void CloseSelectedNotepad()
        {
            if (SelectedNotepadItem != null)
                CloseNotepadItem(SelectedNotepadItem);
        }

        /// <summary>
        /// Closes every single Notepad Item inside the list on the left
        /// </summary>
        private void CloseAllNotepads()
        {
            NotepadItems.Clear();
            Information.Show($"Cleared {NotepadItems.Count} NotepadItems", InfoTypes.FileIO);
            Notepad = null;
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates and returns completely default <see cref="NotepadListItem"/>, with preset fonts,
        /// fontsizes, etc, ready to be added to the list on the left.
        /// </summary>
        /// <param name="text">The text the Notepad Item will have</param>
        /// <param name="itemName">The header/name of the Notepad Item</param>
        /// <param name="itemPath">The file path of the Notepad Item</param>
        /// <returns></returns>
        public NotepadListItem CreateDefaultStyleNotepadItem(string text, string itemName, string itemPath)
        {
            FontFamily font = string.IsNullOrEmpty(Properties.Settings.Default.DefaultFont)
                ? new FontFamily("Consolas")
                : new FontFamily(Properties.Settings.Default.DefaultFont);
            double fontSize =
                Properties.Settings.Default.DefaultFontSize > 0
                ? Properties.Settings.Default.DefaultFontSize
                : 16;
            FormatModel fm = new FormatModel()
            {
                Family = font,
                Size = fontSize,
                IsWrapped = PreferencesG.WRAP_TEXT_BY_DEFAULT
            };
            return CreateNotepadItem(text, itemName, itemPath, fm);
        }

        /// <summary>
        /// Creates a <see cref="NotepadListItem"/> with the given parameters.
        /// </summary>
        /// <param name="text">The text the Notepad Item will have</param>
        /// <param name="itemName">The header/name of the Notepad Item</param>
        /// <param name="itemPath">The file path of the Notepad Item</param>
        /// <param name="fm">The styles the Notepad Item will have</param>
        /// <returns></returns>
        public NotepadListItem CreateNotepadItem(string text, string itemName, string itemPath, FormatModel fm)
        {
            NotepadListItem nli = new NotepadListItem();
            NotepadViewModel fivm = new NotepadViewModel();
            fivm.Document.Text = text;
            fivm.Document.FileName = itemName;
            fivm.Document.FilePath = itemPath;
            fivm.DocumentFormat = fm;
            fivm.FindResults.OnNextTextFound += Find_OnNextTextFound;
            fivm.HasMadeChanges = false;
            nli.DataContext = fivm;
            nli.OpenInFileExplorer = this.OpenInFileExplorer;
            nli.Open = this.OpenNotepadItem;
            nli.Close = this.CloseNotepadItem;
            //fivm.DocumentFormat.Size = fm.Size;

            return nli;
        }

        public NotepadListItem CreateNotepadItemFromVM(NotepadViewModel notepad)
        {
            NotepadListItem nli = new NotepadListItem();
            nli.DataContext = notepad;
            nli.OpenInFileExplorer = this.OpenInFileExplorer;
            nli.Open = this.OpenNotepadItem;
            nli.Close = this.CloseNotepadItem;
            return nli;
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Sets the selected <see cref="NotepadViewModel"/> using 
        /// the selected <see cref="NotepadListItem"/>, and also other stuff
        /// </summary>
        private void UpdateSelectedNotepad()
        {
            NotepadListItem item = SelectedNotepadItem;
            if (item != null)
            {
                OpenNotepadItem(item);
                Find = item.Notepad.FindResults;
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

        /// <summary>
        /// Opens Windows File Explorer and selects the item who's 
        /// path is inside of the given <see cref="NotepadListItem"/>
        /// </summary>
        /// <param name="nli"></param>
        public void OpenInFileExplorer(NotepadListItem nli)
        {
            if (nli != null && nli.DataContext is NotepadViewModel fivm)
            {
                if (fivm != null)
                {
                    string folderPath = fivm.Document.FilePath;
                    if (File.Exists(folderPath))
                    {
                        ProcessStartInfo info = new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = string.Format("/e, /select, \"{0}\"", folderPath)
                        };
                        Information.Show("Opening File Explorer at selected file location", InfoTypes.FileIO);
                        Process.Start(info);
                    }

                    else
                        Information.Show("FilePath Doesn't Exist", "FilePath null");
                }
            }
        }

        public void OpenNotepadItem(NotepadListItem notepadItem)
        {
            if (notepadItem.Notepad != null )
            {
                Notepad = notepadItem?.Notepad;
            }
        }

        public void OpenNotepadFileFromFileExplorer()
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
                        OpenNotepadFileFromPath(paths, true);
                    }
                    Information.Show($"Opened {i} files", InfoTypes.FileIO);
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from f.explorer"); }
        }

        public void OpenDirectory()
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
                                OpenNotepadFileFromPath(file);
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

        public void OpenNotepadFileFromPath(string path, bool selectFile = false)
        {
            try
            {
                if (File.Exists(path))
                {
                    string text = NotepadActions.ReadFile(path);
                    bool doOpenFile;
                    if (text.Length > (GlobalPreferences.WARN_FILE_SIZE_KB * 1000 /* kb to byte */))
                    {
                        doOpenFile = 
                            MessageBox.Show(
                                "The file is very big in size and might lag the program. Continue to open?",
                                "File very big. Open anyway?",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                    }
                    else
                        doOpenFile = true;


                    if (doOpenFile)
                    {
                        NotepadListItem item = CreateDefaultStyleNotepadItem(text, Path.GetFileName(path), path);
                        AddNotepadItem(item);
                        Information.Show($"Opened file: {path}", InfoTypes.FileIO);
                        if (selectFile && item != null)
                            SelectedNotepadItem = item;
                    }
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from path"); }
        }

        public void ReopenNotepadFromHistory(NotepadViewModel notepad)
        {
            Information.Show($"Reopening {notepad.Document.FileName} from history", "History");
            AddNotepadFromVM(notepad);
        }

        #endregion

        #region Saving

        public void SaveNotepad(NotepadViewModel fivm)
        {
            try
            {
                if (File.Exists(fivm.Document.FilePath))
                {
                    string extension = Path.GetExtension(fivm.Document.FilePath);
                    string folderName = Path.GetDirectoryName(fivm.Document.FilePath);
                    string newFileName =
                        Path.HasExtension(Path.Combine(folderName, fivm.Document.FileName))
                        ? fivm.Document.FileName
                        : fivm.Document.FileName + extension;
                    string newFilePath = Path.Combine(folderName, newFileName);
                    if (fivm.Document.FilePath != newFilePath)
                    {
                        if (MessageBox.Show($"You are about to overwrite {fivm.Document.FilePath} " +
                            $"with {newFilePath}, do you want to continue?", "Overrite file?",
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            SaveFile(fivm.Document.FilePath, fivm.Document.Text);
                            fivm.HasMadeChanges = false;
                            File.Move(fivm.Document.FilePath, newFilePath);
                            fivm.Document.FileName = newFileName;
                            fivm.Document.FilePath = newFilePath;
                        }
                    }
                    else
                    {
                        SaveFile(fivm.Document.FilePath, fivm.Document.Text);
                        fivm.HasMadeChanges = false;
                        fivm.Document.FileName = newFileName;
                        fivm.Document.FilePath = newFilePath;
                    }
                }
                else
                    SaveNotepadAs(fivm);
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving a (manual) notepad item"); }
        }

        public void SaveNotepadAs(NotepadViewModel fivm)
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
                    SaveFile(newFilePath, fivm.Document.Text);
                    fivm.Document.FileName = Path.GetFileName(newFilePath);
                    fivm.Document.FilePath = newFilePath;
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving (manual) notepaditem as..."); }
        }

        public void SaveCurrentNotepad()
        {
            if (NotepadAvaliable)
            {
                Information.Show($"Attempted to save [{Notepad.Document.FileName}]", InfoTypes.FileIO);
                try
                {
                    if (!CheckNotepadNull())
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

        public void SaveCurrentNotepadAs()
        {
            try
            {
                if (!CheckNotepadNull() && NotepadAvaliable)
                    SaveNotepadAs(Notepad);
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving currently selected notepad as..."); }
        }

        public void SaveAllNotepadItems()
        {
            if (NotepadAvaliable)
                foreach (NotepadListItem nli in NotepadItems)
                {
                    if (nli.DataContext is NotepadViewModel fivm)
                        SaveNotepad(fivm);
                }
        }

        public void SaveFile(string path, string text)
        {
            try
            {
                NotepadActions.SaveFile(path, text);
                Information.Show($"Successfully saved {path}", InfoTypes.FileIO);
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving text to file."); }
        }

        #endregion

        // Other

        #region Finding Text

        public void OpenFindWindow()
        {
            FindExpanded = !FindExpanded;
        }

        public void HighlightText(FindResult result)
        {
            HightlightTextCallback?.Invoke(result);
        }

        private void Find_OnNextTextFound(FindResult result)
        {
            HighlightText(result);
        }

        #endregion

        // Private Methods

        #region InfoStatusErrors

        private void Information_InformationAdded(InformationModel e)
        {
            InfoStatusErrorsItems.Insert(0, e);
        }

        private void ClearInfoItems()
        {
            InfoStatusErrorsItems.Clear();
        }

        #endregion

        #region Open in new window

        public void OpenInNewWindow()
        {
            if (SelectedNotepadItem != null)
            {
                if (File.Exists(Notepad.Document.FilePath))
                {
                    MainWindow mWnd = new MainWindow(Notepad.Document.FilePath, false);
                    mWnd.LoadSettings();
                    mWnd.Show();
                    Information.Show($"Opened {Notepad.Document.FileName} in another window", InfoTypes.Information);
                    CloseSelectedNotepad();
                }
                else
                {
                    string tempFilePath = Path.Combine(Path.GetTempPath(), Notepad.Document.FileName);
                    File.WriteAllText(tempFilePath, Notepad.Document.Text);
                    MainWindow mWnd = new MainWindow(tempFilePath, false);
                    mWnd.LoadSettings();
                    mWnd.Show();
                    Information.Show($"Opened {Notepad.Document.FileName} in another window", InfoTypes.Information);
                    CloseSelectedNotepad();
                    File.Delete(tempFilePath);
                }
            }
        }

        #endregion

        /// <summary>
        /// Unattach the static event from the information thingy. i think
        /// this stops a memory leak occouring.
        /// </summary>
        public void ShutdownInformationHook() //idec this method name is bad xd
        {
            Information.InformationAdded -= Information_InformationAdded;
            OurClipboard.ShutdownUpdaterHook();
        }
    }
}
