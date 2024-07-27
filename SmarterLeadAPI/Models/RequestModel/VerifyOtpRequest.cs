namespace SmarterLead.API.Models.RequestModel
{
    public class VerifyOtpRequest
    {
        public string otp { get; set; }
        public string email { get; set; }
    }
}
