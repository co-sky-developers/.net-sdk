using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class OptimizationData : IResponseData
    {
        public static int DefaultOptimizationId = 0;

        public int Id { get; set; }
        public string State { get; set; }
        public int Progress { get; set; }
        public List<Link> Meta { get; set; }
        public VehicleDataSet Vehicles { get; set; }
        public TaskDataSet Tasks { get; set; }
        public LocationDataSet Locations { get; set; }
        public double Value { get; set; }

        public OptimizationData()
        {
            Meta = new List<Link>();
        }
    }
}
