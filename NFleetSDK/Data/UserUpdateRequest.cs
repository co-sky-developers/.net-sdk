namespace NFleet.Data
{
    public class UserUpdateRequest
    {
        public int VersionNumber { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int OptimizationQueueLimit { get; set; }
        public int ProblemLimit { get; set; }
        public int TaskLimit { get; set; }
        public int VehicleLimit { get; set; }
        public int DepotLimit { get; set; }
    }
}
