using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TokenData : IResponseData
    {
        public string AccessToken { get; set; }
        public int ExpirationIn { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public List<Link> Meta { get; set; }

        public TokenData()
        {
            Meta = new List<Link>();
        }

        #region Implementation of IVersioned

        public int VersionNumber { get; set; }

        #endregion
    }
}
