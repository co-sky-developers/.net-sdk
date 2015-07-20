
namespace NFleet.Data
{
    public class SummaryData
    {
        public double TravelDistanceSum { get; set; }
        public double WorkingTimeSum { get; set; }
        public int TotalTaskCount { get; set; }
        public int PlannedTaskCount { get; set; }
        public int TotalVehicleCount { get; set; }
        public int UsedVehicleCount { get; set; }
        public double AccumulatedCost { get; set; }
    }
}
