using AuthFrontEndNextsense.DTOs;

namespace AuthFrontEndNextsense.Services.Interfaces {
	public interface IAuthService {
		Task LoginAsync(LoginRequestDto loginRequest);
		Task LogoutAsync();
	}
}
