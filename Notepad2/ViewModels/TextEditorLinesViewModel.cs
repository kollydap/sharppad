using Notepad2.Notepad;
using Notepad2.Utilities;
using System.Windows;
using System.Windows.Media;

namespace Notepad2.ViewModels
{
    public class TextEditorLinesViewModel : BaseViewModel
    {
        private FormatModel _documentFormat;
        private DocumentModel _document;
        private string _lineCounterText;

        public FormatModel DocumentFormat
        {
            get => _documentFormat;
            set => RaisePropertyChanged(ref _documentFormat, value);
        }

        public DocumentModel Document
        {
            get => _document;
            set => RaisePropertyChanged(ref _document, value);
        }

        public string LineCounterText
        {
            get => _lineCounterText;
            set => RaisePropertyChanged(ref _lineCounterText, value);
        }

        public Size TextEditorSize { get; set; }

        public TextEditorLinesViewModel()
        {
            Document = new DocumentModel();
            DocumentFormat = new FormatModel();
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
            if (Document != null && !string.IsNullOrEmpty(Document.Text))
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

        public void WriteLine(string text)
        {

        }

        public void ClearText()
        {
            LineCounterText = "";
        }

        public int GetLinesCount()
        {
            if (!string.IsNullOrEmpty(Document?.Text))
                return Document.Text.Split('\n').Length;
            return 0;
        }
        public void UpdateDocuments(DocumentModel dm, FormatModel fm)
        {
            if (dm != null && fm != null)
            {
                Document = dm;
                DocumentFormat = fm;
                //DocumentFormat.FontFamilyChanged = FontChanged;
                //DocumentFormat.FontSizeChanged = FontSizeChanged;
                //Document.TextChanged += Document_TextChanged;
                Render();
            }
        }

        private void Document_TextChanged(string newText)
        {
            if (curIndexes != GetLinesCount())
                Render();
        }
    }
}
