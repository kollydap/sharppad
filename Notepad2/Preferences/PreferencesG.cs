namespace SharpPad.Preferences
{
    /// <summary>
    /// A static class for holding application-wide preferences.
    /// </summary>
    public static class PreferencesG
    {
        public static bool SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL { get; set; }
        public static bool SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS { get; set; }
        public static bool SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS { get; set; }

        public static bool CAN_CUT_ENTIRE_LINE_CTRL_X { get; set; }
        public static bool CAN_COPY_ENTIRE_LINE_CTRL_C { get; set; }
        public static bool CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A { get; set; }
        public static bool CAN_ADD_ENTIRE_LINES { get; set; }

        public static bool CAN_ZOOM_EDITOR_CTRL_MWHEEL { get; set; }

        public static bool WRAP_TEXT_BY_DEFAULT { get; set; }

        public static bool CAN_CLOSE_WIN_WITH_CTRL_W { get; set; }
        public static bool CAN_REOPEN_WIN_WITH_CTRL_SHIFT_T { get; set; }

        public static bool CLOSE_NOTEPADLIST_BY_DEFAULT { get; set; }

        public static bool USE_NEW_DRAGDROP_SYSTEM { get; set; }

        public static bool SAVE_OPEN_UNCLOSED_FILES { get; set; }

        public static bool USE_WORD_COUNTER_BY_DEFAULT { get; set; }

        public static bool CHECK_FILENAME_CHANGES_IN_DOCUMENT_WATCHER { get; set; }
        public static bool UNSET_SETTINGS_AAAAHLOL2 { get; set; }
        public static bool UNSET_SETTINGS_AAAAHLOL3 { get; set; }
        public static bool UNSET_SETTINGS_AAAAHLOL4 { get; set; }
        public static bool UNSET_SETTINGS_AAAAHLOL5 { get; set; }
        public static bool UNSET_SETTINGS_AAAAHLOL6 { get; set; }


        public static void SavePropertiesToFile()
        {
            Properties.Settings.Default.horzScrlShfMWhl = SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL;
            Properties.Settings.Default.horzScrlCtrlArrKy = SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS;
            Properties.Settings.Default.vertScrlCtrlArrKy = SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS;

            Properties.Settings.Default.cutLnCtrlX = CAN_CUT_ENTIRE_LINE_CTRL_X;
            Properties.Settings.Default.cpyLnCtrlC = CAN_COPY_ENTIRE_LINE_CTRL_C;
            Properties.Settings.Default.SlctLnCtrSftA = CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A;
            Properties.Settings.Default.newLnCtrlEntr = CAN_ADD_ENTIRE_LINES;

            Properties.Settings.Default.zoomCtrlMWhel = CAN_ZOOM_EDITOR_CTRL_MWHEEL;

            Properties.Settings.Default.wrapByDefault = WRAP_TEXT_BY_DEFAULT;

            Properties.Settings.Default.closeWinWithCtrlW = CAN_CLOSE_WIN_WITH_CTRL_W;
            Properties.Settings.Default.canOpnWndCtrlShftT = CAN_REOPEN_WIN_WITH_CTRL_SHIFT_T;

            Properties.Settings.Default.closeNLstOnStrt = CLOSE_NOTEPADLIST_BY_DEFAULT;

            Properties.Settings.Default.useNewDDSys = USE_NEW_DRAGDROP_SYSTEM;

            Properties.Settings.Default.saveOpnUnsvdFiles = SAVE_OPEN_UNCLOSED_FILES;

            Properties.Settings.Default.useWordCounterByDefault = USE_WORD_COUNTER_BY_DEFAULT;

            Properties.Settings.Default.chkFNameInDocWatch = CHECK_FILENAME_CHANGES_IN_DOCUMENT_WATCHER;
            Properties.Settings.Default.unset2 = UNSET_SETTINGS_AAAAHLOL2;
            Properties.Settings.Default.unset3 = UNSET_SETTINGS_AAAAHLOL3;
            Properties.Settings.Default.unset4 = UNSET_SETTINGS_AAAAHLOL4;
            Properties.Settings.Default.unset5 = UNSET_SETTINGS_AAAAHLOL5;
            Properties.Settings.Default.unset6 = UNSET_SETTINGS_AAAAHLOL6;

            Properties.Settings.Default.Save();
        }

        public static void LoadFromPropertiesFile()
        {
            SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL = Properties.Settings.Default.horzScrlShfMWhl;
            SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS = Properties.Settings.Default.horzScrlCtrlArrKy;
            SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS = Properties.Settings.Default.vertScrlCtrlArrKy;

            CAN_CUT_ENTIRE_LINE_CTRL_X = Properties.Settings.Default.cutLnCtrlX;
            CAN_COPY_ENTIRE_LINE_CTRL_C = Properties.Settings.Default.cpyLnCtrlC;
            CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A = Properties.Settings.Default.SlctLnCtrSftA;
            CAN_ADD_ENTIRE_LINES = Properties.Settings.Default.newLnCtrlEntr;

            CAN_ZOOM_EDITOR_CTRL_MWHEEL = Properties.Settings.Default.zoomCtrlMWhel;

            WRAP_TEXT_BY_DEFAULT = Properties.Settings.Default.wrapByDefault;

            CAN_CLOSE_WIN_WITH_CTRL_W = Properties.Settings.Default.closeWinWithCtrlW;
            CAN_REOPEN_WIN_WITH_CTRL_SHIFT_T = Properties.Settings.Default.canOpnWndCtrlShftT;

            CLOSE_NOTEPADLIST_BY_DEFAULT = Properties.Settings.Default.closeNLstOnStrt;

            USE_NEW_DRAGDROP_SYSTEM = Properties.Settings.Default.useNewDDSys;

            SAVE_OPEN_UNCLOSED_FILES = Properties.Settings.Default.saveOpnUnsvdFiles;

            USE_WORD_COUNTER_BY_DEFAULT = Properties.Settings.Default.useWordCounterByDefault;

            CHECK_FILENAME_CHANGES_IN_DOCUMENT_WATCHER = Properties.Settings.Default.chkFNameInDocWatch;
            UNSET_SETTINGS_AAAAHLOL2 = Properties.Settings.Default.unset2;
            UNSET_SETTINGS_AAAAHLOL3 = Properties.Settings.Default.unset3;
            UNSET_SETTINGS_AAAAHLOL4 = Properties.Settings.Default.unset4;
            UNSET_SETTINGS_AAAAHLOL5 = Properties.Settings.Default.unset5;
            UNSET_SETTINGS_AAAAHLOL6 = Properties.Settings.Default.unset6;
        }
    }
}
