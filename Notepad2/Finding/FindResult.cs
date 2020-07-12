namespace Notepad2.Finding
{
    public class FindResult
    {
        public int StartIndex { get; set; }
        public int WordLength { get; set; }
        public string PreviewFoundText { get; set; }

        public FindResult(int start, int end)
        {
            StartIndex = start;
            WordLength = end - start;
        }
        public FindResult(int start, int end, string previewFound)
        {
            StartIndex = start;
            WordLength = end - start;
            PreviewFoundText = previewFound;
        }
    }
}
