using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class NFleetRequestException : IOException
    {
        public List<ErrorData> Items { get; set; } 

        public NFleetRequestException(string message) : base(message)
        {
            
        }
    }
}
