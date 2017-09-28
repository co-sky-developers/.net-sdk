using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationAreaError
    {
        public ModificationAreaData ModificationArea { get; set; }
        public List<ErrorData> Errors { get; set; }
    }
}