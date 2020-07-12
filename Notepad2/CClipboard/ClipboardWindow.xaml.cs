using Notepad2.Utilities;
using System.Windows;
using System.Windows.Input;

namespace Notepad2.CClipboard
{
    /// <summary>
    /// Interaction logic for ClipboardWindow.xaml
    /// </summary>
    public partial class ClipboardWindow : Window
    {
        public ClipboardWindow()
        {
            InitializeComponent();
            ShowInTaskbar = true;
        }

        public void ShowWindow()
        {
            this.Show();
            MoveToMouseCursor();
        }

        private void MoveToMouseCursor()
        {
            Point mPos = MouseLocationHelper.GetLocation();
            Left = mPos.X - (ActualWidth / 2);
            Top = mPos.Y - 15;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
