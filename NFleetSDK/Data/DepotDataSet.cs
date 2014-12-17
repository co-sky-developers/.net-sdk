using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class DepotDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.depotset";
        public static string MIMEVersion = "2.2";

        int IVersioned.VersionNumber { get; set; }
        public List<VehicleData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public DepotDataSet()
        {
            Items = new List<VehicleData>();
            Meta = new List<Link>();
        }
    }
}
