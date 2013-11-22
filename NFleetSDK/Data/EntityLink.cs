using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class EntityLink : IResponseData
    {
        [IgnoreDataMember]
        public int VersionNumber { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }

        public EntityLink()
        {
            Meta = new List<Link>();
        }
    }
}
