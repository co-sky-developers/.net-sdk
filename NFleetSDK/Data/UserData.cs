using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class UserData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.user";
        public static string MIMEVersion = "2.0";

        [IgnoreDataMember]
        public int VersionNumber { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ProblemLimit { get; set; }
        [DataMember]
        public int VehicleLimit { get; set; }
        [DataMember]
        public int TaskLimit { get; set; }
        [DataMember]
        public int OptimizationQueueLimit { get; set; }
        [DataMember]
        public int OptimizationCount { get; set; }

        [DataMember]
        public List<Link> Meta { get; set; }
    }
}
