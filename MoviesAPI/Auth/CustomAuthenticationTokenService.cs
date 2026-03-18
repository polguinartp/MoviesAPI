using System;
using System.Security.Claims;
using System.Text;

namespace MoviesAPI.Auth;

public class CustomAuthenticationTokenService : ICustomAuthenticationTokenService
{
	public ClaimsPrincipal Read(string value)
	{
		try
		{
			var decodedBytes = Convert.FromBase64String(value);
			var decodedString = Encoding.UTF8.GetString(decodedBytes);

			var splittedData = decodedString.Split(['|']);

			return new ClaimsPrincipal(new ClaimsIdentity(
				[
						new(ClaimTypes.NameIdentifier, splittedData[0]),
						new(ClaimTypes.Role, splittedData[1]),
				], CustomAuthenticationSchemeOptions.AuthenticationScheme)
			);
		}
		catch (Exception ex)
		{
			throw new ReadTokenException(value, ex);
		}
	}
}
