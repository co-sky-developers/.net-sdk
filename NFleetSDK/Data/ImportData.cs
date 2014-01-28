using System.Collections.Generic;

namespace NFleet.Data
{
    public class ImportData : IResponseData
    {
        public List<Link> Meta { get; set; }
        public int ErrorCount { get; set; }
        public string State { get; set; }
        public List<VehicleError> Vehicles { get; set; }
        public List<TaskError> Tasks { get; set; }

        public ImportData()
        {
            Meta = new List<Link>();
            Vehicles = new List<VehicleError>();
            Tasks = new List<TaskError>();
        }
    }
}
