using AuthZI.Security;
using Microsoft.AspNetCore.Authentication;

namespace AuthZI.MicrosoftOrleans.IdentityServer.SampleApiAndSiloHost;

public class AccessTokenProvider : IAccessTokenProvider
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  private readonly string _accessToken;

  public AccessTokenProvider(IHttpContextAccessor httpContextAccessorResolver)
  {
    // ReSharper disable once JoinNullCheckWithUsage
    if (httpContextAccessorResolver == null)
    {
      throw new ArgumentNullException(nameof(httpContextAccessorResolver),
        @"The value for IHttpContextAccessor can not be null.");
    }

    _httpContextAccessor = httpContextAccessorResolver;
    _accessToken = RetrieveToken(_httpContextAccessor);
  }

  private static string RetrieveToken(IHttpContextAccessor httpContextAccessor)
  {
    if (httpContextAccessor.HttpContext != null)
    {
      return httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
    }

    return string.Empty;
  }

  public async Task<string> RetrieveTokenAsync()
  {
    if (_httpContextAccessor.HttpContext == null) return _accessToken;

    // The first approach
    var token1 = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

    // The second approach
    var token2 = _httpContextAccessor.HttpContext.Request
      .Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

    return token1 ?? token2;
  }
}