using System.Collections.Generic;


namespace NFleet.Data
{
    public class PlanData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.plan";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }

        public List<Link> Meta { get; set; }

        public List<VehiclePlanData> Items { get; set; }

        public KPIData KPIs { get; set; }

        public List<TaskData> Unassigned { get; set; }

        public List<DepotData> Depots { get; set; }

        public PlanData()
        {
            Items = new List<VehiclePlanData>();
            Meta = new List<Link>();
        }
    }
}
