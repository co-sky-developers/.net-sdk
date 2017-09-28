namespace NFleet.Data
{
    public class ModificationAreaImportRequest
    {
        public string ModificationName { get; set; }
        public CoordinateData TopLeft { get; set; }
        public CoordinateData BottomRight { get; set; }
        public string DataSource { get; set; }
    }
}