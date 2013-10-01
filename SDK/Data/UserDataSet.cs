using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class UserDataSet : IResponseData
    {
        #region Implementation of IResponseData

        public List<Link> Meta { get; set; }

        #endregion

        public List<UserData> Items { get; set; }
    }
}
