using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode
{
    public class BBCodeParsingException : Exception
    {
        public BBCodeParsingException()
        {
        }
        public BBCodeParsingException(string message)
            : base(message)
        {
        }
    }
}