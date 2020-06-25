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
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    switch (e.Key)
                    {
                        case Key.A:
                            int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
                            int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
                            int lineLength = GetLineLength(lineIdx);
                            SelectionStart = start;
                            SelectionLength = lineLength;
                            break;
                    }
                }
                switch (e.Key)
                {
                    case Key.X:
                        if (SelectedText == "")
                        {
                            int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
                            int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
                            int lineLength = GetLineLength(lineIdx);
                            SelectionStart = start;
                            SelectionLength = lineLength;
                            //SelectedText.Substring(0, SelectedText.IndexOf(Environment.NewLine) + 1);
                        }
                        break;
                    case Key.C:
                        if (SelectedText == "")
                        {
                            int prevCaretIndex = CaretIndex;
                            int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
                            int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
                            int lineLength = GetLineLength(lineIdx);
                            SelectionStart = start;
                            SelectionLength = lineLength;
                            try { Clipboard.SetDataObject(SelectedText); }
                            catch { }
                            CaretIndex = prevCaretIndex;
                            e.Handled = true;
                        }
                        break;

                    case Key.Down:
                        LineDown(); 
                        break;
                    case Key.Up: 
                        LineUp(); 
                        break;
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
