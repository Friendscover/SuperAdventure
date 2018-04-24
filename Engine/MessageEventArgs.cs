using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class MessageEventArgs :EventArgs
    {
        public string Message { get; set; }
        public bool AddExtraNewLine { get; set; }

        public MessageEventArgs(string message, bool addExtraNewLine)
        {
            Message = message;
            AddExtraNewLine = addExtraNewLine;
        }
    }
}
