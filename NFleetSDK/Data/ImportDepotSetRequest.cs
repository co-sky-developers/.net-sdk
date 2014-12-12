using System.Collections.Generic;

namespace NFleet.Data
{
    public class ImportDepotSetRequest
    {
        public List<VehicleUpdateRequest> Items { get; set; }
        public ImportDepotSetRequest()
        {
            Items = new List<VehicleUpdateRequest>();
        }
    }
}
