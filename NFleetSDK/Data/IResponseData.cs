using System.Collections.Generic;

namespace NFleet.Data
{
    public interface IResponseData : IVersioned
    {
        List<Link> Meta { get; }
    }
}
