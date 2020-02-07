namespace DualJwt.Security
{
    public class AllowedControllerAction
    {
        public string Action { get; set; } = "*";
        public string Controller { get; set; }
        public string Method { get; set; }
    }
}
