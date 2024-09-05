namespace SmarterLead.API.Models.RequestModel
{
    public class UserProfile
    {
        public int ClientID { get; set; }
        //public string UserID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string? Address { get; set; }
        public string City { get; set; }
        public string? StateName { get; set; }
        public string Zip { get; set; }
        public string? imagepath { get; set; }
        public string? CompanyName { get; set; }
        public string? birthday { get; set; }
    }
}
