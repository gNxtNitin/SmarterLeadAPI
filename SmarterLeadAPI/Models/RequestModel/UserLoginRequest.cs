namespace SmarterLead.API.Models.RequestModel
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? otp { get; set; }
    }
}
