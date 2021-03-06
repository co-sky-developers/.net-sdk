﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class RoutingProblemData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.problem";
        public static string MIMEVersion = "2.0";

        public RoutingProblemData()
        {
            Meta = new List<Link>();
        }

        [IgnoreDataMember]
        int IVersioned.VersionNumber { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
        [DataMember]
        public DateTime ModifiedDate { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string DataState { get; set; }
        [DataMember]
        public int Progress { get; set; }
    }
}
