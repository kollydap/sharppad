using Notepad2.CClipboard;
using Notepad2.Finding;
using Notepad2.InformationStuff;
using Notepad2.Preferences;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                            if (!PreferencesG.CAN_SELECT_ENTIRE_LINE_CTRL_SHIFT_A)
                                break;
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
                        if (!PreferencesG.CAN_CUT_ENTIRE_LINE_CTRL_X)
                            break;
                        if (SelectedText == "")
                        {
                            int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
                            int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
                            int lineLength = GetLineLength(lineIdx);
                            SelectionStart = start;
                            SelectionLength = lineLength;
                        }
                        break;
                    case Key.C:
                        if (!PreferencesG.CAN_COPY_ENTIRE_LINE_CTRL_C)
                            break;
                        if (SelectedText == "")
                        {
                            int prevCaretIndex = CaretIndex;
                            int start = Text.LastIndexOf(Environment.NewLine, CaretIndex) + 1;
                            int lineIdx = GetLineIndexFromCharacterIndex(CaretIndex);
                            int lineLength = GetLineLength(lineIdx);
                            SelectionStart = start;
                            SelectionLength = lineLength;
                            CustomClipboard.SetObject(SelectedText);
                            CaretIndex = prevCaretIndex;
                            e.Handled = true;
                        }
                        break;

                    case Key.Up:
                        if (!PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS)
                            break;
                        LineUp(); e.Handled = true; 
                        break;
                    case Key.Down:
                        if (!PreferencesG.SCROLL_VERTICAL_WITH_CTRL_ARROWKEYS)
                            break; 
                        LineDown(); e.Handled = true; 
                        break;
                    case Key.Left:
                        if (!PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS)
                            break;
                        LineLeft(); e.Handled = true;
                        break;
                    case Key.Right:
                        if (!PreferencesG.SCROLL_HORIZONTAL_WITH_CTRL_ARROWKEYS)
                            break;
                        LineRight(); e.Handled = true;
                        break;
                }
            }
            // Cant do because holding alt apparently blocks the keydown events...
            //if (Keyboard.Modifiers == ModifierKeys.Alt)
            //{
            //    switch (e.Key)
            //    {
            //        case Key.Left: LineLeft(); e.Handled = true; break;
            //        case Key.Right: LineRight(); e.Handled = true; break;
            //    }
            //}
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
