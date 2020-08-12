using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Finding
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
