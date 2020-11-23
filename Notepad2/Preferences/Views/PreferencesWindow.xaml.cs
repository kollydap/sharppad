using System.Windows;

namespace SharpPad.Preferences.Views
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        public PreferencesViewModel Preferences
        {
            get => this.DataContext as PreferencesViewModel;
            set => this.DataContext = value;
        }

        public PreferencesWindow()
        {
            InitializeComponent();
            Preferences = new PreferencesViewModel();
            Preferences.CloseViewCallback = Close;
            Preferences.LoadPreferences();

            Closing += PreferencesWindow_Closing;
        }

        private void PreferencesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Preferences?.ResetPreferences();
            e.Cancel = true;
            this.Hide();
        }

        // i still cant be bothered to make this MVVMey rip
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PreferencesViewModel prefs)
            {
                prefs.CanCloseWindowsWithCtrlWAndShift = true;
                prefs.CanReopenWindowWithCtrlShiftT = true;
                prefs.UseNewDragDropSystem = true;
                prefs.ScrollVerticallyCtrlArrowKeys = true;
                prefs.ScrollHorizontallyCtrlArrowKeys = false;
                prefs.ScrollHorizontallyShiftMouseWheel = true;
                prefs.CutEntireLineCtrlX = true;
                prefs.CopyEntireLineCtrlC = true;
                prefs.SelectEntireLineCtrlShiftA = true;
                prefs.AddEntireLineCtrlEnter = true;
                prefs.ZoomEditorCtrlScrollwheel = true;
                prefs.WrapTextByDefault = false;
                prefs.CloseNotepadListByDefault = false;
                prefs.SaveOpenUnclosedFiles = true;
                prefs.CheckFileNamesForChangesInDocumentWatcher = true;

                prefs.SaveAndClosePreferencesView();
            }
        }
    }
}
