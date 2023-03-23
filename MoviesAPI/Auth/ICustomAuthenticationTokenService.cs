using System.Security.Claims;

namespace MoviesAPI.Auth
{
    public interface ICustomAuthenticationTokenService
    {
        ClaimsPrincipal Read(string value);
    }
}