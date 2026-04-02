using Infrastructure.Models;

namespace Infrastructure.Auth;

public interface ITokenService
{
	string GenerateToken(LoginRequest request);
}