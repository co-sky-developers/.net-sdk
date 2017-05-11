using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NFleet.Data
{
    public class NFleetRequestException : IOException
    {
        public List<ErrorData> Items { get; set; } 
        public HttpStatusCode StatusCode { get; set; }

        public NFleetRequestException(string message) : base(message)
        {
            
        }
    }
}
