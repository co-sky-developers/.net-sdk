using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class EntityLinkCollection : IResponseData, IVersioned
    {
        [IgnoreDataMember]
        public int VersionNumber { get; set; }
        [DataMember]
        public List<EntityLink> Items { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }

        public EntityLinkCollection()
        {
            Items = new List<EntityLink>();
            Meta = new List<Link>();
        }
    }
}
