using Notepad2.Applications;
using Notepad2.FileExplorer;
using Notepad2.InformationStuff;
using Notepad2.RecyclingBin;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace Notepad2.Notepad
{
    /// <summary>
    /// Interaction logic for NotepadListItem.xaml
    /// </summary>
    public partial class NotepadListItem : UserControl
    {
        public TextDocumentViewModel Notepad
        {
            get => DataContext as TextDocumentViewModel;
            set => DataContext = value;
        }

        // Stores the point within the grip
        private Point GripMouseStartPoint;

        public NotepadListItem()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 1: Notepad.Close?.Invoke(Notepad); break;
                case 2: Notepad.OpenInFileExplorer?.Invoke(Notepad); break;
                case 3: DeleteFile(); break;
                case 4: Notepad.OpenInNewWindowCallback?.Invoke(Notepad); break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void GripMouseMove(object sender, MouseEventArgs e)
        {
            if (GripMouseStartPoint != e.GetPosition(null) && e.LeftButton == MouseButtonState.Pressed)
            {
                if (DataContext is TextDocumentViewModel notepad)
                {
                    try
                    {
                        if (File.Exists(notepad.Document.FilePath))
                        {
                            string[] path1 = new string[1] { notepad.Document.FilePath };
                            SetDraggingStatus(true);
                            DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path1), DragDropEffects.Copy);
                            SetDraggingStatus(false);
                        }
                        else
                        {
                            string tempFilePath = Path.Combine(Path.GetTempPath(), notepad.Document.FileName);
                            File.WriteAllText(tempFilePath, notepad.Document.Text);
                            string[] path = new string[1] { tempFilePath };
                            SetDraggingStatus(true);
                            DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path), DragDropEffects.Copy);
                            File.Delete(tempFilePath);
                            SetDraggingStatus(false);
                        }
                    }
                    catch { }
                }
            }
        }

        public void SetDraggingStatus(bool isDragging)
        {
            if (isDragging)
            {
                BorderThickness = new Thickness(1);
                Information.Show($"Started dragging", "DragDrop");
            }
            else
            {
                BorderThickness = new Thickness(0);
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
                ThisApplication.PropertiesView.Properties.Show();
                if (Notepad.Document.FilePath.IsFile())
                {
                    ThisApplication.PropertiesView.Properties.FetchProperties(Notepad.Document.FilePath);
                }
                else
                {
                    ThisApplication.PropertiesView.Properties.FetchFromDocument(Notepad.Document);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationLib.OpacityControl(this, 0, 1, GlobalPreferences.ANIMATION_SPEED_SEC);
            AnimationLib.MoveToTargetX(this, 0, -100, GlobalPreferences.ANIMATION_SPEED_SEC);
        }
    }
}