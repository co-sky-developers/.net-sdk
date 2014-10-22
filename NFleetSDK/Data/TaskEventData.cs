using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class TaskEventData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.taskevent+json";
        public static string MIMEVersion = "2.0";

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Info { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public List<CapacityData> Capacities { get; set; }
        [DataMember]
        public List<TimeWindowData> TimeWindows { get; set; }
        [DataMember]
        public LocationData Location { get; set; }
        [DataMember]
        public double ServiceTime { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }

        public TaskEventData()
        {
            TimeWindows = new List<TimeWindowData>();
            Capacities = new List<CapacityData>();
            Meta = new List<Link>();
        }
    }
}
