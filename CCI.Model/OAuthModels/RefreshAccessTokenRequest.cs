namespace CCI.Model;

public class RefreshAccessTokenRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RefreshToken { get; set; }
}
