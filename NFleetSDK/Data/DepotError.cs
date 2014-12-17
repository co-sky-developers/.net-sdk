using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class DepotError
    {
        public DepotData Depot { get; set; }
        public List<ErrorData> Errors { get; set; } 
    }
}
