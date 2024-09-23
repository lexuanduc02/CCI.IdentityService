namespace CCI.Model.ResponseModel
{
    public class LoginResponseModel
    {
        public UserProfile UserProfile { get; set; }
        public Token Token { get; set; }
        public string RedirectUri { get; set; }
    }

    public class UserProfile
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        public int ExpireTime { get; set; }
    }

}
