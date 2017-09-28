namespace NFleet.Data
{
    public class ModificationAreaUpdateRequest
    {
        public CoordinateData TopLeft { get; set; }

        public CoordinateData BottomRight { get; set; }

        public string DataSource { get; set; }
    }
}