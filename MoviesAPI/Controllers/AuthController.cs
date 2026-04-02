using Infrastructure.Auth;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokenService) : Controller
{
	[HttpPost]
	public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
	{
		var result = tokenService.GenerateToken(request);
		if (result == default)
		{
			return Unauthorized("Invalid username or password");
		}
		return Ok(result);
	}
}
