namespace SmarterLead.API.Models.ResponseModel
{
    public class User
    {
        public int ClientLoginID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
    }
}
