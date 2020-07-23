using Notepad2.InformationStuff;
using System;
using System.Windows;

namespace Notepad2.CClipboard
{
    public static class CustomClipboard
    {
        public static void SetTextObject(object obj)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, obj);
            }
            catch (Exception e)
            {
                Information.Show($"Failed to set clipboard: {e.Message}", "Clipboard");
            }
        }

        public static object GetTextObject()
        {
            try
            {
                return Clipboard.GetDataObject()?.GetData(typeof(string));
            }
            catch (Exception e)
            {
                Information.Show($"Failed to set clipboard: {e.Message}", "Clipboard");
                return null;
            }
        }
    }
}
