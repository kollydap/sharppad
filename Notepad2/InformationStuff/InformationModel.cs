using Notepad2.Utilities;
using System;

namespace Notepad2.InformationStuff
{
    public class InformationModel
    {
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public InformationModel(string type, DateTime time, string message)
        {
            Type = type;
            Time = time;
            Message = message;
        }
        public InformationModel(InfoTypes type, DateTime time, string message)
        {
            Type = type.ToString();
            Time = time;
            Message = message;
        }
    }
}
