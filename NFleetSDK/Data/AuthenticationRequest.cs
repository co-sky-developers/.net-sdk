namespace NFleet.Data
{
    public class AuthenticationRequest
    {
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}
