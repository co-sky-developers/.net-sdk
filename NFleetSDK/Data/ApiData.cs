using System.Collections.Generic;

namespace NFleet.Data
{
    public class ApiData : IResponseData
    {
        public List<Link> Meta { get; private set; }

        public ApiData()
        {
            Meta = new List<Link>();
        }
    }
}
