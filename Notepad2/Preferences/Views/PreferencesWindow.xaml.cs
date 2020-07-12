using System.Windows;

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
