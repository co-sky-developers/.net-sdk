using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public interface IResponseData : IVersioned
    {
        List<Link> Meta { get; }
    }
}
