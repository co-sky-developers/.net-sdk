using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class UserData : IResponseData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.user+json";
        public static string MIMEVersion = "2.0";

        [IgnoreDataMember]
        public int VersionNumber { get; set; }
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public List<Link> Meta { get; set; }
    }
}
