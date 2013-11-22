using System.Collections.Generic;

namespace NFleet.Data
{
    public interface IResponseData
    {
        List<Link> Meta { get; }
    }
}
