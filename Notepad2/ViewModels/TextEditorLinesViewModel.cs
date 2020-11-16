using Notepad2.Notepad;
using Notepad2.Utilities;
using System.Windows;
using System.Windows.Media;

namespace Notepad2.ViewModels
{
    public class TextEditorLinesViewModel : BaseViewModel
    {
        private FormatViewModel _documentFormat;
        private DocumentViewModel _document;
        private string _lineCounterText;

        public FormatViewModel DocumentFormat
        {
            get => _documentFormat;
            set => RaisePropertyChanged(ref _documentFormat, value);
        }

        public DocumentViewModel Document
        {
            get => _document;
            set => RaisePropertyChanged(ref _document, value);
        }

        public string LineCounterText
        {
            get => _lineCounterText;
            set => RaisePropertyChanged(ref _lineCounterText, value);
        }

        public int LinesCount { get; set; }

        public Size TextEditorSize { get; set; }

        public TextEditorLinesViewModel()
        {
            Document = new DocumentViewModel();
            DocumentFormat = new FormatViewModel();
            Render();
        }

        public void FontChanged(FontFamily newFont)
        {
            Render();
        }

        public void FontSizeChanged(double newSize)
        {
            Render();
        }

        public void SizeChanged(SizeChangedEventArgs e)
        {
            TextEditorSize = e.NewSize;
            Render();
        }

        int curIndexes;

        public void Render()
        {
            ClearText();
            if (Document != null && !Document.Text.IsEmpty())
            {
                //string text = Document.Text;
                //string[] lines = text.Split('\n');
                curIndexes = GetLinesCount();
                for (int i = 0; i < curIndexes; i++)
                {
                    LineCounterText += $"{i}\n";
                    //WriteLine(i.ToString());
                }
            }
        }

        public void ClearText()
        {
            LineCounterText = "";
        }

        public int GetLinesCount()
        {
            if (Document?.Text.IsEmpty() == false)
                return Document.Text.Split('\n').Length;
            return 0;
        }
    }
}
