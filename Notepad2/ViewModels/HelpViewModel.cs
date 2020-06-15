using Notepad2.Utilities;
using Notepad2.Views;
using System.Windows.Input;

namespace Notepad2.ViewModels
{
    public class HelpViewModel
    {
        public ICommand HelpCommand { get; set; }

        private HelpBox HelpWindow = new HelpBox();

        public HelpViewModel()
        {
            HelpCommand = new Command(ShowHelpWindow);
        }

        public void ShowHelpWindow()
        {
            HelpWindow.Show();
        }

        public void Shutdown()
        {
            HelpWindow.Close();
        }
    }
}
