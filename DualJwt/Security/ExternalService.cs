using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DualJwt.Security
{
    public class ExternalService
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<AllowedControllerAction> AllowedActions { get; set; }

        public bool CanAccessControllerAction(AllowedControllerAction controllerAction)
        {
            if (AllowedActions == null || !AllowedActions.Any() || controllerAction == null) return false;

            return AllowedActions.Any(x => Regex.IsMatch(controllerAction.Action.ToLower(), x.Action.WildCardToRegex())
                && Regex.IsMatch(controllerAction.Controller.ToLower(), x.Controller.WildCardToRegex())
                && Regex.IsMatch(controllerAction.Method.ToLower(), x.Method.WildCardToRegex()));
        }
    }
}
