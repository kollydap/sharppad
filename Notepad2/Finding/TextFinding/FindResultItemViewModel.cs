using Notepad2.Utilities;
using System;

namespace Notepad2.Finding.TextFinding
{
    public class FindResultItemViewModel : BaseViewModel
    {
        public FindResult Result { get; set; }

        public Action<FindResult> HighlightCallback { get; set; }

        public void Highlight()
        {
            HighlightCallback?.Invoke(Result);
        }
    }
}
