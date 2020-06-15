using Notepad2.Finding;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Notepad2.SimpleEditor
{
    public class SimpleTextEditor : TextBox
    {
        public SimpleTextEditor()
        {
        }

        public int GetLinesCount()
        {
            return Text.Split('\n').Length;
        }

        public void HighlightSearchResult(FindResult result)
        {
            if (result.StartIndex >= 0 &&
                result.StartIndex < Text.Length &&
                result.WordLength > 0 &&
                result.StartIndex + result.WordLength < Text.Length)
            {
                try
                {
                    Focus();
                    ScrollToLine(GetLineIndexFromCharacterIndex(result.StartIndex));
                    Select(result.StartIndex, result.WordLength);
                }
                catch { }
            }
        }

        public Rect GetCaretLocation()
        {
            if (CaretIndex >= 0)
            {
                Rect rect = GetRectFromCharacterIndex(CaretIndex);
                if (double.IsInfinity(rect.X) || double.IsInfinity(rect.Y))
                    return new Rect(0, 0, 0, 0);
                return rect;
            }
            else return new Rect(0, 0, 0, 0);
        }
    }
}
