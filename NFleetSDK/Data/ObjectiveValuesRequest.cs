namespace NFleet.Data
{
    public class ObjectiveValuesRequest
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
