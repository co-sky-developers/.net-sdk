using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class UserData : IResponseData
    {
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

        #region Implementation of IResponseData
        [DataMember]
        public List<Link> Meta { get; set; }
        

        #endregion
    }
}
