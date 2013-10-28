using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class RouteEventDataSet : IResponseData
    {
        public List<RouteEventData> Items { get; set; }

        public List<Link> Meta { get; set; }

        public RouteEventDataSet()
        {
            Items = new List<RouteEventData>();

            Meta = new List<Link>();
        }

        #region Implementation of IVersioned

        public int VersionNumber { get; set; }

        #endregion
    }
}
