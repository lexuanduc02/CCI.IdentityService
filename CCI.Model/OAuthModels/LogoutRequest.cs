using System.ComponentModel.DataAnnotations;

namespace CCI.Model.OAuthModels
{
    public class LogoutRequest
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public string AccessToken { get; set; }
    }
}
