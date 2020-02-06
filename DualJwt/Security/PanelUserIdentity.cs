using System.Security.Principal;

namespace DualJwt.Security
{
    public class PanelUserIdentity : IIdentity
    {
        public string AuthenticationType => "JWT";
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
