namespace NFleetSDK.Data
{
    public class RoutingProblemUpdateRequest : IVersioned
    {
        public int VersionNumber { get; set; }
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}
