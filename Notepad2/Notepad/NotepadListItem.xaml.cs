using Notepad2.FileExplorer;
using Notepad2.InformationStuff;
using Notepad2.Notepad.DragDropping;
using Notepad2.Preferences;
using Notepad2.Utilities;
using System.IO;
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
        public NotepadItemViewModel Model
        {
            get => DataContext as NotepadItemViewModel;
            set => DataContext = value;
        }

        // Stores the point within the grip
        private Point GripMouseStartPoint;
        private Point ControlMouseStartPoint;
        private bool IsDragging;

        public NotepadListItem()
        {
            InitializeComponent();
            Loaded += NotepadListItem_Loaded;
            // idk if it needs to be preview or normal. preview works tho so meh.
            PreviewKeyDown += TopNotepadListItem_PreviewKeyDown;
        }

        private void TopNotepadListItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (((Keyboard.Modifiers == ModifierKeys.Control) && Keyboard.IsKeyDown(Key.R)))
            {
                HighlightFileName();
                e.Handled = true;
            }
        }

        private void RenameFileClick(object sender, RoutedEventArgs e)
        {
            HighlightFileName();
        }

        private void HighlightFileName()
        {
            fileNameBox.Focus();
            string fileName = Path.GetFileNameWithoutExtension(Model.Notepad.Document.FileName);
            fileNameBox.Select(0, fileName.Length);
        }

        private void NotepadListItem_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationHelpers.OpacityControl(this, 0, 1, GlobalPreferences.ANIMATION_SPEED_SEC);
            AnimationHelpers.MoveToTargetX(this, 0, 225, GlobalPreferences.ANIMATION_SPEED_SEC);
            if (Model != null)
                Model.HighlightFileNameCallback = HighlightFileName;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 1: Model.Remove(); break;
                case 2: Model.OpenInFileExplorer(); break;
                case 3: Model.DeleteFile(); break;
                case 4: Model.OpenInAnotherWindow(); break;
            }
        }

        private void CloseNotepadClick(object sender, RoutedEventArgs e)
        {
            Model.Remove();
        }

        private void ControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                Model.Remove();
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
            if (PreferencesG.USE_NEW_DRAGDROP_SYSTEM && !Model.Notepad.Document.FilePath.IsFile())
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
                        DragDropFileWatchers.DoingDragDrop(Model.Notepad);
                        string prefixedPath = Path.Combine(Path.GetTempPath(), DragDropNameHelper.GetPrefixedFileName(Model.Notepad.Document.FileName));
                        string[] fileList = new string[] { prefixedPath };
                        File.WriteAllText(prefixedPath, Model.Notepad.Document.Text);
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

        private void SetFileExtensionsClicks(object sender, RoutedEventArgs e)
        {
            string extension = ((FrameworkElement)sender).Uid;
            Model.SetFileExtension(extension);
        }

        /// <summary>
        /// Returns wheter the controls (which can freeze the app due to bugs) are not going to freeze the app.
        /// e.g, filename/path box isn't focused nor is the app currently dragging.
        /// </summary>
        /// <returns></returns>
        private bool CanStartDragDrop()
        {
            return
                !IsDragging &&
                !fileNameBox.IsMouseOver &&
                !fileNameBox.IsFocused;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            // Wrapping the entire thing in a try catch block just incase
            // Had to add a check to the textbox because it can freeze the entire UI
            // if you click and drag over the textbox. idk why though, calls a COM exception.
            if (CanStartDragDrop())
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
                        int edgeOffsetX = 10, edgeOffsetY = 10;

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
                            if (Model.Notepad.Document.FilePath.IsFile())
                            {
                                string[] path1 = new string[1] { Model.Notepad.Document.FilePath };
                                SetDraggingStatus(true);
                                DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path1), DragDropEffects.Copy);
                                SetDraggingStatus(false);
                            }
                            else
                            {
                                SetDraggingStatus(true);
                                string tempFilePath = Path.Combine(Path.GetTempPath(), Model.Notepad.Document.FileName);
                                File.WriteAllText(tempFilePath, Model.Notepad.Document.Text);
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