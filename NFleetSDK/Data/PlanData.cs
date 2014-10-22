using System.Collections.Generic;


namespace NFleet.Data
{
    public class PlanData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.plan+json";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }

        public List<Link> Meta { get; set; }

        public List<FieldsItem> Items { get; set; }

        public KPIData KPIs { get; set; }

        public PlanData()
        {
            Items = new List<FieldsItem>();
            Meta = new List<Link>();
        }
    }
}
