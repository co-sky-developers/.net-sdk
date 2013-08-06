using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class ErrorData
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public List<Link> Meta { get; private set; }

        public ErrorData()
        {
            Meta = new List<Link>();
        }
    }
}
