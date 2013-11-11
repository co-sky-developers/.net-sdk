using System.Collections.Generic;

namespace NFleet.Data
{
    public class UserDataSet : IResponseData
    {
        #region Implementation of IResponseData

        public List<Link> Meta { get; set; }

        #endregion

        public int VersionNumber { get; set; }
        public List<UserData> Items { get; set; }
    }
}
