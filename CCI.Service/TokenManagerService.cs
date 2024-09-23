using CCI.Model.Options;
using CCI.Service.Contractors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace CCI.Service;

public class TokenManagerService : ITokenManagerService
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public TokenManagerService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
    }
    public async Task<bool> IsCurrentActiveToken()
        => await IsActiveAsync(GetCurrentAsync());

    public async Task DeactivateCurrentAsync()
        => await DeactivateAsync(GetCurrentAsync());

    public async Task<bool> IsActiveAsync(string token)
        => await _cache.GetStringAsync(GetKey(token)) == null;

    public async Task DeactivateAsync(string token)
        => await _cache.SetStringAsync(GetKey(token),
            " ", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromHours(1)
            });

    private string GetCurrentAsync()
    {
        var authorizationHeader = _httpContextAccessor
            .HttpContext.Request.Headers["authorization"];

        return authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Single().Split("Bearer").Last();
    }

    private static string GetKey(string token)
        => $"tokens:{token}:deactivated";
}
