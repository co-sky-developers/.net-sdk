using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NFleet.Data
{
    public class DepotData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.depot";
        public static string MIMEVersion = "2.2";


        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Info1 { get; set; }
        [DataMember]
        public List<CapacityData> Capacities { get; set; }
        [DataMember]
        public LocationData Location { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string DataSource { get; set; }
        [IgnoreDataMember]
        int IVersioned.VersionNumber { get; set; }

        public DepotData()
        {
            Capacities = new List<CapacityData>();
            Meta = new List<Link>();

        }
    }
}
