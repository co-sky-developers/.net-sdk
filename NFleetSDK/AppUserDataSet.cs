using System.Collections.Generic;
using NFleet.Data;

namespace NFleet
{
    public class AppUserDataSet : IResponseData
    {
        public List<AppUserData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public AppUserDataSet()
        {
            Items = new List<AppUserData>();
            Meta = new List<Link>();
        }
    }
}