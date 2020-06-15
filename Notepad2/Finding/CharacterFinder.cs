using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Finding
{
    public static class CharacterFinder
    {
        public enum FindSettings
        {
            None,
            CaseSensitive,
        }
        /// <summary>
        /// Finds every occourance of a word and returns a list of FindResults
        /// </summary>
        /// <param name="toFind">The string you want to find.</param>
        /// <param name="heapOfText">The text which will be searched</param>
        /// <returns></returns>
        public static List<FindResult> FindTextOccurrences(this string heapOfText, string toFind, FindSettings settings)
        {
            if (string.IsNullOrEmpty(heapOfText))
                return null;
            string heap = heapOfText;
            string tofind = toFind;
            switch (settings)
            {
                case FindSettings.CaseSensitive:
                    heap = heapOfText;
                    tofind = toFind;
                    break;
                case FindSettings.None:
                    heap = heapOfText.ToLower();
                    tofind = toFind.ToLower();
                    break;
            }

            List<FindResult> indexes = new List<FindResult>();
            try
            {
                for (int index = 0; ; index += tofind.Length)
                {
                    try
                    {
                        index = heap.IndexOf(tofind, index);
                        if (index == -1)
                            return indexes;
                        FindResult fr =
                            new FindResult(
                                index,
                                index + tofind.Length,
                                heap.GetRegionOfText(index, index + tofind.Length, 5, 5));
                        indexes.Add(fr);
                    }
                    catch { return indexes; }
                }
            }
            catch { return indexes; }
        }

        /// <summary>
        /// Extracts a region of text from within some other text. If the region tries to go lower
        /// than 0 or after the text's length, it will conpensate for that.
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="startIndex">The start index of the text you already have</param>
        /// <param name="textBefore">the number of chars to get before the text you already have</param>
        /// <param name="textAfter">the number of chars to get after the text your already have</param>
        /// <returns></returns>
        public static string GetRegionOfText(this string text, int startIndex, int endIndex, int textBefore, int textAfter)
        {
            string before = GetBeforeText(text, startIndex, textBefore);
            string after = GetAfterText(text, endIndex, textAfter);
            string word;
            try { word = text.Substring(startIndex, endIndex - startIndex); }
            catch { word = "[ERROR]"; }

            return $"{before}{word}{after}";
        }

        public static string GetBeforeText(string text, int startIndex, int numberOfCharsBefore)
        {
            string before = "";
            for (int i = startIndex; i > startIndex - numberOfCharsBefore; i--)
            {
                if (i - 1 >= 0) try { before += text[i - 1]; } catch { }
            }
            return ReverseString(before);
        }

        public static string GetAfterText(string text, int endIndex, int numberOfCharsAfter)
        {
            string after = "";
            for (int i = 0; i < numberOfCharsAfter; i++)
            {
                if ((endIndex + i) < text.Length) try { after += text[endIndex + i]; } catch { }
            }
            return after;
        }

        /// <summary>
        /// Reverses the direction of a string: 
        /// ABCDEFG turns into GFEDCBA. 12345 to 54321
        /// </summary>
        /// <param name="text">The text to be reversed</param>
        /// <returns>The reversed text given</returns>
        public static string ReverseString(string text)
        {
            string final = "";
            for (int i = text.Length - 1; i >= 0; i--)
            {
                try { final += text[i]; } catch { }
            }
            return final;
        }
    }
}
