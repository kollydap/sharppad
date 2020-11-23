using SharpPad.Finding.TextFinding;

namespace SharpPad.Interfaces
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
