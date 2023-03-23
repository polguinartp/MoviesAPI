using Microsoft.AspNetCore.Authentication;

namespace MoviesAPI.Auth
{
    public class CustomAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string AuthenticationScheme = "CustomAuthentication";
    }
}
