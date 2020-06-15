using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using MS.Internal;
using MS.Utility;

namespace Notepad2.Controls
{
    public class NumberRange
    {
        public int Start { get; set; }
        public int End { get; set; }

        public NumberRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public List<int> CalculateArray()
        {
            if (End > 0)
            {
                List<int> _array = new List<int>();
                for (int i = Start; i <= End; i++)
                {
                    _array.Add(i);
                }
                return _array;
            }
            else return new List<int>();
        }
    }
}
