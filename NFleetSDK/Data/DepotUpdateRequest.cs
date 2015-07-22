using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    public class DepotUpdateRequest
    {
        public static string MIMEType = DepotData.MIMEType;
        public static string MIMEVersion = DepotData.MIMEVersion;

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
        public string Type { get; set; }
        [DataMember]
        public string DataSource { get; set; }
        [DataMember]
        public double StoppingTime { get; set; }
    }
}
