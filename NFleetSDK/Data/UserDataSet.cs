using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class UserDataSet: IResponseData, IVersioned
    {
        int IVersioned.VersionNumber { get; set; }
        public List<UserData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public UserDataSet()
        {
            Items = new List<UserData>();
            Meta = new List<Link>();
        }
    }
}
