﻿using System.Collections.Generic;

namespace NFleet.Data
{
    public class CreateDepotRequest 
    {
        public int DepotId { get; set; }

        public string Name { get; set; }
        public string Info1 { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public LocationData Location { get; set; }
        public string Type { get; set; }
        public string DataSource { get; set; }
        public int VersionNumber { get; set; }

        public CreateDepotRequest()
        {
            Capacities = new List<CapacityData>();
        }
    }
}
