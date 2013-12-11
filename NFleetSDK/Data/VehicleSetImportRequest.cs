using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleSetImportRequest
    {
        public List<VehicleUpdateRequest> Items { get; set; } 
        public VehicleSetImportRequest()
        {
            Items = new List<VehicleUpdateRequest>();
        }
    }
}
