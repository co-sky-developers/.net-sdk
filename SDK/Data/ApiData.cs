using System.Collections.Generic;

namespace NFleetSDK.Data
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
