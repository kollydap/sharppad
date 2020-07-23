using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Views
{
    public enum NewWinPrefs
    {
        OpenBlankWindow = 1,
        OpenDefaultNotepadAfterLaunch = 2,
        OpenFilesInParams = 4,
        CanSaveSettings = 8, 
        CanLoadSettings = 16,
        CannotSaveSettings = 32, 
        CannotLoadSettings = 64
    }
}
