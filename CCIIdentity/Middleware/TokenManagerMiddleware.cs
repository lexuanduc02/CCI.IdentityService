using System.Net;
using CCI.Service.Contractors;

namespace CCIIdentity.Middleware;

public class TokenManagerMiddleware : IMiddleware
{
    private readonly ITokenManagerService _tokenManagerService;

    public TokenManagerMiddleware(ITokenManagerService tokenManagerService)
    {
        _tokenManagerService = tokenManagerService;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (await _tokenManagerService.IsCurrentActiveToken())
        {
            await next(context);

            return;
        }
        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
    }
}