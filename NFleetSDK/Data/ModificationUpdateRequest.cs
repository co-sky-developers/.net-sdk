using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationUpdateRequest
    {
        public string Name { get; set; }

        public string Info1 { get; set; }        

        public List<string> SpeedProfiles { get; set; }

        public double SpeedFactor { get; set; }

        public string DataSource { get; set; }
    }
}