namespace SmarterLead.API.Models.RequestModel
{
    public class UserProfile
    {
        public int ClientLoginID { get; set; }
        public string UserID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string birthday { get; set; }
        public string imagepath { get; set; }
    }
}
