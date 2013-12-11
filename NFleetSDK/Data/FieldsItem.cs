using System.Collections.Generic;

namespace NFleet.Data
{
    public class FieldsItem
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public List<RouteEventData> Events { get; set; } 
    }
}
