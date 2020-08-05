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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PreferencesViewModel prefs)
            {
                prefs.ScrollVerticallyCtrlArrowKeys = true;
                prefs.ScrollHorizontallyShiftMouseWheel = true;
                prefs.CutEntireLineCtrlX = true;
                prefs.CopyEntireLineCtrlC = true;
                prefs.SelectEntireLineCtrlShiftA = true;
                prefs.AddEntireLineCtrlEnter = true;
                prefs.ZoomEditorCtrlScrollwheel = true;
                prefs.WrapTextByDefault = true;
                prefs.CanCloseWindowsWithCtrlWAndShift = true;
                prefs.CanReopenWindowWithCtrlShiftT = true;
                prefs.CloseNotepadListByDefault = false;

                prefs.SavePreferences();
            }
        }
    }
}
