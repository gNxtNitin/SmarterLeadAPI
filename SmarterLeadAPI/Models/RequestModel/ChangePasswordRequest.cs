namespace SmarterLead.API.Models.RequestModel
{
    public class ChangePasswordRequest
    {
        public int ClientLoginID { get; set; }
        public string oldpwd { get; set; }
        public string newpwd { get; set; }
    }
}
