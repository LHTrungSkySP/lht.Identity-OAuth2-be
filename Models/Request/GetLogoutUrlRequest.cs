namespace MSC.Identity.Models.Request
{
    public class GetLogoutUrlRequest
    {
        public string ReturnURL { get; set; }
        public bool IsLocal { get; set; }
    }
}
