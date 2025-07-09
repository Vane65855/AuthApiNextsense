using AuthApiNextsense.Models;

namespace AuthApiNextsense.Services.Interfaces {
	public interface ITokenService {
		string GenerateJwtToken(Account account);
	}
}
