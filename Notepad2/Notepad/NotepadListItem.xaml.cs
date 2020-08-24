using Notepad2.Applications;
using Notepad2.FileExplorer;
using Notepad2.InformationStuff;
using Notepad2.Notepad.DragDropping;
using Notepad2.Preferences;
using Notepad2.RecyclingBin;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace Notepad2.Notepad
{
    /// <summary>
    /// A user control containing a ViewModel containing 
    /// notepad information (documents, formats, find results, etc)
    /// </summary>
    public partial class NotepadListItem : UserControl
    {
        /// <summary>
        /// The ViewModel
        /// </summary>
        public TextDocumentViewModel Notepad
        {
            get => DataContext as TextDocumentViewModel;
            set => DataContext = value;
        }

        // Stores the point within the grip
        private Point GripMouseStartPoint;
        private Point ControlMouseStartPoint;

        public NotepadListItem()
        {
            InitializeComponent();

            Loaded += NotepadListItem_Loaded;
        }

        private void NotepadListItem_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationLib.OpacityControl(this, 0, 1, GlobalPreferences.ANIMATION_SPEED_SEC);
            AnimationLib.MoveToTargetX(this, 0, -100, GlobalPreferences.ANIMATION_SPEED_SEC);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 1: Notepad.Close?.Invoke(Notepad); break;
                case 2: Notepad.Document.FilePath.OpenInFileExplorer(); break;
                case 3: DeleteFile(); break;
                case 4: Notepad.OpenInNewWindowCallback?.Invoke(Notepad); break;
            }
        }

        private void CloseNotepadClick(object sender, RoutedEventArgs e)
        {
            Notepad.Close?.Invoke(Notepad);
        }

        private void ControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                Notepad.Close?.Invoke(Notepad);
        }

        private void GripLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                GripMouseStartPoint = e.GetPosition(null);
        }

        private void GripMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void GripMouseMove(object sender, MouseEventArgs e)
        {
            bool canDrag;
            if (PreferencesG.USE_NEW_DRAGDROP_SYSTEM && !Notepad.Document.FilePath.IsFile())
            {
                Cursor = Cursors.Hand;
                canDrag = true;
            }
            else
            {
                Cursor = Cursors.No;
                canDrag = false;
            }

            if (GripMouseStartPoint != e.GetPosition(null) && e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    if (canDrag)
                    {
                        SetDraggingStatus(true);
                        FileWatchers.DoingDragDrop(this.Notepad);
                        string prefixedPath = Path.Combine(Path.GetTempPath(), DragDropNameHelper.GetPrefixedFileName(Notepad.Document.FileName));
                        string[] fileList = new string[] { prefixedPath };
                        File.WriteAllText(prefixedPath, Notepad.Document.Text);
                        DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, fileList), DragDropEffects.Move);
                        SetDraggingStatus(false);
                    }
                }
                catch { }
            }
        }

        public void SetDraggingStatus(bool isDragging)
        {
            IsDragging = isDragging;
            if (isDragging)
            {
                grd.IsEnabled = false;
                //BorderThickness = new Thickness(1);
                Information.Show($"Started dragging", "DragDrop");
            }
            else
            {
                grd.IsEnabled = true;
                //BorderThickness = new Thickness(0);
                Information.Show($"Drag Drop completed", "DragDrop");
            }
        }

        public void DeleteFile()
        {
            string fileName = Notepad.Document.FilePath;
            if (File.Exists(Notepad.Document.FilePath))
                Task.Run(() => RecycleBin.SilentSend(fileName));
            Notepad.Close?.Invoke(Notepad);
        }

        private void SetFileExtensionsClicks(object sender, RoutedEventArgs e)
        {
            string notepadName = Notepad.Document.FileName;
            string extension = ((FrameworkElement)sender).Uid;
            Notepad.Document.FileName = FileExtensionsHelper.GetFileExtension(notepadName, extension);
        }

        private void OpenInAnotherWindow(object sender, RoutedEventArgs e)
        {
            Notepad.OpenInNewWindowCallback?.Invoke(Notepad);
        }

        private void ShowPropertiesClick(object sender, RoutedEventArgs e)
        {
            if (Notepad?.Document != null)
            {
                WindowManager.PropertiesView.Properties.Show();
                if (Notepad.Document.FilePath.IsFile())
                {
                    if (!Notepad.HasMadeChanges)
                        WindowManager.PropertiesView.Properties.FetchProperties(Notepad.Document.FilePath);
                    else
                        WindowManager.PropertiesView.Properties.FetchFromDocument(Notepad.Document);
                }
                else
                {
                    WindowManager.PropertiesView.Properties.FetchFromDocument(Notepad.Document);
                }
            }
        }

        public bool IsDragging { get; set; }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            // Wrapping the entire thing in a try catch block
            // just incase
            // Had to add a check to the textbox because it can freeze the entire UI
            // if you click and drag over the textbox. idk why though, calls a COM exception.
            if (!IsDragging && !fileNameBox.IsMouseOver && !fileNameBox.IsFocused)
            {
                try
                {
                    Point orgPos = ControlMouseStartPoint;
                    Point newPos = e.GetPosition(this);
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        bool canDoDrag = false;
                        int mouseXDragOffset = 16;
                        int mouseYDragOffset = 10;
                        int edgeOffsetX = 10;
                        int edgeOffsetY = 10;

                        // Checks if you drag a bit awawy from where you clicked.
                        if (newPos.X < (orgPos.X - mouseXDragOffset) || newPos.X > (orgPos.X + mouseXDragOffset))
                            canDoDrag = true;
                        if (newPos.Y < (orgPos.Y - mouseYDragOffset) || newPos.Y > (orgPos.Y + mouseYDragOffset))
                            canDoDrag = true;

                        // Checks if you drag near to the edges of the border.
                        if (newPos.X < edgeOffsetX || newPos.X > (ActualWidth - edgeOffsetX))
                            canDoDrag = true;
                        if (newPos.Y < edgeOffsetY || newPos.Y > (ActualHeight - edgeOffsetY))
                            canDoDrag = true;

                        if (canDoDrag)
                        {
                            if (Notepad.Document.FilePath.IsFile())
                            {
                                string[] path1 = new string[1] { Notepad.Document.FilePath };
                                SetDraggingStatus(true);
                                DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path1), DragDropEffects.Copy);
                                SetDraggingStatus(false);
                            }
                            else
                            {
                                SetDraggingStatus(true);
                                string tempFilePath = Path.Combine(Path.GetTempPath(), Notepad.Document.FileName);
                                File.WriteAllText(tempFilePath, Notepad.Document.Text);
                                string[] path = new string[1] { tempFilePath };
                                DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path), DragDropEffects.Copy);
                                File.Delete(tempFilePath);
                                SetDraggingStatus(false);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ControlMouseStartPoint = e.GetPosition(this);
        }
    }
}