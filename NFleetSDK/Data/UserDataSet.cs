using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class UserDataSet: IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.userset";
        public static string MIMEVersion = "2.0";

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
