using System.ComponentModel.DataAnnotations;

namespace CCI.Model.OAuthModels
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        // [Required]
        // public string RedirectUri { get; set; }
        // public bool RememberMe { get; set; }
    }
}
