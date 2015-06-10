using System.Collections.Generic;
using NFleet.Data;

namespace NFleet
{
    public class AppUserData : IResponseData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<Link> Meta { get; set; }

        public AppUserData()
        {
            Meta = new List<Link>();
        }
    }
}