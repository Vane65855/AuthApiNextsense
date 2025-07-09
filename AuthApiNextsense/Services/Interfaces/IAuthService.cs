using AuthApiNextsense.DTOs;

namespace AuthApiNextsense.Services.Interfaces {
	public interface IAuthService {
		Task<string> LoginAsync(LoginRequestDto loginRequest);
	}
}
