namespace NFleet.Data
{
    public class MapImportRequest
    {
        public string ImageData { get; set; }
        public double MapLeft { get; set; }
        public double MapBottom { get; set; }
        public double MapWidth { get; set; }
        public double MapHeight { get; set; }
    }
}