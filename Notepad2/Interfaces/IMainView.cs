using Notepad2.Finding;
using Notepad2.Notepad;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Interfaces
{
    public interface IMainView
    {
        void HighlightFindResult(FindResult result);
        void FocusFindInput(bool focusOrNot);
        void ScrollItemsIntoView();
        void ShowItemsSearcherWindow();
    }
}
