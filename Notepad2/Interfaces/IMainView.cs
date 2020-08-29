using Notepad2.Finding.TextFinding;

namespace Notepad2.Interfaces
{
    public interface IMainView
    {
        void HighlightFindResult(FindResult result);
        void FocusFindInput(bool focusOrNot);
        void ScrollItemsIntoView();
        void ShowItemsSearcherWindow();
        void ShowOrHideNotepadsList(bool show);
        void ShowOrHideTopNotepadsList(bool show);
    }
}
