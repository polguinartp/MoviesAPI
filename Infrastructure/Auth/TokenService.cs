using Infrastructure.Auth;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesAPI.Handlers;

public class TokenService(IConfiguration configuration) : ITokenService
{
	public string GenerateToken(LoginRequest request)
	{
		if (request.Username != "admin" && request.Password != "some_password")
		{
			return null!;
		}

		var claims = new[]
		{
			new Claim(ClaimTypes.Name, request.Username),
			new Claim(ClaimTypes.Role, "Admin")
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthenticationKey"]));
		var credentails = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: credentails
		);
		var jwt = new JwtSecurityTokenHandler().WriteToken(token);

		return jwt;
	}
}