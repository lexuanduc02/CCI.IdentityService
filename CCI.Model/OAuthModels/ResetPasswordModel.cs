﻿namespace CCI.Model.OAuthModels
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ComfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
