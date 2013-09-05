namespace NFleetSDK.Data
{
    public class OptimizationUpdateRequestData
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int OptimizationId { get; set; }
        public string State { get; set; }
    }
}
