using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationAreaSetImportRequest
    {
        public List<ModificationAreaImportRequest> Items { get; set; }

        public ModificationAreaSetImportRequest()
        {
            Items = new List<ModificationAreaImportRequest>();
        }
    }
}