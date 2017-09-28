using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationError
    {
        public ModificationData Modification { get; set; }
        public List<ErrorData> Errors { get; set; } 
    }
}