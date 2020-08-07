using Notepad2.Finding;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Interfaces
{
    public interface IMainView
    {
        void AnimateNotepadItem(NotepadListItem nli, AnimationFlag flag);
        void HighlightFindResult(FindResult result);
        void FocusFindInput(bool focusOrNot);
        void ScrollItemsIntoView();
    }
}
