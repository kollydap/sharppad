using Notepad2.Finding;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad2.SimpleEditor
{
    public class SimpleTextEditor : TextBox
    {
        public SimpleTextEditor()
        {

        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                switch (e.Key)
                {
                    case Key.X:
                    case Key.C:
                        if (SelectedText == "")
                        {
                            var start = Text.LastIndexOf(Environment.NewLine, CaretIndex);
                            var lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
                            var lineLength = GetLineLength(lineIdx);
                            SelectionStart = start + 1;
                            SelectionLength = lineLength;
                            SelectedText.Substring(0, SelectedText.IndexOf(Environment.NewLine) + 1);
                        }
                        break;
                        //case Key.Tab:
                        //    string tab = new string(' ', 4);
                        //    int caretPosition = base.CaretIndex;
                        //    base.Text = base.Text.Insert(caretPosition, tab);
                        //    base.CaretIndex = caretPosition + 4 + 1;
                        //    e.Handled = true;
                        //    break;
                }
            }
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
