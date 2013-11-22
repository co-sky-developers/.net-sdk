using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class TaskEventDataSet : IResponseData
    {
        [IgnoreDataMember]
        public int VersionNumber { get; set; }
        [DataMember]
        public List<TaskEventData> Items { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }

        public TaskEventDataSet()
        {
            Items = new List<TaskEventData>();
            Meta = new List<Link>();
        }
    }
}
