using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
