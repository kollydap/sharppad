using Notepad2.Finding.TextFinding;

namespace Notepad2.Interfaces
{
    public interface IMainView
    {
        void ReplaceEditorText(FindResult result, string replaceWith);
        void FocusFindInput();
        void HighlightFindResult(FindResult result, bool focusTextEditor);
        void ShowItemsSearcherWindow();
        void ShowOrHideNotepadsList(bool show);
        void ShowOrHideTopNotepadsList(bool show);
    }
}
