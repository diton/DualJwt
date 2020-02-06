using System.Security.Principal;

namespace DualJwt.Security
{
    public class PanelUserPrincipal : IPrincipal
    {
        private PanelUserIdentity _panelUserIdentity;

        public PanelUserPrincipal()
        {

        }

        public PanelUserPrincipal(PanelUserIdentity panelUserIdentity)
        {
            _panelUserIdentity = panelUserIdentity;
        }

        public IIdentity Identity => _panelUserIdentity;

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
