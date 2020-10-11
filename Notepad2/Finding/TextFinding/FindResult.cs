namespace Notepad2.Finding.TextFinding
{
    public class FindResult
    {
        public int StartIndex { get; set; }
        public int WordLength { get; set; }

        public FindResult(int start, int end)
        {
            StartIndex = start;
            WordLength = end - start;
        }
    }
}
