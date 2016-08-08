using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoadSetImportRequest
    {
        public List<RoadUpdateRequest> Items { get; set; }
        public double Scale { get; set; }

        public RoadSetImportRequest()
        {
            Items = new List<RoadUpdateRequest>();
        }
    }
}