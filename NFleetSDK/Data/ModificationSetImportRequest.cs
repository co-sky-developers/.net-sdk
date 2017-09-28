using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationSetImportRequest
    {
        public List<ModificationUpdateRequest> Items { get; set; }

        public ModificationSetImportRequest()
        {
            Items = new List<ModificationUpdateRequest>();
        }
    }
}