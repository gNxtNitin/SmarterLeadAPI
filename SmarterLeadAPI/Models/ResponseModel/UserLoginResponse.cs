﻿namespace SmarterLead.API.Models.ResponseModel
{
    public class UserLoginResponse
    {
        public int ClientLoginId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string Token { get; set; }
    }
}
