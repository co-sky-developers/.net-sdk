namespace NFleetSDK.Data
{
    public class RoutingProblemUpdateRequest
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}
