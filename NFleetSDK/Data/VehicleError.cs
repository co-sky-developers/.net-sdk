using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleError
    {
        public VehicleData Vehicle { get; set; }
        public List<ErrorData> Errors { get; set; }
    }
}
