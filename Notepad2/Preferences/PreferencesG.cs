namespace Notepad2.Preferences
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
        public static bool CAN_ADD_ENTIRE_LINE_CTRL_ENTER { get; set; }

        public static bool CAN_ZOOM_EDITOR_CTRL_MWHEEL { get; set; }

        public static bool WRAP_TEXT_BY_DEFAULT { get; set; }

        public static void SaveToProperties()
        {
            Properties.Settings.Default.horzScrlShfMWhl   = SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL;
            Properties.Settings.Default.horzScrlCtrlArrKy = SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS;
            Properties.Settings.Default.vertScrlCtrlArrKy = SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS;

            Properties.Settings.Default.cutLnCtrlX        = CAN_CUT_ENTIRE_LINE_CTRL_X;
            Properties.Settings.Default.cpyLnCtrlC        = CAN_COPY_ENTIRE_LINE_CTRL_C;
            Properties.Settings.Default.SlctLnCtrSftA     = CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A;
            Properties.Settings.Default.newLnCtrlEntr     = CAN_ADD_ENTIRE_LINE_CTRL_ENTER;

            Properties.Settings.Default.zoomCtrlMWhel     = CAN_ZOOM_EDITOR_CTRL_MWHEEL;

            Properties.Settings.Default.wrapByDefault     = WRAP_TEXT_BY_DEFAULT;

            Properties.Settings.Default.Save();
        }

        public static void LoadFromProperties()
        {
            SCROLL_HORIZONTAL_WITH_SHIFT_MOUSEWHEEL = Properties.Settings.Default.horzScrlShfMWhl;
            SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS   = Properties.Settings.Default.horzScrlCtrlArrKy;
            SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS     = Properties.Settings.Default.vertScrlCtrlArrKy;

            CAN_CUT_ENTIRE_LINE_CTRL_X              = Properties.Settings.Default.cutLnCtrlX;
            CAN_COPY_ENTIRE_LINE_CTRL_C             = Properties.Settings.Default.cpyLnCtrlC;
            CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A     = Properties.Settings.Default.SlctLnCtrSftA;
            CAN_ADD_ENTIRE_LINE_CTRL_ENTER          = Properties.Settings.Default.newLnCtrlEntr;

            CAN_ZOOM_EDITOR_CTRL_MWHEEL             = Properties.Settings.Default.zoomCtrlMWhel;

            WRAP_TEXT_BY_DEFAULT                    = Properties.Settings.Default.wrapByDefault;
        }
    }
}
