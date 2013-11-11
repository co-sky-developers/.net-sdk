using System.Collections.Generic;

namespace NFleet.Data
{
    public class UserData : IResponseData
    {
        public int VersionNumber { get; set; }
        public int Id { get; set; }

        #region Implementation of IResponseData

        public List<Link> Meta { get; set; }
        

        #endregion
    }
}
