using System.ComponentModel.DataAnnotations;

namespace SmarterLead.API.Models.RequestModel
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name length can't be more than 100 characters")]
        public string firstName { get; set; }

        [StringLength(100, ErrorMessage = "Last Name length can't be more than 100 characters")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password length can't be more than 100 characters")]
        public string password { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email length can't be more than 100 characters")]
        public string email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(15, ErrorMessage = "Phone length can't be more than 15 characters")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255, ErrorMessage = "Address length can't be more than 255 characters")]
        public string address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City length can't be more than 50 characters")]
        public string city { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(2, ErrorMessage = "State length can't be more than 2 characters")]
        public string statecode { get; set; }

        [Required(ErrorMessage = "ZipCode is required")]
        [StringLength(6, ErrorMessage = "ZipCode length can't be more than 6 characters")]
        public string zipcode { get; set; }

        public string otp { get; set; }
        public string? company { get; set; }
        public List<string> roleid { get; set; }
        //public string? tkn { get; set; }



    }
}
