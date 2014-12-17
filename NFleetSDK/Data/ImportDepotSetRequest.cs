using System.Collections.Generic;

namespace NFleet.Data
{
    public class ImportDepotSetRequest
    {
        public List<UpdateDepotRequest> Items { get; set; }
        public ImportDepotSetRequest()
        {
            Items = new List<UpdateDepotRequest>();
        }
    }
}
