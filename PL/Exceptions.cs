using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    public class PlFormatException : Exception
    {
        public PlFormatException(string? message) : base(message) { }
    }
}
