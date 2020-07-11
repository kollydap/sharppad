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

namespace Notepad2.Preferences.Views
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            InitializeComponent();

            Closing += PreferencesWindow_Closing;
        }

        private void PreferencesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is PreferencesViewModel prefs)
            {
                prefs.UnloadPreferences();
            }

            e.Cancel = true;
            this.Hide();
        }
    }
}
