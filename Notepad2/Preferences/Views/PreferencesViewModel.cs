using Notepad2.InformationStuff;
using Notepad2.Utilities;
using System.Windows.Input;

namespace Notepad2.Preferences.Views
{
    public class PreferencesViewModel : BaseViewModel
    {
        private bool _scrollVerticalCtrlArrows;
        private bool _scrollHorizontalCtrlArrows;
        private bool _scrollHorizontalShiftMWheel;

        private bool _cutLineCtrlX;
        private bool _copyLineCtrlC;
        private bool _selectLineCtrlShiftA;
        private bool _addLineCtrlEnter;

        private bool _zoomEditorCtrlMWheel;

        private bool _wrapTextByDefault;

        public bool ScrollVerticallyCtrlArrowKeys
        {
            get => _scrollVerticalCtrlArrows;
            set => RaisePropertyChanged(ref _scrollVerticalCtrlArrows, value);
        }

        public bool ScrollHorizontallyCtrlArrowKeys
        {
            get => _scrollHorizontalCtrlArrows;
            set => RaisePropertyChanged(ref _scrollHorizontalCtrlArrows, value);
        }

        public bool ScrollHorizontallyShiftMouseWheel
        {
            get => _scrollHorizontalShiftMWheel;
            set => RaisePropertyChanged(ref _scrollHorizontalShiftMWheel, value);
        }


        public bool CutEntireLineCtrlX
        {
            get => _cutLineCtrlX;
            set => RaisePropertyChanged(ref _cutLineCtrlX, value);
        }

        public bool CopyEntireLineCtrlC
        {
            get => _copyLineCtrlC;
            set => RaisePropertyChanged(ref _copyLineCtrlC, value);
        }

        public bool SelectEntireLineCtrlShiftA
        {
            get => _selectLineCtrlShiftA;
            set => RaisePropertyChanged(ref _selectLineCtrlShiftA, value);
        }

        public bool AddEntireLineCtrlEnter
        {
            get => _addLineCtrlEnter;
            set => RaisePropertyChanged(ref _addLineCtrlEnter, value);
        }


        public bool ZoomEditorCtrlScrollwheel
        {
            get => _zoomEditorCtrlMWheel;
            set => RaisePropertyChanged(ref _zoomEditorCtrlMWheel, value);
        }


        public bool WrapTextByDefault
        {
            get => _wrapTextByDefault;
            set => RaisePropertyChanged(ref _wrapTextByDefault, value);
        }

        public PreferencesWindow PreferencesView { get; set; }

        public ICommand RefreshCommand { get; private set; }
        public ICommand ShowPreferencesCommand { get; private set; }
        public ICommand SavePreferencesCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public PreferencesViewModel()
        {
            PreferencesView = new PreferencesWindow
            {
                DataContext = this
            };

            RefreshCommand = new Command(LoadOldPreferences);
            ShowPreferencesCommand = new Command(ShowPreferencesWindow);
            SavePreferencesCommand = new Command(SavePreferences);
            CancelCommand = new Command(Cancel);
        }

        public void ShowPreferencesWindow()
        {
            LoadOldPreferences();
            PreferencesView?.Show();
        }

        public void LoadOldPreferences()
        {
            PreferencesG.LoadFromProperties();
            LoadPreferencesVariables();
        }

        public void LoadPreferencesVariables()
        {
            ScrollHorizontallyShiftMouseWheel = PreferencesG.SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL;
            ScrollHorizontallyCtrlArrowKeys   = PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS;
            ScrollVerticallyCtrlArrowKeys     = PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS;
                                              
            CutEntireLineCtrlX                = PreferencesG.CAN_CUT_ENTIRE_LINE_CTRL_X;
            CopyEntireLineCtrlC               = PreferencesG.CAN_COPY_ENTIRE_LINE_CTRL_C;
            SelectEntireLineCtrlShiftA        = PreferencesG.CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A;
            AddEntireLineCtrlEnter            = PreferencesG.CAN_ADD_ENTIRE_LINE_CTRL_ENTER;

            ZoomEditorCtrlScrollwheel         = PreferencesG.CAN_ZOOM_EDITOR_CTRL_MWHEEL;

            WrapTextByDefault                 = PreferencesG.WRAP_TEXT_BY_DEFAULT;
        }

        public void UpdatePreferenceVariables()
        {
            PreferencesG.SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL = ScrollHorizontallyShiftMouseWheel;
            PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS   = ScrollHorizontallyCtrlArrowKeys;
            PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS     = ScrollVerticallyCtrlArrowKeys;

            PreferencesG.CAN_CUT_ENTIRE_LINE_CTRL_X              = CutEntireLineCtrlX;
            PreferencesG.CAN_COPY_ENTIRE_LINE_CTRL_C             = CopyEntireLineCtrlC;
            PreferencesG.CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A     = SelectEntireLineCtrlShiftA;
            PreferencesG.CAN_ADD_ENTIRE_LINE_CTRL_ENTER          = AddEntireLineCtrlEnter;

            PreferencesG.CAN_ZOOM_EDITOR_CTRL_MWHEEL             = ZoomEditorCtrlScrollwheel;

            PreferencesG.WRAP_TEXT_BY_DEFAULT                    = WrapTextByDefault;
        }

        public void SavePreferences()
        {
            UpdatePreferenceVariables();
            PreferencesG.SaveToProperties();
            Information.Show("Properties Saved!", "Properties");
            ClosePreferencesWindow();
        }

        public void UnloadPreferences()
        {
            LoadOldPreferences();
            UpdatePreferenceVariables();
        }

        public void Cancel()
        {
            UnloadPreferences();
            ClosePreferencesWindow();
        }

        public void ClosePreferencesWindow()
        {
            PreferencesView?.Close();
        }
    }
}
