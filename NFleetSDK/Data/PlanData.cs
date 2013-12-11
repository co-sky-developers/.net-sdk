using System;
using System.Collections.Generic;


namespace NFleet.Data
{
    public class PlanData : IResponseData, IVersioned
    {
        int IVersioned.VersionNumber { get; set; }

        public List<Link> Meta { get; set; }

        public List<FieldsItem> Items { get; set; }

        public PlanData()
        {
            Items = new List<FieldsItem>();
            Meta = new List<Link>();
        }
    }
}
