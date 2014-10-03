namespace NFleet.Data
{
    public class DeleteTasksRequest
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int VersionNumber { get; set; }
    }
}
