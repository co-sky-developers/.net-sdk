using System.Collections.Generic;

namespace NFleet.Data
{
    public class ImportData : IResponseData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.import";
        public static string MIMEVersion = "2.2";

        public List<Link> Meta { get; set; }
        public int ErrorCount { get; set; }
        public string State { get; set; }
        public List<VehicleError> Vehicles { get; set; }
        public List<TaskError> Tasks { get; set; }
        public List<DepotError> Depots { get; set; } 
        public List<ModificationError> Modifications { get; set; }
        public List<ModificationAreaError> ModificationAreas { get; set; }

        public ImportData()
        {
            Meta = new List<Link>();
            Vehicles = new List<VehicleError>();
            Tasks = new List<TaskError>();
            Depots = new List<DepotError>();
            Modifications = new List<ModificationError>();
            ModificationAreas = new List<ModificationAreaError>();
        }
    }
}
