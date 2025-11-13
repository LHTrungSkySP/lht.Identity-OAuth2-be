namespace MSC.Identity.Models.Request
{
    public class GetLoginUrlRequest
    {
        public string ReturnURL { get; set; }
        public string FEDomain { get; set; }
        public bool IsLocal {  get; set; }
    }
}
