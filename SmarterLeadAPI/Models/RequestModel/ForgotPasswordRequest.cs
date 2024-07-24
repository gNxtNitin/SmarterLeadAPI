using System.ComponentModel.DataAnnotations;

namespace SmarterLead.API.Models.RequestModel
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
