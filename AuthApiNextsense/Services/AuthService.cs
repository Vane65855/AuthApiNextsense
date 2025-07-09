using AuthApiNextsense.DTOs;
using AuthApiNextsense.Helpers;
using AuthApiNextsense.Repositories.Interfaces;
using AuthApiNextsense.Services.Interfaces;
using System.Security.Authentication;

namespace AuthApiNextsense.Services {
	public class AuthService : IAuthService {
		private readonly IAccountRepository _accountRepository;
		private readonly ITokenService _tokenService;

		public AuthService(IAccountRepository accountRepository, ITokenService tokenService) {
			_accountRepository = accountRepository;
			_tokenService = tokenService;
		}

		public async Task<string> LoginAsync(LoginRequestDto loginRequest) {
			var account = await _accountRepository.GetByUsernameAsync(loginRequest.Username);
			// Check if account exists, is active, and password is valid
			if (account == null || !PasswordHashHelper.VerifyPasswordHash(loginRequest.Password, account.PasswordHash)) throw new InvalidCredentialException("Invalid username or password.");
			if (!account.IsActive) throw new InvalidCredentialException("Your account has been deactivated.");
			return _tokenService.GenerateJwtToken(account);
		}
	}
}
