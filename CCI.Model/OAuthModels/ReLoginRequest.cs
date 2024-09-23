using System.ComponentModel.DataAnnotations;

namespace CCI.Model.OAuthModels;

public class ReLoginRequest
{
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string ClientSecret { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
}
